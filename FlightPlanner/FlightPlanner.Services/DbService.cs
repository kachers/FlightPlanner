using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;

namespace FlightPlanner.Services
{
    public class DbService : IDbService
    {
        public IQueryable<T> Query<T>() where T : Entity
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> QueryById<T>(int id) where T : Entity
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get<T>() where T : Entity
        {
            throw new NotImplementedException();
        }

        public void Create<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : Entity
        {
            throw new NotImplementedException();
        }
    };

    {

    }
}