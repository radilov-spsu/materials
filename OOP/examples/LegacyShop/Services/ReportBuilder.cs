using System;
using System.Collections.Generic;
using System.Text;
using LegacyShop.Model;

namespace LegacyShop.Services
{
    public class ReportBuilder
    {
        public string BuildSalesReport(List<Order> orders)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ОТЧЁТ ПО ПРОДАЖАМ ===");

            decimal grandTotal = 0;
            foreach (Order order in orders)
            {
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

                sb.AppendLine(order.Number + "  " + FormatCustomer(order.Customer) +
                              "  " + Math.Round(goods, 2) + " " + order.Currency +
                              "  [" + order.Status + "]");
                grandTotal = grandTotal + Math.Round(goods, 2);
            }

            sb.AppendLine("Всего: " + grandTotal + " MDL");
            return sb.ToString();
        }

        public string FormatCustomer(Customer customer)
        {
            string title = customer.IsVip ? "VIP " : "";
            string address = customer.Zip + ", " + customer.City + ", " +
                             customer.Street + " " + customer.House;
            string contacts = customer.Email;
            if (!string.IsNullOrEmpty(customer.Phone))
            {
                contacts = contacts + " / " + customer.Phone;
            }
            return title + customer.FullName + " (" + contacts + "; " + address + ")";
        }

        public string BuildCategoryReport(List<Order> orders)
        {
            Dictionary<string, decimal> byRootCategory = new Dictionary<string, decimal>();

            foreach (Order order in orders)
            {
                foreach (OrderLine line in order.Lines)
                {
                    string root = line.Product.Category.Parent.Name.ToUpper();
                    if (!byRootCategory.ContainsKey(root))
                    {
                        byRootCategory[root] = 0;
                    }
                    byRootCategory[root] = byRootCategory[root] + line.Quantity * line.UnitPrice;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ПРОДАЖИ ПО КАТЕГОРИЯМ ===");
            foreach (KeyValuePair<string, decimal> pair in byRootCategory)
            {
                sb.AppendLine("  " + pair.Key.PadRight(16) + pair.Value + " MDL");
            }
            return sb.ToString();
        }
    }
}
