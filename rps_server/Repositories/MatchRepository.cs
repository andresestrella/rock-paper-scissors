using rps_server.Data;
using rps_server.Entities;
using rps_server.Helpers;

// using BCrypt.Net;

namespace rps_server.Repository
{
    public interface IMatchRepository
    {
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
