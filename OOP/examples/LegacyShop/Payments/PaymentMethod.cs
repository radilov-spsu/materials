using System;
using LegacyShop.Model;

namespace LegacyShop.Payments
{
    public abstract class PaymentMethod
    {
        public string Code;

        public abstract decimal Charge(Order order);
        public abstract void Refund(Order order, decimal amount);
        public abstract string GetReceiptUrl(Order order);
        public abstract bool SupportsInstallments();
    }

    public class CardPayment : PaymentMethod
    {
        private readonly string _acquirerUrl;

        public CardPayment(string acquirerUrl)
        {
            Code = "card";
            _acquirerUrl = acquirerUrl;
        }

        public override decimal Charge(Order order)
        {
            Console.WriteLine("  [acquirer] списываем " + order.Total + " " + order.Currency +
                              " через " + _acquirerUrl);
            return order.Total;
        }

        public override void Refund(Order order, decimal amount)
        {
            Console.WriteLine("  [acquirer] возврат " + amount + " " + order.Currency);
        }

        public override string GetReceiptUrl(Order order)
        {
            return _acquirerUrl + "/receipts/" + order.Number;
        }

        public override bool SupportsInstallments()
        {
            return true;
        }
    }

    public class WalletPayment : PaymentMethod
    {
        public WalletPayment()
        {
            Code = "wallet";
        }

        public override decimal Charge(Order order)
        {
            Console.WriteLine("  [wallet] списываем " + order.Total + " " + order.Currency);
            return order.Total;
        }

        public override void Refund(Order order, decimal amount)
        {
            Console.WriteLine("  [wallet] возврат " + amount + " " + order.Currency);
        }

        public override string GetReceiptUrl(Order order)
        {
            return "https://wallet.example/r/" + order.Number;
        }

        public override bool SupportsInstallments()
        {
            return false;
        }
    }

    public class CashPayment : PaymentMethod
    {
        public CashPayment()
        {
            Code = "cash";
        }

        public override decimal Charge(Order order)
        {
            Console.WriteLine("  [cash] курьер получит " + order.Total + " " + order.Currency);
            return order.Total;
        }

        public override void Refund(Order order, decimal amount)
        {
            throw new NotSupportedException("Наличные возвращает бухгалтерия вручную");
        }

        public override string GetReceiptUrl(Order order)
        {
            throw new NotSupportedException("У наличной оплаты нет электронного чека");
        }

        public override bool SupportsInstallments()
        {
            throw new NotSupportedException("Рассрочка наличными невозможна");
        }
    }
}
