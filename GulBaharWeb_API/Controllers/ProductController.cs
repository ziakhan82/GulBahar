using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_Common_Func_Lib;
using GulBahar_Models_Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GulBaharWeb_API.Controllers
{
	[Route("api/[controller]")]

	public class ProductController : ControllerBase
	{
		private readonly IProductRepository _productRepository;
		public ProductController(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		[HttpGet]
		//[Authorize(Roles = SD.Role_Customer)]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _productRepository.GetAll());
		}

		[HttpGet("{productId}")]
		public async Task<IActionResult> Get(int? productId)
		{
			if (productId == null || productId == 0)
			{ 
				return BadRequest(new ErrorModelDTO()
				{
					ErrorMessage = "Invalid Id",
					StatusCode = StatusCodes.Status400BadRequest
				});
			}

			var product = await _productRepository.Get(productId.Value);
			if(product == null) 
			{
				return BadRequest(new ErrorModelDTO()
				{
					ErrorMessage = "Invalid Id",
					StatusCode = StatusCodes.Status404NotFound
				});
			}
			return Ok(product);
		}
	}
}
