using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Dtos
{
    public class CharacterToAddOrUpdateDto
    {
        public string Name { get; set; }
        public ICollection<int> EpisodesIds { get; set; }
        public ICollection<int> FriendsIds { get; set; }
    }
}
