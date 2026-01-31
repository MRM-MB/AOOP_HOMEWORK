# Homework 1 - Image_Editor_App

**⚠️ Note for the Professor:**
We have included a sample file, `panda.txt`, in `/ImageEditorApp` for you to test the program.
You can also upload a custom `.b2img.txt` file to check how the application processes different inputs.

![App Screenshot](../Screenshots/Assignment1.webp)

## Assignment Overview

This assignment involves creating an application that reads `.b2img.txt` files, loads them into memory, displays and modifies the values using a graphical user interface, and writes the modified data to an output file. The application was developed using the Avalonia framework to handle the GUI.

## Features Implemented

- **File Handling:** Load `.b2img.txt` files and parse their content.
- **Graphical User Interface (GUI):** A user-friendly interface to display and edit the image.
- **Modify & Save:** Allows users to modify the image and save it to a new file.
- **Bonus Features:**
  - **Flip Image:** Supports both horizontal and vertical flipping of the image.
  - **Detailed Code Documentation:** Comments added to clarify Avalonia framework usage for future reference.

## 👥 Contributors & Roles

### **Manish Raj Moriche** & **[Luigi Colluto](https://github.com/Lucol24)**

The coding was done in a peer-to-peer manner, alternating between the two contributors for error-checking and improvements. Specific contributions include:

### **[Luigi Colluto](https://github.com/Lucol24)**

- Focused on the **presentation** of the application.
- Developed most of the **GUI and visual aspects**.
- Implemented improvements in the **logic** part for **input handling and maintainability**.

### **Manish Raj Moriche**

- Handled the **logic** of the program, including button interactions and grid display logic.
- Worked on the **grid logic** to upload, read, modify, and save the .txt files.
- Supervised and optimized code to improve **efficiency and readability** in the GUI section.

### **Collaborative Efforts**

- Both members worked together on the **flip rotation feature** (horizontal and vertical).
- Detailed comments were written to explain Avalonia framework integration, especially since this was our first project using Avalonia and Axaml.

## 📁 Project Structure

```plaintext
IMAGEEDITORAPP/
├── bin/                        # Compiled binaries
├── obj/                        # Intermediate object files
├── Icons/                      # Folder for application icons
├── MainWindow.axaml            # UI layout for the main window
├── MainWindow.axaml.cs         # Backend logic for the main window
├── Program.cs                  # Main program entry point
├── App.axaml                   # Application-level UI structure
├── App.axaml.cs                # Application-level logic
├── app.manifest                # Application manifest file
├── ImageEditorApp.csproj       # Project configuration file
├── panda.txt                   # Sample input file
└── README.md                   # Project documentation (this file)
```

---

## **How to Run**  

1. Open the project in an IDE that supports C# and Avalonia.
2. Run the project to launch the GUI.
3. Load an image file (`.b2img.txt`).
4. Modify the image using the GUI.
5. Save the modified image to a new file.

---
