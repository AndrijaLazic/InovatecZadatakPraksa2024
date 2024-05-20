using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Models
{
    public class AppConfig
    {
        public CsvConfig csvConfig { get; set; }
    }
    public class CsvConfig
    {
        public string Delimiter { get; set; }
        public string fileDirectory { get; set; }
    }
}
