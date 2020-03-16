using Microsoft.EntityFrameworkCore.Internal;
using StarWars.API.Models;
using System.Collections.Generic;

namespace StarWars.API.Repository
{
    public class Seed
    {
        public static void SeedCharacters(StarWarsContext context)
        {
            if (context.Episodes.Any()) return;

            ICollection<Episode> episodes = new List<Episode>
            {
                new Episode { Name = "The Phantom Menace" },
                new Episode { Name = "Attack of the Clones" },
                new Episode { Name = "Revenge of the Sith" },
                new Episode { Name = "A New Hope" },
                new Episode { Name = "The Empire Strikes Back" },
                new Episode { Name = "Return of the Jedi" },
                new Episode { Name = "The Force Awakens" },
                new Episode { Name = "The Last Jedi" },
                new Episode { Name = "The Rise of Skywalker" },
            };

            foreach (var episode in episodes)
                context.Episodes.Add(episode);

            var lukeSkywalker = new Character
            {
                Name = "Luke Skywalker"
            };
            context.Characters.Add(lukeSkywalker);

            var darthVader = new Character
            {
                Name = "Darth Vader"
            };
            context.Characters.Add(darthVader);
                
            context.SaveChanges();
        }
    }
}