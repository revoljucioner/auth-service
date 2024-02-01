using AccessManager.Models.Requests.Auth;
using System.Threading.Tasks;

namespace AuthService.Contracts
{
    public interface ILoginProvider
    {
        Task<string> Login(LoginRequest request);
    }
}
