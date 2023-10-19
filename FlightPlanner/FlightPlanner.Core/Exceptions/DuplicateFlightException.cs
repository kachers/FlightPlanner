namespace FlightPlanner.Core.Exceptions
{
    public class DuplicateFlightException : Exception
    {
        public DuplicateFlightException() : base("Flight already exists")
        {
        }
    }
}
