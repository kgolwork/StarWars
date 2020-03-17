using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWars.API.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CharacterEpisode> Episodes { get; set; }
        public virtual ICollection<Friendship> Friends { get; set; }
        public virtual ICollection<Friendship> FriendsOfMine { get; set; }
    }
}