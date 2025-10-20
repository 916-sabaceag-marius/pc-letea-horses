USE master;
GO

-- Close existing connections and set the database to single-user mode
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Honse')
BEGIN
    ALTER DATABASE [Honse] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Honse];
END
GO

CREATE DATABASE [Honse];