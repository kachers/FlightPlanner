using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Models;

namespace FlightPlanner
{
    public class AutoMapperConfig
    {
        public static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AirportRequest, Airport>()
                    .ForMember(f =>
                        f.Id, opt =>
                        opt.Ignore())
                    .ForMember(a => 
                        a.AirportCode, opt=> 
                        opt.MapFrom(d => d.Airport));
                cfg.CreateMap<Airport, AirportRequest>()
                    .ForMember(a => a.Airport, opt => opt.MapFrom(d => d.AirportCode));
                cfg.CreateMap<FlightRequest, Flight>();
                cfg.CreateMap<Flight, FlightRequest>();
            });
            #if DEBUG
            configuration.AssertConfigurationIsValid();
            #endif
            var mapper = configuration.CreateMapper();
            return mapper;
        }
    }
}
