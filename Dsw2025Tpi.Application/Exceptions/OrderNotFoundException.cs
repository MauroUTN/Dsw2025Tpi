using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Dsw2025Tpi.Application.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(Guid orderId)
        : base($"No se encontró la orden con ID: {orderId}")
    {
    }
}
