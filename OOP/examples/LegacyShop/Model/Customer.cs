namespace LegacyShop.Model
{
    public class Customer : ShopEntity
    {
        public string FullName;
        public string Email;
        public string Phone;

        public string City;
        public string Street;
        public string House;
        public string Zip;
        public string CountryCode;

        public bool IsVip;
        public decimal TotalSpent;
        public string TotalSpentCurrency;
    }
}
