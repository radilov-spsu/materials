namespace LegacyShop.Model
{
    public class Product : ShopEntity
    {
        public string Sku;
        public string Title;
        public decimal Price;
        public string Currency;
        public int WeightGrams;
        public Category Category;
    }
}
