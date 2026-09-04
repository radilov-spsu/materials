using System;

namespace LegacyShop.Services
{
    public class OrderIdFactory
    {
        public string Create()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}
