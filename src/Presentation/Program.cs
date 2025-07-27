using Application.Extensions;
using Domain;
using Domain.Settings;
using Infrastructure;
using Persistence;
using Presentation.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PaymentServiceSettings>(builder.Configuration.GetSection("IranKish"));
builder.AddInfrastructureServices();
builder.AddPersistenceServices();
builder.AddApplicationServices();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ErrorHandleFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Payment}/{action=Index}/{id?}");

app.Run();
