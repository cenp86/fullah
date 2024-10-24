using UalaAccounting.api.Extensions;
using UalaAccounting.api.HealthChecks;
using UalaAccounting.api.Logging;
using UalaAccounting.api.Logging.Enrichers;
using Amazon.S3;
using Amazon.Runtime;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//var awsOptions = configuration.GetAWSOptions();
//awsOptions.Credentials = new BasicAWSCredentials("ASIART7A34XJHB3FOIMD", "rIDXCtwbSUDWtuWF0ECljR6/YqaKvz4ZlLU5rCsS");




// Add services to the container.
builder.Services.AddSerilogServices();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddServicesMambu(configuration);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseHealthCheck();
app.UseLoggerMiddleware();
//app.UseAuthorization();

app.MapControllers();

app.Run();

