using Microsoft.AspNetCore.Identity;

namespace ProjectManager.Data
{
    public class UserProfile : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
