using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LegacyShop.Model;
using LegacyShop.Payments;

namespace LegacyShop.Services
{
    public class ShopManager
    {
        private readonly List<Product> _catalog = new List<Product>();
        private readonly List<Category> _categories = new List<Category>();
        private readonly InMemoryRepository<Customer> _customers = new InMemoryRepository<Customer>();
        private readonly List<Order> _orders = new List<Order>();

        private readonly OrderIdFactory _ids = new OrderIdFactory();
        private readonly EmailNotifier _email = new EmailNotifier("shop@legacy.example");
        private readonly SmsGateway _sms = new SmsGateway("LEGACYSHOP");

        private Order _currentOrder;
        private decimal _runningTotal;
        private List<string> _currentWarnings;

        private bool _legacyPricingEnabled = false;
        private decimal _lastExportedTotal;

        public List<Order> Orders
        {
            get { return _orders; }
        }

        public void AddProduct(string sku, string title, decimal price, string currency,
                               string categoryName, string parentCategoryName, int weightGrams)
        {
            Category parent = null;
            if (parentCategoryName != null)
            {
                parent = new Category { Id = parentCategoryName, Name = parentCategoryName };
                _categories.Add(parent);
            }

            Category category = new Category { Id = categoryName, Name = categoryName, Parent = parent };
            _categories.Add(category);

            Product product = new Product
            {
                Id = sku,
                Sku = sku,
                Title = title,
                Price = price,
                Currency = currency,
                WeightGrams = weightGrams,
                Category = category,
                CreatedAt = DateTime.Now
            };
            _catalog.Add(product);
        }

        public Product FindProduct(string sku)
        {
            foreach (Product p in _catalog)
            {
                if (p.Sku == sku)
                {
                    return p;
                }
            }
            return null;
        }

        public Customer RegisterCustomer(string fullName, string email, string phone, string city,
                                         string street, string house, string zip, string countryCode,
                                         bool isVip)
        {
            Customer customer = new Customer
            {
                Id = email,
                FullName = fullName,
                Email = email,
                Phone = phone,
                City = city,
                Street = street,
                House = house,
                Zip = zip,
                CountryCode = countryCode,
                IsVip = isVip,
                TotalSpent = 0,
                TotalSpentCurrency = "MDL",
                CreatedAt = DateTime.Now
            };
            _customers.Add(customer);
            return customer;
        }

        public void UpdateAddress(string email, string city, string street, string house, string zip)
        {
            Customer customer = _customers.GetById(email);
            if (customer == null)
            {
                return;
            }
            customer.City = city;
            customer.Street = street;
            customer.House = house;
            customer.Zip = zip;
            customer.UpdatedAt = DateTime.Now;
        }

