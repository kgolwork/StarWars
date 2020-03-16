using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StarWars.API.Controllers;
using StarWars.API.Dtos;
using StarWars.API.Helpers;
using StarWars.API.Models;
using StarWars.API.Repository;

namespace StarWarsAPI.UnitTests
{
    public class SampleTests
    {
        private Mock<IStarWarsRepository> _repository;
        private IMapper _mapper;
        private ILogger<CharactersController> _logger;
        private CharactersController _controller;
        
        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IStarWarsRepository>();
            _logger = new Logger<CharactersController>(new LoggerFactory());
            
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);
            
            _controller = new CharactersController(_repository.Object, _mapper, _logger);
        }

        [Test]
        public async Task Get_CorrectCharacterId_CallGetFromRepository()
        {
            _repository.Setup(r => r.Get(1, true)).Returns(Task.FromResult(new Character
            {
                Name = "Darth Vader"
            }));
            
            var result = await _controller.Get(1);

            _repository.Verify(r => r.Get(1, true), Times.Once());
        }

        [Test]
        public async Task Get_CorrectCharacterIdAndIncludeFriends_ReturnCharacter()
        {
            _repository.Setup(r => r.Get(1, true)).Returns(Task.FromResult(new Character
            {
                Id = 1,
                Name = "Darth Vader",
                Friends = new HashSet<Friendship>()
            }));
            
            var result = await _controller.Get(1);
            
            Assert.NotNull(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            Assert.IsInstanceOf(typeof(CharacterDto), okResult.Value);
            var characterDto = okResult.Value as CharacterDto;
            
            Assert.AreEqual(1, characterDto.Id);
            Assert.AreEqual("Darth Vader", characterDto.Name);
        }
        
        [Test]
        public async Task Get_IncorrectCharacterId_ReturnNotFound()
        {
            var result = await _controller.Get(-1);
            
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }
    }
}