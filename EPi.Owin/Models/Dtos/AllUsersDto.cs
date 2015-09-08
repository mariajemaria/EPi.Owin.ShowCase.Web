using System.Collections.Generic;
using EPi.Owin.Identity;

namespace EPi.Owin.Models.Dtos
{
    public class AllUsersDto
    {
        public int TotalNoOfUsers { get; set; }
        public List<ApplicationUser> Users { get; set; } 
    }
}
