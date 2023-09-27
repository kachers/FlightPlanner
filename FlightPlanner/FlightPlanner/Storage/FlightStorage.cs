using FlightPlanner.Exceptions;
using FlightPlanner.Exceptions;
using FlightPlanner.Models;

namespace FlightPlanner.Storage;

public class FlightStorage
{
    private static List<Flight> _flightStorage = new();
    private static int _id;

    public void AddFlight(Flight flight)
    {
        if (string.IsNullOrEmpty(flight.To.Country) ||
           string.IsNullOrEmpty(flight.To.City) ||
           string.IsNullOrEmpty(flight.To.AirportCode) ||
           string.IsNullOrEmpty(flight.From.Country) ||
           string.IsNullOrEmpty(flight.From.City) ||
           string.IsNullOrEmpty(flight.From.AirportCode) ||
           string.IsNullOrEmpty(flight.Carrier) ||
           string.IsNullOrEmpty(flight.DepartureTime) ||
           string.IsNullOrEmpty(flight.ArrivalTime))
        {
            throw new InvalidFlightException();
        }

        if (flight.From.AirportCode.ToLower().Trim().Equals(flight.To.AirportCode.ToLower().Trim()) &&
            flight.From.City.ToLower().Trim().Equals(flight.To.City.ToLower().Trim()) &&
            flight.From.Country.ToLower().Trim().Equals(flight.To.Country.ToLower().Trim()))
        {
            throw new InvalidFlightException();
        }

        if (DateTime.Parse(flight.ArrivalTime).CompareTo(DateTime.Parse(flight.DepartureTime)) <= 0)
        {
            throw new InvalidFlightException();
        }

        var duplicate = _flightStorage.Where(f =>
            f.To.AirportCode.Equals(flight.To.AirportCode) && 
            f.From.AirportCode.Equals(flight.From.AirportCode) && 
            f.DepartureTime.Equals(flight.DepartureTime)).ToList();

        if (duplicate.Count > 0)
        {
            throw new DuplicateFlightException();
        }

        flight.Id = _id++;
        _flightStorage.Add(flight);
    }

    public Flight GetFlight(int id)
    {
        var result = _flightStorage.FirstOrDefault(flight => flight.Id.Equals(id));

        return result ?? throw new InvalidFlightException();
    }

    public void DeleteFlight(int id)
    {
        var flight = _flightStorage.FirstOrDefault(flight => flight.Id.Equals(id));

        if (flight != null)
        {
            _flightStorage.RemoveAt(_flightStorage.IndexOf(flight));
        }
    }

    public List<Airport> SearchAirports(string search)
    {
        search = search.Trim();
        var searchTerm = search.ToLower();

        return _flightStorage
            .Select(flight => new Airport
            {
                Country = flight.From.Country,
                City = flight.From.City,
                AirportCode = flight.From.AirportCode
            })
            .Where(airport =>
                airport.AirportCode.ToLower().Contains(searchTerm) ||
                airport.City.ToLower().Contains(searchTerm) ||
                airport.Country.ToLower().Contains(searchTerm))
            .ToList();
    }

    public PageResult SearchFlights(SearchFlightRequest req)
    {
        if (string.IsNullOrEmpty(req.From) ||
            string.IsNullOrEmpty(req.To) ||
            string.IsNullOrEmpty(req.DepartureDate))
        {
            throw new InvalidFlightException();
        }

        if (req.From.ToLower().Trim().Equals(req.To.ToLower().Trim()))
        {
            throw new InvalidFlightException();
        }

        var flights = _flightStorage.Where(flight =>
            flight.From.AirportCode.Equals(req.From) &&
            flight.To.AirportCode.Equals(req.To) &&
            flight.DepartureTime.Contains(req.DepartureDate)).ToList();

        List<Flight> result = new();
        result.AddRange(flights);

        PageResult pageResult = new PageResult
        {
            Page = (int)Math.Floor((decimal)result.Count / 100),
            TotalItems = result.Count,
            Items = result
        };

        return pageResult;
    }

    public void Clear()
    {
        _flightStorage.Clear();
    }
}