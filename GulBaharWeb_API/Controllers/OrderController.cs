using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_Models_Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
	}
}
