-- ============================================
-- Users Table
-- ============================================

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Username NVARCHAR(100) NOT NULL,

    Email NVARCHAR(150) NOT NULL UNIQUE,

    PasswordHash NVARCHAR(500) NOT NULL,

    Role NVARCHAR(20) NOT NULL,

    CreatedDate DATETIME NOT NULL
    DEFAULT GETDATE()
);



-- ============================================
-- Departments Table
-- ============================================

CREATE TABLE Departments
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Name NVARCHAR(100) NOT NULL,

    Description NVARCHAR(250) NULL,

    CreatedDate DATETIME NOT NULL
    DEFAULT GETDATE()
);



-- ============================================
-- Employees Table
-- ============================================

CREATE TABLE Employees
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Name NVARCHAR(100) NOT NULL,

    Email NVARCHAR(150) NOT NULL UNIQUE,

    Phone NVARCHAR(20) NULL,

    Salary DECIMAL(18,2) NOT NULL,

    DepartmentId INT NOT NULL,

    IsActive BIT NOT NULL
    DEFAULT 1,

    CreatedDate DATETIME NOT NULL
    DEFAULT GETDATE(),

    CONSTRAINT FK_Employees_Departments
    FOREIGN KEY (DepartmentId)
    REFERENCES Departments(Id)
);