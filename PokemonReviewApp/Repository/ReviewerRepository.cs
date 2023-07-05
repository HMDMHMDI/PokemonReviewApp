using PokemonReviewApp.Data;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository;

public class ReviewerRepository : IReviewerRepository
{
    private readonly DataContext _context;

    public ReviewerRepository(DataContext context)
    {
        _context = context;
    }
    public ICollection<Reviewer> GetReviewers()
    {
        return _context.Reviewers.OrderBy(e => e.Id).ToList();
    }

    public Reviewer GetReviewer(int reviewerId)
    {
        return _context.Reviewers.Where(e => e.Id == reviewerId).FirstOrDefault();
    }

    public ICollection<Review> GetReviewsByReviewer(int reviewerId)
    {
        return _context.Reviews.Where(e => e.Reviewer.Id == reviewerId).ToList();
    }

    public bool ReviewerExists(int reviewerId)
    {
        return _context.Reviewers.Any(e => e.Id == reviewerId);
    }

    public bool CreateReviewer(Reviewer reviewer)
    {
        _context.Add(reviewer);
        return Save();
    }

    public bool UpdateReviewer(Reviewer reviewer)
    {
        _context.Update(reviewer);
        return Save();
    }

    public bool DeleteReviewer(Reviewer reviewer)
    {
        _context.Remove(reviewer);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}