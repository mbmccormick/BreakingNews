using System.Collections.Generic;

namespace BreakingNews.API.Models
{
    public class OngoingTopicsResponse
    {
        public List<TopicItem> objects { get; set; }
        public Meta meta { get; set; }
    }
}
