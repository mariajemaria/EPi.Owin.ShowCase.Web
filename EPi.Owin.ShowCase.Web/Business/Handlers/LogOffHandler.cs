using System.Web;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Services;
using EPiServer.Web;

namespace EPi.Owin.ShowCase.Web.Business.Handlers
{
    public class LogOffHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            AuthenticationService.LogOff(new OwinInputBase { OwinContext = context.ContextBaseOrNull().GetOwinContext() });
            context.Response.Redirect("/Account");
        }
    }
}