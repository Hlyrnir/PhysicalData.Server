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
using PhysicalData.Api.Cors;
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

webBuilder.Services.AddOptions<PassportHashSetting>()
    .Bind(webBuilder.Configuration.GetSection(PassportHashSetting.SectionName))
    .ValidateOnStart();

webBuilder.Services.AddScoped<IPassportHasher, PassportHasher>();

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

webBuilder.Services.AddOptions<JwtTokenSetting>()
    .Bind(webBuilder.Configuration.GetSection(JwtTokenSetting.SectionName))
    .ValidateOnStart();

webBuilder.Services.AddCors(optCors =>
{
    optCors.AddPolicy(DefaultCorsPolicy.Name, DefaultCorsPolicy.RestrictedOrigin(webBuilder.Configuration["Cors:Origin"]!));
});

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

webBuilder.Services.AddAuthorization(optAuthorization =>
{
    optAuthorization.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();

    optAuthorization.AddPolicy(PassportAuthorizationPolicy.Name, PassportAuthorizationPolicy.WithPassport());
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

webBuilder.Services.AddEndpointsApiExplorer();
webBuilder.Services.AddSwaggerGen();
webBuilder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOption>();

WebApplication webApplication = webBuilder.Build();

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

webApplication.UseRouting();

webApplication.UseCors(DefaultCorsPolicy.Name);

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapHealthChecks("/_health")
    .AllowAnonymous();

webApplication.AddEndpointVersionSet();

webApplication.AddAuthenticationEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);
webApplication.AddPassportEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);
webApplication.AddPassportHolderEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);
webApplication.AddPassportTokenEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);
webApplication.AddPassportVisaEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);

webApplication.AddPhysicalDataEndpointVersionSet();

webApplication.AddPhysicalDimensionEndpoint(
    sCorsPolicyName: DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);
webApplication.AddTimePeriodEndpoint(
    sCorsPolicyName:DefaultCorsPolicy.Name,
    sAuthorizationPolicyName: PassportAuthorizationPolicy.Name);

await webApplication.RunAsync();