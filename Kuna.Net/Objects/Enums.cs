﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public enum OrderSide
    {
        Buy,
        Sell
    }
    public enum OrderType
    {
        Limit,
        Market
    }
    public enum OrderState
    {
        Wait,
        Cancel,
        Done
    }
}
