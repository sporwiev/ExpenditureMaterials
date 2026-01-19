using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class HistoryCountFilters
    {
        public int? GetId() => Id;
        public int Id { get; set; }
        public int filter_id { get; set; }
        public Filter? Filter { get; set; }
        public double count { get; set; }
        public int is_current { get; set; }
        public string? date { get; set; }
    }
}
