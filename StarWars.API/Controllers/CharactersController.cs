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

            _repository.Add(character);
            if (!await _repository.SaveAll())
                throw new Exception("Failed to add new character");

            if (characterToAddDto.FriendsIds.Any())
            {
                foreach (var friendId in characterToAddDto.FriendsIds)
                    await _repository.AddFriendship(character.Id, friendId);
            }

            if (characterToAddDto.EpisodesIds.Any())
            {
                foreach (var episodeId in characterToAddDto.EpisodesIds)
                    await _repository.AddCharacterEpisode(character.Id, episodeId);
            }

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
            var character = await _repository.Get(id, includeRelated: false);

            if (character == null)
                return NotFound();

            var friendsIds = await _repository.GetFriendsIds(id);

            foreach (var friendId in friendsIds)
                _repository.DeleteFriendship(character.Id, friendId);

            _repository.Delete(character);
            if (await _repository.SaveAll())
                return Ok(id);
            
            throw new Exception($"Failed to remove character {id}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CharacterToAddOrUpdateDto characterToUpdate)
        {
            var character = await _repository.Get(id, includeRelated: false);

            if (character == null)
                return NotFound();

            _mapper.Map<CharacterToAddOrUpdateDto, Character>(characterToUpdate, character);

            await this.UpdateFriendship(id, characterToUpdate.FriendsIds);
            await this.UpdateCharacterEpisodes(id, characterToUpdate.EpisodesIds);

            if (await _repository.SaveAll()) { 
                var updatedCharacter = await _repository.Get(character.Id);
                var characterToReturn = _mapper.Map<Character, CharacterDto>(updatedCharacter);

                return Ok(characterToReturn);
            }

            throw new Exception($"Failed to update character {id}");
        }

        private async Task UpdateFriendship(int characterId, ICollection<int> newFriendsIds)
        {
            var friendsIds = await _repository.GetFriendsIds(characterId);

            var friendsIdsToAdd = newFriendsIds.Except(friendsIds);
            var friendsIdsToRemove = friendsIds.Except(newFriendsIds);

            foreach (var friendId in friendsIdsToAdd)
                await _repository.AddFriendship(characterId, friendId);

            foreach (var friendId in friendsIdsToRemove)
                _repository.DeleteFriendship(characterId, friendId);
        }

        private async Task UpdateCharacterEpisodes(int characterId, ICollection<int> newEpisodesIds)
        {
            var episodes = await _repository.GetEpisodesForCharacter(characterId);
            var episodesIds = episodes.Select(e => e.Id);

            var episodesIdsToAdd = newEpisodesIds.Except(episodesIds);
            var episodesIdsToRemove = episodesIds.Except(newEpisodesIds);

            foreach (var episodeId in episodesIdsToAdd)
                await _repository.AddCharacterEpisode(characterId, episodeId);

            foreach (var episodeId in episodesIdsToRemove)
                _repository.DeleteCharacterEpisode(characterId, episodeId);
        }
    }
}