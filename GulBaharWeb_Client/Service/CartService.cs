using Blazored.LocalStorage;
using GulBahar_Common_Func_Lib;
using GulBaharWeb_Client.ViewModels;
using GulBaharWeb_Client.Service.Iservice;

namespace GulBaharWeb_Client.Service
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        public event Action OnChange;
        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task incrementCart(ShoppingCart cartToAdd)
        {
            var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
            bool itemInCart = false; // if the item that user wants to add already exists in the cart, and by delful =f

            if (cart == null)
            {
                // if the cart is empty a new list of shopping cart will be assigned which will be empty
                cart = new List<ShoppingCart>();
            }
            // tp check if the item already exists, if the item exists in the SC, we increment the count, rather than
            // adding a duplicate record
            foreach (var item in cart)
            {
                // if both are equal means that rcord already exists in the shopping cart
                if (item.ProductId == cartToAdd.ProductId && item.ProductPriceId == cartToAdd.ProductPriceId)
                {
                    itemInCart = true;
                    item.Count += cartToAdd.Count; // if the item already exists in the shooping cart
                }
            }// but if we calling for the first time that will not exists
            if (!itemInCart) // if the item is not in the shooping cart, then we add that to the cart object
            {
                cart.Add(new ShoppingCart()
                {
                    ProductId = cartToAdd.ProductId,
                    ProductPriceId = cartToAdd.ProductPriceId,
                    Count = cartToAdd.Count
                });
            }
            // final cart variable has the updated record, so I set the local storage with the new card
            // when setting the item we need two things the key name and the value that will be stored in LS
            await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
            OnChange.Invoke();

        }
    
        public async Task DerementCart(ShoppingCart CartToDecrement)
        {
            // when decreming we will again get all the items in the cart variable, we dont need boolean flag
            // becuase the item should be in the shopping cart
            var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
            // if the count is 0 or 1 remove the item

            for (int i = 0; i < cart.Count; i++)
            {
                // if both are eqaul we found the object that needs to be decrement or remove
                if (cart[i].ProductId == CartToDecrement.ProductId && cart[i].ProductPriceId == CartToDecrement.ProductPriceId)
                {
                    if (cart[i].Count == 1 || CartToDecrement.Count == 0)
                    {
                        // on cart we remove the current cart item
                        cart.Remove(cart[i]);
                    }
                    else
                    {
                        // if not we decrement,by the count that is set in the cartToDecrment
                        cart[i].Count -= CartToDecrement.Count;

                    }
                }
            }
            await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
            OnChange.Invoke();
        }
    }
}
