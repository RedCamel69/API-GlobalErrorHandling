
using API_GlobalErrorHandling.ExceptionHandler;
using API_GlobalErrorHandling.Model;
using API_GlobalErrorHandling.Services;

namespace API_GlobalErrorHandling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            
            // add support for traditional api
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IFilmRepository, FilmRepository>()
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();

            var app = builder.Build();

            
            app.UseStatusCodePages();
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            // add routing for traditional api
            app.MapControllers();
           
            app.MapGet("/films", async (IFilmRepository repository) =>
            {
                IEnumerable<Film> films = await repository.GetAllAsync();
                return Results.Ok(films);
            });

            app.MapGet("/film/{id}", async (IFilmRepository repository, int id) =>
            {
                Film? film = await repository.GetAsync(id);
                return film is not null ? Results.Ok(film) : Results.NotFound();
            });

            app.Run();
        }
    }
}
