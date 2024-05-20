using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.Abstractions
{
    public interface IFileMenagmentModule
    {
        public List<dynamic> ReadFile(string filename);
        public void WriteFile<T>(string filename, IEnumerable<T> records);
    }
}
