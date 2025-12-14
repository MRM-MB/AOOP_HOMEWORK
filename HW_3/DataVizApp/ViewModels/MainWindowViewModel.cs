using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataVizApp.Models;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataVizApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Music Streaming Dashboard";
    
    public List<MusicData> MusicRecords { get; }
    
    public ISeries[] TopPlatformsSeries { get; private set; } = Array.Empty<ISeries>();
    public Axis[] TopPlatformsXAxes { get; private set; } = Array.Empty<Axis>();
    public Axis[] TopPlatformsYAxes { get; private set; } = Array.Empty<Axis>();

    public ISeries[] AgeGroupStreamingSeries { get; private set; } = Array.Empty<ISeries>();
    public Axis[] AgeGroupXAxes { get; private set; } = Array.Empty<Axis>();
    public Axis[] AgeGroupYAxes { get; private set; } = Array.Empty<Axis>();

    private ISeries[] _genrePieSeries = Array.Empty<ISeries>();
    public ISeries[] GenrePieSeries 
    {
        get => _genrePieSeries;
        private set
        {
            _genrePieSeries = value;
            OnPropertyChanged(nameof(GenrePieSeries));
        }
    }

    private ISeries[] _subscriptionTypePieSeries = Array.Empty<ISeries>();
    public ISeries[] SubscriptionTypePieSeries
    {
        get => _subscriptionTypePieSeries;
        private set
        {
            _subscriptionTypePieSeries = value;
            OnPropertyChanged(nameof(SubscriptionTypePieSeries));
        }
    }

    public ISeries[] TopArtistsSeries { get; private set; } = Array.Empty<ISeries>();
    public Axis[] TopArtistsXAxes { get; private set; } = Array.Empty<Axis>();
    public Axis[] TopArtistsYAxes { get; private set; } = Array.Empty<Axis>();

    // Chart selection - modified for adding back removed charts
    public ObservableCollection<string> AvailableCharts 
    { 
        get
        {
            var chartsList = new ObservableCollection<string>();
            
            // If there are no hidden charts (all charts are visible), return empty collection
            if (HiddenCharts.Count == 0)
                return chartsList;
            
            // Add the "Show All Charts" option at the top if there are multiple hidden charts
            if (HiddenCharts.Count > 1)
                chartsList.Add("Show All Charts");
            
            // Add individual hidden charts
            if (HiddenCharts.Contains("Top Streaming Platforms by Usage"))
                chartsList.Add("Top Streaming Platforms by Usage");
                
            if (HiddenCharts.Contains("Most Streamed Music Genres"))
                chartsList.Add("Most Streamed Music Genres");
                
            if (HiddenCharts.Contains("Most Popular Subscription Type"))
                chartsList.Add("Most Popular Subscription Type");
                
            if (HiddenCharts.Contains("Minutes Streamed per day by Age"))
                chartsList.Add("Minutes Streamed per day by Age");

            if (HiddenCharts.Contains("Top Artists by Listeners"))
                chartsList.Add("Top Artists by Listeners");
                
            return chartsList;
        }
    }

    // Track which charts are hidden from view
    private HashSet<string> _hiddenCharts = new HashSet<string>();
    private HashSet<string> HiddenCharts 
    { 
        get => _hiddenCharts;
        set
        {
            _hiddenCharts = value;
            OnPropertyChanged(nameof(AvailableCharts));
        }
    }

    [ObservableProperty]
    private string _selectedChart = string.Empty;

    [ObservableProperty]
    private bool _showPlatformsChart = true; // Set to true by default

    [ObservableProperty]
    private bool _showGenrePieChart = true; // Set to true by default

    [ObservableProperty]
    private bool _showSubscriptionTypePieChart = true; // Set to true by default

    [ObservableProperty]
    private bool _showAverageMinutesStreamedByAgeGroupChart = true; // Set to true by default

    [ObservableProperty]
    private bool _showTopArtistsChart = true; // Set to true by default

    [ObservableProperty]
    private int _topItemsToDisplay = 5; // Default value

    [ObservableProperty]
    private bool _showEmptyState;

    // Chart position tracking properties
    public ObservableCollection<ChartPosition> ChartPositions { get; } = new();

    public int TopPlatformsRow => ChartPositions.FirstOrDefault(p => p.ChartId == "TopPlatforms")?.Row ?? 0;
    public int TopPlatformsColumn => ChartPositions.FirstOrDefault(p => p.ChartId == "TopPlatforms")?.Column ?? 0;
    public int TopPlatformsRowSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "TopPlatforms")?.RowSpan ?? 1;
    public int TopPlatformsColumnSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "TopPlatforms")?.ColumnSpan ?? 1;

    public int GenrePieRow => ChartPositions.FirstOrDefault(p => p.ChartId == "GenrePie")?.Row ?? 0;
    public int GenrePieColumn => ChartPositions.FirstOrDefault(p => p.ChartId == "GenrePie")?.Column ?? 0;
    public int GenrePieRowSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "GenrePie")?.RowSpan ?? 1;
    public int GenrePieColumnSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "GenrePie")?.ColumnSpan ?? 1;

    public int SubscriptionTypePieRow => ChartPositions.FirstOrDefault(p => p.ChartId == "SubscriptionTypePie")?.Row ?? 0;
    public int SubscriptionTypePieColumn => ChartPositions.FirstOrDefault(p => p.ChartId == "SubscriptionTypePie")?.Column ?? 0;
    public int SubscriptionTypePieRowSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "SubscriptionTypePie")?.RowSpan ?? 1;
    public int SubscriptionTypePieColumnSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "SubscriptionTypePie")?.ColumnSpan ?? 1;

    public int AgeGroupStreamingRow => ChartPositions.FirstOrDefault(p => p.ChartId == "AgeGroupStreaming")?.Row ?? 0;
    public int AgeGroupStreamingColumn => ChartPositions.FirstOrDefault(p => p.ChartId == "AgeGroupStreaming")?.Column ?? 0;
    public int AgeGroupStreamingRowSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "AgeGroupStreaming")?.RowSpan ?? 1;
    public int AgeGroupStreamingColumnSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "AgeGroupStreaming")?.ColumnSpan ?? 1;

    public int TopArtistsRow => ChartPositions.FirstOrDefault(p => p.ChartId == "TopArtists")?.Row ?? 0;
    public int TopArtistsColumn => ChartPositions.FirstOrDefault(p => p.ChartId == "TopArtists")?.Column ?? 0;
    public int TopArtistsRowSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "TopArtists")?.RowSpan ?? 1;
    public int TopArtistsColumnSpan => ChartPositions.FirstOrDefault(p => p.ChartId == "TopArtists")?.ColumnSpan ?? 1;

    public MainWindowViewModel()
    {
        string projectRoot = AppContext.BaseDirectory;
        string filePath = Path.Combine(projectRoot, "..", "..", "..", "Data", "GlobalMusicStreaming.csv");

        // Load data
        MusicRecords = File.Exists(filePath) ? CsvLoader.LoadData(filePath) : new List<MusicData>();
        
        // Initialize charts (data will be ready even if charts aren't visible)
        InitializeTopPlatformsChart();
        InitializeGenrePieChart();
        InitializeSubscriptionPieChart();
        InitializeAverageMinutesStreamedByAgeGroup();
        InitializeTopArtistsChart();
        
        // Start with all charts hidden
        ShowPlatformsChart = false;
        ShowGenrePieChart = false;
        ShowSubscriptionTypePieChart = false;
        ShowAverageMinutesStreamedByAgeGroupChart = false;
        ShowTopArtistsChart = false;
        
        // Add all charts to the hidden charts collection
        HiddenCharts.Add("Top Streaming Platforms by Usage");
        HiddenCharts.Add("Most Streamed Music Genres");
        HiddenCharts.Add("Most Popular Subscription Type");
        HiddenCharts.Add("Minutes Streamed per day by Age");
        HiddenCharts.Add("Top Artists by Listeners");
        
        // Show empty state since no charts are visible
        ShowEmptyState = true;

        // Initialize chart positions after setting visibility
        UpdateChartPositions();
    }

    partial void OnSelectedChartChanged(string value)
    {
        // Now we use this to facilitate adding charts back through the UI
    }

    partial void OnTopItemsToDisplayChanged(int value)
    {
        // Re-initialize bar chart and update UI
        InitializeTopPlatformsChart();
        OnPropertyChanged(nameof(TopPlatformsSeries));
        OnPropertyChanged(nameof(TopPlatformsXAxes));
        OnPropertyChanged(nameof(TopPlatformsYAxes));

        _genrePieSeries = Array.Empty<ISeries>();
        OnPropertyChanged(nameof(GenrePieSeries));

        InitializeAverageMinutesStreamedByAgeGroup();   
        OnPropertyChanged(nameof(AgeGroupStreamingSeries));
        OnPropertyChanged(nameof(AgeGroupXAxes));
        OnPropertyChanged(nameof(AgeGroupYAxes));
        
        InitializeTopArtistsChart();
        OnPropertyChanged(nameof(TopArtistsSeries));
        OnPropertyChanged(nameof(TopArtistsXAxes));
        OnPropertyChanged(nameof(TopArtistsYAxes));
        
        Avalonia.Threading.Dispatcher.UIThread.Post(() => {
            // New chart series
            InitializeGenrePieChart();
            OnPropertyChanged(nameof(GenrePieSeries));
            
            // Schedule another update to ensure rendering completes
            Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                OnPropertyChanged(nameof(GenrePieSeries));
            }, Avalonia.Threading.DispatcherPriority.Render);
        });
    }

    partial void OnShowPlatformsChartChanged(bool value)
    {
        UpdateChartPositions();
    }

    partial void OnShowGenrePieChartChanged(bool value)
    {
        UpdateChartPositions();
    }

    partial void OnShowSubscriptionTypePieChartChanged(bool value)
    {
        UpdateChartPositions();
    }

    partial void OnShowAverageMinutesStreamedByAgeGroupChartChanged(bool value)
    {
        UpdateChartPositions();
    }

    partial void OnShowTopArtistsChartChanged(bool value)
    {
        UpdateChartPositions();
    }

    private void UpdateChartVisibility()
    {
        // Check how many charts are hidden
        int hiddenCount = HiddenCharts.Count;
        
        // If all charts are hidden, show the empty state
        ShowEmptyState = hiddenCount >= 5;
        
        // Update visibility of each chart
        ShowPlatformsChart = !HiddenCharts.Contains("Top Streaming Platforms by Usage");
        ShowGenrePieChart = !HiddenCharts.Contains("Most Streamed Music Genres");
        ShowSubscriptionTypePieChart = !HiddenCharts.Contains("Most Popular Subscription Type");
        ShowAverageMinutesStreamedByAgeGroupChart = !HiddenCharts.Contains("Minutes Streamed per day by Age");
        ShowTopArtistsChart = !HiddenCharts.Contains("Top Artists by Listeners");
    }
    
    public void UpdateChartPositions()
    {
        // Clear existing positions
        ChartPositions.Clear();
        
        // Get currently visible charts in a defined order
        var visibleCharts = new List<string>();
        
        // Define chart priority order - first visible chart gets the first position
        string[] chartOrder = new[] { 
            "TopPlatforms", 
            "GenrePie", 
            "SubscriptionTypePie", 
            "AgeGroupStreaming", 
            "TopArtists" 
        };
        
        // Add charts in the specified order
        foreach (string chartId in chartOrder)
        {
            bool isVisible = chartId switch
            {
                "TopPlatforms" => ShowPlatformsChart,
                "GenrePie" => ShowGenrePieChart,
                "SubscriptionTypePie" => ShowSubscriptionTypePieChart,
                "AgeGroupStreaming" => ShowAverageMinutesStreamedByAgeGroupChart,
                "TopArtists" => ShowTopArtistsChart,
                _ => false
            };
            
            if (isVisible)
                visibleCharts.Add(chartId);
        }
        
        int count = visibleCharts.Count;
        
        // Assign positions based on the number of visible charts
        switch (count)
        {
            case 1: // One chart - full screen
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[0], 
                    Row = 0, 
                    Column = 0, 
                    RowSpan = 2, 
                    ColumnSpan = 3 
                });
                break;
                
            case 2: // Two charts - half and half horizontally
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[0], 
                    Row = 0, 
                    Column = 0,
                    ColumnSpan = 1,
                    RowSpan = 2
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[1], 
                    Row = 0, 
                    Column = 1,
                    ColumnSpan = 2,
                    RowSpan = 2
                });
                break;
                
            case 3: // Three charts - two on top, one on bottom
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[0], 
                    Row = 0, 
                    Column = 0,
                    ColumnSpan = 1
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[1], 
                    Row = 0, 
                    Column = 1,
                    ColumnSpan = 2
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[2], 
                    Row = 1, 
                    Column = 0,
                    ColumnSpan = 3
                });
                break;
                
            case 4: // Four charts - one in each corner (2x2 grid)
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[0], 
                    Row = 0, 
                    Column = 0 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[1], 
                    Row = 0, 
                    Column = 1,
                    ColumnSpan = 2
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[2], 
                    Row = 1, 
                    Column = 0 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[3], 
                    Row = 1, 
                    Column = 1,
                    ColumnSpan = 2
                });
                break;
                
            case 5: // Five charts - current layout (3x2 grid with 5 positions)
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[0], 
                    Row = 0, 
                    Column = 0 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[1], 
                    Row = 0, 
                    Column = 1 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[2], 
                    Row = 0, 
                    Column = 2 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[3], 
                    Row = 1, 
                    Column = 0 
                });
                ChartPositions.Add(new ChartPosition { 
                    ChartId = visibleCharts[4], 
                    Row = 1, 
                    Column = 1 
                });
                break;
        }
        
        // Debug output to verify chart positions
        Console.WriteLine($"Visible charts: {count}");
        foreach (var position in ChartPositions)
        {
            Console.WriteLine($"Chart {position.ChartId}: Row={position.Row}, Col={position.Column}, RowSpan={position.RowSpan}, ColSpan={position.ColumnSpan}");
        }
        
        // Notify UI for each individual position property
        OnPropertyChanged(nameof(ChartPositions));
        OnPropertyChanged(nameof(TopPlatformsRow));
        OnPropertyChanged(nameof(TopPlatformsColumn));
        OnPropertyChanged(nameof(TopPlatformsRowSpan));
        OnPropertyChanged(nameof(TopPlatformsColumnSpan));
        
        OnPropertyChanged(nameof(GenrePieRow));
        OnPropertyChanged(nameof(GenrePieColumn));
        OnPropertyChanged(nameof(GenrePieRowSpan));
        OnPropertyChanged(nameof(GenrePieColumnSpan));
        
        OnPropertyChanged(nameof(SubscriptionTypePieRow));
        OnPropertyChanged(nameof(SubscriptionTypePieColumn));
        OnPropertyChanged(nameof(SubscriptionTypePieRowSpan));
        OnPropertyChanged(nameof(SubscriptionTypePieColumnSpan));
        
        OnPropertyChanged(nameof(AgeGroupStreamingRow));
        OnPropertyChanged(nameof(AgeGroupStreamingColumn));
        OnPropertyChanged(nameof(AgeGroupStreamingRowSpan));
        OnPropertyChanged(nameof(AgeGroupStreamingColumnSpan));
        
        OnPropertyChanged(nameof(TopArtistsRow));
        OnPropertyChanged(nameof(TopArtistsColumn));
        OnPropertyChanged(nameof(TopArtistsRowSpan));
        OnPropertyChanged(nameof(TopArtistsColumnSpan));
    }

    private bool GetChartPosition(string chartId, out int row, out int column, out int rowSpan, out int columnSpan)
    {
        var position = ChartPositions.FirstOrDefault(p => p.ChartId == chartId);
        if (position != null)
        {
            row = position.Row;
            column = position.Column;
            rowSpan = position.RowSpan;
            columnSpan = position.ColumnSpan;
            return true;
        }
        
        row = 0;
        column = 0;
        rowSpan = 1;
        columnSpan = 1;
        return false;
    }

    private void InitializeTopPlatformsChart()
    {
        try
        {
            // Group by streaming platform and count occurrences to determine usage
            var platformUsage = MusicRecords
                .Where(record => !string.IsNullOrEmpty(record.StreamingPlatform))
                .GroupBy(record => record.StreamingPlatform)
                .Select(group => new { Platform = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .Take(TopItemsToDisplay) // Show only the top N countries
                .ToList(); // Get all platforms first
            
            if (platformUsage.Count == 0)
            {
                return;
            }
            
            // Color palette with brighter colors
            var colors = new SKColor[] 
            {
                new SKColor(0, 116, 217),     // Bright Blue
                new SKColor(255, 65, 54),     // Bright Red
                new SKColor(46, 204, 64),     // Bright Green
                new SKColor(177, 13, 201),    // Vibrant Purple
                new SKColor(255, 133, 27),    // Bright Orange
                new SKColor(57, 204, 204),    // Teal
                new SKColor(250, 128, 114),   // Salmon
                new SKColor(240, 230, 140),   // Khaki
                new SKColor(152, 251, 152),   // Pale Green
                new SKColor(135, 206, 235),   // Sky Blue
                new SKColor(221, 160, 221),   // Plum
                new SKColor(255, 182, 193),   // Light Pink
                new SKColor(255, 215, 0),     // Gold
                new SKColor(176, 196, 222),   // Light Steel Blue
                new SKColor(255, 192, 203)    // Pink
            };
            
            // Create bar chart series with thicker columns and bouncy animation
            TopPlatformsSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = platformUsage.Select(x => x.Count).ToArray(),
                    Name = "Number of Users",
                    Fill = new LinearGradientPaint(
                        new[] { colors[0], colors[0].WithAlpha(120) },
                        new SKPoint(0, 0), new SKPoint(0, 1)
                    ),
                    Stroke = new SolidColorPaint(colors[0].WithAlpha(220), 2), // Thicker stroke
                    MaxBarWidth = 70, // Make columns thicker
                    Padding = 5,      // Reduce padding between bars
                    DataLabelsSize = 14,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => point.Model.ToString(),
                    
                    // Bouncy animation Yes 😂!
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1500),
                    EasingFunction = EasingFunctions.BounceOut
                }
            };
            
            // Configure X Axis with animations and styling
            TopPlatformsXAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Platform",
                    Labels = platformUsage.Select(x => x.Platform).ToArray(),
                    LabelsRotation = 45,
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200)
                }
            };
            
            // Configure Y Axis with animations and styling
            TopPlatformsYAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Number of Users",
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200),
                    MinLimit = 0
                }
            };
        }
        catch
        {
            Console.WriteLine("Error creating top platforms chart");
        }
    }
    
    private void InitializeGenrePieChart()
    {
        try
        {
            // Extract genre data directly from the "Top Genre" column (5th column)
            var genreCounts = MusicRecords
                .Where(record => !string.IsNullOrEmpty(record.TopGenre))
                .GroupBy(record => record.TopGenre)
                .Select(group => new GenreData { 
                    Genre = group.Key, 
                    Count = group.Count() 
                })
                .OrderByDescending(g => g.Count)
                .Take(TopItemsToDisplay) // Show only the top N countries
                .ToList(); // Get all genres first

            foreach (var genre in genreCounts)
            {
                Console.WriteLine($"Genre: {genre.Genre}, Count: {genre.Count}");
            }
            
            if (genreCounts.Count == 0)
            {
                GenrePieSeries = Array.Empty<ISeries>();
                return;
            }
            
            // Vibrant colors for the pie chart
            var colors = new SKColor[]
            {
                new SKColor(255, 99, 132),    // Red
                new SKColor(54, 162, 235),    // Blue
                new SKColor(255, 206, 86),    // Yellow
                new SKColor(75, 192, 192),    // Teal
                new SKColor(153, 102, 255),   // Purple
                new SKColor(255, 159, 64),    // Orange
                new SKColor(46, 204, 113),    // Green
                new SKColor(236, 64, 122)     // Pink
            };

            // Create a list of pie series, one for each genre
            var pieSeries = new List<ISeries>();
            int totalCount = genreCounts.Sum(x => x.Count);
            
            // Make the animation slower for better visualization
            var animationSpeed = TimeSpan.FromMilliseconds(1200);
            
            for (int i = 0; i < genreCounts.Count; i++)
            {
                // Calculate percentage for display
                double percentage = (double)genreCounts[i].Count * 100 / totalCount;
                
                pieSeries.Add(new PieSeries<int>
                {
                    Values = new[] { genreCounts[i].Count },
                    Name = $"{genreCounts[i].Genre} ({Math.Round(percentage, 1)}%)",
                    Fill = new SolidColorPaint(colors[i % colors.Length]),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 14,
                    DataLabelsFormatter = point => $"{Math.Round(percentage, 1)}%",
                    AnimationsSpeed = animationSpeed, // Slower animation
                    EasingFunction = EasingFunctions.BounceOut // Return to bouncy animation
                });
            }
            
            GenrePieSeries = pieSeries.ToArray();
        }
        catch
        {            
            Console.WriteLine("Error initializing genre pie chart");
            GenrePieSeries = Array.Empty<ISeries>();
        }
    }

    private void InitializeSubscriptionPieChart()
    {
        try
        {
            // Group by subscription type only
            var subscriptionCounts = MusicRecords
                .Where(record => !string.IsNullOrEmpty(record.SubscriptionType))
                .GroupBy(record => record.SubscriptionType)
                .Select(group => new
                {
                    Subscription = group.Key,
                    Count = group.Count()
                })
                .ToList();

            if (subscriptionCounts.Count == 0)
            {
                SubscriptionTypePieSeries = Array.Empty<ISeries>();
                return;
            }

            // Separate colors for Free and Premium
            var freeColor = new SKColor(54, 162, 235);  // Blue
            var premiumColor = new SKColor(255, 99, 132); // Red

            // Create a list of pie series
            var pieSeries = new List<ISeries>();
            int total = subscriptionCounts.Sum(s => s.Count);

            foreach (var subscription in subscriptionCounts)
            {
                double percentage = (double)subscription.Count * 100 / total;
                pieSeries.Add(new PieSeries<int>
                {
                    Values = new[] { subscription.Count },
                    Name = subscription.Subscription + " (" + Math.Round(percentage, 1) + "%)",
                    Fill = new SolidColorPaint(subscription.Subscription == "Free" ? freeColor : premiumColor),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 14,
                    DataLabelsFormatter = point => Math.Round(percentage, 1) + "%",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200),
                    EasingFunction = EasingFunctions.BounceOut
                });
            }

            SubscriptionTypePieSeries = pieSeries.ToArray();
        }
        catch
        {
            Console.WriteLine("Error initializing subscription pie chart");
            SubscriptionTypePieSeries = Array.Empty<ISeries>();
        }
    }

    private void InitializeAverageMinutesStreamedByAgeGroup()
    {
        try
        {
            var ageGroups = new List<(int Min, int Max, string Label)>
            {
                (18, 25, "18-25"),
                (26, 33, "26-33"),
                (34, 40, "36-40"),
                (41, 56, "46-56"),
                (57, int.MaxValue, "57+")
            };

            var ageGroupAverages = ageGroups
                .Select(group => new
                {
                    AgeRange = group.Label,
                    AverageMinutes = MusicRecords
                        .Where(record => record.Age >= group.Min && record.Age <= group.Max)
                        .Select(record => record.MinutesStreamedPerDay)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderBy(g => g.AgeRange) 
                .Take(TopItemsToDisplay) // Show only the top N countries
                .ToList();

            // Check if there is no data
            if (ageGroupAverages.Count == 0)
            {
                AgeGroupStreamingSeries = Array.Empty<ISeries>();
                return;
            }

            // Create a single column series with average minutes per day
            AgeGroupStreamingSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = ageGroupAverages.Select(g => g.AverageMinutes).ToArray(),
                    Name = "Avg Minutes",
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 14,
                    DataLabelsFormatter = point => Math.Round(point.Model, 1) + " min",
                    Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(new SKColor(54, 162, 235)), // Blue
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200),
                    EasingFunction = EasingFunctions.BounceOut
                }
            };

            // Configure X Axis with animations and styling
            AgeGroupXAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Age range",
                    Labels = ageGroupAverages.Select(g => g.AgeRange).ToArray(),
                    LabelsRotation = 0,
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200)
                }
            };

            AgeGroupYAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Avg minutes per day",
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200),
                    MinLimit = 0
                }
            };
        }
        catch
        {
            Console.WriteLine("Error initializing age group chart");
            AgeGroupStreamingSeries = Array.Empty<ISeries>();
        }
    }

    private void InitializeTopArtistsChart()
    {
        try
        {
            // Group by most played artist and count occurrences
            var artistCounts = MusicRecords
                .Where(record => !string.IsNullOrEmpty(record.MostPlayedArtist))
                .GroupBy(record => record.MostPlayedArtist)
                .Select(group => new { Artist = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .Take(TopItemsToDisplay)
                .ToList();
            
            if (artistCounts.Count == 0)
            {
                return;
            }
            
            // Use a purple color scheme for this chart
            var color = new SKColor(153, 102, 255); // Purple
            
            // Create bar chart series
            TopArtistsSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = artistCounts.Select(x => x.Count).ToArray(),
                    Name = "Number of Listeners",
                    Fill = new LinearGradientPaint(
                        new[] { color, color.WithAlpha(120) },
                        new SKPoint(0, 0), new SKPoint(0, 1)
                    ),
                    Stroke = new SolidColorPaint(color.WithAlpha(220), 2),
                    MaxBarWidth = 70,
                    Padding = 5,
                    DataLabelsSize = 14,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => point.Model.ToString(),
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1500),
                    EasingFunction = EasingFunctions.BounceOut
                }
            };
            
            // Configure X Axis
            TopArtistsXAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Artist",
                    Labels = artistCounts.Select(x => x.Artist).ToArray(),
                    LabelsRotation = 45,
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200)
                }
            };
            
            // Configure Y Axis
            TopArtistsYAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Number of Listeners",
                    LabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1200),
                    MinLimit = 0
                }
            };
        }
        catch
        {
            Console.WriteLine("Error creating top artists chart");
        }
    }

    // Add a simple class to hold genre data
    private class GenreData
    {
        public string Genre { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ChartPosition
    {
        public string ChartId { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;
    }

    [RelayCommand]
    public void RemoveChart(string chartId)
    {
        string? chartToRemove = null;
        
        switch (chartId)
        {
            case "TopPlatforms":
                chartToRemove = "Top Streaming Platforms by Usage";
                ShowPlatformsChart = false;
                break;
            case "GenrePie":
                chartToRemove = "Most Streamed Music Genres";
                ShowGenrePieChart = false;
                break;
            case "SubscriptionTypePie":
                chartToRemove = "Most Popular Subscription Type";
                ShowSubscriptionTypePieChart = false;
                break;
            case "AgeGroupStreaming":
                chartToRemove = "Minutes Streamed per day by Age";
                ShowAverageMinutesStreamedByAgeGroupChart = false;
                break;
            case "TopArtists":
                chartToRemove = "Top Artists by Listeners";
                ShowTopArtistsChart = false;
                break;
        }

        // Mark the chart as hidden if it was found
        if (chartToRemove != null)
        {
            HiddenCharts.Add(chartToRemove);
            OnPropertyChanged(nameof(AvailableCharts));
            
            // Show empty state if all charts are hidden
            ShowEmptyState = HiddenCharts.Count >= 5;
        }

        // Update chart positions after changing visibility
        UpdateChartPositions();
    }

    [RelayCommand]
    public void ShowChart(string chartName)
    {
        if (string.IsNullOrEmpty(chartName)) return;
        
        // Special case for "Show All Charts"
        if (chartName == "Show All Charts")
        {
            // Show all charts
            ShowPlatformsChart = true;
            ShowGenrePieChart = true;
            ShowSubscriptionTypePieChart = true;
            ShowAverageMinutesStreamedByAgeGroupChart = true;
            ShowTopArtistsChart = true;
            
            // Clear all charts from hidden collection
            HiddenCharts.Clear();
            
            // Hide empty state since charts are now visible
            ShowEmptyState = false;
            
            // Update available charts
            OnPropertyChanged(nameof(AvailableCharts));
            
            // Clear selection
            SelectedChart = string.Empty;
            
            // Update chart positions after changing visibility
            UpdateChartPositions();
            
            return;
        }
        
        // Regular case for individual charts
        if (HiddenCharts.Contains(chartName))
        {
            HiddenCharts.Remove(chartName);
            OnPropertyChanged(nameof(AvailableCharts));
            
            // Update individual chart visibility
            switch (chartName)
            {
                case "Top Streaming Platforms by Usage":
                    ShowPlatformsChart = true;
                    break;
                case "Most Streamed Music Genres":
                    ShowGenrePieChart = true;
                    break;
                case "Most Popular Subscription Type":
                    ShowSubscriptionTypePieChart = true;
                    break;
                case "Minutes Streamed per day by Age":
                    ShowAverageMinutesStreamedByAgeGroupChart = true;
                    break;
                case "Top Artists by Listeners":
                    ShowTopArtistsChart = true;
                    break;
            }
            
            // Hide empty state when at least one chart is visible
            ShowEmptyState = false;
            
            // Clear selection
            SelectedChart = string.Empty;
            
            // Update chart positions after changing visibility
            UpdateChartPositions();
        }
    }
}
