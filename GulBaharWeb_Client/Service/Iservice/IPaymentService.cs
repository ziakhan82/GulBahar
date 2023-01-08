using GulBahar_Models_Lib;

namespace GulBaharWeb_Client.Service.Iservice
{
	public interface IPaymentService
	{
	public Task<SuccessModelDTO> Checkout(StripPaymentDTO model);
	}
}
