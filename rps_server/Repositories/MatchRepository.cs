
using rps_server.Data;
using rps_server.Entities;
using rps_server.Helpers;

// using BCrypt.Net;

namespace rps_server.Repository
{
    public interface IMatchRepository
    {
        // I will only be using findOrCreate initially, until I implement authentication
        IEnumerable<Match> GetAll();
        IEnumerable<Match> GetAllByUserId(int userId);
        Match GetById(int id);
        void Create(Match match);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly DataContext _context;

        public MatchRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Match> GetAllByUserId(int userId)
        {
            var matches = _context.Matches.Where(x => x.UserId == userId);
            if (matches == null)
                throw new KeyNotFoundException("User not found");
            return matches;
        }

        public IEnumerable<Match> GetAll()
        {
            return _context.Matches;
        }


        public Match GetById(int MatchId)
        {
            return getMatch(MatchId);
        }

        public void Create(Match match)
        {
            _context.Matches.Add(match);
            _context.SaveChanges();
        }


        public void Save()
        {
            _context.SaveChanges();
        }

        /*private bool disposed = false;*/

        //As a context object is a heavy object or you can say time-consuming object
        //So, once the operations are done we need to dispose of the same using Dispose method
        //The UserDBContext class inherited from DbContext class and the DbContext class
        //is Inherited from the IDisposable interface

        /*protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }*/

        /*public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }*/

        // helper methods

        private Match getMatch(int id)
        {
            var match = _context.Matches.Find(id);
            if (match == null)
                throw new KeyNotFoundException("Match not found");
            return match;
        }
    }
}
