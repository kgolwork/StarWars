using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Dtos
{
    public class CharacterQueryDto
    {
        public int Page { get; set; }
        public byte PageSize { get; set; }
    }
}
