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

// Varsayılan Admin Hesabı Oluşturma (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PingWatch.Data.AppDbContext>();

    // Eğer Users tablosunda hiç kayıt yoksa varsayılan admini ekle
    if (!context.Users.Any())
    {
        context.Users.Add(new PingWatch.Models.User
        {
            Username = "admin",
            PasswordHash = PingWatch.Helpers.PasswordHelper.HashPassword("admin123"),
            Role = "Admin"
        });
        context.SaveChanges();      
    }
}

app.Run();