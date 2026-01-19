using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.material
{
    public class Oil
    {
        public List<Subunit> subUnits = new List<Subunit>();
        public string? type {  get; set; }
        public string? brend { get; set; }
        public string? vilocity { get; set; }
        public string? value { get; set; }

        

        
        public override string ToString()
        {
            string result = type + "_" + brend + "_" + vilocity;

            return result == "__" ? null : result;
        }
    }
}
    
