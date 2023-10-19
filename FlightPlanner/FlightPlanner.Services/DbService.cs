using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Services
{
    public class DbService : IDbService
    {
        protected FlightPlannerDbContext _context;

        public DbService(FlightPlannerDbContext context)
        {
            _context = context;
        }
        public IQueryable<T> Query<T>() where T : Entity
        {
            return _context.Set<T>().AsQueryable();
        }

        public IQueryable<T> QueryById<T>(int id) where T : Entity
        {
            return _context.Set<T>().Where(e => e.Id == id);
        }

        public IEnumerable<T> Get<T>() where T : Entity
        {
            return _context.Set<T>().ToList();
        }

        public T? GetById<T>(int id) where T : Entity
        {
            var entity = _context.Set<T>()
                .Include("From")
                .Include("To")
                .SingleOrDefault(e => e.Id == id);

            return entity ?? throw new InvalidFlightException();
        }

        public void Create<T>(T entity) where T : Entity
        {
            if (entity is Flight flight)
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

                var duplicate = _context.Flights.Where(f =>
                    f.To.AirportCode.Equals(flight.To.AirportCode) &&
                    f.From.AirportCode.Equals(flight.From.AirportCode) &&
                    f.DepartureTime.Equals(flight.DepartureTime)).ToList();

                if (duplicate.Count > 0)
                {
                    throw new DuplicateFlightException();
                }
            }
            
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Update<T>(T entity) where T : Entity
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete<T>(T entity) where T : Entity
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
    };
}