using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem(decimal unitPrice , int quantity )
        {
            UnitPrice = unitPrice;
            Quantity = quantity;

        }
        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice; set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El precio debe ser mayor a cero.");
                }
                else
                {
                    _unitPrice = value;
                }
            }
        }
        private int _quantity;
        public int Quantity { get => _quantity ; set {
                if (value < 0)
                {
                    throw new ArgumentException("La cantidad no debe ser negativa.");
                }
                else
                {
                    _quantity = value;
                }
            } }
        public decimal Subtotal => UnitPrice * Quantity;

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;

    }

}
