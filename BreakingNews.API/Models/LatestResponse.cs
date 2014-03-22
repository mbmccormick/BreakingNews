using BreakingNews.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class LatestResponse
    {
        public List<Post> objects { get; set; }
        public Meta meta { get; set; }
    }

    public class Meta
    {
        public string next { get; set; }
        public int limit { get; set; }
    }
}
