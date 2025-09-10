USE [LmsDB]
GO

-- Insert Courses
INSERT INTO Courses (Name, Description, Starts, Ends) 
VALUES
('Mathematics 101', 'Introductory mathematics course', '2024-09-01', '2024-12-15'),
('Physics 101', 'Basic physics course', '2024-09-01', '2024-12-15'),
('.NET 2025', 'Lexicons påbyggnadsutbildning i .NET', '2024-09-01', '2025-05-31');


-- Insert Modules
INSERT INTO Modules (Name, Description, StartsAt, EndsAt, CourseId) 
VALUES
('Algebra', 'Algebra basics', '2024-09-01', '2024-10-15', 1),
('Calculus', 'Introduction to calculus', '2024-10-16', '2024-12-15', 1),
('Mechanics', 'Fundamentals of mechanics', '2024-09-01', '2024-10-15', 2),
('Thermodynamics', 'Basic thermodynamics', '2024-10-16', '2024-12-15', 2),
('Databasdesign', 'Modul om databasdesign', '2024-09-01', '2024-10-15', 3),
('Javascript', 'Modul om Javascript', '2024-10-16', '2024-11-30', 3);


-- Insert Activities
INSERT INTO Activities (Name, Description, StartsAt, EndsAt, ModuleId, ActivityTypeId) 
VALUES
('Algebra Lecture 1', 'Introduction to algebra', '2024-09-02', '2024-09-02', 1, 1),
('Algebra Lab 1', 'Algebra exercises', '2024-09-05', '2024-09-05', 1, 2),
('Calculus Exam', 'Final calculus exam', '2024-12-10', '2024-12-10', 2, 3),
('Mechanics Lecture 1', 'Introduction to mechanics', '2024-09-03', '2024-09-03', 3, 1),
('Thermodynamics Lab', 'Thermodynamics experiments', '2024-11-01', '2024-11-01', 4, 2),
('Föreläsning - ER-diagram', 'Grundläggande föreläsning om ER-diagram', '2024-09-05', '2024-09-05', 5, 1),
('Inlämningsuppgift - Databasdesign', 'Inlämningsuppgift om databasdesign', '2024-10-10', '2024-10-15', 5, 3),
('Övning - JS-funktioner', 'Övningstillfälle för javascript-funktioner', '2024-10-20', '2024-10-20', 6, 2);
