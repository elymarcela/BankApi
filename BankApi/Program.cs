using BankApi.Data;
using BankApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// EF Core + SQLite
builder.Services.AddDbContext<BankDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// DI de servicios
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

// Aplicar migraciones al inicio
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BankDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

// Servir wwwroot/index.html como p�gina de inicio
app.UseDefaultFiles();   // busca index.html
app.UseStaticFiles();    // sirve archivos est�ticos

app.MapControllers();

// Cualquier ruta desconocida cae a la SPA
app.MapFallbackToFile("/index.html");

app.Run();
