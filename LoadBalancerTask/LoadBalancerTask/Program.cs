using FluentValidation.AspNetCore;
using LoadBalancer.DAL.Persistence;
using LoadBalancerAPI.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomServices();

var rabbitHostName = Environment.GetEnvironmentVariable("Redis_HOSTNAME") ?? "host.docker.internal";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = rabbitHostName;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var redis = ConnectionMultiplexer.Connect(rabbitHostName);
builder.Services.AddDataProtection()
  .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

builder.Services.AddControllers().AddFluentValidation();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect($"{rabbitHostName},abortConnect=false");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BalancerDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStr")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options
    .WithOrigins(new[] { "http://localhost:3000", " https://localhost:7227", " http://localhost:8082", "http://localhost:8080", "http://localhost:4200", "http://localhost:5000", "http://localhost:8085", "http://localhost:6379" })
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = "text/plain";

        // Host info
        var name = Dns.GetHostName(); // get container id
        var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        Console.WriteLine($"Host Name: {Environment.MachineName} \t {name}\t {ip}");
        await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
        await context.Response.WriteAsync(Environment.NewLine);

        // Request method, scheme, and path
        await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

        // Headers
        await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
        foreach (var (key, value) in context.Request.Headers)
        {
            await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
        }
        await context.Response.WriteAsync(Environment.NewLine);

        // Connection: RemoteIp
        await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
    });
});

app.Run();
