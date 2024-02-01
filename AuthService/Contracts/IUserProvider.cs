using AccessManager.Models.DataModels;
using AccessManager.Models.Enum;
using System;
using System.Threading.Tasks;

namespace AuthService.Contracts
{
    public interface IUserProvider
    {
        Task Add(UserModel request);

        Task<UserModel> Get(Guid userId);

        Task<UserModel> Get(string email);

        Task SetRole(Guid userId, UserRole newRole);
    }
}
