using System;
using System.Threading.Tasks;
using Microsoft.Entity.FrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using MvcMovie.Services;
using Xunit;

namespace Tests;

public class MovieService_CRUDTest
{
    private static MovieService CreateService()
    {
        var dbContext = new DbContextOptionsBuilder<MvcMovieContext>();
        var inMemDb = dbContext.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var options = inMemDb.Options;
        var context = new MvcMovieContext(options);

        return new MovieService(context);
    }

    [Fact]
    public async Task MovieService_CanPerformCRUDOp()
    {
        var svc = CreateService();

        // Create
        var movie = new Movie {
            Title = "Inception",
            Genre = "Sci-Fi",
            Price = 18.99M
            Rating = "PG-13",
            ReleaseDate = DateTime.Parse("2010-05-01")
        };

        await svc.AddAsync(movie);

        // Read
        var read = await svc.GetByIdAsync(movie.Id);
        Assert.NotNull(saved);
        Assert.Equal(movie.Title, saved.Title);

        // Update
        read.Title = "Inception (20th year Anniversary Edition");
        await svc.UpdateAsync(read);
        var updated = await svc.GetByIdAsync(movie.Id);
        Assert.NotNull(updated);
        Assert.Equal(read.Title, updated.Title);

        // Delete
        await svc.DeleteAsync(movie.Id);
        var deleted = await svc.GetByIdAsync(movie.Id);
        Assert.Null(deleted);
    }
}
