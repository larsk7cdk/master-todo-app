-- Create ToDo database if they don't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToDo-db')
BEGIN
    CREATE DATABASE ToDo-db;
    PRINT 'Database ToDo-db created successfully';
END
ELSE
BEGIN
    PRINT 'Database ToDo-db already exists';
END
GO
