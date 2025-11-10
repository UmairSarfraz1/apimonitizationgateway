using ApiMonetizationGateway.API.Middleware;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Core.IServices;
using ApiMonetizationGateway.Infrastructure.Data;
using ApiMonetizationGateway.Infrastructure.Repository;
using ApiMonetizationGateway.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

//DI Registered
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IApiUsageLogRepository, ApiUsageLogRepository>();
builder.Services.AddScoped<IMonthlyUsageSummaryRepository, MonthlyUsageSummaryRepository>();
builder.Services.AddScoped<ITierRepository, TierRepository>();

builder.Services.AddScoped<IRateLimitingService, RateLimitingService>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RateLimitMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
