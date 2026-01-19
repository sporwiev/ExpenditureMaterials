using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class GreaseCount
    {
        public int? GetId() => id;
        public int? id { get; set; }


        public int? is_editing { get; set; }

        public int? id_grease { get; set; }

        public Grease? Grease { get; set; }

        public double? count { get; set; }

        public string date { get; set; }
    }
}
