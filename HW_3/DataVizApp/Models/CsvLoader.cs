using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using DataVizApp.Models;

namespace DataVizApp.Models;

public static class CsvLoader
{
    public static List<MusicData> LoadData(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });
            
            return csv.GetRecords<MusicData>().ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading CSV data: {ex.Message}");
            return new List<MusicData>();
        }
    }
}
