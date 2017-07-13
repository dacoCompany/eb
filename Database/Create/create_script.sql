	
	BEGIN TRANSACTION

		/***** Creating tables *****/
		PRINT 'Creating Table [User_Account]'
		CREATE TABLE [User_Account]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
			[Title] VARCHAR(20),
			[First_Name] VARCHAR(50),
			[Second_Name] VARCHAR(50),
			[Surname] VARCHAR(50),
			[Unique_Name] VARCHAR(50) UNIQUE,
			[Phone_Number] INT NOT NULL,
			[Additional_Phone_Number] INT,
			[Email] VARCHAR(50) UNIQUE NOT NULL,
			[Password] VARCHAR(255) NOT NULL,
			[Salt] VARCHAR (255) NOT NULL,
			[Ico] VARCHAR(20),
			[Dic] VARCHAR(20),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(2) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_USER_ACCOUNT_ID PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Account_Type]'
		CREATE TABLE [Account_Type]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
			[Name] VARCHAR(30) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_Account_Type_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [User_Role]'
		CREATE TABLE [User_Role]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
			[Name] VARCHAR(30) NOT NULL,
			[Description] VARCHAR(100),
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_User_Role_Id PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Location]'
		CREATE TABLE [Location]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
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
			[Id] INT IDENTITY(1,1) NOT NULL,
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

			/***** Creating foregin keys ******/		
		PRINT 'Creating foreign key FK_User_Account_Account_Type_Id'
		ALTER TABLE [User_Account]
		ADD [At_Id] INT NOT NULL
		ALTER TABLE [User_Account]
		ADD CONSTRAINT FK_User_Account_Account_Type_Id FOREIGN KEY (At_Id) REFERENCES Account_Type(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_Address_User_Account_Id'
		ALTER TABLE [Address]
		ADD [Ua_Id] INT NOT NULL
		ALTER TABLE [Address]
		ADD CONSTRAINT FK_Address_User_Account_Id FOREIGN KEY (Ua_Id) REFERENCES User_Account(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_Address_Location_Id'
		ALTER TABLE [Address]
		ADD [Loc_Id] INT NOT NULL
		ALTER TABLE [Address]
		ADD CONSTRAINT FK_Address_Location_Id FOREIGN KEY (Loc_Id) REFERENCES Location(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_User_User_Role_Id'
		ALTER TABLE [User_Account]
		ADD [Ro_Id] INT NOT NULL
		ALTER TABLE [User_Account]
		ADD CONSTRAINT FK_User_User_Role_Id FOREIGN KEY (Ro_Id) REFERENCES User_Role(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_Category_Main_Category_Id'
		ALTER TABLE [Category]
		ADD [M_Cat_Id] INT NOT NULL
		ALTER TABLE [Category]
		ADD CONSTRAINT FK_Category_Main_Category_Id FOREIGN KEY (M_Cat_Id) REFERENCES Main_Category(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_Sub_Category_Category_Id'
		ALTER TABLE [Sub_Category]
		ADD [Cat_Id] INT NOT NULL
		ALTER TABLE [Sub_Category]
		ADD CONSTRAINT FK_Sub_Category_Category_Id FOREIGN KEY (Cat_Id) REFERENCES Category(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_User_Account_Sub_Category_Id'
		ALTER TABLE [User_Account]
		ADD [Sub_Cat_Id] INT
		ALTER TABLE [User_Account]
		ADD CONSTRAINT FK_User_Account_Sub_Category_Id FOREIGN KEY (Sub_Cat_Id) REFERENCES Sub_Category(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign key FK_Batch_Attachment_Attachment_Id'
		ALTER TABLE [Batch_Attachment]
		ADD [Att_Id] INT NOT NULL
		ALTER TABLE [Batch_Attachment]
		ADD CONSTRAINT FK_Batch_Attachment_Attachment_Id FOREIGN KEY (Att_Id) REFERENCES Attachment(Id)
		PRINT 'Foreign key successfully created'
		
		PRINT 'Creating foreign key FK_User_Account_Batch_Attachment_Id'
		ALTER TABLE [User_Account]
		ADD [Batch_Att_Id] INT NOT NULL
		ALTER TABLE [User_Account]
		ADD CONSTRAINT FK_User_Account_Batch_Attachment_Id FOREIGN KEY (Batch_Att_Id) REFERENCES Batch_Attachment(Id)
		PRINT 'Foreign key successfully created'
				
		COMMIT TRANSACTION


		