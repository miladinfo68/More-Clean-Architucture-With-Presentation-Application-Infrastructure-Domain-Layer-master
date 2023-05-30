using CleanArch.WebAPI.ExtensionMethods;
using CleanArch.WebAPI.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApiLayerServices(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
        {
            ["activated"] = false
        };
    });
}

app.MapControllers();

app.UseGlobalExceptionHandler();

app.Run();
