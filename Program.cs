using Todo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  //informando aos asp.net core que vamos usar controllers
builder.Services.AddDbContext<AppDbContext>();  //informando aos asp.net core que vamos usar o banco de dados


var app = builder.Build();

app.MapControllers(); 
app.Run();
