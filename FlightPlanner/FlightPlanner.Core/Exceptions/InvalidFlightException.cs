namespace FlightPlanner.Core.Exceptions
{
    public class InvalidFlightException : Exception
    {
        public InvalidFlightException() : base("Flight origin and destination cannot be the same")
        {
        }
    }
}
