namespace FlightPlanner.Exceptions;

public class DuplicateFlightException : Exception
{
    public DuplicateFlightException() : base("Flight already exists")
    {
    }
}