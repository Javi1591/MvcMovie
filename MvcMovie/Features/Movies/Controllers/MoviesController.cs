using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using MvcMovie.Features.Movies.Models;
using Microsoft.Extensions.Logging;
using MvcMovie.Features.Movies.Services;

namespace MvcMovie.Features.Movies.Controllers
{
    // /movies
    [Route("movies")]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movies;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movies, ILogger<MoviesController> logger)
        {
            _movies = movies;
            _logger = logger;
        }

        // GET /movies
        [HttpGet("")]
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            IEnumerable<Movie> all = await _movies.GetAllAsync();
            IEnumerable<Movie> movies = all;
            IEnumerable<string?> genreQuery = all.Select(movie => movie.Genre).Distinct();

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title != null && s.Title.ToUpper().Contains(searchString, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation("Searching by {searchString}", searchString);
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
                _logger.LogInformation("Searching by genre {movieGenre}", movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(genreQuery),
                Movies = movies.ToList()
            };

            return View(movieGenreVM);
        }

        // GET: /movies/details/5
        [HttpGet("/details/{id:int}", Name = "MovieDetails")]
        public async Task<IActionResult> Details(int id)
        {
            {
                var movies = await _movies.GetByIdAsync(id);
                _logger.LogInformation("Displaying details for movie {id}", id);
                return View(movies);
            }
        }

        // GET: /movies/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            _logger.LogInformation("Create GET");
            return View();
        }

        // POST: /movies/create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Create POST model invalid.");
                return View(movie);
            }

            await _movies.AddAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        // GET: /movies/edit/5
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            {
                var movies = await _movies.GetByIdAsync(id);
                _logger.LogInformation("Edit GET, movie id {id}", id);
                return View(movies);
            }
        }

        // POST: /movies/edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Edit POST model is invalid.");
                return View(movie);
            }

            await _movies.UpdateAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        // GET: /movies/delete/5
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            _logger.LogInformation("Delete GET, movie by {id}", id);
            return View(movie);
        }

        // POST: /movies/delete/5
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movies.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /movies/bygenre
        [HttpGet("bygenre/{genre}")]
        public async Task<IActionResult> ByGenre(string genre)
        {
            var all = await _movies.GetAllAsync();
            var movies = all.Where(movie => movie.Genre != null && string.Equals(movie.Genre, genre, StringComparison.OrdinalIgnoreCase));
            var viewModel = new MovieGenreViewModel
            {
                Genres = new SelectList(all.Select(m => m.Genre).Distinct()),
                Movies = movies.ToList(),
                MovieGenre = genre
            };

            return View("Index", viewModel);
        }

        // GET: /movies/released/2010/5
        [HttpGet("released/{year:int:min(1900)}/{month:int:range(1,12)?}")]
        public async Task<IActionResult> Released(int year, int? month)
        {
            var all = await _movies.GetAllAsync();
            var movies = all.Where(movie => movie.ReleaseDate.Year == year && (month == 0 ? true : movie.ReleaseDate.Month == month));
            var viewModel = new MovieGenreViewModel
            {
                Genres = new SelectList(all.Select(m => m.Genre).Distinct()),
                Movies = movies.ToList()
            };

            return View("Index", viewModel);
        }
    }
}
