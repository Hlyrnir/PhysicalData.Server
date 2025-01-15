using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Passport.Abstraction.Authentication;
using Passport.Api.Endpoint;
using PhysicalData.Api;
using PhysicalData.Api.Authorization;
using PhysicalData.Api.DataProtection;
using PhysicalData.Api.Endpoint;
using PhysicalData.Api.Health;
using PhysicalData.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

WebApplicationBuilder webBuilder = WebApplication.CreateBuilder(args);

webBuilder.Services.AddPhysicalDataServiceCollection()
    .AddAuthorization()
    .AddSqliteDatabase(sConnectionStringName: "PhysicalData");

// Add hasher
webBuilder.Services.AddOptions<PassportHashSetting>()
    .Bind(webBuilder.Configuration.GetSection(PassportHashSetting.SectionName))
    .ValidateOnStart();

webBuilder.Services.AddScoped<IPassportHasher, PassportHasher>();

// Add data protection
webBuilder.Services.AddOptions<DataProtectionSetting>()
    .Bind(webBuilder.Configuration.GetSection(DataProtectionSetting.SectionName))
    .ValidateOnStart();

webBuilder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(webBuilder.Configuration["DataProtection:KeyStoragePath"]!))
    .SetApplicationName(DataProtectionSetting.ApplicationName);
webBuilder.Services.AddTransient(prvService =>
{
    return prvService.GetDataProtector(DataProtectionSetting.DataProtectorPurpose);
});

webBuilder.Services.AddHealthChecks()
    .AddCheck<HealthCheck>(HealthCheck.Name);

// Add jwt authentication
webBuilder.Services.AddOptions<JwtTokenSetting>()
    .Bind(webBuilder.Configuration.GetSection(JwtTokenSetting.SectionName))
    .ValidateOnStart();

webBuilder.Services.AddAuthentication(optAuthentication =>
{
    optAuthentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    optAuthentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    optAuthentication.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(optJwtBearer =>
{
    optJwtBearer.RequireHttpsMetadata = true;
    optJwtBearer.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.FromSeconds(15),

        ValidIssuer = webBuilder.Configuration["JwtSetting:Issuer"],
        ValidAudience = webBuilder.Configuration["JwtSetting:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webBuilder.Configuration["JwtSetting:SecretKey"]!)),

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

//// see https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-7.0
webBuilder.Services.AddAuthorization(optAuthorization =>
{
    optAuthorization.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();

    optAuthorization.AddPolicy(EndpointPolicy.Name, EndpointPolicy.EndpointWithPassport());
});

webBuilder.Services.AddApiVersioning(optVersion =>
{
    optVersion.DefaultApiVersion = new ApiVersion(1.0);
    optVersion.AssumeDefaultVersionWhenUnspecified = true;
    optVersion.ReportApiVersions = true;
    optVersion.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
});

webBuilder.Services.AddExceptionHandler<GlobalExceptionHandler>();
webBuilder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
webBuilder.Services.AddEndpointsApiExplorer();
webBuilder.Services.AddSwaggerGen();
webBuilder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOption>();

WebApplication webApplication = webBuilder.Build();

// Configure the HTTP request pipeline.
if (webApplication.Environment.IsEnvironment("Development"))
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(optSwagger =>
    {
        optSwagger.DocumentTitle = "DEVELOPMENT - API of PhysicalData";
    });
}
else if (webApplication.Environment.IsEnvironment("Testing"))
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(optSwagger =>
    {
        optSwagger.DocumentTitle = "TESTING - API of PhysicalData";
    });
}

webApplication.UseExceptionHandler();
webApplication.UseHsts();

webApplication.UseHttpsRedirection();

// Use JWT bearer token for authentication
webApplication.UseAuthentication();
// Authorize only authenticated user to endpoints
webApplication.UseAuthorization();

webApplication.MapHealthChecks("/_health")
    .AllowAnonymous();

webApplication.AddEndpointVersionSet();

webApplication.AddAuthenticationEndpoint(EndpointPolicy.Name);
webApplication.AddPassportEndpoint(EndpointPolicy.Name);
webApplication.AddPassportHolderEndpoint(EndpointPolicy.Name);
webApplication.AddPassportTokenEndpoint(EndpointPolicy.Name);
webApplication.AddPassportVisaEndpoint(EndpointPolicy.Name);

webApplication.AddPhysicalDataEndpointVersionSet();

webApplication.AddPhysicalDimensionEndpoint(EndpointPolicy.Name);
webApplication.AddTimePeriodEndpoint(EndpointPolicy.Name);

webApplication.Run();