# 🎓 Lexicon LMS (LexNET)

Lexicon LMS (Learning Management System) is a **full-stack Blazor Web App** designed to simplify and centralize communication between teachers, students.  
It provides scheduling, course materials, assignments, and submissions all in one place, built with **Clean Architecture** and modern .NET practices.

---

## 📑 Table of Contents
- [📌 Project Overview](#project-overview)
- [🏠 App Home Page](#-app-home-page)
- [📐 Clean Architecture Overview](#-clean-architecture-overview)
- [📋 Core Features](#-core-features)
  - [👥 Users](#-users)
  - [📚 Courses & Modules](#-courses--modules)
  - [📅 Activities & Scheduling](#-activities--scheduling)
  - [📄 Documents & Submissions](#-documents--submissions)
- [🛠️ Technologies Used](#-technologies-used)
- [🚀 Getting Started](#-getting-started)
- [🧪 Testing](#-testing)
- [🌐 Deployment](#-deployment)
- [🌀 Scrum Process](#-scrum-process)

---

## Project Overview
Lexicon LMS (LexNET) is designed to:
- Give **students** quick access to courses, modules, schedules, and assignments.
- Allow **teachers** to easily manage classes, modules, activities, and documents.
- Keep the system **lightweight, user-friendly, and responsive** compared to traditional heavy LMS platforms.

---

## 🏠 App Home Page
![LMS Homepage](wwwroot/Images/HomePage.png)

---

## 📐 Clean Architecture Overview
The solution follows **Clean Architecture** for scalability and maintainability:

- 🧠 **Domain**  
  Core business models and rules: `User`, `Course`, `Module`, `Activity`, `Document`.

- ⚙️ **Application**  
  Interfaces, services, commands/queries (CQRS), DTOs, and validation.

- 🗄️ **Infrastructure**  
  Data access with **Entity Framework Core (code-first)**.  
  Includes database context, migrations, and repository implementations.

- 🌐 **Presentation (Blazor Web App)**  
  Frontend built with **Blazor**, using **Razor Components** and **Bootstrap 5** for UI.  
  Communicates with the API via `HttpClient`.

- 🧪 **Tests**  
  Unit tests for services and controllers (xUnit, optional TDD).

---

## 📋 Core Features

### 👥 Users
- Roles: **Student** and **Teacher**.
- Authentication and login.
- Teachers can create/edit students and teachers.

### 📚 Courses & Modules
- Each student belongs to a single course.
- Courses contain **modules** with start and end dates.
- Teachers can create/edit courses and modules.

### 📅 Activities & Scheduling
- Modules contain **activities** (lectures, exercises, assignments, etc.).
- Activities have types, names, descriptions, and start/end times.
- No overlaps outside module or course times.

### 📄 Documents & Submissions
- Teachers upload course, module, or activity documents.
- Students upload assignment submissions.
- Timestamps, uploader info, and metadata stored.

---

## 🛠️ Technologies Used
- 🔷 **.NET 9.0**
- 🌐 **Blazor Web App** with **Razor Components**
- 🎨 **Bootstrap 5** for responsive UI
- 💽 **Entity Framework Core** (code-first)
- 🗄️ **SQL Server / PostgreSQL**
- 🔐 **ASP.NET Core Identity**
- 🧪 **xUnit** (optional, for TDD)
- 🖥️ **Visual Studio 2022+**
- 🌍 **Git & GitHub** for version control (feature branches, dev & main)

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server or PostgreSQL
- Visual Studio 2022+

### Setup
1. **Clone the Repository**
   ```bash
   git clone https://github.com/YourUser/LexiconLMS.git
   cd LexiconLMS
