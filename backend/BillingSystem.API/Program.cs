using BillingSystem.Application;
using BillingSystem.Domain;
using BillingSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var pricingRules = builder.Configuration.GetSection("Billing:PricingRules").Get<List<PricingRule>>() ?? new List<PricingRule>();

builder.Services.AddSingleton(new PricingConfiguration(pricingRules));
builder.Services.AddSingleton<IUsageStore, InMemoryUsageStore>();
builder.Services.AddSingleton<BillingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("frontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
