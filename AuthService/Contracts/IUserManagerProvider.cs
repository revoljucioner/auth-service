using AccessManager.Models.Enum;
using AccessManager.Models.Requests.Auth;
using AccessManager.Models.Responses;
using System;
using System.Threading.Tasks;

namespace AuthService.Contracts
{
    public interface IUserManagerProvider
    {
        Task<string> Register(RegisterUserRequest request);

        Task<GetUserInfoResponse> GetUserInfo(Guid userId);

        Task SetUserRole(Guid userId, UserRole newRole);
    }
}
