using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Reflection;
using AuthService.Contracts;
using AccessManager.Models.Responses;
using AccessManager.Models.DataModels;
using AccessManager.Models.Enum;
using AccessManager.Sso;
using AccessManager.Models.Requests.Auth;

namespace AuthService.Providers
{
    public partial class UserManager : IUserManagerProvider, ILoginProvider
    {
        private readonly UserCacheProvider _userCacheProvider;
        private readonly UserDbProvider _userDbProvider;
        private readonly TokenService _tokenService;

        public UserManager(UserCacheProvider userCacheProvider,
            UserDbProvider userDbProvider,
            TokenService tokenService)
        {
            _userCacheProvider = userCacheProvider;
            _userDbProvider = userDbProvider;
            _tokenService = tokenService;
        }

        public async Task<string> Register(RegisterUserRequest request)
        {
            if (!_userDbProvider.IsUniqueEmail(request.Email))
                throw new AmbiguousMatchException();

            var newUser = new UserModel
            {
                Id = request.EmployeeId,
                Email = request.Email,
                Password = request.Password,
                Role = request.Role ?? UserRole.Staff
            };

            await _userDbProvider.Add(newUser);
            await _userCacheProvider.Add(newUser);

            var token = _tokenService.GetToken(newUser);

            return token;
        }

        public async Task<string> Login(LoginRequest request)
        {
            UserModel userModel;

            try
            {
                userModel = await _userCacheProvider.Get(request.Email);
            }
            catch (InvalidOperationException ex)
            {
                if (_userDbProvider.IsUniqueEmail(request.Email))
                    throw new UnauthorizedAccessException();

                userModel = await _userDbProvider.Get(request.Email);

                await _userCacheProvider.Add(userModel);
            }

            if (userModel.Password != request.Password)
            {
                throw new UnauthorizedAccessException();
            }

            return _tokenService.GetToken(userModel);
        }

        public async Task<GetUserInfoResponse> GetUserInfo(Guid userId)
        {
            UserModel userModel;


            try
            {
                userModel = await _userCacheProvider.Get(userId);
            }
            catch (InvalidOperationException ex)
            {
                userModel = await _userDbProvider.Get(userId);

                await _userCacheProvider.Add(userModel);
            }

            return new GetUserInfoResponse
            {
                Email = userModel.Email,
                Role = userModel.Role
            };
        }

        public async Task SetUserRole(Guid userId, UserRole newRole)
        {
            await _userDbProvider.SetRole(userId, newRole);

            try
            {
                await _userCacheProvider.SetRole(userId, newRole);
            }
            catch (InvalidOperationException e)
            {
                var userDb = await _userDbProvider.Get(userId);

                await _userCacheProvider.Add(userDb);
            }
        }
    }
}
