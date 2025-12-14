using System.Collections.Generic;

namespace KitchenSimulator.Models;

public class Recipe
{
    public string Name { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public List<string> Equipment { get; set; } = new();
    public List<RecipeStep> Steps { get; set; } = new();
}