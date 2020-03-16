using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Models
{
    public class CharacterEpisode
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
    }
}
