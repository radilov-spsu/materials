using System;
using System.Collections.Generic;
using LegacyShop.Model;
using LegacyShop.Payments;
using LegacyShop.Services;

namespace LegacyShop
{
    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ShopManager shop = new ShopManager();
            Warehouse warehouse = new Warehouse();
            ShopFacade facade = new ShopFacade(shop, warehouse);
            ReportBuilder reports = new ReportBuilder();

            shop.AddProduct("KB-101", "Клавиатура механическая", 890m, "MDL", "Клавиатуры", "Периферия", 1100);
            shop.AddProduct("MS-204", "Мышь беспроводная", 320m, "MDL", "Мыши", "Периферия", 90);
            shop.AddProduct("CB-015", "Кабель USB-C, 2 м", 75m, "MDL", "Кабели", "Аксессуары", 60);
            shop.AddProduct("HD-800", "Наушники накладные", 1450m, "MDL", "Наушники", "Аудио", 300);

            warehouse.Receive("KB-101", 5);
            warehouse.Receive("MS-204", 3);
            warehouse.Receive("CB-015", 40);
            warehouse.Receive("HD-800", 0);

            shop.RegisterCustomer("Ион Морару", "ion@example.md", "+37377112233",
                                  "Тирасполь", "ул. Ленина", "12", "3300", "MD", false);
            shop.RegisterCustomer("Анна Кожокару", "anna@example.md", "+37368445566",
                                  "Бендеры", "ул. Суворова", "7", "3200", "MD", true);

            Console.WriteLine("--- Заказ 1: обычный покупатель, курьер, карта ---");
            Cart cart1 = new Cart();
            cart1.Add(facade.FindProduct("KB-101"), 1);
            cart1.Add(facade.FindProduct("CB-015"), 12);
            Order order1 = shop.PlaceOrder(cart1, "ion@example.md", "courier", "card", "WELCOME10");
            facade.Reserve(order1);
            Console.WriteLine(shop.PrintReceipt(order1));

            PaymentMethod payment1 = shop.CreatePaymentMethod(order1.PaymentType);
            payment1.Charge(order1);
            facade.MarkPaid(order1.Number);
            facade.MarkShipped(order1.Number);
            facade.MarkDelivered(order1.Number);
            Console.WriteLine("  Чек в личном кабинете: " + payment1.GetReceiptUrl(order1));
            Console.WriteLine();

            Console.WriteLine("--- Заказ 2: VIP, курьер, наличные, товара не хватает ---");
            Cart cart2 = new Cart();
            cart2.Add(facade.FindProduct("MS-204"), 5);
            cart2.Add(facade.FindProduct("HD-800"), 1);
            Order order2 = shop.PlaceOrder(cart2, "anna@example.md", "courier", "cash", null);
            facade.Reserve(order2);
            Console.WriteLine(shop.PrintReceipt(order2));
            foreach (string warning in order2.Warnings)
            {
                Console.WriteLine("  ! " + warning);
            }
            Console.WriteLine();

            Console.WriteLine("--- Возврат по второму заказу ---");
            PaymentMethod payment2 = shop.CreatePaymentMethod(order2.PaymentType);
            try
            {
                payment2.Refund(order2, 100m);
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine("  Не получилось: " + ex.Message);
            }
            Console.WriteLine();

            List<Order> all = facade.GetOrders();
            Console.WriteLine(reports.BuildSalesReport(all));
            Console.WriteLine(reports.BuildCategoryReport(all));

            shop.ExportOrdersToCsv("orders.csv");
            Console.WriteLine("Выгрузка сохранена в orders.csv");
        }
    }
}
