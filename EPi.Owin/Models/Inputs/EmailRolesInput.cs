using System.Collections.Generic;

namespace EPi.Owin.Models.Inputs
{
    public class EmailRolesInput : OwinInputBase
    {
        public List<string> Roles { get; set; }
        public string Email { get; set; } 
    }
}