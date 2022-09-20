using IdGen.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MustIt.Kernel.Services.Implements;
using MustIt.Kernel.Services.Interfaces;
using SignHelperApp.Data;
using SignHelperApp.Options;
using SignHelperApp.Repositories.Implements;
using SignHelperApp.Repositories.Interfaces;
using SignHelperApp.Services.Implements;
using SignHelperApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var serverVersion = new MySqlServerVersion(new Version(8, 0, 28));
builder.Services.AddDbContext<DataContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("default"), serverVersion));
builder.Services.AddIdGen(1);

builder.Services.AddScoped<IDownloaderService, DownloaderService>();
builder.Services.AddScoped<ISignHelperService, SignHelperService>();
builder.Services.AddScoped<ITemplatesRepository, TemplatesRepository>();
builder.Services.AddScoped<ISignRequestsRepository, SignRequestsRepository>();

builder.Services.AddOptions<ApplicationOptions>();
builder.Services.AddOptions<ApplicationErrors>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
