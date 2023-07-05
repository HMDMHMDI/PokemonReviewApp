using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICountryRepository _countryRepository;

    public OwnerController(IMapper mapper , IOwnerRepository ownerRepository , ICountryRepository countryRepository)
    {
        _mapper = mapper;
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owners);
    }

    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();
        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
        if (!ModelState.IsValid)
            return BadRequest(owner);
        return Ok(owner);
    }
    [HttpGet("{ownerId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonByOwner(int ownerId)
    {
        var owner = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owner);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateOwner([FromQuery] int countryId , [FromBody] OwnerDto ownerCreate)
    {
        if (ownerCreate == null) return BadRequest(ModelState);
        var owners = _ownerRepository.GetOwners()
            .Where(e => e.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
            .FirstOrDefault();
        if (owners != null)
        {
            ModelState.AddModelError("" , "Owner already Exist");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ownerMap = _mapper.Map<Owner>(ownerCreate);
        ownerMap.Country = _countryRepository.GetCountry(countryId);
        if (!_ownerRepository.CreateOwner(ownerMap))
        {
            ModelState.AddModelError("" , "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateOwner([FromBody] OwnerDto updatedOwner, int ownerId)
    {
        if (updatedOwner == null) return BadRequest(ModelState);
        if (ownerId != updatedOwner.Id) return BadRequest(ModelState);
        if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();
        if (!ModelState.IsValid) return BadRequest();
        var ownerMap = _mapper.Map<Owner>(updatedOwner);
        if (!_ownerRepository.UpdateOwner(ownerMap))
        {
            ModelState.AddModelError("" , "Something wrong while updating");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId)) return NotFound();
        var ownerToDelete = _ownerRepository.GetOwner(ownerId);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!_ownerRepository.DeleteOwner(ownerToDelete))
        {
            ModelState.AddModelError("" , "Something wrong");
        }

        return NoContent();
    }
}