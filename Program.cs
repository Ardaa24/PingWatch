using PingWatch.Extensions;
using PingWatch.Infrastructure.Data;
using PingWatch.Infrastructure.Data.Seed;
using PingWatch.Core.Interfaces.Services;
using PingWatch.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Servis Kayıtları ─────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Database Seed ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DatabaseSeeder.SeedAsync(context, hasher);
}

app.Run();