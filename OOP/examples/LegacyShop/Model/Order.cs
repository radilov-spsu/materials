using System;
using System.Collections.Generic;

namespace LegacyShop.Model
{
    public class Order : ShopEntity
    {
        public string Number;
        public Customer Customer;

        // Склад и отчёты работают с этим списком напрямую.
        public List<OrderLine> Lines = new List<OrderLine>();

        public string Status;
        public string DeliveryType;
        public string PaymentType;
        public string CouponCode;

        public decimal GoodsTotal;
        public decimal DeliveryCost;
        public decimal Total;
        public string Currency;

        public DateTime PlacedAt;
        public DateTime? PaidAt;
        public DateTime? ShippedAt;
        public DateTime? DeliveredAt;
        public DateTime ExpectedAt;

        public List<string> Warnings = new List<string>();
    }
}
