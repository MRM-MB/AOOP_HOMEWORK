using Avalonia;                  // Core Avalonia types and functionality
using Avalonia.Controls;         // UI controls like Window, Button, etc.
using Avalonia.Media;            // Graphics and styling (e.g., Brushes)
using Avalonia.Platform.Storage; // File system interactions for Avalonia

// Standard .NET namespaces:
using System;                    // Basic .NET types and utilities
using System.IO;                 // File and stream handling operations

namespace ImageEditorApp;

public partial class MainWindow : Window
{
    // _pixelData: 2D array to store pixel values (0 or 1).  Represents the image.
    private int[,]? _pixelData;

    // _height: Stores the height of the image.
    private int _height;

    // _width: Stores the width of the image.
    private int _width;

    // MainWindow: Constructor for the MainWindow class.
    public MainWindow()
    {
        InitializeComponent(); // Initializes the Avalonia components defined in the XAML.
    }

    /// <summary>
    /// Handles the "Open File" button click event.  Opens a file picker, reads the selected
    /// .b2img.txt file, and loads the image data. Displays any errors that occur.
    /// </summary>
    private async void OpenFile_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Configure and open the file picker.
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open .b2img.txt File", // Sets the title of the file picker dialog.
            AllowMultiple = false, // Prevents the user from selecting multiple files.
            FileTypeFilter = new[] { new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } } } // Filters for .txt files.
        });

        // Check if a file was selected.
        if (files.Count > 0)
        {
            try
            {
                var file = files[0]; // Get the first selected file.

                // Open the file for reading using a stream. 'await using' ensures proper disposal.
                await using var stream = await file.OpenReadAsync();
                using var reader = new StreamReader(stream); // Create a StreamReader to read text from the stream.
                LoadImage(reader); // Call LoadImage to parse the image data from the file.
                StatusText.Text = $"✅ Loaded: {file.Name}"; // Update the status text with the file name.
                StatusText.Foreground = Brushes.Black; // Reset text color to black.
            }
            catch (Exception ex)
            {
                StatusText.Text = $"⚠️ Error: {ex.Message}"; // Display the error message.
                StatusText.Foreground = Brushes.Red; // Set text color to red for errors.
            }
        }
    }

    /// <summary>
    /// Loads the image data from the provided StreamReader.  Parses the dimensions and pixel
    /// values, performs validation, and calls RenderImage to display the image.
    /// </summary>
    private void LoadImage(StreamReader reader)
    {
        // Hide the placeholder
        PlaceholderBorder.Opacity = 0;
    
        // Display the image grid (PixelGrid)
        PixelGrid.IsVisible = true;
        
        // Read all lines from the stream, split into an array, and remove empty entries.
        var lines = reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Validate file format: must contain at least two lines.
        if (lines.Length < 2)
        {
            throw new InvalidDataException("⚠️ The file must contain at least two lines: dimensions and pixel values.");
        }

        // Split the first line into dimensions (height and width).
        var dimensions = lines[0].Split(' ');

        // Validate that the first line contains exactly two numbers.
        if (dimensions.Length != 2)
        {
            throw new InvalidDataException("⚠️ The first line must contain exactly two numbers: height and width.");
        }

        // Parse and validate the height.
        if (!int.TryParse(dimensions[0], out _height) || _height <= 0)
        {
            throw new InvalidDataException("⚠️ The height must be a positive integer.");
        }

        // Parse and validate the width.
        if (!int.TryParse(dimensions[1], out _width) || _width <= 0)
        {
            throw new InvalidDataException("⚠️ The width must be a positive integer.");
        }

        // Get the pixel values from the second line.
        var pixelValues = lines[1];

        // Calculate the expected number of characters (pixels).
        int expectedLength = _height * _width;

        // Validate pixel values length
        if (pixelValues.Length != expectedLength)
        {
            int difference = pixelValues.Length - expectedLength;
            string errorMessage;

            if (difference < 0)
            {
                errorMessage = $"⚠️ The pixel values are missing {-difference} characters. Expected {expectedLength} characters (0s and 1s).";
            }
            else
            {
                errorMessage = $"⚠️ The pixel values have {difference} extra characters. Expected {expectedLength} characters (0s and 1s).";
            }

            throw new InvalidDataException(errorMessage);
        }

        // Initialize the pixel data array with the parsed dimensions.
        _pixelData = new int[_height, _width];

        // Parse pixel values into the 2D array.
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                // Convert character to integer (0 or 1) and store in the pixelData array.
                _pixelData[i, j] = int.Parse(pixelValues[i * _width + j].ToString());
            }
        }

        RenderImage(); // Call RenderImage to display the loaded image.
    }

    /// <summary>
    /// Renders the image in the UniformGrid.  Creates buttons for each pixel and sets
    /// their background color based on the pixel value (0 or 1).
    /// </summary>
    private void RenderImage()
    {
        PixelGrid.Children.Clear(); // Clear any existing buttons from the grid.
        PixelGrid.Rows = _height; // Set the number of rows in the grid based on the image height.
        PixelGrid.Columns = _width; // Set the number of columns in the grid based on the image width.

        // Iterate through each pixel in the image.
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                // Create a border for each button to provide spacing and a subtle border.
                var border = new Border
                {
                    BorderBrush = Brushes.LightGray, // Subtle border color.
                    BorderThickness = new Thickness(0.5), // Thinner border.
                    Width = 40, // Button size
                    Height = 40, // Button size
                    Margin = Thickness.Parse("0.75"), // Spacing between square buttons

                    // Create a Button for each pixel.
                    Child = new Button
                    {
                        // Set the button's background color based on the pixel value (0 or 1).
                        Background = _pixelData![i, j] == 1 ? Brushes.Black : Brushes.White,
                        Width = 40, // Button size
                        Height = 40, // Button size
                        Padding = new Thickness(0) // Remove padding
                    }
                };

                // Capture the current row and column indices for the event handler.
                int row = i;
                int col = j;
                // Attach a click event handler to each button to toggle the pixel value.
                ((Button)border.Child).Click += (s, e) => TogglePixel(row, col);
                PixelGrid.Children.Add(border); // Add the border (containing the button) to the grid.
            }
        }
    }

    /// <summary>
    /// Toggles the pixel value (0 <-> 1) and updates the button color.
    /// </summary>
    private void TogglePixel(int row, int col)
    {
        // Toggle the pixel value between 0 and 1.
        _pixelData![row, col] = _pixelData[row, col] == 0 ? 1 : 0;

        // Access the Border control at the specified row and column.
        var border = PixelGrid.Children[row * _width + col] as Border;

        // Access the Button control inside the Border.
        var button = border?.Child as Button;

        // Update the button's background color based on the new pixel value.
        button!.Background = _pixelData[row, col] == 1 ? Brushes.Black : Brushes.White;
    }

    /// <summary>
    /// Handles the "Save File" button click event. Opens a save file dialog and saves the
    /// current image data to the selected .b2img.txt file.
    /// </summary>
    private async void SaveFile_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Check if an image has been loaded.
        if (_pixelData == null)
        {
            StatusText.Text = "⚠️ No image loaded."; // Display a message if no image is loaded.
            StatusText.Foreground = Brushes.Red; // Set text color to red for errors
            return;
        }

        // Configure and open the save file dialog.
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save .b2img.txt File", // Sets the title of the save file dialog.
            FileTypeChoices = new[] { new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } } }, // Sets the file type filter to .txt files.
            DefaultExtension = "txt" // Sets the default file extension to .txt.
        });

        // Check if a file was selected.
        if (file != null)
        {
            try
            {
                // Open the file for writing using a stream. 'await using' ensures proper disposal.
                await using var stream = await file.OpenWriteAsync();
                using var writer = new StreamWriter(stream); // Create a StreamWriter to write text to the stream.
                SaveImage(writer); // Call SaveImage to write the image data to the file.
                StatusText.Text = $"✅ Saved: {file.Name}"; // Update the status text with the file name.
                StatusText.Foreground = Brushes.Black; // Reset text color to black.
            }
            catch (Exception ex)
            {
                StatusText.Text = $"⚠️ Error: {ex.Message}"; // Display the error message.
                StatusText.Foreground = Brushes.Red; // Set text color to red for errors.
            }
        }
    }

    /// <summary>
    /// Saves the image data to the provided StreamWriter. Writes the height, width, and
    /// pixel values to the file.
    /// </summary>
    private void SaveImage(StreamWriter writer)
    {
        writer.WriteLine($"{_height} {_width}"); // Write height and width to the first line.

        // Iterate through the pixel data array.
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                writer.Write(_pixelData![i, j]); // Write each pixel value (0 or 1) to the file.
            }
        }
    }

    /// <summary>
    /// Handles the "Flip Vertically" button click event.  Flips the image vertically and updates the UI.
    /// </summary>
    private void FlipVertically_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Check if an image has been loaded.
        if (_pixelData == null)
        {
            StatusText.Text = $"⚠️ No image loaded.";
            StatusText.Foreground = Brushes.Red;
            return;
        }

        // Flip the image vertically by calling the FlipVertically method.
        _pixelData = FlipVertically(_pixelData);

        // Update the UI by re-rendering the image.
        RenderImage();
        StatusText.Text = $"✅ Image flipped vertically.";
    }

    /// <summary>
    /// Flips the 2D array vertically.
    /// </summary>
    private int[,] FlipVertically(int[,] originalGrid)
    {
        int height = originalGrid.GetLength(0); // Get the height of the grid.
        int width = originalGrid.GetLength(1); // Get the width of the grid.
        int[,] flippedGrid = new int[height, width]; // Create a new array to store the flipped image.

        // Iterate through the original grid and copy values to the flipped grid in reverse row order.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flippedGrid[y, x] = originalGrid[height - 1 - y, x]; // Copy the pixel from the bottom row to the top.
            }
        }
        return flippedGrid; // Return the flipped grid.
    }

    /// <summary>
    /// Handles the "Flip Horizontally" button click event. Flips the image horizontally and updates the UI.
    /// </summary>
    private void FlipHorizontally_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Check if an image has been loaded.
        if (_pixelData == null)
        {
            StatusText.Text = "⚠️ No image loaded.";
            StatusText.Foreground = Brushes.Red;
            return;
        }

        // Flip the image horizontally by calling the FlipHorizontally method.
        _pixelData = FlipHorizontally(_pixelData);

        // Update the UI by re-rendering the image.
        RenderImage();
        StatusText.Text = "✅ Image flipped horizontally.";
    }

    /// <summary>
    /// Flips the 2D array horizontally.
    /// </summary>
    private int[,] FlipHorizontally(int[,] originalGrid)
    {
        int height = originalGrid.GetLength(0); // Get the height of the grid.
        int width = originalGrid.GetLength(1);  // Get the width of the grid.
        int[,] flippedGrid = new int[height, width]; // Create a new array to store the flipped image.

        // Iterate through the original grid and copy values to the flipped grid in reverse column order.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flippedGrid[y, x] = originalGrid[y, width - 1 - x]; // Copy the pixel from the rightmost column to the leftmost.
            }
        }

        return flippedGrid; // Return the flipped grid.
    }
}