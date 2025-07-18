using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {
        public Product(string sku , string internalCode , string name , string description , decimal currentUnitPrice , int stockQuantity , bool isActive)
        {
            Sku = sku;
            InternalCode = internalCode;
            Name = name;
            Description = description;
            CurrentUnitPrice = currentUnitPrice;
            StockQuantity = stockQuantity;
            IsActive = isActive;
        }
        public string Sku { get; set; }
        public string InternalCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        private decimal _currentUnitPrice;
        public decimal CurrentUnitPrice
        {
            get => _currentUnitPrice; set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El precio debe ser mayor a cero.");
                }
                else
                {
                    _currentUnitPrice = value;
                }
            }
        }
        private int _stockQuantity;
        public int StockQuantity
        {
            get => _stockQuantity; set
            {
                if (value < 0)
                {
                    throw new ArgumentException("La cantidad de stock no debe ser negativa.");
                }
                else
                {
                    _stockQuantity = value;
                }
            }
        }
        public bool IsActive { get; set; } = true;
        //Relaciones
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
