using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SignalRTemplate.Authorization;
using SignalRTemplate.Shared.Extensions;
using SignalRTemplate.Shared.Injectables;
using SignalRTemplate.SignalR;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication().AddJwtBearer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiServer API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "Bearer",
        BearerFormat = "JWT",
        Description = "Please enter authorization key",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                         Reference = new OpenApiReference()
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                    },
                    Enumerable.Empty<string>().ToList()
                }
            });
    c.CustomSchemaIds(x => x.FullName?.Replace("+", "."));
});

builder.Services.AddSignalR().AddJsonProtocol(option =>
{
    option.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthorizationHandler, ChannelHubAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChannelHubAuthorizationPolicy", policy =>
    {
        policy.Requirements.Add(new ChannelHubAuthorizationRequirement());
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.Scan(scan => scan
                               .FromEntryAssembly()
                               .AddClasses(
                                   classes => classes.AssignableTo<ITransient>()
                                )
                               .AsSelfWithInterfaces()
                               .WithTransientLifetime()
                               .AddClasses(
                                   classes => classes.AssignableTo<IScoped>()
                                )
                               .AsSelfWithInterfaces()
                               .WithScopedLifetime()
                               .AddClasses(
                                   classes => classes.AssignableTo<ISingleton>())
                               .AsSelfWithInterfaces()
                               .WithSingletonLifetime()
                               );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

//app.MapControllers();
app.MapHub<ChannelHub>("/channel", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

//var pushRouter = app.MapGroup("push").WithOpenApi();

//pushRouter.MapPost("all",
//    async ([FromBody] ChannelMessageInfo messageInfo, IHubContext<ChannelHub> hubContext)
//    => await hubContext.Clients.Group(messageInfo.Id).SendAsync("receiveMessage", messageInfo.Message));

//pushRouter.MapPost("user",
//    async ([FromBody] ChannelMessageInfo messageInfo, IHubContext<ChannelHub> hubContext)
//    => await hubContext.Clients.Group(messageInfo.Id).SendAsync("receiveMessage", messageInfo.Message));

app.MapEndPoints();
app.Run();

//class ChannelMessageInfo
//{
//    public string Message { get; set; }
//    public string Id { get; set; }
//}
