namespace KitchenSimulator.Models;

public class Ingredient
{
    public string Name { get; set; } = "";
    public string Quantity { get; set; } = ""; // Keeping it string because JSON says "500", not 500.0
    public string Unit { get; set; } = "";
}