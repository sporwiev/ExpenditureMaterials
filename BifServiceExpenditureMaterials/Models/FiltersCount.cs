using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class FiltersCount
    {
        public int? GetId() => Id;
        public int? Id { get; set; }
        public int? is_editing { get; set; }

        public int? id_filter { get; set; }

        public Filter? Filter { get; set; }

        public double? count { get; set; }

        public string date { get; set; }
    }
}
