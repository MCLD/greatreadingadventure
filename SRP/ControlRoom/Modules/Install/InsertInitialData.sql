INSERT INTO Tenant (
	[Name],
	[LandingName],
	[AdminName],
	[isActiveFlag],
	[isMasterFlag],
	[Description],
	[DomainName],
	[showNotifications],
	[showOffers],
	[showBadges],
	[showEvents],
	[NotificationsMenuText],
	[OffersMenuText],
	[BadgesMenuText],
	[EventsMenuText],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'Master Tenant',
	'Great Reading Adventure',
	'Master Tenant',
	1,
	1,
	'<p>Master Great Reading Adventure Tenant</p>',
	'www.greatreadingadventure.com',
	0,
	0,
	0,
	0,
	'',
	'',
	'',
	'',
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	1000,
	'Modify Security Permission',
	'Allows a user to access any of the screens in the security module',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	2000,
	'Delete Program Permission',
	'Allows a user to trigger the Purge Program functionality',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	2100,
	'Add Program Permission',
	'Allows a user to create a new Program',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	2200,
	'Modify  Program Permission',
	'Allows a user to create a new Program',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	3000,
	'System Setup Permission',
	'Allows a user to access any of the screens used to perform the setup of the system',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4000,
	'Drawings Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4100,
	'Settings Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4200,
	'Reports Management',
	'Allows access to reports',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4300,
	'Games Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4400,
	'Book Lists Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4500,
	'Events Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4600,
	'Offers Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4700,
	'Badges Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4800,
	'Avatars Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	4900,
	'Awards Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5000,
	'Notifications Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5100,
	'Patrons Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5200,
	'Test Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5300,
	'Reviews Management',
	'Allows access to the respective maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	8000,
	'Master Tenant Administrator',
	'Allows access to the tenant maintenance screens.',
	NULL
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'SysName',
	'Great Reading Adventure',
	'Text',
	'TextBox',
	0,
	'System Name',
	'Friendly name given to the application',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'ContactEmail',
	'info@test.greatreadingadventure.com',
	'Text',
	'TextBox',
	0,
	'Contact Email Address',
	'The user exposed contact us email address',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'ContactName',
	'System Administrator',
	'Text',
	'TextBox',
	0,
	'Contact Name',
	'The name associated with the contact email address.',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'FromEmailName',
	'Webmaster',
	'Text',
	'TextBox',
	0,
	'From Email Name',
	'Name of the person impersonated in system generated messages (e.g. Webmaster)',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'FromEmailAddress',
	'info@test.greatreadingadventure.com',
	'Text',
	'TextBox',
	0,
	'From Email Address',
	'System generated messages email address ',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'PageSize',
	'10',
	'int',
	'TextBox',
	0,
	'# of Items Per Page',
	'Page Size',
	'',
	'5',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'MaxPtsDay',
	'500',
	'int',
	'TextBox',
	0,
	'Max Points Per Day',
	'Maximum Points/Day allowed for logging',
	'',
	'500',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'MaxMin',
	'60',
	'int',
	'TextBox',
	0,
	'Max Log Min/Entry',
	'Maximum Minutes/Entry allowed for logging',
	'',
	'60',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'MaxBook',
	'2',
	'int',
	'TextBox',
	0,
	'Max Log Books/Entry',
	'Maximum Books/Entry allowed for logging',
	'',
	'5',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'MaxPage',
	'90',
	'int',
	'TextBox',
	0,
	'Max Log Page/Entry',
	'Maximum Page/Entry allowed for logging',
	'',
	'100',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPSettings (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'DupEvtCodes',
	'False',
	'int',
	'Checkbox',
	0,
	'Duplicate Secret Codes',
	'Whether or not duplicate secret codes are allowed',
	'',
	'',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'Branch',
	'Library Branches',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'Library District',
	'Library District',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'School Type',
	'Types of schools',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'School',
	'School Names',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'School District',
	'School District Names',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'Badge Category',
	'Badge Category',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'Badge Age Group',
	'Badge Age Group',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CodeType (
	[isSystem],
	[CodeTypeName],
	[Description],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	'Badge Location',
	'Badge Location',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO CustomEventFields (
	[Use1],
	[Label1],
	[DDValues1],
	[Use2],
	[Use3],
	[Label2],
	[Label3],
	[DDValues2],
	[DDValues3],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	0,
	'Custom 1',
	'',
	0,
	0,
	'Custom 2',
	'Custom 3',
	'',
	'',
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)

INSERT INTO CustomRegistrationFields (
	[Use1],
	[Label1],
	[DDValues1],
	[Use2],
	[Use3],
	[Use4],
	[Use5],
	[Label2],
	[Label3],
	[Label4],
	[Label5],
	[DDValues2],
	[DDValues3],
	[DDValues4],
	[DDValues5],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	0,
	'Custom 1',
	'1',
	0,
	0,
	0,
	0,
	'',
	'',
	'',
	'',
	'',
	'',
	'',
	'',
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO RegistrationSettings (
	[Literacy1Label],
	[Literacy2Label],
	[DOB_Prompt],
	[Age_Prompt],
	[SchoolGrade_Prompt],
	[FirstName_Prompt],
	[MiddleName_Prompt],
	[LastName_Prompt],
	[Gender_Prompt],
	[EmailAddress_Prompt],
	[PhoneNumber_Prompt],
	[StreetAddress1_Prompt],
	[StreetAddress2_Prompt],
	[City_Prompt],
	[State_Prompt],
	[ZipCode_Prompt],
	[Country_Prompt],
	[County_Prompt],
	[ParentGuardianFirstName_Prompt],
	[ParentGuardianLastName_Prompt],
	[ParentGuardianMiddleName_Prompt],
	[PrimaryLibrary_Prompt],
	[LibraryCard_Prompt],
	[SchoolName_Prompt],
	[District_Prompt],
	[Teacher_Prompt],
	[GroupTeamName_Prompt],
	[SchoolType_Prompt],
	[LiteracyLevel1_Prompt],
	[LiteracyLevel2_Prompt],
	[ParentPermFlag_Prompt],
	[Over18Flag_Prompt],
	[ShareFlag_Prompt],
	[TermsOfUseflag_Prompt],
	[Custom1_Prompt],
	[Custom2_Prompt],
	[Custom3_Prompt],
	[Custom4_Prompt],
	[Custom5_Prompt],
	[DOB_Req],
	[Age_Req],
	[SchoolGrade_Req],
	[FirstName_Req],
	[MiddleName_Req],
	[LastName_Req],
	[Gender_Req],
	[EmailAddress_Req],
	[PhoneNumber_Req],
	[StreetAddress1_Req],
	[StreetAddress2_Req],
	[City_Req],
	[State_Req],
	[ZipCode_Req],
	[Country_Req],
	[County_Req],
	[ParentGuardianFirstName_Req],
	[ParentGuardianLastName_Req],
	[ParentGuardianMiddleName_Req],
	[PrimaryLibrary_Req],
	[LibraryCard_Req],
	[SchoolName_Req],
	[District_Req],
	[Teacher_Req],
	[GroupTeamName_Req],
	[SchoolType_Req],
	[LiteracyLevel1_Req],
	[LiteracyLevel2_Req],
	[ParentPermFlag_Req],
	[Over18Flag_Req],
	[ShareFlag_Req],
	[TermsOfUseflag_Req],
	[Custom1_Req],
	[Custom2_Req],
	[Custom3_Req],
	[Custom4_Req],
	[Custom5_Req],
	[DOB_Show],
	[Age_Show],
	[SchoolGrade_Show],
	[FirstName_Show],
	[MiddleName_Show],
	[LastName_Show],
	[Gender_Show],
	[EmailAddress_Show],
	[PhoneNumber_Show],
	[StreetAddress1_Show],
	[StreetAddress2_Show],
	[City_Show],
	[State_Show],
	[ZipCode_Show],
	[Country_Show],
	[County_Show],
	[ParentGuardianFirstName_Show],
	[ParentGuardianLastName_Show],
	[ParentGuardianMiddleName_Show],
	[PrimaryLibrary_Show],
	[LibraryCard_Show],
	[SchoolName_Show],
	[District_Show],
	[Teacher_Show],
	[GroupTeamName_Show],
	[SchoolType_Show],
	[LiteracyLevel1_Show],
	[LiteracyLevel2_Show],
	[ParentPermFlag_Show],
	[Over18Flag_Show],
	[ShareFlag_Show],
	[TermsOfUseflag_Show],
	[Custom1_Show],
	[Custom2_Show],
	[Custom3_Show],
	[Custom4_Show],
	[Custom5_Show],
	[DOB_Edit],
	[Age_Edit],
	[SchoolGrade_Edit],
	[FirstName_Edit],
	[MiddleName_Edit],
	[LastName_Edit],
	[Gender_Edit],
	[EmailAddress_Edit],
	[PhoneNumber_Edit],
	[StreetAddress1_Edit],
	[StreetAddress2_Edit],
	[City_Edit],
	[State_Edit],
	[ZipCode_Edit],
	[Country_Edit],
	[County_Edit],
	[ParentGuardianFirstName_Edit],
	[ParentGuardianLastName_Edit],
	[ParentGuardianMiddleName_Edit],
	[PrimaryLibrary_Edit],
	[LibraryCard_Edit],
	[SchoolName_Edit],
	[District_Edit],
	[Teacher_Edit],
	[GroupTeamName_Edit],
	[SchoolType_Edit],
	[LiteracyLevel1_Edit],
	[LiteracyLevel2_Edit],
	[ParentPermFlag_Edit],
	[Over18Flag_Edit],
	[ShareFlag_Edit],
	[TermsOfUseflag_Edit],
	[Custom1_Edit],
	[Custom2_Edit],
	[Custom3_Edit],
	[Custom4_Edit],
	[Custom5_Edit],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[SDistrict_Prompt],
	[SDistrict_Req],
	[SDistrict_Show],
	[SDistrict_Edit],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'AR Level',
	'Lexile Level',
	0,
	0,
	0,
	1,
	0,
	1,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	1,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	1,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	0,
	0,
	0,
	0,
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPGroups (
	[GroupName],
	[GroupDescription],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'Administrator',
	'Full Access',
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPUser (
	[Username],
	[Password],
	[FirstName],
	[LastName],
	[EmailAddress],
	[Division],
	[Department],
	[Title],
	[IsActive],
	[MustResetPassword],
	[IsDeleted],
	[LastPasswordReset],
	[DeletedDate],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	'sysadmin',
	'150000:2t2eayBf+XQTMQNS/E2MyYJYRLwQHVL4:OEGFWEkidgSP85PP3Yg5IZOUS04=',
	'System',
	'Administrator',
	'info@test.greatreadingadventure.com',
	'',
	'',
	'',
	1,
	1,
	0,
	GETDATE(),
	NULL,
	GETDATE(),
	'sysadmin',
	GETDATE(),
	'N/A',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	'',
	'',
	''
	)
GO

INSERT INTO SRPUserGroups (
	[UID],
	[GID],
	[AddedDate],
	[AddedUser]
	)
SELECT UID,
	GID,
	GETDATE(),
	'sysadmin'
FROM SRPUser
FULL JOIN SRPGroups ON 1 = 1
GO

INSERT INTO SRPGroupPermissions (
	[GID],
	[PermissionID],
	[AddedDate],
	[AddedUser]
	)
SELECT GID,
	PermissionID,
	GETDATE(),
	'sysadmin'
FROM SRPGroups
FULL JOIN SRPPermissionsMaster ON 1 = 1
GO

SET IDENTITY_INSERT [dbo].[Programs] ON
GO

INSERT [dbo].[Programs] (
	[PID],
	[AdminName],
	[Title],
	[TabName],
	[POrder],
	[IsActive],
	[IsHidden],
	[StartDate],
	[EndDate],
	[MaxAge],
	[MaxGrade],
	[LoggingStart],
	[LoggingEnd],
	[ParentalConsentFlag],
	[ParentalConsentText],
	[PatronReviewFlag],
	[LogoutURL],
	[ProgramGameID],
	[HTML1],
	[HTML2],
	[HTML3],
	[HTML4],
	[HTML5],
	[HTML6],
	[BannerImage],
	[RegistrationBadgeID],
	[CompletionPoints],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[LastModDate],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3],
	[PreTestID],
	[PostTestID],
	[PreTestMandatory],
	[PretestEndDate],
	[PostTestStartDate]
	)
VALUES (
	1,
	N'The Great Reading Adventure',
	N'The Great Reading Adventure',
	N'Reading Program',
	1,
	1,
	0,
	DATEADD(d, 0, DATEDIFF(d, 0, GETDATE())),
	DATEADD(yy, 1, DATEADD(d, 0, DATEDIFF(d, 0, GETDATE()))),
	0,
	0,
	DATEADD(d, 0, DATEDIFF(d, 0, GETDATE())),
	DATEADD(yy, 1, DATEADD(d, 0, DATEDIFF(d, 0, GETDATE()))),
	0,
	N'',
	0,
	N'',
	0,
	N'<p>Welcome to the Great Reading Adventure! Sign up and log your reading to enjoy adventures, take on challenges, earn badges, and attend events!</p>',
	N'',
	N'<p>For more information about this reading program, visit your local library!</p><p>While you''re there, go on a journey of discovery! Visit fabulous destinations such as Hogwarts, Narnia, Oz, and Middle Earth!</p>',
	N'<p>Reading for 20 minutes a day helps build a strong lifelong reading habit.</p><p>Reading is fundamental for developing literacy skills!</p>',
	N'',
	N'<p>Our reading program has not started yet! Sign up now and you''ll be ready to log your reading the day the program starts!</p>',
	N'',
	0,
	0,
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N'',
	0,
	0,
	0,
	NULL,
	NULL
	)
GO

SET IDENTITY_INSERT [dbo].[Programs] OFF
GO

SET IDENTITY_INSERT [dbo].[ProgramGamePointConversion] ON
GO

INSERT [dbo].[ProgramGamePointConversion] (
	[PGCID],
	[PGID],
	[ActivityTypeId],
	[ActivityCount],
	[PointCount],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser]
	)
VALUES (
	1,
	1,
	0,
	1,
	0,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin'
	)
GO

INSERT [dbo].[ProgramGamePointConversion] (
	[PGCID],
	[PGID],
	[ActivityTypeId],
	[ActivityCount],
	[PointCount],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser]
	)
VALUES (
	2,
	1,
	1,
	1,
	0,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin'
	)
GO

INSERT [dbo].[ProgramGamePointConversion] (
	[PGCID],
	[PGID],
	[ActivityTypeId],
	[ActivityCount],
	[PointCount],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser]
	)
VALUES (
	3,
	1,
	3,
	1,
	1,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin'
	)
GO

SET IDENTITY_INSERT [dbo].[ProgramGamePointConversion] OFF
GO

SET IDENTITY_INSERT [dbo].[Avatar] ON
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	1,
	N'Percy',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	2,
	N'Katniss',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	3,
	N'Othello',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	4,
	N'Hester',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	5,
	N'Gandalf',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

INSERT [dbo].[Avatar] (
	[AID],
	[Name],
	[Gender],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID],
	[FldInt1],
	[FldInt2],
	[FldInt3],
	[FldBit1],
	[FldBit2],
	[FldBit3],
	[FldText1],
	[FldText2],
	[FldText3]
	)
VALUES (
	6,
	N'Cersei',
	N'O',
	GetDate(),
	N'sysadmin',
	GetDate(),
	N'sysadmin',
	1,
	0,
	0,
	0,
	0,
	0,
	0,
	N'',
	N'',
	N''
	)
GO

SET IDENTITY_INSERT [dbo].[Avatar] OFF
GO

IF (
		EXISTS (
			SELECT *
			FROM [INFORMATION_SCHEMA].[TABLES]
			WHERE [TABLE_SCHEMA] = 'dbo'
				AND [TABLE_NAME] = 'SRPHistory'
			)
		)
BEGIN
	INSERT [dbo].[SRPHistory] (
		[When],
		[Who],
		[Event],
		[VersionMajor],
		[VersionMinor],
		[VersionPatch],
		[Description]
		)
	VALUES (
		GETDATE(),
		N'sysadmin',
		'Initial configuration',
		3,
		0,
		3,
		'Performed initial configuration with a single program'
		)
END
