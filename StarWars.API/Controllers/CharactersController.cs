using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarWars.API.Repository;
using StarWars.API.Dtos;
using StarWars.API.Models;
using System.Collections.Generic;

namespace StarWars.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly IStarWarsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CharactersController> _logger;

        public CharactersController(IStarWarsRepository repository, IMapper mapper, ILogger<CharactersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<QueryResultDto<CharacterDto>> GetAll([FromQuery] CharacterQueryDto filterDto)
        {
            var filter = _mapper.Map<CharacterQueryDto, CharacterQuery>(filterDto);
            var queryResult = await _repository.GetAll(filter);
            return _mapper.Map<QueryResult<Character>, QueryResultDto<CharacterDto>>(queryResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var character = await _repository.Get(id);

            if (character == null)
                return NotFound();

            var characterToReturn = _mapper.Map<Character, CharacterDto>(character);

            return Ok(characterToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CharacterToAddOrUpdateDto characterToAddDto)
        {
            var character = _mapper.Map<CharacterToAddOrUpdateDto, Character>(characterToAddDto);

            character = this.ChangeCharacterFriends(character, characterToAddDto.FriendsIds);
            character = this.ChangeCharacterEpisodes(character, characterToAddDto.EpisodesIds);

            _repository.Add(character);

            if (await _repository.SaveAll())
            {
                var newCharacter = await _repository.Get(character.Id);
                var characterToReturn = _mapper.Map<Character, CharacterDto>(newCharacter);

                //Or we can simply return NoContent()
                return Ok(characterToReturn);
            }

            throw new Exception("Failed to add new character");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var character = await _repository.GetWithoutFriends(id);

            if (character == null)
                return NotFound();

            _repository.Delete(character);
            if (await _repository.SaveAll())
                return Ok(id);
            
            throw new Exception($"Failed to remove character {id}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CharacterToAddOrUpdateDto characterToUpdate)
        {
            var character = await _repository.Get(id, includeRelated: true);

            if (character == null)
                return NotFound();
            
            if (characterToUpdate.FriendsIds.Contains((character.Id)))
                throw new Exception("Character can not be friend to himself");

            _mapper.Map<CharacterToAddOrUpdateDto, Character>(characterToUpdate, character);

            character = this.ChangeCharacterFriends(character, characterToUpdate.FriendsIds);
            character = this.ChangeCharacterEpisodes(character, characterToUpdate.EpisodesIds);

            if (await _repository.SaveAll()) { 
                var updatedCharacter = await _repository.Get(character.Id);
                var characterToReturn = _mapper.Map<Character, CharacterDto>(updatedCharacter);

                return Ok(characterToReturn);
            }

            throw new Exception($"Failed to update character {id}");
        }

        private Character ChangeCharacterFriends(Character characterToChange, ICollection<int> newFriendsIds)
        {
            ICollection<Friendship> friends = new List<Friendship>();

            foreach (var friendId in newFriendsIds)
                friends.Add(new Friendship
                {
                    FriendId = friendId
                });

            ICollection<Friendship> friendsOfMine = new List<Friendship>();

            foreach (var friendId in newFriendsIds)
                friendsOfMine.Add(new Friendship
                {
                    CharacterId = friendId,
                    Friend = characterToChange
                });

            characterToChange.Friends = friends;
            characterToChange.FriendsOfMine = friendsOfMine;

            return characterToChange;
        }

        private Character ChangeCharacterEpisodes(Character characterToChange, ICollection<int> newEpisodesIds)
        {
            ICollection<CharacterEpisode> characterEpisodes = new List<CharacterEpisode>();

            foreach (var episodeId in newEpisodesIds)
                characterEpisodes.Add(new CharacterEpisode
                {
                    EpisodeId = episodeId
                });

            characterToChange.Episodes = characterEpisodes;

            return characterToChange;
        }
    }
}