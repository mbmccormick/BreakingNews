using System.Collections.Generic;

namespace BreakingNews.API.Models
{
    public class PopularPostsResponse
    {
        public List<Post> items { get; set; }
        public List<Post> media { get; set; }
    }
}
