using PokemonReviewApp.Models;

namespace PokemonReviewApp.interfaces;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int id);
    Pokemon GetPokemon(string name);
    decimal GetPokemonRating(int pokeId);
    bool PokemonExists(int pokeId);
    bool CreatePokemon(Pokemon pokemon , int ownerId , int categoryId);
    bool UpdatePokemon(Pokemon pokemon, int ownerId , int categoryId);
    bool DeletePokemon(Pokemon pokemon);
    bool Save(); 
}