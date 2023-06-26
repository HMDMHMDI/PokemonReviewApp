 using Microsoft.EntityFrameworkCore;
 using PokemonReviewApp;
 using PokemonReviewApp.Data;
 using PokemonReviewApp.interfaces;
 using PokemonReviewApp.Models;
 using PokemonReviewApp.Repository;

 var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); 
builder.Services.AddTransient<Seed>();
 builder.Services.AddScoped<IPokemonRepository, PokemonRepository >();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 builder.Services.AddDbContext<DataContext>(options =>
 {
     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
     options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
 });
 
var app = builder.Build();

 if (args.Length == 1 && args[0].ToLower() == "seeddata")
     SeedData(app);

 void SeedData(IHost app)
 {
     var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

     using (var scope = scopedFactory.CreateScope())
     {
         var service = scope.ServiceProvider.GetService<Seed>();
         service.SeedDataContext();
     }
 }
 
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