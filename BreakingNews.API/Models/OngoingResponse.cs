using BreakingNews.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingNews.API.Models
{
    public class OngoingResponse
    {
        public List<Topic> objects { get; set; }
        public Meta meta { get; set; }
    }
}
