using System.Collections.Generic;

namespace BreakingNews.API.Models
{
    public class LatestPostsResponse
    {
        public List<Post> objects { get; set; }
        public Meta meta { get; set; }
    }
}
