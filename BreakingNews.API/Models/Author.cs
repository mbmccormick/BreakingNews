using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingNews.API.Models
{
    public class Author
    {
        public string first_name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string last_name { get; set; }
        public string resource_uri { get; set; }
        public bool is_staff { get; set; }
        public int id { get; set; }
    }
}
