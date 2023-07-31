namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public string Username { get; set; }
        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;
                foreach (var item in Items)
                {
                    totalprice += item.Price * item.Quantity;
                }
                return totalprice;
            }
        }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public ShoppingCart() { }
        public ShoppingCart(string username)
        {
            Username = username;
        }
    }
}
