using GulBaharWeb_Client.ViewModels;

namespace GulBaharWeb_Client.Service.Iservice
{
    public interface ICartService
    {
        public event Action OnChange;
        Task DerementCart(ShoppingCart shoppingCart);
        Task incrementCart(ShoppingCart shoppingCart);
    }
}
