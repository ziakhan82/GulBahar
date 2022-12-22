using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBaharLib.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
		public double Price { get; set; }
		public bool IsActive { get; set; }
        public List<ProductProp> ProductProperties { get; set; }
    }
}
