using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitchenSimulator.Models;
using KitchenSimulator.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace KitchenSimulator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<RecipeViewModel> activeRecipes = new();

    [ObservableProperty]
    private ObservableCollection<string> recipeHistory = new();

    [RelayCommand]
    private async Task StartSimulationAsync()
    {
        ActiveRecipes.Clear();

        var data = await RecipeLoader.LoadKitchenDataAsync("Data/recipes.json");

        foreach (var recipe in data.Recipes)
        {
            var recipeSteps = recipe.Steps.Select(s => new RecipeStep
            {
                Step = s.Step,
                Duration = s.Duration
            }).ToArray();

            var vm = new RecipeViewModel(this, recipe.Name, recipeSteps);
            ActiveRecipes.Add(vm);
            _ = vm.StartCommand.ExecuteAsync(null); // Use the StartCommand instead of calling RunAsync directly
        }
    }
}