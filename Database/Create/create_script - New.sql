	
	BEGIN TRANSACTION

		/***** Creating tables *****/
		PRINT 'Creating Table [UserDetails]'
		CREATE TABLE [UserDetails]
		(
			[Id] INT IDENTITY(1,1),
			[Title] VARCHAR(20),
			[FirstName] VARCHAR(50) NOT NULL,
			[SecondName] VARCHAR(50),
			[Surname] VARCHAR(50) NOT NULL,
			[DisplayName] VARCHAR(50) NOT NULL,
			[PhoneNumber] VARCHAR(20),
			[AdditionalPhoneNumber] VARCHAR(20),
			[Email] VARCHAR(50) UNIQUE NOT NULL,
			[Salt] VARCHAR (25) NOT NULL,
			[Password] VARCHAR(255) NOT NULL,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKUserDetailsId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [UserRole]'
		CREATE TABLE [UserRole]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKUserRoleId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [UserPermission]'
		CREATE TABLE [UserPermission]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKUserPermissionId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [UserRole2UserPermission]'
		CREATE TABLE [UserRole2UserPermission]
		(
			[Id] INT IDENTITY(1,1),
			[UserRoleId] INT,
			[UserPermissionId] INT,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKUserRole2UserPermissionId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [CompanyDetails]'
		CREATE TABLE [CompanyDetails]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(100) NOT NULL,
			[Description] VARCHAR(250),
			[PhoneNumber] VARCHAR(20),
			[AdditionalPhoneNumber] VARCHAR(20),
			[Ico] INT,
			[Dic] INT,
			[IsCompanyVerified] BIT NOT NULL DEFAULT 'false',
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyDetailsId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [CompanyDetails2UserDetails]'
		CREATE TABLE [CompanyDetails2UserDetails]
		(
			[Id] INT IDENTITY(1,1),
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyDetails2UserDetails PRIMARY KEY(Id)
		)

		PRINT 'Creating Table [CompanyType]'
		CREATE TABLE [CompanyType]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[Description] VARCHAR(100),
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyTypeId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [CompanyRole]'
		CREATE TABLE [CompanyRole]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL UNIQUE,
			[Code] VARCHAR(30) NOT NULL UNIQUE,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyRoleId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [CompanyPermission]'
		CREATE TABLE [CompanyPermission]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(30) NOT NULL,
			[Code] VARCHAR(30) NOT NULL,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyPermissionId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [CompanyRole2CompanyPermission]'
		CREATE TABLE [CompanyRole2CompanyPermission]
		(
			[Id] INT IDENTITY(1,1),
			[CompanyRoleId] INT,
			[CompanyPermissionId] INT,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCompanyRole2CompanyPermissionId PRIMARY KEY(Id) 
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
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKLocationId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Address]'
		CREATE TABLE [Address]
		(
			[Id] INT IDENTITY(1,1),
			[Street] VARCHAR(100),
			[Number] VARCHAR(10),
			[IsDeliveryAddress] BIT DEFAULT 'false',
			[IsBillingAddress] BIT DEFAULT 'false',
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKAddressId PRIMARY KEY(Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Category]'
		CREATE TABLE [Category]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCategoryId PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [SubCategory]'
		CREATE TABLE [SubCategory]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKSubCategoryId PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [BatchAttachment]'
		CREATE TABLE [BatchAttachment]
		(
			[Id] INT IDENTITY(1,1),
			[Name] VARCHAR(50) NOT NULL,
			[Description] VARCHAR(100),
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKBatchAttachmentId PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'
		
		PRINT 'Creating Table [Attachment]'
		CREATE TABLE [Attachment]
		(
			[Id] INT IDENTITY(1,1),
			[OriginalUrl] VARCHAR(255) NOT NULL,
			[ThumbnailUrl] VARCHAR(255),			
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKAttachmentId PRIMARY KEY (Id),
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [Category2CompanyDetails]'
		CREATE TABLE [Category2CompanyDetails]
		(
			[Id] INT IDENTITY(1,1),
			[CompanyDetailsId] INT,
			[CategoryId] INT,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKCategory2CompanyDetailsId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		PRINT 'Creating Table [SubCategory2CompanyDetails]'
		CREATE TABLE [SubCategory2CompanyDetails]
		(
			[Id] INT IDENTITY(1,1),
			[CompanyDetailsId] INT,
			[SubCategoryId] INT,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PKSubCategory2CompanyDetailsId PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'

		/***** Creating foreign keys *****/

		PRINT 'Creating foreign keys for table [CompanyDetails2UserDetails]'
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD [CompanyDetailsId] INT NOT NULL
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD	[UserDetailsId] INT NOT NULL
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD	CONSTRAINT FKCompanyDetails FOREIGN KEY (CompanyDetailsId) REFERENCES CompanyDetails(Id)
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD	CONSTRAINT FKUserDetails FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD [CompanyRoleId] INT NOT NULL
		ALTER TABLE [CompanyDetails2UserDetails]
		ADD CONSTRAINT FKCompanyDetails2UserDetails FOREIGN KEY (CompanyRoleId) REFERENCES CompanyRole(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [CompanyDetails]'
		ALTER TABLE [CompanyDetails]
		ADD [CompanyTypeId] INT NOT NULL
		ALTER TABLE [CompanyDetails]
		ADD	CONSTRAINT FKCompanyType FOREIGN KEY (CompanyTypeId) REFERENCES CompanyType(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [SubCategory]'
		ALTER TABLE [SubCategory]
		ADD [CategoryId] INT NOT NULL
		ALTER TABLE [SubCategory]
		ADD CONSTRAINT FKCategory FOREIGN KEY (CategoryId) REFERENCES Category(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [CompanyRole2CompanyPermission]'
		ALTER TABLE [CompanyRole2CompanyPermission]
		ADD CONSTRAINT FKCompanyRole FOREIGN KEY (CompanyRoleId) REFERENCES CompanyRole(Id)
		ALTER TABLE [CompanyRole2CompanyPermission]
		ADD CONSTRAINT FKCompanyPermission FOREIGN KEY (CompanyPermissionId) REFERENCES CompanyPermission(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [UserRole2UserPermission]'
		ALTER TABLE [UserRole2UserPermission]
		ADD CONSTRAINT FKUserRole FOREIGN KEY (UserRoleId) REFERENCES UserRole(Id)
		ALTER TABLE [UserRole2UserPermission]
		ADD CONSTRAINT FKUserPermission FOREIGN KEY (UserPermissionId) REFERENCES UserPermission(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [UserDetails]'
		ALTER TABLE [UserDetails]
		ADD [UserRoleId] INT NOT NULL
		ALTER TABLE [UserDetails]
		ADD CONSTRAINT FKUserDetailsUserRole FOREIGN KEY (UserRoleId) REFERENCES UserRole(Id)			
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Address]'
		ALTER TABLE [Address]
		ADD [LocationId] INT NOT NULL
		ALTER TABLE [Address]
		ADD CONSTRAINT FKAddressLocationId FOREIGN KEY (LocationId) REFERENCES Location(Id)
		ALTER TABLE [Address]
		ADD [UserDetailsId] INT
		ALTER TABLE [Address]
		ADD CONSTRAINT FKAddressUserDetailsId FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
		ALTER TABLE [Address]
		ADD [CompanyDetailsId] INT
		ALTER TABLE [Address]
		ADD CONSTRAINT FKAddressCompanyDetailsId FOREIGN KEY (CompanyDetailsId) REFERENCES CompanyDetails(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [Attachment]'
		ALTER TABLE [Attachment]
		ADD [BatchAttId] INT NOT NULL
		ALTER TABLE [Attachment]
		ADD CONSTRAINT FKAttachmentBatchAttachmentId FOREIGN KEY (BatchAttId) REFERENCES BatchAttachment(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [BatchAttachment]'
		ALTER TABLE [BatchAttachment]
		ADD [CompanyDetailsId] INT NOT NULL
		ALTER TABLE [BatchAttachment]
		ADD CONSTRAINT FKCompanyDetailsBatchAttachmentId FOREIGN KEY (CompanyDetailsId) REFERENCES CompanyDetails(Id)
		PRINT 'Foreign key successfully created'

		PRINT 'Creating foreign keys for table [Category2CompanyDetails]'
		ALTER TABLE [Category2CompanyDetails]
		ADD	CONSTRAINT FKCategory2CompanyDetailsCompanyDetails FOREIGN KEY (CompanyDetailsId) REFERENCES CompanyDetails(Id)
		ALTER TABLE [Category2CompanyDetails]
		ADD	CONSTRAINT FKCategory2CompanyDetailsCategory FOREIGN KEY (CategoryId) REFERENCES Category(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Creating foreign keys for table [SubCategory2CompanyDetails]'
		ALTER TABLE [SubCategory2CompanyDetails]
		ADD	CONSTRAINT FKSubCategory2CompanyDetailsCompanyDetails FOREIGN KEY (CompanyDetailsId) REFERENCES CompanyDetails(Id)
		ALTER TABLE [SubCategory2CompanyDetails]
		ADD	CONSTRAINT FKSubCategory2CompanyDetailsSubCategory FOREIGN KEY (SubCategoryId) REFERENCES SubCategory(Id)
		PRINT 'Foreign keys successfully created'

		PRINT 'Create script finished successfully'
				
	COMMIT TRANSACTION
