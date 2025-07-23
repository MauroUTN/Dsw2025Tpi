using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IProductsManagementService
    {
        Task<ProductModel.responseProductModel?> GetProductById(Guid id);
        Task<IEnumerable<ProductModel.responseProductModel>?> GetAllProducts();
        Task<ProductModel.responseProductModel> AddProduct(ProductModel.requestProductModel request);
        Task<ProductModel.responseProductModel> UpdateProduct(Guid id, ProductModel.requestProductModel request);
        Task<ProductModel.responseProductModel> PatchProduct(Guid id);
    }
}
