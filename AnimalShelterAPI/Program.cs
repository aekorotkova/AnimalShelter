using AnimalShelterAPI;

var builder = WebApplication.CreateBuilder(args);

// берём строку подключения и передаём в DbHelper
var connectionString = builder.Configuration.GetConnectionString("AnimalShelterDB");
DbHelper.Initialize(connectionString);

// подключаем контроллеры
builder.Services.AddControllers();

// swagger для документации
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// swagger включён
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();