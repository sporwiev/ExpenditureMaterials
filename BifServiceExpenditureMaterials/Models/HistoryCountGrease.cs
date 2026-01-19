using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class HistoryCountGrease
    {
        public int? GetId() => Id;
        public int Id { get; set; }
        public int grease_id { get; set; }
        public Grease? Grease { get; set; }
        public double count { get; set; }
        public int is_current { get; set; }
        public string? date { get; set; }
    }
}
