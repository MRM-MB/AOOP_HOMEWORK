# Homework 3 - Data Visualization App

## 📌 Assignment Overview

The purpose of this assignment is to develop a **fully functional data visualization application** that allows users to load, process, and analyze datasets using interactive graphs. The project emphasizes software architecture best practices and usability.

**Preset Queries**: The application implements **5 out of 10** preset query visualizations for analyzing music streaming data:

- Top Streaming Platforms by Usage
- Most Streamed Music Genres
- Most Popular Subscription Type
- Minutes Streamed per Day by Age
- Top Artists by Listeners

## 📐 UI Mock-up

Below is the initial **UI sketch/mock-up** designed for the application. It helped in planning the layout and user experience before development.

![UI Mock-up](../Screenshots/Assignment3.webp)

## 🎯 Features Implemented

### **Basic Features:**

- Read datasets from CSV files and process them using **LINQ** (grouping, sorting, filtering, etc.).  
- Provide a **dashboard** with interactive charts.  
- Offer **preset queries** that users can select from a control panel.  
- Support at least **two types of charts** (e.g., pie charts, bar charts, line charts, etc.).  
- Allow users to **add and remove graphs dynamically**.  
- Ensure an **intuitive and modern UI** with custom styling.  
- **Resizable Charts:** Charts can be resized dynamically while maintaining data representation.  **Extra Point (2 Points)**.

## 👥 Contributors & Roles

### **Manish Raj Moriche** & **[Luigi Colluto](https://github.com/Lucol24)**

Both team members contributed to the overall application architecture, UI design, debugging and the creation of **Top Artists by Listeners** graph preset query.

### **Manish Raj Moriche**

- Implemented the **"Top Streaming Platforms by Usage"** and **"Most Streamed Music Genre"** charts.
- Designed the **MVVM architecture** and ensured SOLID principles were followed.
- Handled **file I/O operations** and CSV dataset parsing.
- Developed the **extra functionality** for **resizable charts**.

### **[Luigi Colluto](https://github.com/Lucol24)**

- Implemented the **"Most Popular Subscription Type"** and **"Minutes Streamed per Day by Age"** charts.
- Developed the **UI components** using Avalonia and customized themes.
- Designed the **MVVM architecture** and ensured SOLID principles were followed.

### **Collaborative Efforts**

- Both contributors worked on designing and implementing the **user interface (UI)**.
- Brainstormed and structured the **preset queries**.
- Ensured the **application was user-friendly** and had an intuitive experience.
- Debugged and optimized **chart rendering and performance issues**.

## 📁 Project Structure

```plaintext
📦 Data Visualization App
├── 📁 Models       # Data models and dataset representations
├── 📁 ViewModels   # MVVM ViewModels for managing UI logic
├── 📁 Views        # UI components built with Avalonia
├── 📁 Assets    # Static resources such as icons, mock-up images, styles, and themes
├── 📁 Data         # Example CSV datasets
├── 📄 Program.cs   # Main entry point of the application
├── 📄 App.axaml    # Application-level UI structure
└── 📄 README.md    # Project documentation
```

---

## 🚀 **How to Run**

1. Open the project in an IDE that supports C# and Avalonia.

2. Run the project to launch the GUI using:
`dotnet run --project /DataVizApp`

3. When the application first opens, you'll see an empty dashboard with no charts displayed.

4. Use the dropdown menu to select which chart you want to add to the dashboard:
   - "Top Streaming Platforms by Usage" - Shows the most popular streaming platforms based on user count
   - "Most Streamed Music Genres" - Displays distribution of popular music genres
   - "Most Popular Subscription Type" - Shows breakdown between free and premium subscriptions
   - "Minutes Streamed per day by Age" - Shows average streaming time across different age groups
   - "Top Artists by Listeners" - Displays artists with the most listeners
   - "Show All Charts" (available when multiple charts are hidden)

5. Click `"Add"` to display the selected chart on the dashboard.

6. You can remove any chart from the dashboard by clicking its `"Remove"` button, which will make it available again in the dropdown.

7. If all charts are already visible, the dropdown will be empty since there are no hidden charts to add.

8. **Adjust Data Points**: Use the numeric input to adjust the number of data points displayed in the charts:
   - Minimum value: 3 items
   - Maximum value: 15 items
   - Default: 5 items
   - For pie charts, hover over them after changing the value to see the changes take effect

9. **Preset Queries**: Each chart represents a different preset query on the music streaming dataset:
   - Top Platforms chart: Groups data by streaming platform and counts users
   - Genre Pie chart: Groups by music genre and shows percentage distribution
   - Subscription Type chart: Shows distribution between free and premium users
   - Age Group chart: Calculates average streaming minutes per day across age groups
   - Top Artists: Shows most listened-to artists by listener count

10. Charts will automatically resize based on available space, and you can resize the application window to adjust the layout.

11. If you remove all charts, the application will display an empty state message.

---
