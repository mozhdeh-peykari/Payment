using Infrastructure.ExternalServices.IranKish;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Extensions
{
    public static void AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIranKishClient, IranKishClient>();
        builder.Services.AddHttpClient();
    }
}
