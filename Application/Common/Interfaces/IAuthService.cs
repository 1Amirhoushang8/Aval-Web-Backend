using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IAuthService
{
    Task<object> LoginAsync(string username, string password);
    Task<string> RegisterAsync(SignupRequest request);   
}