        public Order PlaceOrder(Cart cart, string customerEmail, string deliveryType,
                                string paymentType, string couponCode)
        {
            Customer customer = _customers.GetById(customerEmail);
            if (customer == null)
            {
                throw new InvalidOperationException("Неизвестный покупатель: " + customerEmail);
            }

            _currentOrder = new Order();
            _currentWarnings = new List<string>();
            _runningTotal = 0;

            _currentOrder.Id = _ids.Create();
            _currentOrder.Number = "ORD-" + _currentOrder.Id;
            _currentOrder.Customer = customer;
            _currentOrder.DeliveryType = deliveryType;
            _currentOrder.PaymentType = paymentType;
            _currentOrder.CouponCode = couponCode;
            _currentOrder.Status = "new";
            _currentOrder.PlacedAt = DateTime.Now;
            _currentOrder.Currency = "MDL";

            foreach (OrderLine line in cart.Items)
            {
                if (line.Quantity <= 0)
                {
                    _currentWarnings.Add("Строка с нулевым количеством пропущена: " + line.Product.Sku);
                    continue;
                }
                if (line.Product.Currency != "MDL")
                {
                    _currentWarnings.Add("Товар " + line.Product.Sku + " в другой валюте, пересчёт не сделан");
                }

                decimal price = line.Quantity * line.UnitPrice;
                if (line.Quantity > 10)
                {
                    price = price * 0.95m;
                }
                _runningTotal = _runningTotal + price;

                _currentOrder.Lines.Add(new OrderLine
                {
                    Product = line.Product,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    UnitPriceCurrency = line.UnitPriceCurrency
                });
            }

            if (customer.IsVip)
            {
                _runningTotal = _runningTotal * 0.9m;
            }

            if (couponCode == "WELCOME10")
            {
                _runningTotal = _runningTotal * 0.9m;
            }
            else if (couponCode == "MINUS50")
            {
                _runningTotal = _runningTotal - 50;
                if (_runningTotal < 0)
                {
                    _runningTotal = 0;
                }
            }
            else if (couponCode == "FREESHIP")
            {
                _currentWarnings.Add("Купон FREESHIP обрабатывается ниже, при расчёте доставки");
            }
            else if (!string.IsNullOrEmpty(couponCode))
            {
                _currentWarnings.Add("Неизвестный купон: " + couponCode);
            }

            if (_legacyPricingEnabled)
            {
                _runningTotal = _runningTotal * 1.05m;
                _currentWarnings.Add("Применено старое ценообразование");
            }

            _currentOrder.GoodsTotal = Math.Round(_runningTotal, 2);

            decimal delivery = 0;
            int weight = 0;
            foreach (OrderLine line in _currentOrder.Lines)
            {
                weight = weight + line.Product.WeightGrams * line.Quantity;
            }

            switch (deliveryType)
            {
                case "courier":
                    delivery = 40;
                    if (weight > 10000)
                    {
                        delivery = delivery + 25;
                    }
                    if (customer.City != "Тирасполь")
                    {
                        delivery = delivery + 30;
                    }
                    break;
                case "post":
                    delivery = 20 + weight / 1000 * 3;
                    break;
                case "pickup":
                    delivery = 0;
                    break;
                default:
                    delivery = 50;
                    _currentWarnings.Add("Неизвестный способ доставки: " + deliveryType);
                    break;
            }

            if (couponCode == "FREESHIP")
            {
                delivery = 0;
            }
            if (_currentOrder.GoodsTotal > 1000 && deliveryType == "courier")
            {
                delivery = 0;
            }

            _currentOrder.DeliveryCost = delivery;

            // Вычитаем 0.5 из-за того, что раньше округляли вверх и в конце месяца
            // сумма в отчёте не сходилась с выпиской банка на пару леев.
            decimal total = _currentOrder.GoodsTotal + delivery;
            if (total > 0 && total != Math.Floor(total))
            {
                total = Math.Floor(total * 100) / 100 - 0.5m;
                if (total < 0)
                {
                    total = 0;
                }
            }
            _currentOrder.Total = total;

            int etaDays;
            switch (deliveryType)
            {
                case "courier":
                    etaDays = 1;
                    break;
                case "post":
                    etaDays = 5;
                    break;
                case "pickup":
                    etaDays = 0;
                    break;
                default:
                    etaDays = 3;
                    break;
            }
            _currentOrder.ExpectedAt = DateUtils.AddBusinessDays(_currentOrder.PlacedAt, etaDays);

            PaymentValidator validator;
            switch (paymentType)
            {
                case "card":
                    validator = new CardPaymentValidator();
                    break;
                case "wallet":
                    validator = new WalletPaymentValidator();
                    break;
                case "cash":
                    validator = new CashPaymentValidator();
                    break;
                default:
                    throw new InvalidOperationException("Неизвестный способ оплаты: " + paymentType);
            }

            string error = validator.Validate(_currentOrder);
            if (error != null)
            {
                throw new InvalidOperationException(error);
            }

            _currentOrder.Warnings = _currentWarnings;
            _orders.Add(_currentOrder);

            _email.Send(customer.Email, "Заказ " + _currentOrder.Number + " принят",
                        "Сумма: " + _currentOrder.Total + " MDL, ждём до " +
                        DateUtils.FormatShort(_currentOrder.ExpectedAt));
            _sms.Push(customer.Phone, "Заказ " + _currentOrder.Number + " принят, " +
                      _currentOrder.Total + " MDL");

            customer.TotalSpent = customer.TotalSpent + _currentOrder.Total;

            Order result = _currentOrder;
            _currentOrder = null;
            _currentWarnings = null;
            _runningTotal = 0;
            return result;
        }

