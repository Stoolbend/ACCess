using ACCess.Helpers;
using ACCess.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACCess.Services
{
    public interface IGameService
    {
        Task<ServerList?> ReadServerListAsync(string? directory = null);
        Task SetServerListAsync(ServerList data, string? directory = null);
    }

    public class GameService : IGameService
    {
        public async Task<ServerList?> ReadServerListAsync(string? directory = null)
        {
            ServerList result = new ServerList();

            string filePath;
            if (string.IsNullOrWhiteSpace(directory))
                filePath = Path.Combine(FileHelper.GetDefaultDirectory(), "serverList.json");
            else
                filePath = Path.Combine(directory, "serverList.json");

            return File.Exists(filePath) ? JsonSerializer.Deserialize<ServerList>(await File.ReadAllTextAsync(filePath)) : null;
        }

        public async Task SetServerListAsync(ServerList data, string? directory = null)
        {
            string filePath;
            if (string.IsNullOrWhiteSpace(directory))
                filePath = Path.Combine(FileHelper.GetDefaultDirectory(), "serverList.json");
            else
                filePath = Path.Combine(directory, "serverList.json");

            // Serialize & write contents to file
            using (var sw = new StreamWriter(filePath))
            {
                await sw.WriteLineAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
        }
    }
}
