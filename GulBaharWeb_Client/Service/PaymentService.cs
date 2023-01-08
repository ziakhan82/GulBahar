using GulBahar_Models_Lib;
using GulBaharWeb_Client.Service.Iservice;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace GulBaharWeb_Client.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly HttpClient _httpClient;
		public PaymentService(HttpClient httpClient)
		{
			_httpClient= httpClient;
		}

		public async Task<SuccessModelDTO> Checkout(StripPaymentDTO model)
		{
			try
			{

				var content = JsonConvert.SerializeObject(model);
				var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/stripepayment/create", bodyContent); // i will pass the stripe payment as the body conetnet
				string responseResult = response.Content.ReadAsStringAsync().Result;
				if (response.IsSuccessStatusCode)
				{
					var result = JsonConvert.DeserializeObject<SuccessModelDTO>(responseResult);
					return result;
				}
				else
				{
					var errorModel = JsonConvert.DeserializeObject<ErrorModelDTO>(responseResult);
					throw new Exception(errorModel.ErrorMessage);
				}
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
			
		}
	}

		
	
	
}
