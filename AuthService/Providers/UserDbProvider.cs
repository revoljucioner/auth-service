using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AuthService.Contracts;
using AccessManager.Models.Enum;
using AccessManager.Models.DataModels;
using AccessManager.Models.Database;

namespace AuthService.Providers
{
    public partial class UserDbProvider : IUserProvider
    {
        private readonly Func<DataContext> _dbContextFunc;

        public UserDbProvider(IConfiguration configuration)
        {
            _dbContextFunc = new Func<DataContext>(() => new DataContext(configuration["ConnectionStrings:UserDatabase"]));
        }

        public async Task SetRole(Guid userId, UserRole newRole)
        {
            using (var context = _dbContextFunc())
            {
                var userDb = context.User.Single(i => i.Id == userId);

                if (userDb.Role != newRole)
                    userDb.Role = newRole;

                context.Update(userDb);

                await context.SaveChangesAsync();
            }
        }

        public async Task Add(UserModel newUser)
        {
            var newDbUser = new UserDbModel
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Password = newUser.Password,
                Role = newUser.Role
            };

            using (var context = _dbContextFunc())
            {
                var createdUser = context.Add(newDbUser);

                await context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> Get(Guid userId)
        {
            using (var context = _dbContextFunc())
            {
                var dbUser = context.User.Single(i => i.Id == userId);

                return (UserModel)dbUser;
            }
        }

        public async Task<UserModel> Get(string email)
        {
            using (var context = _dbContextFunc())
            {
                var dbUser = context.User.Single(i => i.Email == email);

                return (UserModel)dbUser;
            }
        }

        public bool IsUniqueEmail(string email)
        {
            using (var context = _dbContextFunc())
            {
                return !context.User.Any(i => i.Email == email);
            }
        }
    }
}
