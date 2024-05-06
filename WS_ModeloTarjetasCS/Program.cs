using EstructurasDatosPG3CS.Colas;
using EstructurasDatosPG3CS.ListasSimples;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<EstructurasDatosPG3CS.ListasSimples.clsLSLista>();
builder.Services.AddScoped<EstructurasDatosPG3CS.Colas.clsCLista>();
//builder.Services.AddSingleton<EstructurasDatosPG3CS.ListasSimples.clsLSLista>();
builder.Services.AddSingleton<clsLSLista>();
builder.Services.AddSingleton<clsCLista>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
