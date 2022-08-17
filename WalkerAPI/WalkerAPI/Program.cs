//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();

var builder = WebApplication.CreateBuilder(args).Inject();


// Ĭ����Ȩ���ƣ�����Ȩ�ļ��ɣ����������� `[Authorize]` ����
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
