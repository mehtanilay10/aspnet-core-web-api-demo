using System;

namespace DemoApp.Models.ApiModels.Auth
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public UserDetailsDto UserDetails { get; set; }
    }
}
