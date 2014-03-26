using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingNews.API.Models
{
    public class Meta
    {
        public string previous { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
    }
}
