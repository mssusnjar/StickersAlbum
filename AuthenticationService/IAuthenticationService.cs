using Microsoft.ServiceFabric.Services.Remoting;

namespace AuthenticationService
{
    public interface IAuthenticationService : IService
    {
        public Task<bool> Register(string username, string password);

        public Task<bool> Login(string username, string password);
    }
}
