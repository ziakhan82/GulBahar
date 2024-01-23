using GulBahar_Models_Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace GulBaharWeb_API.Controllers
{
	
    [Route("api/[controller]/[action]")]
    public class StripepaymentController : Controller
	{
		private readonly IConfiguration _configuration;
		public StripepaymentController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		[Authorize]
		[ActionName("create")]
		public async Task<IActionResult> Create([FromBody] StripPaymentDTO paymentDTO)
		{
			try
			{

				var domain = _configuration.GetValue<string>("Gul-Bahar_URl");

				 var options = new SessionCreateOptions
				{
					SuccessUrl = domain+paymentDTO.SuccessUrl,
					CancelUrl = domain+paymentDTO.CancelUrl,
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					PaymentMethodTypes = new List<string> { "card" }
				};

				// building line items for all the products in order details
				foreach (var item in paymentDTO.Order.OrderDetails) // loop through all order details
				{
					var sessionLineItem = new SessionLineItemOptions // indivula session line item
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100), //20.00 -> 2000
							Currency = "USD",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Name
							}
						},
						Quantity = item.Count // based on the price an quatity it will automaically multiply them to get order total
					};
					options.LineItems.Add(sessionLineItem); // adding indiviual line items to options
				}
				//calling strip and creating strip session
				var service = new SessionService(); // creating new session service
				Session session = service.Create(options); // creating options, and we get back session object which has sessionId
				return Ok(new SuccessModelDTO()
				{
					Data = session.Id /*+ ";" + session.PaymentIntentId*/
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new ErrorModelDTO()
				{
					ErrorMessage = ex.Message
				});
			}
		}
	}
}
