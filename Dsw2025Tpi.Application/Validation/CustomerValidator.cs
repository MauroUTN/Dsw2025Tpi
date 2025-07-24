using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Validation
{
    public static class CustomerValidator
    {
        public static void Validate(CustomerModel.RequestCustomerModel request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "El modelo de cliente no puede ser nulo");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("El nombre del cliente no puede estar vacío");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("El correo electrónico del cliente no puede estar vacío");
            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException("El número de teléfono del cliente no puede estar vacío");
        }
    }
}
