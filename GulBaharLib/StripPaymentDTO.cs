using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_Models_Lib
{
    public class StripPaymentDTO
    {
        public StripPaymentDTO() 
        {
            SuccessUrl = "OrderConfirmation";

            CancelUrl = "Summary";

		}
        public OrderDTO Order { get; set; }
        public string SuccessUrl { get; set; }

        public string CancelUrl { get; set; }
    }
}
