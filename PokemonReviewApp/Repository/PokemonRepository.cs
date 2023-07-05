using Microsoft.EntityFrameworkCore.Internal;
using PokemonReviewApp.Data;
using PokemonReviewApp.interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository;

public class PokemonRepository : IPokemonRepository
{
    private readonly DataContext _context;

    public PokemonRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Pokemon> GetPokemons()
    {
        return _context.Pokemons.OrderBy(p => p.Id).ToList();
    }

    public Pokemon GetPokemon(int id)
    {
        return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
    }

    public Pokemon GetPokemon(string name)
    {
        return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
    }

    public decimal GetPokemonRating(int pokeId)
    {
        var review = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);
        if (review.Count() <= 0)
        {
            return 0;
        }

        return ((decimal)review.Sum(r => r.Rating)) / review.Count(); 
    }

    public bool PokemonExists(int pokeId)
    {
        return _context.Pokemons.Any(e => e.Id == pokeId);
    }

    public bool CreatePokemon(Pokemon pokemon , int ownerId , int categoryId)
    {
        var pokemonOwnerEntity = _context.Owners.Where(e => e.Id == ownerId).FirstOrDefault();
        var category = _context.Categories.Where(e => e.Id == categoryId).FirstOrDefault();
        var pokemonOwner = new PokemonOwner()
        {
            Owner = pokemonOwnerEntity,
            Pokemon = pokemon
        };
        _context.Add(pokemonOwner);

        var pokemonCategory = new PokemonCategory()
        {
            Category = category,
            Pokemon = pokemon
        };
        _context.Add(pokemonCategory);
        _context.Add(pokemon);  
        return Save();
    }

    public bool UpdatePokemon(Pokemon pokemon, int ownerId, int categoryId)
    {
        _context.Update(pokemon);
        return Save();
    }

    public bool DeletePokemon(Pokemon pokemon)
    {
        _context.Remove(pokemon);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}