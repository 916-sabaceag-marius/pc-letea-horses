USE master;
GO

-- Close existing connections and set the database to single-user mode
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Honse')
BEGIN
    ALTER DATABASE [Honse] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Honse];
END
GO

-- Wait for SQL Server to be fully ready
DECLARE @i INT = 0;
WHILE @i < 30 -- 30 attempts with 1 second delay = 30 seconds max wait
BEGIN
    BEGIN TRY
        CREATE DATABASE [Honse];
        PRINT 'Database created successfully';
        BREAK;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 1801 -- Database already exists
        BEGIN
            PRINT 'Database already exists';
            BREAK;
        END
        ELSE
        BEGIN
            PRINT 'Waiting for SQL Server to be ready...';
            WAITFOR DELAY '00:00:01'; -- Wait 1 second
            SET @i = @i + 1;
        END
    END CATCH
END
GO