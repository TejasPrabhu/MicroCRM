using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/Platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            System.Console.WriteLine($"--> Getting commands for platform {platformId}");

            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commands = _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("commandId", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            System.Console.WriteLine($"--> Getting command {commandId} for platform {platformId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var command = _repo.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            System.Console.WriteLine($"--> Creating command {commandDto.CommandLine} for platform {platformId}");

            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDto);

            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandsForPlatform),
                                  new { platformId = platformId, commandId = commandReadDto.Id },
                                  commandReadDto);
        }

    }
}