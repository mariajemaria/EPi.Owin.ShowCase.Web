using System.Collections.Generic;
using System.Security.Claims;

namespace EPi.Owin.Models.Inputs
{
    public class ClaimsInput : OwinInputBase
    {
        public List<Claim> Claims { get; set; }
    }
}
