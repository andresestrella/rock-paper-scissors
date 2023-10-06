using rps_server.Data;
using rps_server.Hubs;
using rps_server.Repository;
using rps_server.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*", "https://example.com", "localhost")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseRouting();
// app.UseAuthorization();

// UseCors must be called before MapHub.
app.UseCors();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapHub<GameHub>("/gameHub");

app.Run();
