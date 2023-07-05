using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReviewController : Controller
{
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewerRepository _reviewerRepository;

    public ReviewController(IMapper mapper , IReviewRepository reviewRepository , IPokemonRepository pokemonRepository,
        IReviewerRepository reviewerRepository)
    {
        _mapper = mapper;
        _reviewRepository = reviewRepository;
        _pokemonRepository = pokemonRepository;
        _reviewerRepository = reviewerRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public IActionResult GetReviews()
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }
    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(review);
    }

    [HttpGet("pokemon,{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsForAPokemon(int pokeId)
    {
        var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(review);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromBody] ReviewDto reviewCreate , [FromQuery] int reviewerId , [FromQuery] int pokeId)
    {
        if (reviewCreate == null) return BadRequest(ModelState);
        var reviews = _reviewRepository.GetReviews()
            .Where(e => e.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
            .FirstOrDefault();
        if (reviews != null)
        {
            ModelState.AddModelError("" , "Review Already exist");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var reviewMap = _mapper.Map<Review>(reviewCreate);
        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
        if (!_reviewRepository.CreateReview(reviewMap))
        {
            ModelState.AddModelError("" , "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{reviewId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
    {
        if (updatedReview == null) return BadRequest(ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (reviewId != updatedReview.Id) return BadRequest(ModelState);
        if (!_reviewRepository.ReviewExists(reviewId)) return NotFound();
        var reviewMap = _mapper.Map<Review>(updatedReview);
        if (!_reviewRepository.UpdateReview(reviewMap))
        {
            ModelState.AddModelError("" , "something went wrong");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{reviewId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId)) return NotFound();
        var reviewToDelete = _reviewRepository.GetReview(reviewId);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!_reviewRepository.DeleteReview(reviewToDelete))
        {
            ModelState.AddModelError("" , "Something wrong");
        }
        return NoContent();
    }

    [HttpDelete("/DeleteReviewsByReviewer/{reviewerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReviewsByReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId)) return NotFound();

        var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId).ToList();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!_reviewRepository.DeleteReviews(reviewsToDelete))
        {
            ModelState.AddModelError("" , "error delete reviews");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}