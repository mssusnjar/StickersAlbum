using AuthenticationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;

namespace StickerAlbumFrontend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly FabricClient fabricClient;
        private readonly ILogger<AuthenticationController> logger;
        private readonly Uri serviceUri;

        public AuthenticationController(FabricClient fabricClient, ILogger<AuthenticationController> logger)
        {
            this.fabricClient = fabricClient;
            this.logger = logger;
            serviceUri = new Uri("fabric:/StickerAlbum/AuthenticationService");
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserCredentialsBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("RegisterUser - received request to register user with {username}", body.Username);

            var authService = GetAuthenticationService(body.Username);

            var response = await authService.Register(body.Username, body.Password);

            if (!response)
            {
                logger.LogInformation("RegisterUser - request to register user with {username} failed", body.Username);
                return BadRequest(response);
            }

            logger.LogInformation("RegisterUser - request to register user with {username} succeeded", body.Username);

            return Ok(response);
        }

        [HttpPut("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserCredentialsBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("LoginUser - received request to login user with {username}", body.Username);

            var authService = GetAuthenticationService(body.Username);

            var response = await authService.Login(body.Username, body.Password);

            if (!response)
            {
                logger.LogInformation("LoginUser - request to login user with {username} failed", body.Username);
                return Unauthorized(response);
            }

            logger.LogInformation("LoginUser - request to login user with {username} succeeded", body.Username);

            return Ok(response);
        }

        private long GetPartitionKey(string partitionKey)
        {
            return Char.ToUpper(partitionKey.First()) - 'A';
        }

        private IAuthenticationService GetAuthenticationService(string partitionKey)
        {
            return ServiceProxy.Create<IAuthenticationService>(serviceUri, new ServicePartitionKey(GetPartitionKey(partitionKey)));
        }
    }
}
