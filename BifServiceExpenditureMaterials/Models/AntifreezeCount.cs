using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class AntifreezeCount
    {
        public int? GetId() => id;
        public int? id { get; set; }


        public int? is_editing { get; set; }

        public int? id_antifreeze { get; set; }

        public Antifreeze? Antifreeze { get; set; }

        public double? count { get; set; }

        public string date { get; set; }
    }
}
