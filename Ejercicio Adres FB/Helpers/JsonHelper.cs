using Ejercicio_Adres_FB.Models;
using System.Text.Json;

namespace Ejercicio_Adres_FB.Helpers

{
    public class JsonHelper
    {
        private static readonly string _filePath = "Data/Data.json";


        // Método para leer todas las adquisiciones del archivo, incluyendo las eliminadas
        public static List<AdquisicionModel> ReadJsonFileAll()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(new List<AdquisicionModel>()));
            }

            string json = File.ReadAllText(_filePath);
            var acquisitions = JsonSerializer.Deserialize<List<AdquisicionModel>>(json) ?? new List<AdquisicionModel>();
            return acquisitions;
        }
        // Método para obtener solo las adquisiciones activas
        public static List<AdquisicionModel> ReadJsonFileActive()
        {
            var allAcquisitions = ReadJsonFileAll();
            var activeAcquisitions = allAcquisitions.Where(a => !a.Eliminado).ToList();
            return activeAcquisitions;
        }

        public static void WriteJsonFile(List<AdquisicionModel> acquisitions)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(acquisitions, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
