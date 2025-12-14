using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using KitchenSimulator.Models;
using System.Collections.Generic;

namespace KitchenSimulator.ViewModels;

public partial class RecipeViewModel : ViewModelBase
{
    [ObservableProperty] private string name;
    [ObservableProperty] private string currentStep;
    [ObservableProperty] private double progress;
    [ObservableProperty] private bool isRunning;
    [ObservableProperty] private bool isPaused;
    [ObservableProperty] private bool canStart = true;
    [ObservableProperty] private int timeLeftSeconds;
    [ObservableProperty] private string timeLeftFormatted = "N/A";
    [ObservableProperty] private string recipeEmoji;

    private readonly RecipeStep[] steps;
    private CancellationTokenSource? cancellationTokenSource;
    private TaskCompletionSource<bool>? pauseCompletionSource;
    private int currentStepIndex = 0;
    private int totalDuration = 0;
    private MainWindowViewModel _mainViewModel;


    public RecipeViewModel(MainWindowViewModel mainViewModel, string name, RecipeStep[] steps)
    {
        _mainViewModel = mainViewModel;
        Name = name;
        this.steps = steps;
        CurrentStep = "Ready to start";
        
        // Set appropriate emoji based on recipe name
        RecipeEmoji = GetEmojiForRecipe(name);
        
        // Calculate total duration for the recipe
        foreach (var step in steps)
        {
            totalDuration += step.Duration;
        }
        
        TimeLeftSeconds = totalDuration;
        UpdateTimeLeftFormatted();
    }

    private string GetEmojiForRecipe(string recipeName)
    {
        return recipeName.ToLower() switch
        {
            var n when n.Contains("pizza") => "🍕",
            var n when n.Contains("pasta") => "🍝",
            var n when n.Contains("chicken") => "🍗",
            var n when n.Contains("beef") => "🥩",
            var n when n.Contains("chocolate") || n.Contains("pancake") => "🥞",
            var n when n.Contains("seafood") || n.Contains("paella") => "🦐",
            var n when n.Contains("vegetable") || n.Contains("stir fry") => "🥦",
            var n when n.Contains("lamb") || n.Contains("curry") => "🍛",
            _ => "🍽️" // Default food emoji
        };
    }

    private void UpdateTimeLeftFormatted()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(TimeLeftSeconds);
        TimeLeftFormatted = timeSpan.TotalHours >= 1 
            ? $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s"
            : $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
    }

    [RelayCommand]
    public async Task StartAsync()
    {
        if (IsRunning) return;
        
        IsRunning = true;
        IsPaused = false;
        CanStart = false;
        
        cancellationTokenSource = new CancellationTokenSource();
        
        try
        {
            await RunAsync(cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            CurrentStep = "Recipe cancelled";
        }
        finally
        {
            IsRunning = false;
            IsPaused = false;
            CanStart = true;
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    public void Pause()
    {
        if (!IsRunning || IsPaused) return;
        
        IsPaused = true;
        pauseCompletionSource = new TaskCompletionSource<bool>();
        CurrentStep = $"Paused: {CurrentStep}";
    }

    [RelayCommand]
    public void Resume()
    {
        if (!IsRunning || !IsPaused) return;
        
        IsPaused = false;
        pauseCompletionSource?.SetResult(true);
        pauseCompletionSource = null;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        int total = steps.Length;
        int elapsedDuration = 0;
        
        for (int i = 0; i < total; i++)
        {
            // Check for cancellation
            if (cancellationToken.IsCancellationRequested)
                return;

            currentStepIndex = i;
            CurrentStep = steps[i].Step;
            Progress = i / (double)total * 100;

            // Handle pausing and wait for resume
            if (IsPaused)
            {
                await WaitForResumeAsync();
                // After resume, show the current step without "Paused:" prefix
                CurrentStep = steps[i].Step;
            }

            // Execute the delay with cancellation support
            int delayInMilliseconds = steps[i].Duration * 1000;
            await ExecuteDelayWithPauseSupport(steps[i].Duration, cancellationToken);
            
            elapsedDuration += steps[i].Duration;
            TimeLeftSeconds = totalDuration - elapsedDuration;
            UpdateTimeLeftFormatted();
        }

        CurrentStep = "Done!";
        Progress = 100;
        TimeLeftSeconds = 0;
        string recipeHistoryEntry = $"✔️ {RecipeEmoji} {Name} ×1  —  Completed at {DateTime.Now:HH:mm:ss}";
        _mainViewModel.RecipeHistory.Add(recipeHistoryEntry);
        UpdateTimeLeftFormatted();
    }

    private async Task WaitForResumeAsync()
    {
        if (pauseCompletionSource != null)
        {
            await pauseCompletionSource.Task;
        }
    }

    private async Task ExecuteDelayWithPauseSupport(int durationInSeconds, CancellationToken cancellationToken)
    {
        int elapsed = 0;
        int checkInterval = 1; // Check every second

        while (elapsed < durationInSeconds)
        {
            // If paused, wait for resume
            if (IsPaused)
            {
                await WaitForResumeAsync();
            }

            // Check for cancellation
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            // Wait for a second
            await Task.Delay(1000, cancellationToken);
            elapsed += checkInterval;
            
            // Update time left
            TimeLeftSeconds = totalDuration - (elapsed + GetPreviousStepsDuration());
            UpdateTimeLeftFormatted();
            
            // Update progress within current step
            double stepProgress = elapsed / (double)steps[currentStepIndex].Duration;
            double overallStepProgress = currentStepIndex / (double)steps.Length;
            Progress = (overallStepProgress + stepProgress / steps.Length) * 100;
        }
    }
    
    private int GetPreviousStepsDuration()
    {
        int duration = 0;
        for (int i = 0; i < currentStepIndex; i++)
        {
            duration += steps[i].Duration;
        }
        return duration;
    }
}