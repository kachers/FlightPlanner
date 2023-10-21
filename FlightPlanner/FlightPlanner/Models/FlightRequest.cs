using FlightPlanner.Core.Models;

namespace FlightPlanner.Models
{
    public class FlightRequest : Entity
    {
        public AirportRequest From { get; set; }
        public AirportRequest To { get; set; }
        public string Carrier { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
    }
}
