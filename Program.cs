using Todo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  //informando aos asp.net core que vamos usar controllers

//configurando o CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>();  //informando aos asp.net core que vamos usar o banco de dados


var app = builder.Build();
app.UseCors(); //informando aos asp.net core que vamos usar o CORS
app.MapControllers(); 
app.Run();
