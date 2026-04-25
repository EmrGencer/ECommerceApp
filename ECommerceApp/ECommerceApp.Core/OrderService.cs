namespace ECommerceApp.Core
{
    public class OrderService
    {
        public bool PlaceOrder(Cart cart)
        {
            if (cart.Items.Count == 0) return false;

            decimal totalAmount = cart.GetTotalPrice();
            bool paymentResult = ProcessPayment(totalAmount);

            if (paymentResult)
            {
                foreach (var item in cart.Items)
                {
                    item.Stock -= 1; 
                }
                return true;
            }
            return false;
        }

        public virtual bool ProcessPayment(decimal amount)
        {
            if (amount < 0) return true; 
            return true;
        }
    }
}