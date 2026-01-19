using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Models
{
    public class Material
    {
        public int GetId() => Id;
        public int Id { get; set; }
        public string? Ячейка { get; set; }
        public string? ТипТраты { get; set; }
        public string? НомерЯчейки { get; set; }
        public string? Ответственный { get; set; }
        public string? Месяц { get; set; }
        public int? Год { get; set; }
        public string? oilCode { get; set; }
        public int? oil_id { get; set; }
        public string? subunit_id { get; set; }
        public Oil? Oil { get; set; }
        public int? filter_id { get; set; }
        public Filter? Filter { get; set; }
        public int? motor_id { get; set; }
        public Motors? Motor { get; set; }
        public int? grease_id { get; set; }
        public Grease? Grease { get; set; }
        public int? other_id { get; set; }
        public Other? Other { get; set; }
        public int? antifreeze_id { get; set; }
        public Antifreeze? Antifreeze { get; set; }

        public int? countmaterial_id { get; set; }
        public CountMaterials? CountMaterials { get; set; }
        public string? message { get; set; }
    }
}
