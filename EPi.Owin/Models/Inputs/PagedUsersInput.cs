
using System.Collections.Generic;

namespace EPi.Owin.Models.Inputs
{
    public class PagedUsersInput : OwinInputBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchForEmail { get; set; }
        public List<string> EmailsToCompareForDeletion { get; set; } 
    }
}
