using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class Other
    {
        public int GetId() => Id;
        public int Id { get; set; }
        public int? Clock { get; set; }
        public int? Mileage { get; set; }
    }
}
