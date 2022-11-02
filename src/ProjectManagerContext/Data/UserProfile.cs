using Microsoft.AspNetCore.Identity;

namespace ProjectManager.Data
{
    public class UserProfile : IdentityUser
    {
        public UserProfile()
        {
            Reports = new List<UserProfile>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid? LineManagerId { get; set; }
        public virtual UserProfile? LineManager { get; set; }

        public virtual IEnumerable<UserProfile> Reports { get; set; }
    }
}
