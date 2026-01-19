using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class Filter
    {
        public int GetId() => id;
        public int id { get; set; }
        public string? Name { get; set; }
    }
}
