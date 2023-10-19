using FlightPlanner.Exceptions;
using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Storage;

public class FlightStorage
{
    private readonly FlightPlannerDbContext _context;

    public FlightStorage(FlightPlannerDbContext context)
    {
        _context = context;
    }

    //public void AddFlight(Flight flight){ }

    //public Flight GetFlight(int id) { }

    public void DeleteFlight(int id)
    {
        var flight = _context.Flights.SingleOrDefault(flight => flight.Id == id);

        if (flight != null)
        {
            _context.Flights.Remove(flight);
            _context.SaveChanges();
        }
    }

    public List<Airport> SearchAirports(string search)
    {
        search = search.Trim();
        var searchTerm = search.ToLower();

        return _context.Airports
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

        var flights = _context.Flights.Where(flight =>
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
        _context.Flights.RemoveRange(_context.Flights);
        _context.Airports.RemoveRange(_context.Airports);
        _context.SaveChanges();
    }
}