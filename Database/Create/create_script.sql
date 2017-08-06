	
	BEGIN TRANSACTION

		/***** Creating tables *****/
		PRINT 'Creating Table [User_Details]'
		CREATE TABLE [User_Details]
		(
			[Id] INT IDENTITY(1,1),
			[Title] VARCHAR(20),
			[First_Name] VARCHAR(50) NOT NULL,
			[Second_Name] VARCHAR(50),
			[Surname] VARCHAR(50) NOT NULL,
			[Display_Name] VARCHAR(50) NOT NULL,
			[Phone_Number] INT,
			[Additional_Phone_Number] INT,
			[Email] VARCHAR(50) UNIQUE NOT NULL,
			[Password] VARCHAR(255) NOT NULL,
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_USER_DETAILS_ID PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [User_Role]'
		CREATE TABLE [User_Role]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_User_Role_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [User_Permission]'
		CREATE TABLE [User_Permission]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_User_Permission_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [User_Role_2_User_Permission]'
		CREATE TABLE [User_Role_2_User_Permission]
		(
			[Id] INT IDENTITY(1,1),
			[User_Role_Id] INT,
			[User_Permission_Id] INT,
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_User_Role_2_User_Permission_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Company_Details]'
		CREATE TABLE [Company_Details]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(100) NOT NULL,
			[Description] VARCHAR(250),
			[Phone_Number] INT,
			[Additional_Phone_Number] INT,
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_COMPANY_DETAILS_ID PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Company_Details_2_User_Details]'
		CREATE TABLE [Company_Details_2_User_Details]
		(
			[Id] INT IDENTITY(1,1),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_COMPANY_DETAILS_2_USER_DETAILS PRIMARY KEY(Id)
		)

		PRINT 'Creating Table [Company_Type]'
		CREATE TABLE [Company_Type]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Company_Type_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Company_Role]'
		CREATE TABLE [Company_Role]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[Description] VARCHAR(100),
			[Is_Custom] BIT NOT NULL DEFAULT 'false',
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Company_Role_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Company_Permission]'
		CREATE TABLE [Company_Permission]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL,
			[Code] VARCHAR(30) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Company_Permission_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Company_Role_2_Company_Permission]'
		CREATE TABLE [Company_Role_2_Company_Permission]
		(
			[Id] INT IDENTITY(1,1),
			[Company_Role_Id] INT,
			[Company_Permission_Id] INT,
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Company_Role_2_Company_Permission_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Location]'
		CREATE TABLE [Location]
		(
			[Id] INT IDENTITY(1,1),
			[Country] VARCHAR(100),
			[PostalCode] VARCHAR(100),
			[City] VARCHAR(100),
			[County] VARCHAR(100),
			[District] VARCHAR(100),
			[Lat] DECIMAL(10,4),
			[Lon] DECIMAL(10,4),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Location_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Address]'
		CREATE TABLE [Address]
		(
			[Id] INT IDENTITY(1,1),
			[Street] VARCHAR(100),
			[Number] VARCHAR(10),
			[Is_DeliveryAddress] BIT DEFAULT 'false',
			[Is_BillingAddress] BIT DEFAULT 'false',
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Address_Id PRIMARY KEY(Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Main_Category]'
		CREATE TABLE [Main_Category]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Main_Category_Id PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Category]'
		CREATE TABLE [Category]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Category_Id PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Sub_Category]'
		CREATE TABLE [Sub_Category]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Sub_Category_Id PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Batch_Attachment]'
		CREATE TABLE [Batch_Attachment]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Batch_Attachment_Id PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'
		
		PRINT 'Creating Table [Attachment]'
		CREATE TABLE [Attachment]
		(
			[Id] INT IDENTITY(1,1),
			[Original_Url] VARCHAR(255) NOT NULL,
			[Thumbnail_Url] VARCHAR(255),			
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Attachment_Id PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		/***** Creating foreign keys *****/

		PRINT 'Creating foreign keys for table [Company_Details_2_User_Details]'
		ALTER TABLE [Company_Details_2_User_Details]
		ADD [Company_Details_Id] INT NOT NULL
		ALTER TABLE [Company_Details_2_User_Details]
		ADD	[User_Details_Id] INT NOT NULL
		ALTER TABLE [Company_Details_2_User_Details]
		ADD	CONSTRAINT FK_Company_Details FOREIGN KEY (Company_Details_Id) REFERENCES Company_Details(Id)
		ALTER TABLE [Company_Details_2_User_Details]
		ADD	CONSTRAINT FK_User_Details FOREIGN KEY (User_Details_Id) REFERENCES User_Details(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Company_Details]'
		ALTER TABLE [Company_Details]
		ADD [Company_Type_Id] INT NOT NULL
		ALTER TABLE [Company_Details]
		ADD [Sub_Category_Id] INT NOT NULL
		ALTER TABLE [Company_Details]
		ADD	CONSTRAINT FK_Company_Type FOREIGN KEY (Company_Type_Id) REFERENCES Company_Type(Id)
		ALTER TABLE [Company_Details]
		ADD	CONSTRAINT FK_Sub_Category FOREIGN KEY (Sub_Category_Id) REFERENCES Sub_Category(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign key for table [Category]'
		ALTER TABLE [Category]
		ADD [Main_Category_Id] INT NOT NULL
		ALTER TABLE [Category]
		ADD CONSTRAINT FK_Main_Category FOREIGN KEY (Main_Category_Id) REFERENCES Main_Category(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign keys for table [Sub_Category]'
		ALTER TABLE [Sub_Category]
		ADD [Category_Id] INT NOT NULL
		ALTER TABLE [Sub_Category]
		ADD CONSTRAINT FK_Category FOREIGN KEY (Category_Id) REFERENCES Category(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Company_Role_2_Company_Permission]'
		ALTER TABLE [Company_Role_2_Company_Permission]
		ADD CONSTRAINT FK_Company_Role FOREIGN KEY (Company_Role_Id) REFERENCES Company_Role(Id)
		ALTER TABLE [Company_Role_2_Company_Permission]
		ADD CONSTRAINT FK_Company_Permission FOREIGN KEY (Company_Permission_Id) REFERENCES Company_Permission(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [User_Role_2_User_Permission]'
		ALTER TABLE [User_Role_2_User_Permission]
		ADD CONSTRAINT FK_User_Role FOREIGN KEY (User_Role_Id) REFERENCES User_Role(Id)
		ALTER TABLE [User_Role_2_User_Permission]
		ADD CONSTRAINT FK_User_Permission FOREIGN KEY (User_Permission_Id) REFERENCES User_Permission(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [User_Details]'
		ALTER TABLE [User_Details]
		ADD [User_Role_Id] INT NOT NULL
		ALTER TABLE [User_Details]
		ADD CONSTRAINT FK_User_Details_User_Role FOREIGN KEY (User_Role_Id) REFERENCES User_Role(Id)
		ALTER TABLE [User_Details]
		ADD [Company_Role_Id] INT
		ALTER TABLE [User_Details]
		ADD CONSTRAINT FK_User_Details_Company_Role FOREIGN KEY (Company_Role_Id) REFERENCES Company_Role(Id)	
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Address]'
		ALTER TABLE [Address]
		ADD [Location_Id] INT NOT NULL
		ALTER TABLE [Address]
		ADD CONSTRAINT FK_Address_Location_Id FOREIGN KEY (Location_Id) REFERENCES Location(Id)
		ALTER TABLE [Address]
		ADD [User_Details_Id] INT
		ALTER TABLE [Address]
		ADD CONSTRAINT FK_Address_User_Details_Id FOREIGN KEY (User_Details_Id) REFERENCES User_Details(Id)
		ALTER TABLE [Address]
		ADD [Company_Details_Id] INT
		ALTER TABLE [Address]
		ADD CONSTRAINT FK_Address_Company_Details_Id FOREIGN KEY (Company_Details_Id) REFERENCES Company_Details(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Attachment]'
		ALTER TABLE [Attachment]
		ADD [Batch_Att_Id] INT NOT NULL
		ALTER TABLE [Attachment]
		ADD CONSTRAINT FK_Attachment_Batch_Attachment_Id FOREIGN KEY (Batch_Att_Id) REFERENCES Batch_Attachment(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Batch_Attachment]'
		ALTER TABLE [Batch_Attachment]
		ADD [Company_Details_Id] INT NOT NULL
		ALTER TABLE [Batch_Attachment]
		ADD CONSTRAINT FK_Company_Details_Batch_Attachment_Id FOREIGN KEY (Company_Details_Id) REFERENCES Company_Details(Id)
		PRINT 'Foreign key successfully created'
		PRINT 'Create script finished succesfully'
				
	COMMIT TRANSACTION
