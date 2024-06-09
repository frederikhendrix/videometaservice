using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReviewService.Bus;
using VideoMetaService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")?.Trim('"');
var mongoDbDatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")?.Trim('"');
var serviceBusConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")?.Trim('"');
var serviceBusTopicName = Environment.GetEnvironmentVariable("SERVICEBUS_TOPIC_NAME")?.Trim('"');
var serviceBusSubscriptionName = "VideoMetaServiceSubscription";


// Replace placeholders in the configuration.
builder.Configuration["ConnectionStrings:MongoDb"] = mongoDbConnectionString;
builder.Configuration["DatabaseName"] = mongoDbDatabaseName;
builder.Configuration["ServiceBus:ConnectionString"] = serviceBusConnectionString;
builder.Configuration["ServiceBus:TopicName"] = serviceBusTopicName;
builder.Configuration["ServiceBus:SubscriptionName"] = serviceBusSubscriptionName;

// Add services to the container.
builder.Services.AddMongoDatabase(builder.Configuration);
builder.Services.AddService(builder.Configuration);
builder.Services.AddControllers();

// Register the Azure Service Bus MessageSender and MessageReceiverService
builder.Services.AddSingleton(new MessageSender(serviceBusConnectionString, serviceBusTopicName));
builder.Services.AddHostedService(provider =>
    new MessageReceiverService(serviceBusConnectionString, serviceBusTopicName, serviceBusSubscriptionName));

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
