using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class StockInsuficienteException : Exception
    {
        public StockInsuficienteException(string sku)
            : base($"Stock insuficiente para el producto con SKU: {sku}")
        {
        }

    }
