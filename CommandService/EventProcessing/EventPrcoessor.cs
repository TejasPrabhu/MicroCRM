using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string message)
        {
            System.Console.WriteLine("--> Determining Event type");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    System.Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string PlatformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var PlatformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(PlatformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(PlatformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("-->Platform Already Exists");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}