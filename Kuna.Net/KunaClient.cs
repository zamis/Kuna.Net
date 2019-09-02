﻿using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;
using CryptoExchange.Net.Requests;
using Kuna.Net.Converters;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Kuna.Net
{
    public class KunaClient : RestClient, IKunaClient
    {
        public KunaClient():base(new KunaClientOptions(),null)
        {
                
        }
        public KunaClient(KunaClientOptions options) : base(options, options.ApiCredentials == null ? null : new KunaAuthenticationProvider(options.ApiCredentials))
        {  
            postParametersPosition = PostParameters.InUri;
            requestBodyFormat = RequestBodyFormat.Json;
        }
        #region Endpoints
        private const string ServerTimeEndpoint = "timestamp";
        private const string MarketInfoEndpoint = "tickers/{}";
        private const string OrderBookEndpoint = "depth";
        private const string AllTradesEndpoint = "trades";
        private const string AccountInfoEndpoint = "members/me";
        private const string OrdersEndpoint = "orders";
        private const string SingleOrderEndpoint = "order";

        private const string CancelOrderEndpoint = "order/delete";
        private const string MyTradesEndpoint = "trades/my";

        #endregion
        public CallResult<DateTime> GetServerTime()
        {
            var result = ExecuteRequest<string>(GetUrl(ServerTimeEndpoint), "GET").Result;
            long seconds = long.Parse(result.Data);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return new CallResult<DateTime>(dateTime, null);
        }

        public CallResult<KunaTickerInfo> GetMarketInfo(string market)
        {            
            var result = ExecuteRequest<KunaTickerInfo>(GetUrl(FillPathParameter(MarketInfoEndpoint, market)), "GET").Result;
            return new CallResult<KunaTickerInfo>(result.Data, result.Error);
        }

        public CallResult<KunaOrderBook> GetOrderBook(string market)
        {
            var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = ExecuteRequest<KunaOrderBook>(GetUrl(OrderBookEndpoint), "GET", parameters).Result;
            return new CallResult<KunaOrderBook>(result.Data, result.Error);
        }

        public CallResult<List<KunaTrade>> GetTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc")
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, {"order_by",sort } };
            if (toDate != null)
            {
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new TimestampSecondsConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);

            var result = ExecuteRequest<List<KunaTrade>>(GetUrl(AllTradesEndpoint), "GET", parameters).Result;
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }
        public CallResult<KunaAccountInfo> GetAccountInfo()
        {
            var result = ExecuteRequest<KunaAccountInfo>(GetUrl(AccountInfoEndpoint), "GET", null, true).Result;
            return new CallResult<KunaAccountInfo>(result.Data, result.Error);
        }


        public CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "side", JsonConvert.SerializeObject(side,new OrderSideConverter()) },
                { "type", JsonConvert.SerializeObject(type,new OrderTypeConverter()) },
                { "volume", volume.ToString(CultureInfo.GetCultureInfo("en-US")) },
                { "market", market },
                { "price", price.ToString(CultureInfo.GetCultureInfo("en-US")) }
            };

            var result = ExecuteRequest<KunaPlacedOrder>(GetUrl(OrdersEndpoint), "POST", parameters,true).Result;
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }

        public CallResult<KunaPlacedOrder> CancelOrder(long orderId)
        {
            var parameters = new Dictionary<string, object>() { { "id", orderId } };
            var result = ExecuteRequest<KunaPlacedOrder>(GetUrl(CancelOrderEndpoint), "POST", parameters,true).Result;
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }

        public CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc")
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "state", JsonConvert.SerializeObject(orderState, new OrderStatusConverter())},
                { "order_by", sort },
                { "page", page }   
            };
           
            var result = ExecuteRequest<List<KunaPlacedOrder>>(GetUrl(OrdersEndpoint), "GET", parameters,true).Result;
            return new CallResult<List<KunaPlacedOrder>>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrder> GetOrderInfo(long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
            };
           
            var result = ExecuteRequest<KunaPlacedOrder>(GetUrl(SingleOrderEndpoint), "GET", parameters, true).Result;
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }

        public CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort="desc")
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "order_by", sort }, };
            if (toDate != null)
            {                
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new TimestampSecondsConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);
            var result = ExecuteRequest<List<KunaTrade>>(GetUrl(MyTradesEndpoint), "GET", parameters,true).Result;
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }
        #region BaseMethodOverride
        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            var uriString = uri.ToString();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(new Uri(uriString).PathAndQuery, method, parameters, signed);
            if ((method == Constants.GetMethod || method == Constants.DeleteMethod || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
            {
                uriString += "?" + parameters.CreateParamString(true,ArrayParametersSerialization.MultipleValues);
            }

            var request = RequestFactory.Create(uriString);
            request.ContentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            request.Accept = Constants.JsonContentHeader;
            request.Method = method;
            //var headers = new Dictionary<string, string>();


            if ((method == Constants.PostMethod || method == Constants.PutMethod) && postParametersPosition != PostParameters.InUri)
            {
                if (parameters?.Any() == true)
                    WriteParamBody(request, JsonConvert.SerializeObject(parameters));
                else
                    WriteParamBody(request, "{}");
            }

            return request;
        }
        
        protected Uri GetUrl(string endpoint, string version = null)
        {
            return version == null ? new Uri($"{BaseAddress}/{endpoint}") : new Uri($"{BaseAddress}/v{version}/{endpoint}");
        }

        public CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo()
        {
            string url = "https://api.kuna.io/v3/markets";
            var result = ExecuteRequest<List<KunaTraidingPair>>(new Uri(url), "GET", null, false).Result;
            return new CallResult<List<KunaTraidingPair>>(result.Data, result.Error);
        }

        public CallResult<string> Auth()
        {
            string authEndpoint = "https://api.kuna.io/v3/auth/me";
            var result = ExecuteRequest<object>(new Uri(authEndpoint), "GET", null, true).Result;
            return new CallResult<string>("",null);
        }

        public WebCallResult<KunaAccountInfo> GetAccountInfo2()
        {
            var result = ExecuteRequest<KunaAccountInfo>(GetUrl(AccountInfoEndpoint), "GET", null, true).Result;
            return new WebCallResult<KunaAccountInfo>(result.ResponseStatusCode,result.ResponseHeaders,result.Data,result.Error);
        }

        /*
         
         const apiPath = '/v3/auth/kuna_codes/issued-by-me'
const nonce = Date.now().toString()
const body = {}
let signature = `${apiPath}${nonce}${JSON.stringify(body)}`
const sig = crypto.createHmac('sha384', apiSecret).update(signature)
const shex = sig.digest('hex')
const options = {
url: `https://api.kuna.io${apiPath}`,
headers: {
'kun-nonce': nonce,
'kun-apikey': apiKey,
'kun-signature': shex
},
body: body,
json: true
}
request.post(options, (error, response, body) => {
console.log(body);
})
         */
        #endregion

    }
}
