using CsvHelper;
using Newtonsoft.Json;
using RonnieTest.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonnieTest.APIHandler
{
    public class DataHandler
    {
        public APIHandler Data { get; set; }

        public Dictionary<int,User> MyProperty { get; set; } = new Dictionary<int,User>();
        public int Length { get; set; } = 0;


        private readonly HttpClient _httpClient = new HttpClient();
        public DataHandler(APIHandler d) {
            Data = d;
        }
        public async Task LoadUsersAsync()
        {
            foreach (var apiEntry in Data.APIS)
            {
                string url = apiEntry.Key;
                int sourceId = apiEntry.Value;

                try
                {
                    string json = await _httpClient.GetStringAsync(url);
                    var handler = new UserHandler(json, sourceId);
                    var users = handler.ParseUsersFromJson();

                    foreach (var user in users)
                    {
                        MyProperty[++Length] = user;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{url}: {ex.Message}");
                }
            }
        }
        public async Task SaveToFileAsync(string folderPath,string formatInput)
        {
         

            var fileFormat = formatInput.Trim().ToUpper() switch
            {
                "JSON" => ExportFormat.JSON,
                "CSV" => ExportFormat.CSV,
                _ => throw new ArgumentException("Unsupported format")
            };

            string fileName = $"users_assiament.{fileFormat.ToString().ToLower()}";
            string fullPath = Path.Combine(folderPath, fileName);

            try
            {
                if (fileFormat == ExportFormat.JSON)
                {
                    var json = JsonConvert.SerializeObject(MyProperty.Values, Formatting.Indented);
                    await File.WriteAllTextAsync(fullPath, json);
                }
                else if (fileFormat == ExportFormat.CSV)
                {
                    await SaveCsvWithLibraryAsync(MyProperty.Values, fullPath);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }
        private async Task SaveCsvWithLibraryAsync(IEnumerable<User> users, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(users);
        }
    }
}