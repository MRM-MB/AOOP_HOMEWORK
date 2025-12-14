using Avalonia.Controls;
using Avalonia.Reactive; // For EventHandler pattern
using System; // Add System namespace for EventArgs

namespace DataVizApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Fix the event handler with proper signature
        this.DataContextChanged += MainWindow_DataContextChanged;
    }

    private void MainWindow_DataContextChanged(object? sender, EventArgs e)
    {
        // Get the DataContext directly from the window
        if (DataContext is DataVizApp.ViewModels.MainWindowViewModel viewModel)
        {
            // Initialize positions when DataContext is set
            viewModel.UpdateChartPositions();
        }
    }

    private void NumericUpDown_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
    {
        // Force refresh of pie chart when number changes
        if (this.FindControl<LiveChartsCore.SkiaSharpView.Avalonia.PieChart>("GenrePieChart") is LiveChartsCore.SkiaSharpView.Avalonia.PieChart pieChart)
        {
            // Force the chart to refresh by invalidating its measure and arrange
            pieChart.InvalidateMeasure();
            pieChart.InvalidateArrange();
            pieChart.InvalidateVisual();
        }
    }
}