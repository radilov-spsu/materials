using System.Collections.Generic;

namespace LegacyShop.Model
{
    public class Cart
    {
        public List<OrderLine> Items = new List<OrderLine>();

        public void Add(Product product, int quantity)
        {
            Items.Add(new OrderLine
            {
                Product = product,
                Quantity = quantity,
                UnitPrice = product.Price,
                UnitPriceCurrency = product.Currency
            });
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach (OrderLine line in Items)
            {
                decimal price = line.Quantity * line.UnitPrice;
                if (line.Quantity > 10)
                {
                    price = price * 0.95m;
                }
                total = total + price;
            }
            return total;
        }

        public int GetWeightGrams()
        {
            int weight = 0;
            foreach (OrderLine line in Items)
            {
                weight = weight + line.Product.WeightGrams * line.Quantity;
            }
            return weight;
        }
    }
}
