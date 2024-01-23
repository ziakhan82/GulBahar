using GulBahar_Models_Lib;
using System.ComponentModel.DataAnnotations;

namespace GulBaharWeb_Client.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            ProductPrice=new();
            Count = 1;
        }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
        public int Count { get; set; }
        [Required]
        public int SelectedProductPriceId { get; set; } // based on this ID we can populate, product price and then we can get the size and price detials
        public ProductPriceDTO ProductPrice { get; set; }
    }
}
