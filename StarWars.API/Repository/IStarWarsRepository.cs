using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.API.Models;

namespace StarWars.API.Repository
{
    public interface IStarWarsRepository
    {
        Task<Character> Get(int id, bool includeRelated = true);
        void Add(Character character);
        void Delete(Character character);
        Task<QueryResult<Character>> GetAll(CharacterQuery queryObj);
        Task AddFriendship(int characterId, int friendId);
        void DeleteFriendship(int characterId, int friendId);
        Task AddCharacterEpisode(int characterId, int episodeId);
        void DeleteCharacterEpisode(int characterId, int episodeId);
        Task<IEnumerable<int>> GetFriendsIds(int id);
        Task<IEnumerable<Episode>> GetEpisodesForCharacter(int id);
        Task<bool> SaveAll();

    }
}