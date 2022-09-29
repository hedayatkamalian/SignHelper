using IdGen.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MustIt.Kernel.Services.Implements;
using MustIt.Kernel.Services.Interfaces;
using MustKernel.Extentions;
using MustKernel.Options;
using MustKernel.Services.Implements;
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
builder.Services.AddHttpClient();
builder.Services.AddHostedService<TempCleanerBackgroundService>();


builder.Services.AddScoped<IDownloaderService, DownloaderService>();
builder.Services.AddScoped<ISignHelperService, SignHelperService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITemplatesRepository, TemplatesRepository>();
builder.Services.AddScoped<ISignRequestsRepository, SignRequestsRepository>();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)));
builder.Services.Configure<ApplicationErrors>(builder.Configuration.GetSection(nameof(ApplicationErrors)));
builder.Services.Configure<SmsOptions>(builder.Configuration.GetSection(nameof(SmsOptions)));
builder.Services.Configure<EmailServiceOptions>(builder.Configuration.GetSection(nameof(EmailServiceOptions)));
builder.Services.Configure<TempCleanerOptions>(builder.Configuration.GetSection(nameof(TempCleanerOptions)));

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var tempFolder = builder.Configuration.GetValue<string>("ApplicationOptions:Folders:Temp");
app.AddStaticFolder(tempFolder);


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