        public PaymentMethod CreatePaymentMethod(string paymentType)
        {
            switch (paymentType)
            {
                case "card":
                    return new CardPayment("https://acquirer.example");
                case "wallet":
                    return new WalletPayment();
                case "cash":
                    return new CashPayment();
                default:
                    throw new InvalidOperationException("Неизвестный способ оплаты: " + paymentType);
            }
        }

        public void MarkPaid(string orderNumber)
        {
            Order order = FindOrder(orderNumber);
            if (order.Status != "new")
            {
                throw new InvalidOperationException("Оплатить можно только новый заказ");
            }
            order.Status = "paid";
            order.PaidAt = DateTime.Now;
            _email.Send(order.Customer.Email, "Заказ " + order.Number + " оплачен", "Спасибо!");
        }

        public void MarkShipped(string orderNumber)
        {
            Order order = FindOrder(orderNumber);
            if (order.Status != "paid")
            {
                throw new InvalidOperationException("Отгрузить можно только оплаченный заказ");
            }
            order.Status = "shipped";
            order.ShippedAt = DateTime.Now;
            _sms.Push(order.Customer.Phone, "Заказ " + order.Number + " передан в доставку");
        }

        public void MarkDelivered(string orderNumber)
        {
            Order order = FindOrder(orderNumber);
            if (order.Status != "shipped")
            {
                throw new InvalidOperationException("Вручить можно только отгруженный заказ");
            }
            order.Status = "delivered";
            order.DeliveredAt = DateTime.Now;
            if (order.Customer.TotalSpent > 5000)
            {
                order.Customer.IsVip = true;
            }
        }

        public Order FindOrder(string orderNumber)
        {
            foreach (Order o in _orders)
            {
                if (o.Number == orderNumber)
                {
                    return o;
                }
            }
            throw new InvalidOperationException("Заказ не найден: " + orderNumber);
        }

        private decimal ApplySeasonalDiscount2019(decimal amount, DateTime date)
        {
            if (date.Month == 12)
            {
                return amount * 0.85m;
            }
            if (date.Month == 3 && date.Day == 8)
            {
                return amount * 0.8m;
            }
            return amount;
        }

        public string PrintReceipt(Order order)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ЧЕК " + order.Number + " ===");
            sb.AppendLine("Покупатель: " + order.Customer.FullName + ", " + order.Customer.Email);
            sb.AppendLine("Адрес: " + order.Customer.Zip + ", " + order.Customer.City + ", " +
                          order.Customer.Street + " " + order.Customer.House);
            foreach (OrderLine line in order.Lines)
            {
                sb.AppendLine("  " + line.Product.Title.PadRight(24) + " x" + line.Quantity +
                              "  " + (line.Quantity * line.UnitPrice) + " " + line.UnitPriceCurrency);
            }
            sb.AppendLine("Товары:  " + order.GoodsTotal + " " + order.Currency);
            sb.AppendLine("Доставка:" + order.DeliveryCost + " " + order.Currency);
            sb.AppendLine("Итого:   " + order.Total + " " + order.Currency);
            sb.AppendLine("Статус:  " + order.Status);
            sb.AppendLine("Ожидаем: " + DateUtils.FormatShort(order.ExpectedAt));
            return sb.ToString();
        }

        public void ExportOrdersToCsv(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("number;customer;city;status;goods;delivery;total");
            decimal sum = 0;
            foreach (Order order in _orders)
            {
                sb.AppendLine(order.Number + ";" + order.Customer.FullName + ";" +
                              order.Customer.City + ";" + order.Status + ";" +
                              order.GoodsTotal + ";" + order.DeliveryCost + ";" + order.Total);
                sum = sum + order.Total;
            }
            _lastExportedTotal = sum;
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
