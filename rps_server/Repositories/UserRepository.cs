using rps_server.Data;
using rps_server.Entities;
using rps_server.Helpers;
// using BCrypt.Net;

namespace rps_server.Repository
{
    public interface IUserRepository
    {
        // I will only be using findOrCreate initially, until I implement authentication
        User GetByUserName(string username);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Create(User user);
        void Update(int id, User user);
        void Delete(int id);
        User Authenticate(string username);
        void Save();
    }

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public User GetByUserName(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User Authenticate(string username)
        {
            throw new System.NotImplementedException();
        }

        public User GetById(int UserID)
        {
            return getUser(UserID);
        }

        public void Create(User user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new AppException("Password is required");
            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");
            //hash password
            // user.PasswordHash = BCrypt.HashPassword(user.PasswordHash);

            //save to db
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(int id, User user)
        {
        _context.Users.Update(user);
        _context.SaveChanges();
        }

        public void Delete(int UserId)
        {
            User user = _context.Users.Find(UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        //As a context object is a heavy object or you can say time-consuming object
        //So, once the operations are done we need to dispose of the same using Dispose method
        //The UserDBContext class inherited from DbContext class and the DbContext class
        //is Inherited from the IDisposable interface

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
