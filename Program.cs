using ApexLawFirm.API.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
string connectionString = $"server={Env.GetString("MYSQL_HOST")};" +
                          $"port={Env.GetString("MYSQL_PORT")};" +
                          $"database={Env.GetString("MYSQL_DATABASE")};" +
                          $"user={Env.GetString("MYSQL_USER")};" +
                          $"password={Env.GetString("MYSQL_PASSWORD")};";

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(serverOptions =>{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    serverOptions.ListenAnyIP(int.Parse(port));
});
builder.Services.AddDbContext<ApexLawFirmDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31))
    .UseSnakeCaseNamingConvention()));

builder.Services.AddSwaggerGen(options =>{
  options.SwaggerDoc("v1", new OpenApiInfo { Title = "ApexLawFirm API", Version = "v1" });
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter your JWT token in this format: Bearer {your token}"
  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement{{
    new OpenApiSecurityScheme{
      Reference = new OpenApiReference{
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer"
      }
    },
    Array.Empty<string>()
  }});
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>{
  options.TokenValidationParameters = new TokenValidationParameters{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = Env.GetString("JWT_ISSUER"),
    ValidAudience = Env.GetString("JWT_AUDIENCE"),
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT_KEY")!))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>{
  options.AddPolicy("AllowSpecificOrigins", builder =>{
    builder.WithOrigins(Env.GetString("CLIENT_URL")!)
           .AllowAnyMethod()
           .AllowAnyHeader();
  });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()){
  var dbContext = scope.ServiceProvider.GetRequiredService<ApexLawFirmDbContext>();
  if(dbContext.Database.CanConnect()){
    Console.WriteLine("Database connection successful!");
  }
  else{
    Console.WriteLine("Failed to connect to the database.");
  }
}

if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");

app.MapControllers();

app.MapGet("/", () => "Welcome to Apex Law Firm API!");

app.Run();
