namespace EPi.Owin.Models.Inputs
{
    public class EmailRoleInput : OwinInputBase
    {
        public string Role { get; set; }
        public string Email { get; set; } 
    }
}