-- Создание базы данных
CREATE DATABASE HostelDB;
GO

USE HostelDB;
GO

-- Таблица гостей
CREATE TABLE Guests (
    GuestId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    PassportData NVARCHAR(50)
);

-- Таблица номеров
CREATE TABLE Rooms (
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    Capacity INT NOT NULL CHECK (Capacity > 0),
    PricePerNight DECIMAL(10,2) NOT NULL CHECK (PricePerNight >= 0),
    IsActive BIT DEFAULT 1
);

-- Таблица бронирований
CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    GuestId INT NOT NULL,
    RoomId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    Status NVARCHAR(20) DEFAULT 'Active',
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (GuestId) REFERENCES Guests(GuestId),
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
    CHECK (CheckOutDate > CheckInDate)
);

-- Таблица заселений (чек‑ин)
CREATE TABLE CheckIns (
    CheckInId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL UNIQUE,
    ActualCheckInDate DATETIME DEFAULT GETDATE(),
    Notes NVARCHAR(500),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId)
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(100)
);
GO
INSERT INTO Users (Username, Password, FullName)
VALUES ('admin', '12345', 'Администратор системы');