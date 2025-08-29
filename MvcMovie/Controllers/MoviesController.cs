using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.Extensions.Logging;
using MvcMovie.Services;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movies;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movies, ILogger<MoviesController> logger)
        {
            _movies = movies;
            _logger = logger;
        }

        // GET: Movies
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

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            {
                var movies = await _movies.GetByIdAsync(id);
                _logger.LogInformation("Displaying details for movie {id}", id);
                return View(movies);
            }
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Create GET");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            {
                var movies = await _movies.GetByIdAsync(id);
                _logger.LogInformation("Edit GET, movie id {id}", id);
                return View(movies);
            }
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            _logger.LogInformation("Delete GET, movie by {id}", id);
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movies.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
