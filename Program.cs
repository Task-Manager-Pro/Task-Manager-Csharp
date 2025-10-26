using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Todo;
using Todo.Dal;
using Todo.Data;
using Todo.Interfaces;
using Todo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager", Version = "v1" });
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=TaskManagerPro;Trusted_Connection=True;TrustServerCertificate=True;");
});

builder.Services.AddScoped<TaskManagerServices>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<Todo.Interfaces.IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<CategorieTaskService>();
builder.Services.AddScoped<Todo.Dal.TaskDal>();
builder.Services.AddScoped<Todo.Dal.CategorieTaskDal>();
builder.Services.AddScoped<Todo.Dal.UserDal>();


var key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager");
});
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
