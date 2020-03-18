using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.API.Models;

namespace StarWars.API.Repository
{
    public interface IStarWarsRepository
    {
        Task<Character> Get(int id, bool includeRelated = true);
        Task<Character> GetWithoutFriends(int id);
        void Add(Character character);
        void Delete(Character character);
        Task<QueryResult<Character>> GetAll(CharacterQuery queryObj);
        Task<bool> SaveAll();
    }
}