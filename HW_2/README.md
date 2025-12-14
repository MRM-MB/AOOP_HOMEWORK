# Homework 2 - University Management App

**⚠️ Note for the Professor:**
We have included a sample file, `Accounts.txt`, in `/UniversityManagementApp` for you to test the program. You can log in using premade accounts from this file or create a new user using the "Sign Up" feature to explore the application.

![LOG-IN_UI](../Screenshots/README_1.png)

## 📌 Assignment Overview

This assignment involves creating an application that manages university-related data, such as students, teachers, and subjects. The application allows users to log in, sign up, assign subjects to teachers and students, and edit or delete details. It was developed using the Avalonia framework for the graphical user interface.

## 🎯 Features Implemented

- **User Authentication:**
  - Log in with username and password.
  - Passwords are securely hashed for storage **Extra Point (2 Points)**.
  - Sign up to create new accounts.
- **Data Management:**
  - Create, edit, and delete students, teachers, and subjects.
  - Assign subjects to teachers and students.
  - Read and write data to JSON files for persistence.
- **Graphical User Interface (GUI):**
  - A user-friendly dashboard for managing university data.
  - Clear and intuitive design for ease of use.
- **Unit Testing:**
  - Comprehensive unit tests to ensure functionality and reliability.

## 👥 Contributors & Roles

### **Manish Raj Moriche** & **[Luigi Colluto](https://github.com/Lucol24)**

The coding was done collaboratively, with both contributors alternating roles for error-checking and improvements. Specific contributions include:

### **Manish Raj Moriche**

- Developed the **login functionality** and **username/password authentication logic**.
- Worked on the **dashboard UI** and made **editing subject details** possible.
- Created the **Student and Teacher models** and set up files for **ViewModels and Views**.
- Designed and implemented the **UI presentation Log In**.

### **[Luigi Colluto](https://github.com/Lucol24)**

- Developed the **sign-up functionality** and implemented **password hashing**.
- Worked on **assigning subjects** to teachers and students, as well as their **creation and deletion**.
- Implemented functionality for **reading and writing JSON data**.
- Created `Accounts.txt` to documents the teacher and student accounts and updated the log in logic with the hashing implementation.

### **Collaborative Efforts**

- Both contributors worked together on **unit testing** using a pair programming strategy.
- Designed and implemented the **reset and clear functionality**.
- Ensured the application met all requirements and was thoroughly tested.

## 📁 Project Structure

```plaintext
UniversityManagementApp/                    
├── Assets/                     # Folder for application icons
├── Data/                       # JSON files storing students, teachers, and subjects
├── Models/                     # Data models for students, teachers, and subjects
├── Services/                   # Logic and data handling services
│   ├── DataService.cs          # Handles reading and writing JSON data
│   ├── UserManager.cs          # Manages user authentication and account creation
├── ViewModels/                 # ViewModel logic for data binding between Views and Models
├── Views/                      # UI layouts and components
├── Program.cs                  # Main program entry point             
├── Accounts.txt                # Sample accounts file
└── README.md                   # Project documentation (this file)

UniversityManagementAppTests/                    
├── UnitTest1.cs                # Unit tests for verifying application functionality
```

---

## 🚀 **How to Run**

1. Open the project in an IDE that supports C# and Avalonia.
2. Run the project to launch the GUI `dotnet run --project /UniversityManagementApp/`.
3. Log in using an account from `Accounts.txt` or create a new account using the "Sign Up" feature.
4. Use the dashboard to manage students, teachers, and subjects.

---

## 🛠️ **Unit Testing**

Unit tests were created to verify the functionality of key features. Tests focus on:

- **Student Functionality:**
  - Ensure students can enroll in subjects.
  - Verify enrollment persists across application restarts.
- **System-Level Tests:**
  - Ensure data modifications (e.g., adding/removing subjects) persist after restarting the application.
  - Validate that data is correctly read and written to JSON files.
- **Teacher Functionality:**
  - Verify that a teacher can delete a subject.
  - Ensure the subject is removed from the student’s enrolled subjects and the subject list.

### **Running the Tests**

1. Open the project in an IDE.
2. Navigate to the `UniversityManagementAppTests/` folder.
3. Run the tests using the IDE's test runner.

Each test reads from and writes to JSON files, ensuring persistence and data integrity across application sessions. Cleanup is performed at the end of each test to maintain a consistent state.

- **Tracking Test Execution with Console Messages**

We've added some printable `Console.WriteLine` messages to track the execution of each test,  making it possible to see in the terminal when a test starts, when it finishes, and what happens in between.

![Unit_Tests](/HW_2/UniversityManagementApp/Assets/README_2.png)

---
