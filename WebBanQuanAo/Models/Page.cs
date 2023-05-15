using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanQuanAo.Models
{
    public partial class Page
    {
        public int PageId { get; set; }
        public string AuthorName { get; set; }
        public string Contents { get; set; }
        public string Thumb { get; set; }
        public string Title { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
