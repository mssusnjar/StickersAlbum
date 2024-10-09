using System.Runtime.Serialization;

namespace AuthenticationService
{
    [DataContract]
    internal class UserCredentials
    {
        [DataMember]
        public required string Username { get; set; }

        [DataMember]
        public required string Password { get; set; }
    }
}
