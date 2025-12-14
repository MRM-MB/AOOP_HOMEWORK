using System.Collections.Generic;

namespace KitchenSimulator.Models;

public class KitchenData
{
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Recipe> Recipes { get; set; } = new();
}