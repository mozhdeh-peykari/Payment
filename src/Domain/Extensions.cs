using Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class Extensions
{
    public static void AddDomainServices(this WebApplicationBuilder builder)
    {
        //builder.Services.Configure<PaymentServiceSettings>(builder.Configuration.GetSection("IranKish"));
    }
}

