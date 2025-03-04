using System.Security.Claims;
using System.Text;
using CompanyManagement.Services;
using CompanyManagement.Data;
using CompanyManagement.Models;
using CompanyManagement.Repositories;
using CompanyManagement.Seeders;
//using CompanyManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
//using DotnetAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using CompanyManagement.AutoMapper;
using FluentValidation.AspNetCore;
using CompanyManagement.InputModels;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

//Nlog configuration
LogManager.Setup().LoadConfigurationFromAppSettings();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddNLog();

var environment = builder.Environment.EnvironmentName;
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true) // Load appsettings.Test.json for tests
    .AddEnvironmentVariables()
    .Build();
builder.Configuration.AddConfiguration(config);
// builder.Services.AddDbContext<CompanyDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CompanyDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
                
// Register validators explicitly
builder.Services.AddScoped<IValidator<EmployeeInputModel>, EmployeeValidator>();
builder.Services.AddScoped<IValidator<ProjectInputModel>, ProjectValidator>();
builder.Services.AddScoped<IValidator<CompanyInputModel>, CompanyValidator>();


builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUploadImageService, UploadImageService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // lockout duration
         options.Lockout.MaxFailedAccessAttempts = 5; // max failed attempts before lockout
         options.Lockout.AllowedForNewUsers = true; // lockout applies to new users 
    }
    )
    .AddEntityFrameworkStores<CompanyDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddDefaultTokenProviders();


var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        //RoleClaimType = ClaimTypes.Role
    };
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Company Management API", Version = "v1" });
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "file", Format = "binary" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});




// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("WWW-Authenticate");
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

// Middleware
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Company API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.MapControllers();
app.Run();

public partial class Program { }

// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Company Management API", Version = "v1" });

//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Description = "Enter 'Bearer <your-token>'",
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey
//     });

//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             new string[] {}
//         }
//     });
// });

// var jwtSettings = builder.Configuration.GetSection("Jwt");

// string? secretKey = jwtSettings["SecretKey"];
// if (string.IsNullOrEmpty(secretKey))
// {
//     throw new InvalidOperationException("JWT SecretKey is missing in appsettings.json");
// }

// var key = Encoding.UTF8.GetBytes(secretKey);

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidIssuer = jwtSettings["Issuer"],
//         ValidAudience = jwtSettings["Audience"],
//         ValidateLifetime = true,
//         RoleClaimType = "role" 
//     };
// });
