using API_GlobalErrorHandling.Model;
using API_GlobalErrorHandling.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_GlobalErrorHandling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly IFilmRepository filmRepository;

        public FilmController(IFilmRepository filmRepository)
        {
            this.filmRepository = filmRepository;
        }

        // GET: api/<FilmController>
        [HttpGet]
       public async Task<IEnumerable<Film>> Get()
        {
            return await filmRepository.GetAllAsync();
        }

        // GET api/<FilmController>/5
        [HttpGet("{id}")]
        public async Task<Film> Get(int id)
        {
            return await filmRepository.GetAsync(id);
        }

        // POST api/<FilmController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FilmController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FilmController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
