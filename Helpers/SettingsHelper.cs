using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACCess.Helpers
{
    public class SettingsHelper<T>
    {
        private readonly string _filePath;

        public SettingsHelper(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("File name must not be empty.");
            _filePath = Path.Combine(Environment.CurrentDirectory, filename);
        }

        public async Task<T?> LoadAsync() =>
            File.Exists(_filePath) ? JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(_filePath)) : default;

        public async Task SaveAsync(T settings) =>
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(settings));
    }
}
