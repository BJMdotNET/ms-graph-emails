﻿using System.Net.Http.Headers;

namespace MsGraphEmailsFramework.Common
{
    public static class UserAgentHeader
    {
        private const string Name = "CustomUserAgent";

        public static readonly ProductInfoHeaderValue Custom = new ProductInfoHeaderValue(Name, "1.0");
    }
}