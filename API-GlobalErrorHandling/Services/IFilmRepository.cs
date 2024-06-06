using API_GlobalErrorHandling.Model;
using System.Threading.Tasks;

namespace API_GlobalErrorHandling.Services
{
    public interface IFilmRepository
    {
        Task<IEnumerable<Film>> GetAllAsync();

        Task<Film> GetAsync(int id);


    }
}
