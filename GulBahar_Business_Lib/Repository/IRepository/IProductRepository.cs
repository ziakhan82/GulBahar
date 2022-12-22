using GulBahar_Models_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_Business_Lib.Repository.IRepository
{
    public interface IProductRepository
    {
        public Task<ProductDTO> Create(ProductDTO objDTO);
        public Task<ProductDTO> Update(ProductDTO objDTO);
        public Task<int> Delete(int id);
        public Task <ProductDTO> Get(int id);
        public Task<IEnumerable<ProductDTO>> GetAll();
        


    }
}
