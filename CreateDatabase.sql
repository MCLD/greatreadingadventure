/*
	Database create script for the Great Reading Adventure.
	
	This script creates the database, SQL Server logins, and database users.
	
	* It will exit immediately if the database exists.
	* If the SQL Server logins exist, it will continue running without modifying them.
	
	Please change the following passwords below in the script or use a global search and replace on
	this file:
	
	* Database owner login will be created with this password (line 51): ystMPQGurKMmqjHDRkz810sx
	* Database user login will be created with this password (line 67): H4gWdCicQMzasDTU6jICB9CK

	Optionally, you may change these items throughout the script if you wish (note that if you use a
	global search and replace on 'SRP' you will also be changing the first part of the logins and
	users):
	
	* The database it will create is named: SRP
	* The database owner login and user it will create are named: srp_owner
	* The database user login and user it will create are named: srp_user	
*/
USE [master];
GO

PRINT 'Checking if database already exists...';

IF EXISTS (
		SELECT 1
		FROM [master].[dbo].[sysdatabases]
		WHERE [name] = N'SRP'
		)
BEGIN
	RAISERROR (
			'The database already exists, please select a new database name or remove the existing database.',
			20,
			- 1
			)
	WITH LOG;
END

PRINT 'It does not exist. Creating SQL Server logins...';

IF NOT EXISTS (
		SELECT [LOGINNAME]
		FROM [master].[dbo].[syslogins]
		WHERE [LOGINNAME] = 'srp_owner'
		)
BEGIN
	CREATE LOGIN [srp_owner]
		WITH PASSWORD = 'ystMPQGurKMmqjHDRkz810sx',
			DEFAULT_LANGUAGE = [us_english],
			CHECK_POLICY = OFF;
END
ELSE
BEGIN
	PRINT 'Not creating SQL Server login srp_owner becuase it already exists.';
END

IF NOT EXISTS (
		SELECT [LOGINNAME]
		FROM [master].[dbo].[syslogins]
		WHERE [LOGINNAME] = 'srp_user'
		)
BEGIN
	CREATE LOGIN [srp_user]
		WITH PASSWORD = 'H4gWdCicQMzasDTU6jICB9CK',
			DEFAULT_LANGUAGE = [us_english],
			CHECK_POLICY = OFF;
END
ELSE
BEGIN
	PRINT 'Not creating SQL Server login srp_user becuase it already exists.';
END
GO

PRINT 'Creating database...';

CREATE DATABASE [SRP];
GO

ALTER DATABASE [SRP] MODIFY FILE (
	NAME = N'SRP',
	SIZE = 4096 KB,
	MAXSIZE = UNLIMITED,
	FILEGROWTH = 1024 KB
	);
GO

ALTER DATABASE [SRP] MODIFY FILE (
	NAME = N'SRP_log',
	SIZE = 1024 KB,
	MAXSIZE = 512 MB,
	FILEGROWTH = 10 %
	);
GO

ALTER DATABASE [SRP]

SET ANSI_NULL_DEFAULT OFF;
GO

ALTER DATABASE [SRP]

SET ANSI_NULLS OFF;
GO

ALTER DATABASE [SRP]

SET ANSI_PADDING OFF;
GO

ALTER DATABASE [SRP]

SET ANSI_WARNINGS OFF;
GO

ALTER DATABASE [SRP]

SET ARITHABORT OFF;
GO

ALTER DATABASE [SRP]

SET AUTO_CLOSE OFF;
GO

ALTER DATABASE [SRP]

SET AUTO_CREATE_STATISTICS ON;
GO

ALTER DATABASE [SRP]

SET AUTO_SHRINK OFF;
GO

ALTER DATABASE [SRP]

SET AUTO_UPDATE_STATISTICS ON;
GO

ALTER DATABASE [SRP]

SET CURSOR_CLOSE_ON_COMMIT OFF;
GO

ALTER DATABASE [SRP]

SET CURSOR_DEFAULT GLOBAL;
GO

ALTER DATABASE [SRP]

SET CONCAT_NULL_YIELDS_NULL OFF;
GO

ALTER DATABASE [SRP]

SET NUMERIC_ROUNDABORT OFF;
GO

ALTER DATABASE [SRP]

SET QUOTED_IDENTIFIER OFF;
GO

ALTER DATABASE [SRP]

SET RECURSIVE_TRIGGERS OFF;
GO

ALTER DATABASE [SRP]

SET DISABLE_BROKER;
GO

ALTER DATABASE [SRP]

SET AUTO_UPDATE_STATISTICS_ASYNC OFF;
GO

ALTER DATABASE [SRP]

SET DATE_CORRELATION_OPTIMIZATION OFF;
GO

ALTER DATABASE [SRP]

SET TRUSTWORTHY OFF;
GO

ALTER DATABASE [SRP]

SET ALLOW_SNAPSHOT_ISOLATION OFF;
GO

ALTER DATABASE [SRP]

SET PARAMETERIZATION SIMPLE;
GO

ALTER DATABASE [SRP]

SET READ_COMMITTED_SNAPSHOT OFF;
GO

ALTER DATABASE [SRP]

SET HONOR_BROKER_PRIORITY OFF;
GO

ALTER DATABASE [SRP]

SET RECOVERY SIMPLE;
GO

ALTER DATABASE [SRP]

SET MULTI_USER;
GO

ALTER DATABASE [SRP]

SET PAGE_VERIFY CHECKSUM;
GO

ALTER DATABASE [SRP]

SET DB_CHAINING OFF;
GO

ALTER DATABASE [SRP]

SET READ_WRITE;
GO

USE [SRP]

IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
	EXECUTE sp_fulltext_database 'enable';

PRINT 'Creating database users...';

CREATE USER [srp_owner]
FOR LOGIN [srp_owner]
WITH DEFAULT_SCHEMA = [dbo];

EXEC sp_addrolemember @rolename = 'db_owner',
	@membername = 'srp_owner';
GO

CREATE USER [srp_user]
FOR LOGIN [srp_user]
WITH DEFAULT_SCHEMA = [dbo];
GO

EXEC sp_addrolemember @rolename = 'db_datareader',
	@membername = 'srp_user';
GO

EXEC sp_addrolemember @rolename = 'db_datawriter',
	@membername = 'srp_user';
GO

GRANT EXECUTE
	ON SCHEMA::dbo
	TO [srp_user]
	WITH GRANT OPTION;
GO

PRINT 'Script complete.';