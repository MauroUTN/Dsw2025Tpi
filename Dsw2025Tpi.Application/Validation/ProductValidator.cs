using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Validation
{
    public static class ProductValidator
    {
        public static void Validate(ProductModel.requestProductModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku))
                throw new ArgumentException("El Sku del producto no puede estar vacío");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("El nombre del producto no puede estar vacío");
            if (request.CurrentUnitPrice <= 0)
                throw new ArgumentException("El precio unitario del producto debe ser mayor a cero");
            if (request.StockQuantity < 0)
                throw new ArgumentException("La cantidad de stock del producto no puede ser negativa");
        }
    }
}
