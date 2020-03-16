using StarWars.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Models
{
    public class CharacterQuery : IQueryObject
    {
        public int Page { get; set; }
        public byte PageSize { get; set; }
    }
}
