using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            System.Console.WriteLine($"--> Calling Grpc Service {_config["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(
                $"--> Could start Grpc server: {e.Message}");
                return null;
            }
        }
    }
}