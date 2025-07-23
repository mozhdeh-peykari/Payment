using Infrastructure.ExternalServices.IranKish.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish
{
    public interface IIranKishClient
    {
        Task<string> GetTokenAsync(TokenRequest req);

        Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req);
    }
}
