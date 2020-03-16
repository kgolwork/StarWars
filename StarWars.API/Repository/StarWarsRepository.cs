using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarWars.API.Extensions;
using StarWars.API.Models;

namespace StarWars.API.Repository
{
    public class StarWarsRepository : IStarWarsRepository
    {
        private readonly StarWarsContext _context;
        
        public StarWarsRepository(StarWarsContext context)
        {
            _context = context;
        }
        public async Task<Character> Get(int id, bool includeRelated = true)
        {
            if (!includeRelated)
                return await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

            var character = await
                _context.Characters
                .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                .Include(c => c.Episodes)
                    .ThenInclude(e => e.Episode)
                .FirstOrDefaultAsync(c => c.Id == id);

            return character;
        }

        public void Add(Character character)
        {
            _context.Characters.Add(character);         
        }

        public async Task<QueryResult<Character>> GetAll(CharacterQuery queryObj)
        {
            var result = new QueryResult<Character>();

            var query =   
                _context.Characters
                .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                 .Include(c => c.Episodes)
                    .ThenInclude(e => e.Episode)
                .AsQueryable();

            result.TotalItems = await query.CountAsync();

            query = query.ApplyPaging(queryObj);

            result.Items = await query.ToListAsync();

            return result;
        }

        public void Delete(Character character)
        {
            _context.Remove(character);
        }

        public async Task AddCharacterEpisode(int characterId, int episodeId)
        {
            var characterEpisode = new CharacterEpisode
            {
                CharacterId = characterId,
                EpisodeId = episodeId
            };

            await _context.CharacterEpisodes.AddAsync(characterEpisode);
        }

        public void DeleteCharacterEpisode(int characterId, int episodeId)
        {
            var characterEpisode = new CharacterEpisode
            {
                CharacterId = characterId,
                EpisodeId = episodeId
            };

            _context.CharacterEpisodes.Remove(characterEpisode);
        }

        public async Task AddFriendship(int characterId, int friendId)
        {
            var friendshipCharacterToFriend = new Friendship { CharacterId = characterId, FriendId = friendId };
            var friendshipFriendToCharacter = new Friendship { CharacterId = friendId, FriendId = characterId };
            await _context.Friendships.AddAsync(friendshipCharacterToFriend);
            await _context.Friendships.AddAsync(friendshipFriendToCharacter);
        }

        public void DeleteFriendship(int characterId, int friendId)
        {
            var friendshipCharacterToFriend = new Friendship { CharacterId = characterId, FriendId = friendId };
            var friendshipFriendToCharacter = new Friendship { CharacterId = friendId, FriendId = characterId };
            _context.Friendships.Remove(friendshipCharacterToFriend);
            _context.Friendships.Remove(friendshipFriendToCharacter);
        }

        public async Task<IEnumerable<int>> GetFriendsIds(int id)
        {
            return await _context.Friendships
                .Where(f => f.CharacterId == id)
                .Select(f => f.FriendId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Episode>> GetEpisodesForCharacter(int id)
        {
            return await (from episode in _context.Episodes
                    join characterEpisode in _context.CharacterEpisodes on episode.Id equals characterEpisode.EpisodeId
                    where characterEpisode.CharacterId == id
                    select episode).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            var result = await _context.SaveChangesAsync() > 0;
            return result;
        }
    }
}