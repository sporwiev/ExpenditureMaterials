using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class CountMaterials
    {
        public int Id { get; set; }
        public string? count_oil { get; set; }
        public int? count_antifreeze { get; set; }
        public string? count_filter { get; set; }
        public int? count_grease { get; set; }
        public int? count_other_clock { get; set; }
        public int? count_other_milesage { get; set; }
        public string? count_motors { get; set; }

        public string? count_subunit { get; set; }
        public string? count_filterids { get; set; }

        public string? count_filtername { get; set; }
    }
}
