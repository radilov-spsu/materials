using LegacyShop.Model;

namespace LegacyShop.Payments
{
    public abstract class PaymentValidator
    {
        public abstract string Validate(Order order);
    }

    public class CardPaymentValidator : PaymentValidator
    {
        public override string Validate(Order order)
        {
            if (order.Total <= 0)
            {
                return "Картой нельзя оплатить нулевой заказ";
            }
            if (order.Total > 50000)
            {
                return "Сумма выше лимита эквайринга";
            }
            return null;
        }
    }

    public class WalletPaymentValidator : PaymentValidator
    {
        public override string Validate(Order order)
        {
            if (order.Total <= 0)
            {
                return "Кошельком нельзя оплатить нулевой заказ";
            }
            if (order.Customer.CountryCode != "MD")
            {
                return "Кошелёк работает только внутри страны";
            }
            return null;
        }
    }

    public class CashPaymentValidator : PaymentValidator
    {
        public override string Validate(Order order)
        {
            if (order.DeliveryType == "pickup")
            {
                return null;
            }
            if (order.DeliveryType != "courier")
            {
                return "Наличные принимает только курьер";
            }
            return null;
        }
    }
}
