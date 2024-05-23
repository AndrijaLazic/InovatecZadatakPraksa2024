using CsvHelper.Configuration;
using DAL;
using DOMAIN.Models;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;


namespace TestProject.DAL_test
{
    public class CSVModuleTest
    {
        public string filename = "vozila.csv";
        public AppConfig appConfig;
        CsvConfiguration csvConfiguration;
        public CSVModule csvModule;
        public CSVModuleTest()
        {
            string text = File.ReadAllText(@"./AppConfig.json");
            this.appConfig = JsonSerializer.Deserialize<AppConfig>(text)!;
            this.csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = appConfig.csvConfig.Delimiter
            };
            csvModule = new CSVModule(appConfig, csvConfiguration);

        }

        [Fact]
        public void ReadFile_ReturnsArray()
        {
            //act
            List<dynamic> readRows = csvModule.ReadFile(filename);

            readRows.Should().NotBeNull();
            readRows.Count().Should().Be(7);

            ((IDictionary<String, object>)readRows[0]).Should().ContainKey("Id").WhoseValue.Should().Be("1");
            ((IDictionary<String, object>)readRows[0]).Should().ContainKey("TipVozila").WhoseValue.Should().Be("Automobil");
        }

        [Fact]
        public void ReadFile_ThrowException()
        {
            Action action = () => csvModule.ReadFile("123" + filename);
            action.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void WriteFile()
        {
            //act
            List<dynamic> readRows = csvModule.ReadFile(filename);
            csvModule.WriteFile("testFile.csv", readRows);
            List<dynamic> writenRows = csvModule.ReadFile("testFile.csv");
            writenRows.Should().NotBeNull();
            writenRows.Count().Should().Be(7);
        }
    }
}
