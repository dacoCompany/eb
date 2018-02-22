BEGIN;
		PRINT 'Creating Table [EmailTemplate]'
		CREATE TABLE [dbo].[EmailTemplate]
		(
			[Id] INT IDENTITY(1,1) NOT NULL,
			[Type] VARCHAR(30) NOT NULL,
			[Subject] NVARCHAR(60) NOT NULL,
			[Body] NVARCHAR(max) NOT NULL,
			[Language] VARCHAR(6) NOT NULL,
			[IsActive] BIT NOT NULL DEFAULT 'true',
			[DateCreated] DATETIME2(0) DEFAULT CURRENT_TIMESTAMP,
			[DateModified] DATETIME2(2) DEFAULT CURRENT_TIMESTAMP,
			CONSTRAINT PK_EMAIL_TEMPLATE_ID PRIMARY KEY(Id) 
		)
		PRINT 'Table created successfully'
		END 