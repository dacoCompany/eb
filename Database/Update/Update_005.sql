BEGIN;
		PRINT 'Creating Table [EmailTemplate]'
		CREATE TABLE [dbo].[EmailTemplate]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
			[Type] VARCHAR(30) NOT NULL,
			[SkSubject] VARCHAR(50),
			[SkBody] NVARCHAR,
			[CzSubject] VARCHAR(50),
			[CzBody] NVARCHAR,
			[HuSubject] VARCHAR(50),
			[HuBody] NVARCHAR,
			[EnSubject] VARCHAR(50),
			[EnBody] NVARCHAR,
			[Is_Active] BIT NOT NULL DEFAULT 'true',
			[Date_Created] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[Date_Modified] DATETIME2(2) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_EMAIL_TEMPLATE_ID PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'
		END 