using BreakingNews.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class PopularResponse
    {
        public List<Post> items { get; set; }
        public List<Post> media { get; set; }
    }
}
