using System.Fabric;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AuthenticationService
{
    internal sealed class AuthenticationService : StatefulService, IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(StatefulServiceContext context)
            : base(context)
        {
            logger = new LoggerFactory().CreateLogger<AuthenticationService>();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task<bool> Register(string username, string password)
        {
            logger.LogInformation("Trying to register a user with username {username}", username);

            var credentials = await StateManager.GetOrAddAsync<IReliableDictionary<string, UserCredentials>>("credentials");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var existingUser = await credentials.TryGetValueAsync(tx, username);

                if (existingUser.HasValue)
                {
                    logger.LogInformation("User with with username {username} already exists", username);
                    return false;
                }

                var newUser = new UserCredentials() { Username = username, Password = password };

                await credentials.AddAsync(tx, username, newUser);
                await tx.CommitAsync();

                logger.LogInformation("Registered a user with username {username}", username);

                return true;
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            logger.LogInformation("Trying to login a user with username {username}", username);

            var credentials = await StateManager.GetOrAddAsync<IReliableDictionary<string, UserCredentials>>("credentials");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await credentials.TryGetValueAsync(tx, username);

                if (!user.HasValue)
                {
                    logger.LogInformation("User with with username {username} does not exist", username);
                    return false;
                }

                if (user.Value.Password != password)
                {
                    logger.LogInformation("Credentials for username {username} are incorrect", username);
                    return false;
                }

                await tx.CommitAsync();

                logger.LogInformation("User with username {username} logged in successfully", username);

                return true;
            }
        }
    }
}
