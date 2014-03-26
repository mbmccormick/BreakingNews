using BreakingNews.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingNews.API.Models
{
    public class LatestPostsResponse
    {
        public List<Post> objects { get; set; }
        public Meta meta { get; set; }
    }
}
