using GulBahar_Models_Lib;

namespace GulBaharWeb_Client.Service.Iservice
{
	public interface IProductService
	{
		public Task<IEnumerable<ProductDTO>> GetAll();
	    public Task<ProductDTO> Get(int ProductId);
	}
}
