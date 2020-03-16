using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Dtos
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> Episodes { get; set; }
        public ICollection<string> Friends { get; set; }
    }
}
