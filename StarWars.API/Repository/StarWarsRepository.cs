using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
                .Include(c => c.FriendsOfMine)
                .Include(c => c.Episodes)
                    .ThenInclude(e => e.Episode)
                .FirstOrDefaultAsync(c => c.Id == id);

            return character;
        }

        public async Task<Character> GetWithoutFriends(int id)
        {
            var character = await
                _context.Characters
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
            this.DeleteFriends(character);
            _context.Remove(character);
        }

        private void DeleteFriends(Character character)
        {
            var friendsIds = _context.Friendships.Where(ch => ch.CharacterId == character.Id).Select(c => c.FriendId);

            foreach (var friendId in friendsIds)
            {
                var friendshipCharacterToFriend = new Friendship { CharacterId = character.Id , FriendId = friendId };
                var friendshipFriendToCharacter = new Friendship { CharacterId = friendId, FriendId = character.Id};
                _context.Friendships.Remove(friendshipFriendToCharacter);
                _context.Friendships.Remove(friendshipCharacterToFriend);
            }
        }

        public async Task<bool> SaveAll()
        {
            var result = await _context.SaveChangesAsync() > 0;
            return result;
        }
    }
}