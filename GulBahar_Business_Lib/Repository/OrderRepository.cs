using AutoMapper;
using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_Common_Func_Lib;
using GulBahar_DataAcess_Lib;
using GulBahar_DataAcess_Lib.Data;
using GulBahar_DataAcess_Lib.ViewModel;
using GulBahar_Models_Lib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_Business_Lib.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public OrderRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

		public async Task<OrderHeaderDTO> CancelOrder(int id)
		{
			var orderHeader= await _db.OrderHeaders.FindAsync(id);
            if(orderHeader == null)
        {
                return new OrderHeaderDTO();

            }

            if(orderHeader.Status == SD.Status_Pending)
            {
                orderHeader.Status = SD.Status_Cancelled;
                await _db.SaveChangesAsync();
            }

            if(orderHeader.Status == SD.Status_Confirmed) // need to give refund  
            {
                // refund
                var options = new Stripe.RefundCreateOptions
                {
                    Reason = Stripe.RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId // based on paymentId, stripe find the payment and refund that transction

                    
                };

                var service = new Stripe.RefundService();
                Stripe.Refund refund = service.Create(options);
                orderHeader.Status = SD.Status_Refunded;
                await _db.SaveChangesAsync();
            }

            return _mapper.Map<OrderHeader,OrderHeaderDTO>(orderHeader);
		}

		public async Task<OrderDTO> Create(OrderDTO objDTO)
        {
            try
            {
                var obj = _mapper.Map<OrderDTO, Order>(objDTO);
                // When creating order detail order header is required, first I create the order header,
                // retrieves its Id that gets created and then populate that in order detail, when creating an order.
                _db.OrderHeaders.Add(obj.OrderHeader); // obj will have the Id populated
                await _db.SaveChangesAsync();

                // loop through all the order details and popultes its order headerID
                foreach (var details in obj.OrderDetails)
                {
                    details.OrderHeaderId = obj.OrderHeader.Id; //populating the order header Id from obj 


                }
                _db.OrderDetails.AddRange(obj.OrderDetails); // add range adds collection of objects
                await _db.SaveChangesAsync();

                return new OrderDTO()
                {
                    OrderHeader = _mapper.Map<OrderHeader, OrderHeaderDTO>(obj.OrderHeader), 
                    OrderDetails = _mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailDTO>>(obj.OrderDetails).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDTO;
        }

        public async Task<int> Delete(int id)
        {
            var objHeader = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
            if (objHeader != null)
            {
                IEnumerable<OrderDetail> objDetail = _db.OrderDetails.Where(u => u.OrderHeaderId == id);

                _db.OrderDetails.RemoveRange(objDetail);
                _db.OrderHeaders.Remove(objHeader);
                return _db.SaveChanges();

            }
            return 0;

        }

        public async Task<OrderDTO> Get(int id)
        {
            Order order = new()
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id),
                OrderDetails = _db.OrderDetails.Where(u => u.OrderHeaderId == id),
            };
            if (order != null)
            {
                return _mapper.Map<Order, OrderDTO>(order);
            }
            return new OrderDTO();
        }

        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null)
        {
            List<Order> OrderfromDb = new List<Order>();
            IEnumerable<OrderHeader> OrderHeaderList = _db.OrderHeaders;
            IEnumerable<OrderDetail> OrderDetailList = _db.OrderDetails;

            foreach (OrderHeader header in OrderHeaderList)
            {
                Order order = new()
                {
                    OrderHeader = header,
                    OrderDetails = OrderDetailList.Where(u => u.OrderHeaderId == header.Id),
                };
                OrderfromDb.Add(order);
            }
            //some filtering #ToDo
            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(OrderfromDb);

        }


        public async Task<OrderHeaderDTO> MarkPaymentSuccessful(int id, string paymentId)
        {
            var data = await _db.OrderHeaders.FindAsync(id);
            if (data == null)
            {
                return new OrderHeaderDTO();
            }
            if (data.Status == SD.Status_Pending)
            {
                data.PaymentIntentId = paymentId;
                data.Status = SD.Status_Confirmed;
                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHeader, OrderHeaderDTO>(data);
            }
            return new OrderHeaderDTO();
        }

        public async Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO objDTO)
        {
            if (objDTO != null)
            {
                var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == objDTO.Id);
                orderHeaderFromDb.Name = objDTO.Name;
                orderHeaderFromDb.PhoneNumber = objDTO.PhoneNumber;
                orderHeaderFromDb.Carrier = objDTO.Carrier;
                orderHeaderFromDb.Tracking = objDTO.Tracking;
                orderHeaderFromDb.StreetAddress = objDTO.StreetAddress;
                orderHeaderFromDb.City = objDTO.City;
                orderHeaderFromDb.State = objDTO.State;
                orderHeaderFromDb.PostalCode = objDTO.PostalCode;
                orderHeaderFromDb.Status = objDTO.Status;

                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHeader, OrderHeaderDTO>(orderHeaderFromDb);
            }
            return new OrderHeaderDTO();
        }



        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            var data = await _db.OrderHeaders.FindAsync(orderId);
            if (data == null)
            {
                return false;
            }
            data.Status = status;
            if (status == SD.Status_Shipped)
            {
                data.ShippingDate = DateTime.Now;

            }
            await _db.SaveChangesAsync();
            return true;

        }
    }
}
