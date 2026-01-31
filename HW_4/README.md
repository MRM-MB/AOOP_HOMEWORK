# Homework 4 - Kitchen Simulator

## Assignment Overview

This project simulates a **restaurant kitchen** where multiple recipes are prepared concurrently, visualized with real-time animated feedback. The goal was to apply concepts of **multithreading**, **JSON parsing**, and **UI animations** using **Avalonia UI** in a modular and scalable way.

![App Screenshot](../Screenshots/Assignment4.webp)

The simulator takes a set of recipes from a JSON file and simulates their preparation across multiple kitchen stations, where each recipe is processed step-by-step following specific durations. Each step is visualized in the UI using animated progress bars and dynamic updates.

## ✅ Key Objectives Achieved

- JSON Parsing: Recipes and ingredients are loaded from a `recipes.json` file and mapped into models (`Recipe`, `Ingredient`, `RecipeStep`, etc.)
- Multithreading: Kitchen stations (threads) concurrently process recipes while remaining thread-safe.
- Avalonia UI: A modern and responsive interface using MVVM architecture.
- Animated Dashboard: Real-time progress updates via animated progress bars and UI transitions.

## Features Implemented

### Core Functionalities:

- Load and parse JSON data into structured C# models.
- Simulate multiple kitchen stations preparing recipes in parallel.
- Step-by-step processing of each recipe using async delays for accurate duration simulation.
- Thread-safe logic for managing shared resources.
- Real-time UI dashboard displaying:
  - Recipe name
  - Current preparation step
  - Progress (animated percentage and time)
- Start / Pause simulation controls for user interaction.
- Recovery from potential runtime exceptions (e.g., invalid JSON, thread interruptions).

### 💡 Bonus Feature (2 Points):

- **Status Report**: The simulation implements a status report that summarizes completed orders so the user can view the history of orders.

## 📁 Project Structure

```
KitchenSimulator
├── Assets            # Icons and other UI assets
├── Data              # Contains the input recipes.json
├── Models            # Core data models: Recipe, Ingredient, RecipeStep, KitchenData
├── Services          # RecipeLoader service for parsing JSON data
├── ViewModels        # MVVM ViewModels for UI binding and logic
├── Views             # Avalonia UI XAML views
├── App.axaml         # Application-level UI configuration
├── Program.cs        # Application entry point
└── README.md         # Project documentation
```

## 👥 Contributors & Roles

### **Manish Raj Moriche** & **[Luigi Colluto](https://github.com/Lucol24)**

Both members collaborated closely on the design, architecture, and testing of the project, splitting responsibilities equally.

### **Manish Raj Moriche**
- Implemented the **JSON parsing** logic in `RecipeLoader.cs`.
- Developed the **Kitchen Station multithreading logic**.
- Designed and handled **thread synchronization mechanisms**.
- Built the **progress tracking logic** and integrated it with the ViewModels.

### **[Luigi Colluto](https://github.com/Lucol24)**
- Designed the **UI** using Avalonia XAML and animations.
- Created the **MVVM ViewModels** (`MainWindowViewModel`, `RecipeViewModel`).
- Implemented **data binding**, user controls (Start, Pause), and responsiveness logic.
- Integrated the **report status logic** and adjusted history page.

### **Collaborative Tasks**
- Brainstormed and modeled the application structure.
- Designed and tested the recipe simulation pipeline.
- Debugged and handled UI responsiveness issues and multithreading edge cases.
- Wrote **comments and documentation** for maintainability.

## How to Run

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- A C#-compatible IDE (Visual Studio, Rider, VS Code + C# plugin)

### Steps

 1. Clone or download the project repository.
 2. Open the solution `KitchenSimulator.sln` in your IDE.
 3. Ensure `recipes.json` exists under `/Data/` directory with valid recipe data.
 4. Run the application with:
   ```bash
   dotnet run --project KitchenSimulator
   ```

### Usage Guide

- On launch, the application loads the recipes and displays them on the dashboard.
- Press **Start Simulation** to begin the recipe preparation.
- Each recipe will be picked by an available kitchen station (thread).
- Progress is shown live with:
  - Animated progress bars
  - Step name and time remaining
  - Kitchen station name (e.g., "Chef #1", "Head Chef")
- Press **Pause** to suspend the specific ongoing recipe.
- When a recipe is completed, a visual cue (e.g.,  `Done!`) signals its completion and you can press the **Start** button to start that recipe again.
- In the History page is possible to see the history of recipes completed during the session.

---
