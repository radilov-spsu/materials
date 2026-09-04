using System.Collections.Generic;
using LegacyShop.Model;

namespace LegacyShop.Services
{
    public class ShopFacade
    {
        private readonly ShopManager _manager;
        private readonly Warehouse _warehouse;

        public ShopFacade(ShopManager manager, Warehouse warehouse)
        {
            _manager = manager;
            _warehouse = warehouse;
        }

        public Product FindProduct(string sku)
        {
            return _manager.FindProduct(sku);
        }

        public Order FindOrder(string number)
        {
            return _manager.FindOrder(number);
        }

        public void Reserve(Order order)
        {
            _warehouse.Reserve(order);
        }

        public int GetStock(string sku)
        {
            return _warehouse.GetStock(sku);
        }

        public void MarkPaid(string number)
        {
            _manager.MarkPaid(number);
        }

        public void MarkShipped(string number)
        {
            _manager.MarkShipped(number);
        }

        public void MarkDelivered(string number)
        {
            _manager.MarkDelivered(number);
        }

        public List<Order> GetOrders()
        {
            return _manager.Orders;
        }
    }
}
