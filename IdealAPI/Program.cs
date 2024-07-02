using IdealAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PracticeAPI.Data;
using PracticeAPI.Mappings;
using PracticeAPI.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdealAPI.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Serilog.Core;
using Serilog;
using Microsoft.Extensions.Logging;
using IdealAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/Indian_walks.txt",rollingInterval:RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers();
#region
//builder.Services.AddApiVersioning(options =>
//{
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//    options.ReportApiVersions = true;
//});

//builder.Services.AddVersionedApiExplorer(options =>
//{
//    options.GroupNameFormat = "'v'VVV";
//    options.SubstituteApiVersionInUrl = true;
//});
// Add any other services configuration here
#endregion

builder.Services.AddDbContext<WalkDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<WalkAuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IndianWalksAuthConnectionstring")));
builder.Services.AddScoped<ICachingInterface, CacheService>();
builder.Services.AddScoped<IRegionRepository,SqlRegionRepository>();
builder.Services.AddScoped<IWalkRepository,SqlWalkRepository>();
builder.Services.AddScoped<ITokenRepository,TokenRepository>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Please enter your JWT token in the following format: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, // Corrected to SecurityScheme
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "OAuth2", // Corrected to "bearer"
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
//builder.Services.Configure<RouteOptions>(options =>
//{
//    options.ConstraintMap.Add("Guid", typeof(GuidRouteConstraint));
//});


builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("IndianWalks")
    .AddEntityFrameworkStores<WalkAuthDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience=true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Place UseAuthentication and UseAuthorization here
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
