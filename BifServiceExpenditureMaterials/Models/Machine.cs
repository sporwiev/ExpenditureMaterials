using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class machine
    {
        public int GetId() => Id;
        public int Id { get; set; }
        public string? Code { get; set; }
        public int? Год { get; set; }
    }
}
