using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;


namespace Dsw2025Tpi.Application.Services
{
    public class ProductManagementService : IProductsManagementService
    {
        private readonly IRepository _repository;
        public ProductManagementService(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<ProductModel.responseProductModel?> GetProductById(Guid id)
        {
            var product = await _repository.GetById<Product>(id);
            if (product == null)
                throw new EntityNotFoundException($"Product {id} no encontrado");
            return product != null ?
                new ProductModel.responseProductModel(product.Sku, product.Name, product.Description, product.InternalCode, 
                product.StockQuantity, product.CurrentUnitPrice, product.IsActive, product.Id) : null;
        }
        public async Task<IEnumerable<ProductModel.responseProductModel>?> GetAllProducts()
        {
            return (await _repository
                .GetFiltered<Product>(p => p.IsActive))?
                .Select(p => new ProductModel.responseProductModel(p.Sku, p.Name, p.Description, p.InternalCode, p.StockQuantity,
            p.CurrentUnitPrice, p.IsActive, p.Id));
        }
        public async Task<ProductModel.responseProductModel> AddProduct(ProductModel.requestProductModel request)
        {
            //validacion
            var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
            if (exist != null)
                throw new DuplicatedEntityException($"El producto con Sku {request.Sku} ya existe");
            var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description,
                request.CurrentUnitPrice, request.StockQuantity, request.IsActive);
            await _repository.Add(product);
            return new ProductModel.responseProductModel(product.Sku, product.Name, product.Description, product.InternalCode,
                product.StockQuantity, product.CurrentUnitPrice, product.IsActive, product.Id);
        }
        public async Task<ProductModel.responseProductModel> UpdateProduct(Guid Id , ProductModel.requestProductModel request)
        {
            var product = await _repository.GetById<Product>(Id);
            if (product == null)
                throw new EntityNotFoundException($"Producto con Sku {request.Sku} no encontrado");
            //validacion
            product.Sku = request.Sku;
            product.InternalCode = request.InternalCode;
            product.Name = request.Name;
            product.Description = request.Description;
            product.CurrentUnitPrice = request.CurrentUnitPrice;
            product.StockQuantity = request.StockQuantity;
            product.IsActive = request.IsActive;
            await _repository.Update(product);
            return new ProductModel.responseProductModel(product.Sku, product.Name, product.Description, product.InternalCode,
                product.StockQuantity, product.CurrentUnitPrice, product.IsActive, product.Id);
        }
        public async Task<ProductModel.responseProductModel> PatchProduct(Guid id)
        {
            var product = await _repository.GetById<Product>(id);
            if (product == null)
                throw new EntityNotFoundException($"Producto no encontrado");

            product.IsActive = false;
            await _repository.Update(product);
            var active = await _repository.GetById<Product>(id);
            return new ProductModel.responseProductModel(product.Sku, product.Name, product.Description, product.InternalCode,
                product.StockQuantity, product.CurrentUnitPrice, product.IsActive, product.Id);
        }
    }
}
