using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record RegisterModel
    {
        public record RequestRegisterModel(string Name, string Email, string Password, string ConfirmPassword);
        public record ResponseRegisterModel(Guid Id, string Name, string Email, DateTime CreatedAt);
    }
}
