using Microsoft.Owin;

namespace EPi.Owin.Models.Inputs
{
    public class OwinInputBase
    {
        public IOwinContext OwinContext { get; set; }
    }
}
