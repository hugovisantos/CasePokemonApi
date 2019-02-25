using Microsoft.AspNetCore.Mvc;
using PokemonApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PokemonApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IList<Pokemon> _pokemonList;

        public PokemonController()
        {
            using (var stream = new StreamReader("pokemons.json"))
            {
                var result = stream.ReadToEnd();
                this._pokemonList = JsonConvert.DeserializeObject<IList<Pokemon>>(result);
            }
        }
        // POST api/pokemon
        [HttpPost]
        public ActionResult Post([FromBody] Pokemon pokemon)
        {
            _pokemonList.Add(pokemon);

            using (var stream = new StreamWriter("pokemons.json"))
            {
                stream.Write(JsonConvert.SerializeObject(_pokemonList));
            }

            return base.Created($"http://localhost:5000/api/pokemon/{pokemon.Name}", pokemon);
        }

        // POST api/pokemon/{name}
        [HttpGet("{name}")]
        public ActionResult GetFromRoute([FromRoute] string name)
        {
            var pokemon = _pokemonList.FirstOrDefault(p => p.Name.ToLower().Equals(name.ToLower()));

            return base.Ok(pokemon);
        }

        [HttpGet]
        public ActionResult GetFromQuery([FromQuery] string name)
        {
            using (var stream = new StreamReader("pokemons.json"))
            {
                var result = stream.ReadToEnd();
                var pokemonList = JsonConvert.DeserializeObject<IList<Pokemon>>(result);

                var pokemon = pokemonList.FirstOrDefault(p => p.Name.ToLower().Equals(name.ToLower()));

                return base.Ok(pokemon);
            }
        }        

        [HttpGet("types/{type}")]
        public ActionResult Get([FromRoute] Type type, [FromQuery] int attack, [FromQuery] int defense, [FromQuery] int healthPoints)
        {
            var pokemonListResult = this._pokemonList.Where(pokemon => pokemon.Type == type);

            if(pokemonListResult.Any())
            {
                var result = pokemonListResult.Where(pokemon => pokemon.Statistics.Attack == attack || 
                                                                pokemon.Statistics.Defense == defense||
                                                                pokemon.Statistics.HealthPoints == healthPoints);

                return base.Ok(result);
            }
            return base.Ok(pokemonListResult);
            
        }
    }
}
