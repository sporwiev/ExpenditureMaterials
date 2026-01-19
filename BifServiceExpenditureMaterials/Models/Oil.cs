using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class Oil
    {
        public int GetId() => Id;
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
