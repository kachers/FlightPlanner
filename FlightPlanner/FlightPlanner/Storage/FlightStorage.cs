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

    //public void DeleteFlight(int id) { }

    //public List<Airport> SearchAirports(string search) { }



    /*
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


    }
    */

    public void Clear()
    {
        _context.Flights.RemoveRange(_context.Flights);
        _context.Airports.RemoveRange(_context.Airports);
        _context.SaveChanges();
    }
}