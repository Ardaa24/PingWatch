using PingWatch.Services;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<PingWatch.Services.EmailService>(); // Mail servisini kaydet
builder.Services.AddHostedService<PingWorkerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles();

app.UseStaticFiles();

// Varsayılan Kullanıcıları (Admin ve Viewer) Oluşturma (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PingWatch.Data.AppDbContext>();

    // Admin hesabı yoksa oluştur
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        context.Users.Add(new PingWatch.Models.User
        {
            Username = "admin",
            PasswordHash = PingWatch.Helpers.PasswordHelper.HashPassword("admin123"),
            Role = "Admin"
        });
    }

    // Viewer (İzleyici) hesabı yoksa oluştur
    if (!context.Users.Any(u => u.Username == "viewer"))
    {
        context.Users.Add(new PingWatch.Models.User
        {
            Username = "viewer",
            PasswordHash = PingWatch.Helpers.PasswordHelper.HashPassword("viewer123"),
            Role = "Viewer" // Yetkisi sadece izleyici
        });
    }

    // Değişiklikleri veritabanına kaydet
    context.SaveChanges();
}

app.Run();