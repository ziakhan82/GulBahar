using GulBahar_Business_Lib.Repository;
using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_DataAcess_Lib.Data;
using GulBaharWeb_Server.Data;
using GulBaharWeb_Server.Service;
using GulBaharWeb_Server.Service.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Nzc2MTU3QDMxMzkyZTMzMmUzME51TU1BcU9hejlGbDR1b040RUhiUC85MllWeC9DaDFxbTRzdlV5VUR1VHc9");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddDefaultUI() // dont want to stop users from registering and using the application 
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
builder.Services.AddScoped<IFileUpload, FileUpload>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["ApiKey"];

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

seedDatabase();
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub(); // this is where we configure our signalR connectin which power our server app, its a services that talks to the client browser, listen for changes coming
// back to the other direction.
app.MapFallbackToPage("/_Host");



app.Run();

void seedDatabase() // whenever the application starts it will check the intialize method
{
    using (var scope = app.Services.CreateScope()) //creating a scope
    {
        //Extracting the implmentation of fb initalizer
        var dbInitilazer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitilazer.Initialize();
    }
}
