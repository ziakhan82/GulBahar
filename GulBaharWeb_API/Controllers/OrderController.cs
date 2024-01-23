using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_Models_Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace GulBaharWeb_API.Controllers
{
	[Route("api/[controller]/[action]")]

	public class OrderController : ControllerBase
	{
		private readonly IOrderRepository _orderRepository;
		public OrderController(IOrderRepository orderRepository)
		{
            _orderRepository = orderRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _orderRepository.GetAll());
		}

		[HttpGet("{orderHeaderId}")]
		public async Task<IActionResult> Get(int? orderHeaderId)
		{
			if (orderHeaderId == null || orderHeaderId == 0)
			{
				return BadRequest(new ErrorModelDTO()
				{
					ErrorMessage = "Invalid Id",
					StatusCode = StatusCodes.Status400BadRequest
				});
			}

			var OrderHeader = await _orderRepository.Get(orderHeaderId.Value);
			if(OrderHeader == null) 
			{
				return BadRequest(new ErrorModelDTO()
				{
					ErrorMessage = "Invalid Id",
					StatusCode = StatusCodes.Status404NotFound
				});
			}
			return Ok(OrderHeader);
		}

		[HttpPost]
		[ActionName("Create")]

		public async Task<IActionResult> Create([FromBody] StripPaymentDTO paymentDTO)
		{
			paymentDTO.Order.OrderHeader.OrderDate= DateTime.Now;
			var result = await _orderRepository.Create(paymentDTO.Order);
			return Ok(result);
		}

        [HttpPost]
        [ActionName("paymentsuccessful")]

        public async Task<IActionResult> PaymentSuccessful([FromBody] OrderHeaderDTO orderHeaderDTO)
        {
            var service = new SessionService(); // creating new session
            var sessionDetails = service.Get(orderHeaderDTO.SessionId); // on service get session details
            if (sessionDetails.PaymentStatus == "paid") // its hard coded as paid it will always match that as long as its successful 
            {
                var result = await _orderRepository.MarkPaymentSuccessful(orderHeaderDTO.Id,
					sessionDetails.PaymentIntentId);

                if (result==null)
                {
                    return BadRequest(new ErrorModelDTO()
                    {
                        ErrorMessage = "Can not mark payment as successful"
                    });
                }
                return Ok(result);
            }

            return BadRequest();
        }
    }
}
