//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();

var builder = WebApplication.CreateBuilder(args).Inject();


// 默认授权机制，需授权的即可（方法）需贴 `[Authorize]` 特性
builder.Services.AddJwt();

builder.Services.AddControllers().AddInject();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.UseInject();


app.MapControllers();

app.Run();
