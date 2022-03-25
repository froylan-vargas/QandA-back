using DbUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using QandA.Authorization;
using QandA.Data;

var builder = WebApplication.CreateBuilder(args);
var Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

var connectionString = Configuration.GetConnectionString("DefaultConnection");

EnsureDatabase.For.SqlDatabase(connectionString);
var upgrader = DeployChanges.To.SqlDatabase(connectionString, null)
    .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
    .WithTransaction()
    .Build();

if (upgrader.IsUpgradeRequired())
{
    upgrader.PerformUpgrade();
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
        options.AddPolicy("CorsPolicy", builder =>
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(Configuration["FrontEnd"])));

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = Configuration["Auth0:Authority"];
        options.Audience = Configuration["Auth0:Audience"];
    });

    services.AddAuthorization(options =>
        options.AddPolicy("MustBeQuestionAuthor", policy
        => policy.Requirements
            .Add(new MustBeQuestionAuthorRequirement())));

    services.AddHttpClient();
    services.AddMemoryCache();
    services.AddHttpContextAccessor();    

    services.AddScoped<IDataRepository, DataRepository>();    
    services.AddSingleton<IQuestionCache, QuestionCache>();
    services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();
}

