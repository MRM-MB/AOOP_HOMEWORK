using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KitchenSimulator.Models;

namespace KitchenSimulator.Services;

public static class RecipeLoader
{
    public static async Task<KitchenData> LoadKitchenDataAsync(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<KitchenData>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data ?? new KitchenData();
    }
}