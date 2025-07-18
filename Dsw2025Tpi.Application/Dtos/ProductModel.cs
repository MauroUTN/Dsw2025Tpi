using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductModel
    {
        public record requestProductModel (String Sku, String Name, String Description, String InternalCode, int StockQuantity,
            decimal CurrentPrice,bool isActive);
        public record responseProductModel(String Sku, String Name, String Description, String InternalCode, int StockQuantity,
            decimal CurrentPrice, bool isActive,Guid Id);
    }
}
