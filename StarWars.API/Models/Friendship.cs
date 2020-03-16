using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Models
{
    public class Friendship
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public int FriendId { get; set; }
        public Character Friend { get; set; }
    }
}
