using API_GlobalErrorHandling.Model;

namespace API_GlobalErrorHandling.Services
{
    public class FilmRepository : IFilmRepository
    {
        private readonly List<Film> films =
    [
        new Film(1, "The Godfather", "Classic 70's crime thriller"),
        new Film(2, "Jaws", "Classic 70's monster movie"),
        new Film(3, "Saturday Night Fever", "Classic 70's drama")
    ];

        public Task<IEnumerable<Film>> GetAllAsync()
        {

            // Simulate a database connection error
            throw new InvalidOperationException("The database connection is closed!");
        }

        public async Task<Film> GetAsync(int id)
        {
            // 0 or negative ids are not allowed
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
            }

            //illustrate throwing additional exception types
            if (id > 99)
            {
                throw new ArgumentException(nameof(id), "The id must not be greater than 99!");
            }

            return await Task.FromResult(films.Find(film => film.Id == id));
        }
    }
}
