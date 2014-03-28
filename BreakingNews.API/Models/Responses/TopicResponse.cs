using System.Collections.Generic;

namespace BreakingNews.API.Models
{
    public class TopicResponse
    {
        public List<Post> objects { get; set; }
        public Meta meta { get; set; }
    }
}
