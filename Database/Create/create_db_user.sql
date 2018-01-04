USE [master]

	IF EXISTS (SELECT name FROM sys.databases WHERE name = N'testDB')
		DROP DATABASE testDB
	GO
	USE [master]

	/***** Creating database testDB  *****/

	PRINT 'Creating Database'
	CREATE DATABASE testDB
	PRINT 'Database created successfully'

	IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'ebadoadmin')
		BEGIN
			PRINT 'Creating db user'
			CREATE LOGIN ebadoadmin WITH PASSWORD = 'edabo.159'
			PRINT 'User successfully created'
		END
	GO

	PRINT 'Selecting database instance'
	USE testDB

	PRINT 'Assigning user to the database'
	CREATE USER ebadoadmin FOR LOGIN ebadoadmin
	PRINT 'User successfully assigned to the database'

	PRINT 'Assigning permissions to the user'
	EXEC sp_addrolemember N'db_datareader', N'ebadoadmin'
	EXEC sp_addrolemember N'db_datawriter', N'ebadoadmin'
	PRINT 'Permissions successfully assigned to the user'