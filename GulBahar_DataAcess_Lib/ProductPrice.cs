using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_DataAcess_Lib
{
	public class ProductPrice
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		Product product { get; set; }
		public string Size { get; set; }
		public double Price { get; set; }

	}
}
