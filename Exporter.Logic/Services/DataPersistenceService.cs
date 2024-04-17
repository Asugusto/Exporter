using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Exporter.Logic.Services
{
    public class DataPersistenceService
    {
        private const string ResultsDirectory = "Files";

        public DataPersistenceService()
        {
            Directory.CreateDirectory(ResultsDirectory);
        }

        public async Task SaveResultAsync(Guid id, string result)
        {
            string filePath = Path.Combine(ResultsDirectory, $"{id}.json");
            await File.WriteAllTextAsync(filePath, result);
        }

        public async Task<string> GetResultAsync(Guid id)
        {
            string filePath = Path.Combine(ResultsDirectory, $"{id}.json");
            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            else
            {
                throw new FileNotFoundException("Result file not found.");
            }
        }
    }
}
