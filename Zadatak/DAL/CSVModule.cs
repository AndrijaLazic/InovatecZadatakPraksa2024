using CsvHelper;
using CsvHelper.Configuration;
using DOMAIN.Abstractions;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CSVModule : IFileMenagmentModule
    {
        private AppConfig _appConfig;
        private CsvConfiguration _csvConfiguration;

        public CSVModule(AppConfig appConfig, CsvConfiguration csvConfiguration)
        {
            _appConfig = appConfig;
            _csvConfiguration = csvConfiguration;
        }

        public List<dynamic> ReadFile(string filename)
        {
            List<dynamic> records = null;

            using (var reader = new StreamReader(_appConfig.csvConfig.fileDirectory + "/" + filename))
            using (var csv = new CsvReader(reader, _csvConfiguration))
            {
                records = csv.GetRecords<dynamic>().ToList();
            }
            return records;
        }

        public void WriteFile<T>(string filename, IEnumerable<T> records)
        {
            using (var writer = new StreamWriter(_appConfig.csvConfig.fileDirectory + "/" + filename))
            using (var csv = new CsvWriter(writer, _csvConfiguration))
            {
                csv.WriteRecords(records);
            }
        }
    }
}
