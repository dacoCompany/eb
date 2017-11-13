	BEGIN TRANSACTION

	PRINT 'Insert CompanyType data'
	INSERT INTO [CompanyType](Name) VALUES ('PartTime')
	INSERT INTO [CompanyType](Name) VALUES ('SelfEmployed')
	INSERT INTO [CompanyType](Name) VALUES ('Company')
	PRINT 'Insert CompanyType completed'

	PRINT 'Insert UserRole data'
	INSERT INTO [UserRole](Name) VALUES ('User')
	INSERT INTO [UserRole](Name) VALUES ('Admin')
	PRINT'Insert UserRole completed'

	PRINT 'Insert CompanyRole data'
	INSERT INTO CompanyRole(Name) VALUES ('Owner')
	INSERT INTO CompanyRole(Name) VALUES ('Admin')
	INSERT INTO CompanyRole(Name) VALUES ('Member')
	PRINT'Insert CompanyRole completed'

	PRINT 'Insert CompanyPermission data'
	INSERT INTO CompanyPermission(Name) VALUES ('AddMember')
	INSERT INTO CompanyPermission(Name) VALUES ('RemoveMember')
	INSERT INTO CompanyPermission(Name) VALUES ('AddGallery')
	INSERT INTO CompanyPermission(Name) VALUES ('RemoveGallery')
	INSERT INTO CompanyPermission(Name) VALUES ('AddAttachments')
	INSERT INTO CompanyPermission(Name) VALUES ('RemoveAttachments')
	INSERT INTO CompanyPermission(Name) VALUES ('Comment')
	INSERT INTO CompanyPermission(Name) VALUES ('CreateDemand')
	INSERT INTO CompanyPermission(Name) VALUES ('EditDemand')
	INSERT INTO CompanyPermission(Name) VALUES ('DeleteDemand')
	INSERT INTO CompanyPermission(Name) VALUES ('ChangeSettings')
	INSERT INTO CompanyPermission(Name) VALUES ('ReadOnly')
	INSERT INTO CompanyPermission(Name) VALUES ('ChangeAccountSettings')
	PRINT'Insert CompanyPermission completed'

	PRINT 'Insert CompanyRole2CompanyPermission data'
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','1')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','2')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','3')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','4')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','5')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','6')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','7')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','8')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','9')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','10')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('1','11')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','1')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','3')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','5')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','7')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','8')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('2','9')
	INSERT INTO CompanyRole2CompanyPermission(CompanyRoleId,CompanyPermissionId) VALUES ('3','12')
	PRINT'Insert CompanyRole2CompanyPermission completed'

	PRINT 'Insert UserPermission data'
	INSERT INTO UserPermission(Name) VALUES ('Read')
	INSERT INTO UserPermission(Name) VALUES ('Write')
	PRINT'Insert UserPermission completed'

	PRINT 'Insert UserRole2UserPermission data'
	INSERT INTO UserRole2UserPermission(UserRoleId, UserPermissionId) VALUES ('1','2')
	INSERT INTO UserRole2UserPermission(UserRoleId, UserPermissionId) VALUES ('2','1')
	INSERT INTO UserRole2UserPermission(UserRoleId, UserPermissionId) VALUES ('2','2')
	PRINT'Insert UserRole2UserPermission completed'

	COMMIT TRANSACTION


		