using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class HistoryCountOil
    {
        public int? GetId() => Id;
        public int Id { get; set; }
        public int oil_id { get; set; }

        public Oil? Oil { get; set; }
        public double count { get; set; }
        public int is_current { get; set; }
        public string? date { get; set; }
    }
}
