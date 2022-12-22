using GulBahar_Models_Lib;

namespace GulBaharWeb_Client.Service.Iservice
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderDTO>> GetAll(string? userId);
        public Task<OrderDTO> Get(int orderId);
    }
}
