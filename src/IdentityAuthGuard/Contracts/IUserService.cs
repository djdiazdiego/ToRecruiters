using Core.Head.Wrappers;
using IdentityAuthGuard.DTOs;

namespace IdentityAuthGuard.Contracts
{
    public interface IUserService
    {
        Task<Response> CreateAccountAsync(UserDTO userDTO);
        Task<Response> LoginAccountAsync(LoginDTO loginDTO);
        Task<Response> LogoutAsync(string email);
        Task<Response> RefreshToken(TokenDTO dto);
    }
}
