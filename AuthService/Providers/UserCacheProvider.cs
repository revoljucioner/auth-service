using System.Linq;
using System;
using System.Threading.Tasks;
using BackendTestApplicationCore.Core.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using AuthService.Contracts;
using AccessManager.Models.DataModels;
using AccessManager.Models.Enum;

namespace AuthService.Providers
{
    public partial class UserCacheProvider : IUserProvider
    {
        private readonly RetentionDictionary<UserModel> _memoryCache;

        public UserCacheProvider(IConfiguration configuration)
        {
            _memoryCache = new RetentionDictionary<UserModel>(configuration.GetValue<TimeSpan>("CacheTtl"));
        }

        public async Task SetRole(Guid userId, UserRole newRole)
        {
            var user = await Get(userId);

            if (user.Role != newRole)
                user.Role = newRole;
        }

        public async Task Add(UserModel request)
        {
            _memoryCache.Add(request.Id.ToString(), request);
        }

        public Task<UserModel> Get(Guid userId)
        {
            var user = _memoryCache.Get<UserModel>(userId.ToString());

            return Task.FromResult(user);
        }

        public Task<UserModel> Get(string email)
        {
            var user = _memoryCache.Values.Single(i => i.Email == email);

            return Task.FromResult(user);
        }

        internal async Task<IEnumerable<UserModel>> GetCache()
        {
            return _memoryCache.Values;
        }

        internal async Task ResetCache()
        {
            _memoryCache.Reset();
        }
    }
}
