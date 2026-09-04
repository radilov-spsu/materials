using System;
using System.Collections.Generic;
using LegacyShop.Model;

namespace LegacyShop.Services
{
    public class Warehouse
    {
        private readonly Dictionary<string, int> _stock = new Dictionary<string, int>();

        public void Receive(string sku, int quantity)
        {
            if (!_stock.ContainsKey(sku))
            {
                _stock[sku] = 0;
            }
            _stock[sku] = _stock[sku] + quantity;
        }

        public int GetStock(string sku)
        {
            int value;
            if (_stock.TryGetValue(sku, out value))
            {
                return value;
            }
            return 0;
        }

        public void Reserve(Order order)
        {
            List<OrderLine> toRemove = new List<OrderLine>();

            foreach (OrderLine line in order.Lines)
            {
                int available = GetStock(line.Product.Sku);
                if (available >= line.Quantity)
                {
                    _stock[line.Product.Sku] = available - line.Quantity;
                    continue;
                }

                if (available == 0)
                {
                    toRemove.Add(line);
                    order.Warnings.Add("Нет на складе, позиция снята: " + line.Product.Sku);
                }
                else
                {
                    order.Warnings.Add("Уменьшили количество " + line.Product.Sku + " с " +
                                       line.Quantity + " до " + available);
                    line.Quantity = available;
                    _stock[line.Product.Sku] = 0;
                }
            }

            foreach (OrderLine line in toRemove)
            {
                order.Lines.Remove(line);
            }

            decimal goods = 0;
            foreach (OrderLine line in order.Lines)
            {
                decimal price = line.Quantity * line.UnitPrice;
                if (line.Quantity > 10)
                {
                    price = price * 0.95m;
                }
                goods = goods + price;
            }
            if (order.Customer.IsVip)
            {
                goods = goods * 0.9m;
            }
            order.GoodsTotal = Math.Round(goods, 2);
            order.Total = order.GoodsTotal + order.DeliveryCost;

            if (order.Lines.Count == 0)
            {
                order.Status = "cancelled";
            }
        }
    }
}
