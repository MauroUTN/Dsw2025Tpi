using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record CustomerModel
    {
        public record RequestCustomerModel(string Name, string Email, string Phone);

        public record ResponseCustomerModel(Guid Id, string Name, string Email, string Phone);

    }
}
