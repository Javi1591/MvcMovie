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

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;

        // Implement Basic Logging
        // Use ILogger
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MvcMovieContext context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            // Log getting db
            _logger.LogDebug("Searched for {searchString}", searchString);

            if (_context.Movie == null)
            { 
                // Log any nulls when getting db
                _logger.LogError("Object is null at {Time}",DateTimeOffset.UtcNow);

                return Problem("Entity set 'MvcMovieContext.Movie' is null.");
            }

            // Use Linq to get list of genres
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
                select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                // Log filter information
                movies = movies.Where(s => s.Title!.ToUpper().Contains(searchString.ToUpper()));
                _logger.LogDebug("Applied filter");
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                // Log filters for movieGenre
                movies = movies.Where(x => x.Genre == movieGenre);
                _logger.LogDebug("Applied filter by {movieGenre}.", movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };

            // Log getting movieGenreVM
            _logger.LogDebug("Returned {movieGenreVM}", movieGenreVM);

            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // Log any null Details
                _logger.LogWarning("Movie with {id} not found.", DateTimeOffset.UtcNow);

                return NotFound();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                // Log any null movies
                _logger.LogWarning("Movie {id} has null values", id);

                return NotFound();
            }

            // Log found movies
            _logger.LogDebug("Found movie with {id}", id);

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();

                // Log posting of new movie
                _logger.LogDebug("{movie} has been created successfully", movie);

                return RedirectToAction(nameof(Index));
            }

            // Log any errors for new create movie
            _logger.LogDebug("{movie} was NOT created, please see errors.", movie);

            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                // Log any movies not found with id
                _logger.LogWarning("You can NOT edit the movie with {id} because it is not found", id);

                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                // Log any movies with null titles
                _logger.LogWarning("Movie with {title} is null and can NOT be editted at this time. Please try again later.", movie);

                return NotFound();
            }
            // Log editted movie
            _logger.LogDebug("{movie} has been successfully editted.", movie);

            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Log postting of movie edits
                    _logger.LogDebug("{movie} with {id} is being editted.", movie, id);

                    _context.Update(movie);
                    await _context.SaveChangesAsync();

                    // Log postting of successful edits
                    _logger.LogDebug("{movie} with {id} was successsfully editted", movie, id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        // Log any movie id that does NOT exist
                        _logger.LogError("{movie} with {id} does NOT exist, please try again.", movie, id);

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Log any unsuccessful edits
            _logger.LogError("{movie} with {id} was not editted, please try again later.", movie, id);

            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            // Return strongly typed view
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                // Log postting of deleting movie
                _logger.LogDebug("{movie} with {id} was deleted.", movie, id);
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
