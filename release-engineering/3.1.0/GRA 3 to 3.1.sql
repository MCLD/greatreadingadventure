/*
SQL script to update The Great Reading Adventure database from version 3.0.x to 3.1.0.
*/
GO

PRINT N'GRA 3.0.x to 3.1.0 database update script starting...'

BEGIN TRAN UPDATEGRA;

SET ANSI_NULLS,
	ANSI_PADDING,
	ANSI_WARNINGS,
	ARITHABORT,
	CONCAT_NULL_YIELDS_NULL,
	QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

PRINT N'Dropping [dbo].[DF_Award_BadgeID]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_BadgeID];
GO

PRINT N'Dropping [dbo].[DF_Award_BranchID]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_BranchID];
GO

PRINT N'Dropping [dbo].[DF_Award_NumPoints]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_NumPoints];
GO

PRINT N'Dropping [dbo].[DF_Award_BadgeList]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_BadgeList];
GO

PRINT N'Dropping [dbo].[DF_Award_SchoolName]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_SchoolName];
GO

PRINT N'Dropping [dbo].[DF_Award_District]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_District];
GO

PRINT N'Dropping [dbo].[DF_Award_ProgramID]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_ProgramID];
GO

PRINT N'Dropping [dbo].[DF_Award_AddedUser]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_AddedUser];
GO

PRINT N'Dropping [dbo].[DF_Award_AddedDate]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_AddedDate];
GO

PRINT N'Dropping [dbo].[DF_Award_LastModUser]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_LastModUser];
GO

PRINT N'Dropping [dbo].[DF_Award_LastModDate]...';
GO

ALTER TABLE [dbo].[Award]

DROP CONSTRAINT [DF_Award_LastModDate];
GO

PRINT N'Dropping [dbo].[DF_Notifications_Subject]...';
GO

ALTER TABLE [dbo].[Notifications]

DROP CONSTRAINT [DF_Notifications_Subject];
GO

PRINT N'Dropping [dbo].[DF_Offer_RedirectURL]...';
GO

ALTER TABLE [dbo].[Offer]

DROP CONSTRAINT [DF_Offer_RedirectURL];
GO

PRINT N'Dropping [dbo].[DF_Patron_IsMasterAccount]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_IsMasterAccount];
GO

PRINT N'Dropping [dbo].[DF_Patron_RegistrationDate]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_RegistrationDate];
GO

PRINT N'Dropping [dbo].[DF_Patron_Score1]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_Score1];
GO

PRINT N'Dropping [dbo].[DF_Patron_Score2]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_Score2];
GO

PRINT N'Dropping [dbo].[DF_Patron_Score1Pct]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_Score1Pct];
GO

PRINT N'Dropping [dbo].[DF_Patron_Score2Pct]...';
GO

ALTER TABLE [dbo].[Patron]

DROP CONSTRAINT [DF_Patron_Score2Pct];
GO

PRINT N'Dropping [dbo].[DF_Programs_IsActive]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_IsActive];
GO

PRINT N'Dropping [dbo].[DF_Programs_LastModDate]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_LastModDate];
GO

PRINT N'Dropping [dbo].[DF_Programs_AddedUser]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_AddedUser];
GO

PRINT N'Dropping [dbo].[DF_Programs_AddedDate]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_AddedDate];
GO

PRINT N'Dropping [dbo].[DF_Programs_LastModUser]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_LastModUser];
GO

PRINT N'Dropping [dbo].[DF_Programs_CompletionPoints]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_CompletionPoints];
GO

PRINT N'Dropping [dbo].[DF_Programs_ParentalConsentFlag]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_ParentalConsentFlag];
GO

PRINT N'Dropping [dbo].[DF_Programs_IsHidden]...';
GO

ALTER TABLE [dbo].[Programs]

DROP CONSTRAINT [DF_Programs_IsHidden];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_Literacy1Label]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_Literacy1Label];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_AddedUser]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_AddedUser];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_AddedDate]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_AddedDate];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_LastModUser]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_LastModUser];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_LastModDate]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_LastModDate];
GO

PRINT N'Dropping [dbo].[DF_RegistrationSettings_Literacy2Label]...';
GO

ALTER TABLE [dbo].[RegistrationSettings]

DROP CONSTRAINT [DF_RegistrationSettings_Literacy2Label];
GO

PRINT N'Dropping [dbo].[DF_Tenant_BadgesMenuText]...';
GO

ALTER TABLE [dbo].[Tenant]

DROP CONSTRAINT [DF_Tenant_BadgesMenuText];
GO

PRINT N'Dropping [dbo].[DF_Tenant_EventsMenuText]...';
GO

ALTER TABLE [dbo].[Tenant]

DROP CONSTRAINT [DF_Tenant_EventsMenuText];
GO

PRINT N'Dropping [dbo].[DF_Tenant_NotificationsMenuText]...';
GO

ALTER TABLE [dbo].[Tenant]

DROP CONSTRAINT [DF_Tenant_NotificationsMenuText];
GO

PRINT N'Dropping [dbo].[DF_Tenant_OffersMenuText]...';
GO

ALTER TABLE [dbo].[Tenant]

DROP CONSTRAINT [DF_Tenant_OffersMenuText];
GO

PRINT N'Dropping [dbo].[DF_Avatar_AddedDate]...';
GO

ALTER TABLE [dbo].[Avatar]

DROP CONSTRAINT [DF_Avatar_AddedDate];
GO

PRINT N'Dropping [dbo].[DF_Avatar_LastModUser]...';
GO

ALTER TABLE [dbo].[Avatar]

DROP CONSTRAINT [DF_Avatar_LastModUser];
GO

PRINT N'Dropping [dbo].[DF_Avatar_LastModDate]...';
GO

ALTER TABLE [dbo].[Avatar]

DROP CONSTRAINT [DF_Avatar_LastModDate];
GO

PRINT N'Dropping [dbo].[DF_Avatar_AddedUser]...';
GO

ALTER TABLE [dbo].[Avatar]

DROP CONSTRAINT [DF_Avatar_AddedUser];
GO

PRINT N'Dropping [dbo].[FK_ProgramCodes_Programs]...';
GO

ALTER TABLE [dbo].[ProgramCodes]

DROP CONSTRAINT [FK_ProgramCodes_Programs];
GO

PRINT N'Dropping [dbo].[FK_ProgramGamePointConversion_Programs]...';
GO

ALTER TABLE [dbo].[ProgramGamePointConversion]

DROP CONSTRAINT [FK_ProgramGamePointConversion_Programs];
GO

PRINT N'Dropping [dbo].[app_Avatar_Delete]...';
GO

DROP PROCEDURE [dbo].[app_Avatar_Delete];
GO

PRINT N'Dropping [dbo].[app_Avatar_GetAll]...';
GO

DROP PROCEDURE [dbo].[app_Avatar_GetAll];
GO

PRINT N'Dropping [dbo].[app_Avatar_GetByID]...';
GO

DROP PROCEDURE [dbo].[app_Avatar_GetByID];
GO

PRINT N'Dropping [dbo].[app_Avatar_Insert]...';
GO

DROP PROCEDURE [dbo].[app_Avatar_Insert];
GO

PRINT N'Dropping [dbo].[app_Avatar_Update]...';
GO

DROP PROCEDURE [dbo].[app_Avatar_Update];
GO

PRINT N'Dropping [dbo].[app_Event_GetAdminSearch]...';
GO

DROP PROCEDURE [dbo].[app_Event_GetAdminSearch];
GO

PRINT N'Dropping [dbo].[Avatar]...';
GO

DROP TABLE [dbo].[Avatar];
GO

PRINT N'Starting rebuilding table [dbo].[Award]...';
GO

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Award] (
	[AID] INT IDENTITY(1, 1) NOT NULL,
	[AwardName] VARCHAR(80) NULL,
	[BadgeID] INT CONSTRAINT [DF_Award_BadgeID] DEFAULT((0)) NULL,
	[NumPoints] INT CONSTRAINT [DF_Award_NumPoints] DEFAULT((0)) NULL,
	[BranchID] INT CONSTRAINT [DF_Award_BranchID] DEFAULT((0)) NULL,
	[ProgramID] INT CONSTRAINT [DF_Award_ProgramID] DEFAULT((0)) NULL,
	[District] VARCHAR(50) CONSTRAINT [DF_Award_District] DEFAULT('') NULL,
	[SchoolName] VARCHAR(50) CONSTRAINT [DF_Award_SchoolName] DEFAULT('') NULL,
	[BadgeList] VARCHAR(500) CONSTRAINT [DF_Award_BadgeList] DEFAULT('') NULL,
	[BadgesAchieved] INT NULL,
	[LastModDate] DATETIME CONSTRAINT [DF_Award_LastModDate] DEFAULT(getdate()) NULL,
	[LastModUser] VARCHAR(50) CONSTRAINT [DF_Award_LastModUser] DEFAULT('N/A') NULL,
	[AddedDate] DATETIME CONSTRAINT [DF_Award_AddedDate] DEFAULT(getdate()) NULL,
	[AddedUser] VARCHAR(50) CONSTRAINT [DF_Award_AddedUser] DEFAULT('N/A') NULL,
	[GoalPercent] INT NULL,
	[TenID] INT NULL,
	[FldInt1] INT NULL,
	[FldInt2] INT NULL,
	[FldInt3] INT NULL,
	[FldBit1] BIT NULL,
	[FldBit2] BIT NULL,
	[FldBit3] BIT NULL,
	[FldText1] TEXT NULL,
	[FldText2] TEXT NULL,
	[FldText3] TEXT NULL,
	CONSTRAINT [tmp_ms_xx_constraint_PK_Award1] PRIMARY KEY CLUSTERED ([AID] ASC)
	);

IF EXISTS (
		SELECT TOP 1 1
		FROM [dbo].[Award]
		)
BEGIN
	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Award] ON;

	INSERT INTO [dbo].[tmp_ms_xx_Award] (
		[AID],
		[AwardName],
		[BadgeID],
		[NumPoints],
		[BranchID],
		[ProgramID],
		[District],
		[SchoolName],
		[BadgeList],
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
	SELECT [AID],
		[AwardName],
		[BadgeID],
		[NumPoints],
		[BranchID],
		[ProgramID],
		[District],
		[SchoolName],
		[BadgeList],
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
	FROM [dbo].[Award]
	ORDER BY [AID] ASC;

	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Award] OFF;
END

DROP TABLE [dbo].[Award];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Award]',
	N'Award';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Award1]',
	N'PK_Award',
	N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
GO

PRINT N'Altering [dbo].[Badge]...';
GO

ALTER TABLE [dbo].[Badge]

ALTER COLUMN [AdminName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Badge]

ALTER COLUMN [UserName] NVARCHAR(255) NULL;
GO

ALTER TABLE [dbo].[Badge] ADD [HiddenFromPublic] BIT NULL;
GO

PRINT N'Altering [dbo].[BookList]...';
GO

ALTER TABLE [dbo].[BookList]

ALTER COLUMN [AdminName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[BookList]

ALTER COLUMN [ListName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[BookListBooks]...';
GO

ALTER TABLE [dbo].[BookListBooks]

ALTER COLUMN [Author] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[BookListBooks]

ALTER COLUMN [Title] NVARCHAR(MAX) NULL;

ALTER TABLE [dbo].[BookListBooks]

ALTER COLUMN [URL] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Code]...';
GO

ALTER TABLE [dbo].[Code]

ALTER COLUMN [Code] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Code]

ALTER COLUMN [Description] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[CodeType]...';
GO

ALTER TABLE [dbo].[CodeType]

ALTER COLUMN [CodeTypeName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Event]...';
GO

ALTER TABLE [dbo].[Event]

ALTER COLUMN [EventTitle] NVARCHAR(255) NULL;
GO

ALTER TABLE [dbo].[Event] ADD [ExternalLinkToEvent] NVARCHAR(255) NULL,
	[HiddenFromPublic] BIT NULL;
GO

PRINT N'Altering [dbo].[LibraryCrosswalk]...';
GO

ALTER TABLE [dbo].[LibraryCrosswalk] ADD [BranchLink] NVARCHAR(255) NULL,
	[BranchAddress] NVARCHAR(255) NULL,
	[BranchTelephone] NVARCHAR(50) NULL;
GO

PRINT N'Altering [dbo].[Minigame]...';
GO

ALTER TABLE [dbo].[Minigame]

ALTER COLUMN [AdminName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Minigame]

ALTER COLUMN [MiniGameTypeName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Notifications]...';
GO

ALTER TABLE [dbo].[Notifications]

ALTER COLUMN [Subject] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Offer]...';
GO

ALTER TABLE [dbo].[Offer]

ALTER COLUMN [AdminName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Offer]

ALTER COLUMN [RedirectURL] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Offer]

ALTER COLUMN [Title] NVARCHAR(255) NULL;
GO

/*
The column [dbo].[Patron].[AvatarID] is being dropped, data loss could occur.
*/
GO

PRINT N'Starting rebuilding table [dbo].[Patron]...';
GO

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Patron] (
	[PID] INT IDENTITY(100000, 1) NOT NULL,
	[IsMasterAccount] BIT CONSTRAINT [DF_Patron_IsMasterAccount] DEFAULT((0)) NULL,
	[MasterAcctPID] INT NULL,
	[Username] VARCHAR(50) NULL,
	[Password] NVARCHAR(255) NULL,
	[DOB] DATETIME NULL,
	[Age] INT NULL,
	[SchoolGrade] VARCHAR(5) NULL,
	[ProgID] INT NULL,
	[FirstName] VARCHAR(50) NULL,
	[MiddleName] VARCHAR(50) NULL,
	[LastName] VARCHAR(50) NULL,
	[Gender] VARCHAR(1) NULL,
	[EmailAddress] VARCHAR(150) NULL,
	[PhoneNumber] VARCHAR(20) NULL,
	[StreetAddress1] VARCHAR(80) NULL,
	[StreetAddress2] VARCHAR(80) NULL,
	[City] VARCHAR(20) NULL,
	[State] VARCHAR(2) NULL,
	[ZipCode] VARCHAR(10) NULL,
	[Country] VARCHAR(50) NULL,
	[County] VARCHAR(50) NULL,
	[ParentGuardianFirstName] VARCHAR(50) NULL,
	[ParentGuardianLastName] VARCHAR(50) NULL,
	[ParentGuardianMiddleName] VARCHAR(50) NULL,
	[PrimaryLibrary] INT NULL,
	[LibraryCard] VARCHAR(20) NULL,
	[SchoolName] VARCHAR(50) NULL,
	[District] VARCHAR(50) NULL,
	[Teacher] VARCHAR(20) NULL,
	[GroupTeamName] VARCHAR(20) NULL,
	[SchoolType] INT NULL,
	[LiteracyLevel1] INT NULL,
	[LiteracyLevel2] INT NULL,
	[ParentPermFlag] BIT NULL,
	[Over18Flag] BIT NULL,
	[ShareFlag] BIT NULL,
	[TermsOfUseflag] BIT NULL,
	[Custom1] VARCHAR(50) NULL,
	[Custom2] VARCHAR(50) NULL,
	[Custom3] VARCHAR(50) NULL,
	[Custom4] VARCHAR(50) NULL,
	[Custom5] VARCHAR(50) NULL,
	[RegistrationDate] DATETIME CONSTRAINT [DF_Patron_RegistrationDate] DEFAULT(getdate()) NULL,
	[Goal] INT NULL,
	[GoalCache] INT NULL,
	[AvatarState] VARCHAR(50) NULL,
	[SDistrict] INT NULL,
	[TenID] INT NULL,
	[FldInt1] INT NULL,
	[FldInt2] INT NULL,
	[FldInt3] INT NULL,
	[FldBit1] BIT NULL,
	[FldBit2] BIT NULL,
	[FldBit3] BIT NULL,
	[FldText1] TEXT NULL,
	[FldText2] TEXT NULL,
	[FldText3] TEXT NULL,
	[Score1] INT CONSTRAINT [DF_Patron_Score1] DEFAULT((0)) NULL,
	[Score2] INT CONSTRAINT [DF_Patron_Score2] DEFAULT((0)) NULL,
	[Score1Pct] DECIMAL(18, 2) CONSTRAINT [DF_Patron_Score1Pct] DEFAULT((0)) NULL,
	[Score2Pct] DECIMAL(18, 2) CONSTRAINT [DF_Patron_Score2Pct] DEFAULT((0)) NULL,
	[Score1Date] DATETIME NULL,
	[Score2Date] DATETIME NULL,
	CONSTRAINT [tmp_ms_xx_constraint_PK_Patron1] PRIMARY KEY CLUSTERED ([PID] ASC)
	);

IF EXISTS (
		SELECT TOP 1 1
		FROM [dbo].[Patron]
		)
BEGIN
	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Patron] ON;

	INSERT INTO [dbo].[tmp_ms_xx_Patron] (
		[PID],
		[IsMasterAccount],
		[MasterAcctPID],
		[Username],
		[Password],
		[DOB],
		[Age],
		[SchoolGrade],
		[ProgID],
		[FirstName],
		[MiddleName],
		[LastName],
		[Gender],
		[EmailAddress],
		[PhoneNumber],
		[StreetAddress1],
		[StreetAddress2],
		[City],
		[State],
		[ZipCode],
		[Country],
		[County],
		[ParentGuardianFirstName],
		[ParentGuardianLastName],
		[ParentGuardianMiddleName],
		[PrimaryLibrary],
		[LibraryCard],
		[SchoolName],
		[District],
		[Teacher],
		[GroupTeamName],
		[SchoolType],
		[LiteracyLevel1],
		[LiteracyLevel2],
		[ParentPermFlag],
		[Over18Flag],
		[ShareFlag],
		[TermsOfUseflag],
		[Custom1],
		[Custom2],
		[Custom3],
		[Custom4],
		[Custom5],
		[RegistrationDate],
		[SDistrict],
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
		[Score1],
		[Score2],
		[Score1Pct],
		[Score2Pct],
		[Score1Date],
		[Score2Date]
		)
	SELECT [PID],
		[IsMasterAccount],
		[MasterAcctPID],
		[Username],
		[Password],
		[DOB],
		[Age],
		[SchoolGrade],
		[ProgID],
		[FirstName],
		[MiddleName],
		[LastName],
		[Gender],
		[EmailAddress],
		[PhoneNumber],
		[StreetAddress1],
		[StreetAddress2],
		[City],
		[State],
		[ZipCode],
		[Country],
		[County],
		[ParentGuardianFirstName],
		[ParentGuardianLastName],
		[ParentGuardianMiddleName],
		[PrimaryLibrary],
		[LibraryCard],
		[SchoolName],
		[District],
		[Teacher],
		[GroupTeamName],
		[SchoolType],
		[LiteracyLevel1],
		[LiteracyLevel2],
		[ParentPermFlag],
		[Over18Flag],
		[ShareFlag],
		[TermsOfUseflag],
		[Custom1],
		[Custom2],
		[Custom3],
		[Custom4],
		[Custom5],
		[RegistrationDate],
		[SDistrict],
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
		[Score1],
		[Score2],
		[Score1Pct],
		[Score2Pct],
		[Score1Date],
		[Score2Date]
	FROM [dbo].[Patron]
	ORDER BY [PID] ASC;

	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Patron] OFF;
END

DROP TABLE [dbo].[Patron];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Patron]',
	N'Patron';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Patron1]',
	N'PK_Patron',
	N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
GO

PRINT N'Altering [dbo].[PatronReadingLog]...';
GO

ALTER TABLE [dbo].[PatronReadingLog]

ALTER COLUMN [Author] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[PatronReadingLog]

ALTER COLUMN [Title] NVARCHAR(255) NULL;
GO

ALTER TABLE [dbo].[PatronReadingLog] ADD [LoggedAt] DATETIME NULL;
GO

PRINT N'Altering [dbo].[PatronReview]...';
GO

ALTER TABLE [dbo].[PatronReview]

ALTER COLUMN [Author] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[PatronReview]

ALTER COLUMN [Title] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[ProgramGame]...';
GO

ALTER TABLE [dbo].[ProgramGame]

ALTER COLUMN [GameName] NVARCHAR(255) NULL;
GO

PRINT N'Starting rebuilding table [dbo].[Programs]...';
GO

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Programs] (
	[PID] INT IDENTITY(1, 1) NOT NULL,
	[AdminName] NVARCHAR(255) NULL,
	[Title] NVARCHAR(255) NULL,
	[TabName] VARCHAR(20) NULL,
	[POrder] INT NULL,
	[IsActive] BIT CONSTRAINT [DF_Programs_IsActive] DEFAULT((0)) NULL,
	[IsHidden] BIT CONSTRAINT [DF_Programs_IsHidden] DEFAULT((0)) NULL,
	[StartDate] DATETIME NULL,
	[EndDate] DATETIME NULL,
	[MaxAge] INT NULL,
	[MaxGrade] INT NULL,
	[LoggingStart] DATETIME NULL,
	[LoggingEnd] DATETIME NULL,
	[ParentalConsentFlag] BIT CONSTRAINT [DF_Programs_ParentalConsentFlag] DEFAULT((0)) NULL,
	[ParentalConsentText] TEXT NULL,
	[PatronReviewFlag] BIT NULL,
	[RequireBookDetails] BIT NULL,
	[LogoutURL] NVARCHAR(255) NULL,
	[ProgramGameID] INT NULL,
	[HTML1] TEXT NULL,
	[HTML2] TEXT NULL,
	[HTML3] TEXT NULL,
	[HTML4] TEXT NULL,
	[HTML5] TEXT NULL,
	[HTML6] TEXT NULL,
	[BannerImage] NVARCHAR(255) NULL,
	[RegistrationBadgeID] INT NULL,
	[CompletionPoints] INT CONSTRAINT [DF_Programs_CompletionPoints] DEFAULT((0)) NULL,
	[LastModUser] VARCHAR(50) CONSTRAINT [DF_Programs_LastModUser] DEFAULT('N/A') NULL,
	[AddedDate] DATETIME CONSTRAINT [DF_Programs_AddedDate] DEFAULT(getdate()) NULL,
	[AddedUser] VARCHAR(50) CONSTRAINT [DF_Programs_AddedUser] DEFAULT('N/A') NULL,
	[LastModDate] DATETIME CONSTRAINT [DF_Programs_LastModDate] DEFAULT(getdate()) NULL,
	[TenID] INT NULL,
	[FldInt1] INT NULL,
	[FldInt2] INT NULL,
	[FldInt3] INT NULL,
	[FldBit1] BIT NULL,
	[FldBit2] BIT NULL,
	[FldBit3] BIT NULL,
	[FldText1] TEXT NULL,
	[FldText2] TEXT NULL,
	[FldText3] TEXT NULL,
	[PreTestID] INT NULL,
	[PostTestID] INT NULL,
	[PreTestMandatory] BIT NULL,
	[PretestEndDate] DATETIME NULL,
	[PostTestStartDate] DATETIME NULL,
	[GoalDefault] INT NULL,
	[GoalMin] INT NULL,
	[GoalMax] INT NULL,
	[GoalIntervalId] INT NULL,
	[HideSchoolInRegistration] BIT NULL,
	CONSTRAINT [tmp_ms_xx_constraint_PK_Programs1] PRIMARY KEY CLUSTERED ([PID] ASC)
	);

IF EXISTS (
		SELECT TOP 1 1
		FROM [dbo].[Programs]
		)
BEGIN
	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Programs] ON;

	INSERT INTO [dbo].[tmp_ms_xx_Programs] (
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
	SELECT [PID],
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
	FROM [dbo].[Programs]
	ORDER BY [PID] ASC;

	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Programs] OFF;
END

DROP TABLE [dbo].[Programs];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Programs]',
	N'Programs';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Programs1]',
	N'PK_Programs',
	N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
GO

PRINT N'Starting rebuilding table [dbo].[RegistrationSettings]...';
GO

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_RegistrationSettings] (
	[RID] INT IDENTITY(1, 1) NOT NULL,
	[Literacy1Label] VARCHAR(50) CONSTRAINT [DF_RegistrationSettings_Literacy1Label] DEFAULT('AR Level') NULL,
	[Literacy2Label] VARCHAR(50) CONSTRAINT [DF_RegistrationSettings_Literacy2Label] DEFAULT('Lexile Level') NULL,
	[DOB_Prompt] BIT NULL,
	[Age_Prompt] BIT NULL,
	[SchoolGrade_Prompt] BIT NULL,
	[FirstName_Prompt] BIT NULL,
	[MiddleName_Prompt] BIT NULL,
	[LastName_Prompt] BIT NULL,
	[Gender_Prompt] BIT NULL,
	[EmailAddress_Prompt] BIT NULL,
	[PhoneNumber_Prompt] BIT NULL,
	[StreetAddress1_Prompt] BIT NULL,
	[StreetAddress2_Prompt] BIT NULL,
	[City_Prompt] BIT NULL,
	[State_Prompt] BIT NULL,
	[ZipCode_Prompt] BIT NULL,
	[Country_Prompt] BIT NULL,
	[County_Prompt] BIT NULL,
	[ParentGuardianFirstName_Prompt] BIT NULL,
	[ParentGuardianLastName_Prompt] BIT NULL,
	[ParentGuardianMiddleName_Prompt] BIT NULL,
	[PrimaryLibrary_Prompt] BIT NULL,
	[LibraryCard_Prompt] BIT NULL,
	[SchoolName_Prompt] BIT NULL,
	[District_Prompt] BIT NULL,
	[Teacher_Prompt] BIT NULL,
	[GroupTeamName_Prompt] BIT NULL,
	[SchoolType_Prompt] BIT NULL,
	[LiteracyLevel1_Prompt] BIT NULL,
	[LiteracyLevel2_Prompt] BIT NULL,
	[ParentPermFlag_Prompt] BIT NULL,
	[Over18Flag_Prompt] BIT NULL,
	[ShareFlag_Prompt] BIT NULL,
	[TermsOfUseflag_Prompt] BIT NULL,
	[Custom1_Prompt] BIT NULL,
	[Custom2_Prompt] BIT NULL,
	[Custom3_Prompt] BIT NULL,
	[Custom4_Prompt] BIT NULL,
	[Custom5_Prompt] BIT NULL,
	[DOB_Req] BIT NULL,
	[Age_Req] BIT NULL,
	[SchoolGrade_Req] BIT NULL,
	[FirstName_Req] BIT NULL,
	[MiddleName_Req] BIT NULL,
	[LastName_Req] BIT NULL,
	[Gender_Req] BIT NULL,
	[EmailAddress_Req] BIT NULL,
	[PhoneNumber_Req] BIT NULL,
	[StreetAddress1_Req] BIT NULL,
	[StreetAddress2_Req] BIT NULL,
	[City_Req] BIT NULL,
	[State_Req] BIT NULL,
	[ZipCode_Req] BIT NULL,
	[Country_Req] BIT NULL,
	[County_Req] BIT NULL,
	[ParentGuardianFirstName_Req] BIT NULL,
	[ParentGuardianLastName_Req] BIT NULL,
	[ParentGuardianMiddleName_Req] BIT NULL,
	[PrimaryLibrary_Req] BIT NULL,
	[LibraryCard_Req] BIT NULL,
	[SchoolName_Req] BIT NULL,
	[District_Req] BIT NULL,
	[Teacher_Req] BIT NULL,
	[GroupTeamName_Req] BIT NULL,
	[SchoolType_Req] BIT NULL,
	[LiteracyLevel1_Req] BIT NULL,
	[LiteracyLevel2_Req] BIT NULL,
	[ParentPermFlag_Req] BIT NULL,
	[Over18Flag_Req] BIT NULL,
	[ShareFlag_Req] BIT NULL,
	[TermsOfUseflag_Req] BIT NULL,
	[Custom1_Req] BIT NULL,
	[Custom2_Req] BIT NULL,
	[Custom3_Req] BIT NULL,
	[Custom4_Req] BIT NULL,
	[Custom5_Req] BIT NULL,
	[DOB_Show] BIT NULL,
	[Age_Show] BIT NULL,
	[SchoolGrade_Show] BIT NULL,
	[FirstName_Show] BIT NULL,
	[MiddleName_Show] BIT NULL,
	[LastName_Show] BIT NULL,
	[Gender_Show] BIT NULL,
	[EmailAddress_Show] BIT NULL,
	[PhoneNumber_Show] BIT NULL,
	[StreetAddress1_Show] BIT NULL,
	[StreetAddress2_Show] BIT NULL,
	[City_Show] BIT NULL,
	[State_Show] BIT NULL,
	[ZipCode_Show] BIT NULL,
	[Country_Show] BIT NULL,
	[County_Show] BIT NULL,
	[ParentGuardianFirstName_Show] BIT NULL,
	[ParentGuardianLastName_Show] BIT NULL,
	[ParentGuardianMiddleName_Show] BIT NULL,
	[PrimaryLibrary_Show] BIT NULL,
	[LibraryCard_Show] BIT NULL,
	[SchoolName_Show] BIT NULL,
	[District_Show] BIT NULL,
	[Teacher_Show] BIT NULL,
	[GroupTeamName_Show] BIT NULL,
	[SchoolType_Show] BIT NULL,
	[LiteracyLevel1_Show] BIT NULL,
	[LiteracyLevel2_Show] BIT NULL,
	[ParentPermFlag_Show] BIT NULL,
	[Over18Flag_Show] BIT NULL,
	[ShareFlag_Show] BIT NULL,
	[TermsOfUseflag_Show] BIT NULL,
	[Custom1_Show] BIT NULL,
	[Custom2_Show] BIT NULL,
	[Custom3_Show] BIT NULL,
	[Custom4_Show] BIT NULL,
	[Custom5_Show] BIT NULL,
	[DOB_Edit] BIT NULL,
	[Age_Edit] BIT NULL,
	[SchoolGrade_Edit] BIT NULL,
	[FirstName_Edit] BIT NULL,
	[MiddleName_Edit] BIT NULL,
	[LastName_Edit] BIT NULL,
	[Gender_Edit] BIT NULL,
	[EmailAddress_Edit] BIT NULL,
	[PhoneNumber_Edit] BIT NULL,
	[StreetAddress1_Edit] BIT NULL,
	[StreetAddress2_Edit] BIT NULL,
	[City_Edit] BIT NULL,
	[State_Edit] BIT NULL,
	[ZipCode_Edit] BIT NULL,
	[Country_Edit] BIT NULL,
	[County_Edit] BIT NULL,
	[ParentGuardianFirstName_Edit] BIT NULL,
	[ParentGuardianLastName_Edit] BIT NULL,
	[ParentGuardianMiddleName_Edit] BIT NULL,
	[PrimaryLibrary_Edit] BIT NULL,
	[LibraryCard_Edit] BIT NULL,
	[SchoolName_Edit] BIT NULL,
	[District_Edit] BIT NULL,
	[Teacher_Edit] BIT NULL,
	[GroupTeamName_Edit] BIT NULL,
	[SchoolType_Edit] BIT NULL,
	[LiteracyLevel1_Edit] BIT NULL,
	[LiteracyLevel2_Edit] BIT NULL,
	[ParentPermFlag_Edit] BIT NULL,
	[Over18Flag_Edit] BIT NULL,
	[ShareFlag_Edit] BIT NULL,
	[TermsOfUseflag_Edit] BIT NULL,
	[Custom1_Edit] BIT NULL,
	[Custom2_Edit] BIT NULL,
	[Custom3_Edit] BIT NULL,
	[Custom4_Edit] BIT NULL,
	[Custom5_Edit] BIT NULL,
	[LastModDate] DATETIME CONSTRAINT [DF_RegistrationSettings_LastModDate] DEFAULT(getdate()) NULL,
	[LastModUser] VARCHAR(50) CONSTRAINT [DF_RegistrationSettings_LastModUser] DEFAULT('N/A') NULL,
	[AddedDate] DATETIME CONSTRAINT [DF_RegistrationSettings_AddedDate] DEFAULT(getdate()) NULL,
	[AddedUser] VARCHAR(50) CONSTRAINT [DF_RegistrationSettings_AddedUser] DEFAULT('N/A') NULL,
	[SDistrict_Prompt] BIT NULL,
	[SDistrict_Req] BIT NULL,
	[SDistrict_Show] BIT NULL,
	[SDistrict_Edit] BIT NULL,
	[Goal_Prompt] BIT NULL,
	[Goal_Req] BIT NULL,
	[Goal_Show] BIT NULL,
	[Goal_Edit] BIT NULL,
	[TenID] INT NULL,
	[FldInt1] INT NULL,
	[FldInt2] INT NULL,
	[FldInt3] INT NULL,
	[FldBit1] BIT NULL,
	[FldBit2] BIT NULL,
	[FldBit3] BIT NULL,
	[FldText1] TEXT NULL,
	[FldText2] TEXT NULL,
	[FldText3] TEXT NULL,
	CONSTRAINT [tmp_ms_xx_constraint_PK_RegistrationSettings1] PRIMARY KEY CLUSTERED ([RID] ASC)
	);

IF EXISTS (
		SELECT TOP 1 1
		FROM [dbo].[RegistrationSettings]
		)
BEGIN
	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_RegistrationSettings] ON;

	INSERT INTO [dbo].[tmp_ms_xx_RegistrationSettings] (
		[RID],
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
	SELECT [RID],
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
	FROM [dbo].[RegistrationSettings]
	ORDER BY [RID] ASC;

	SET IDENTITY_INSERT [dbo].[tmp_ms_xx_RegistrationSettings] OFF;
END

DROP TABLE [dbo].[RegistrationSettings];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_RegistrationSettings]',
	N'RegistrationSettings';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_RegistrationSettings1]',
	N'PK_RegistrationSettings',
	N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
GO

PRINT N'Altering [dbo].[ReportTemplate]...';
GO

ALTER TABLE [dbo].[ReportTemplate]

ALTER COLUMN [ReportName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[SentEmailLog]...';
GO

ALTER TABLE [dbo].[SentEmailLog]

ALTER COLUMN [SentFrom] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[SentEmailLog]

ALTER COLUMN [SentTo] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[SentEmailLog]

ALTER COLUMN [Subject] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[SRPGroups]...';
GO

ALTER TABLE [dbo].[SRPGroups]

ALTER COLUMN [GroupName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[SRPPermissionsMaster]...';
GO

ALTER TABLE [dbo].[SRPPermissionsMaster]

ALTER COLUMN [PermissionDesc] NVARCHAR(MAX) NULL;

ALTER TABLE [dbo].[SRPPermissionsMaster]

ALTER COLUMN [PermissionName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[SRPReport]...';
GO

ALTER TABLE [dbo].[SRPReport]

ALTER COLUMN [ReportName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[SRPSettings]...';
GO

ALTER TABLE [dbo].[SRPSettings]

ALTER COLUMN [Name] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[SRPSettings]

ALTER COLUMN [ValueList] NVARCHAR(MAX) NULL;
GO

PRINT N'Altering [dbo].[SRPUser]...';
GO

ALTER TABLE [dbo].[SRPUser]

ALTER COLUMN [EmailAddress] NVARCHAR(255) NOT NULL;

ALTER TABLE [dbo].[SRPUser]

ALTER COLUMN [FirstName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[SRPUser]

ALTER COLUMN [LastName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Survey]...';
GO

ALTER TABLE [dbo].[Survey]

ALTER COLUMN [LongName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Survey]

ALTER COLUMN [Name] NVARCHAR(255) NULL;
GO

ALTER TABLE [dbo].[Survey] ADD [BadgeId] INT NULL;
GO

PRINT N'Altering [dbo].[SurveyAnswers]...';
GO

ALTER TABLE [dbo].[SurveyAnswers]

ALTER COLUMN [ChoiceAnswerIDs] NVARCHAR(MAX) NULL;
GO

PRINT N'Altering [dbo].[SurveyQuestion]...';
GO

ALTER TABLE [dbo].[SurveyQuestion]

ALTER COLUMN [QName] NVARCHAR(255) NULL;
GO

PRINT N'Altering [dbo].[Tenant]...';
GO

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [AdminName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [BadgesMenuText] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [DomainName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [EventsMenuText] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [LandingName] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [Name] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [NotificationsMenuText] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Tenant]

ALTER COLUMN [OffersMenuText] NVARCHAR(255) NULL;
GO

PRINT N'Creating [dbo].[AvatarPart]...';
GO

CREATE TABLE [dbo].[AvatarPart] (
	[APID] INT IDENTITY(1, 1) NOT NULL,
	[Name] VARCHAR(50) NULL,
	[Gender] VARCHAR(50) NULL,
	[ComponentID] INT NULL,
	[BadgeID] INT NULL,
	[Ordering] INT NULL,
	[LastModDate] DATETIME NULL,
	[LastModUser] VARCHAR(50) NULL,
	[AddedDate] DATETIME NULL,
	[AddedUser] VARCHAR(50) NULL,
	[TenID] INT NULL,
	CONSTRAINT [PK_AvatarPart] PRIMARY KEY CLUSTERED ([APID] ASC)
	);
GO

PRINT N'Creating [dbo].[SRPHistory]...';
GO

CREATE TABLE [dbo].[SRPHistory] (
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[When] DATETIME NOT NULL,
	[Who] NVARCHAR(255) NOT NULL,
	[Event] NVARCHAR(255) NOT NULL,
	[VersionMajor] INT NOT NULL,
	[VersionMinor] INT NOT NULL,
	[VersionPatch] INT NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC)
	);
GO

PRINT N'Creating [dbo].[DF_Notifications_Subject]...';
GO

ALTER TABLE [dbo].[Notifications] ADD CONSTRAINT [DF_Notifications_Subject] DEFAULT('')
FOR [Subject];
GO

PRINT N'Creating [dbo].[DF_Offer_RedirectURL]...';
GO

ALTER TABLE [dbo].[Offer] ADD CONSTRAINT [DF_Offer_RedirectURL] DEFAULT('')
FOR [RedirectURL];
GO

PRINT N'Creating [dbo].[DF_Tenant_BadgesMenuText]...';
GO

ALTER TABLE [dbo].[Tenant] ADD CONSTRAINT [DF_Tenant_BadgesMenuText] DEFAULT('Badges')
FOR [BadgesMenuText];
GO

PRINT N'Creating [dbo].[DF_Tenant_EventsMenuText]...';
GO

ALTER TABLE [dbo].[Tenant] ADD CONSTRAINT [DF_Tenant_EventsMenuText] DEFAULT('Events')
FOR [EventsMenuText];
GO

PRINT N'Creating [dbo].[DF_Tenant_NotificationsMenuText]...';
GO

ALTER TABLE [dbo].[Tenant] ADD CONSTRAINT [DF_Tenant_NotificationsMenuText] DEFAULT('Mail')
FOR [NotificationsMenuText];
GO

PRINT N'Creating [dbo].[DF_Tenant_OffersMenuText]...';
GO

ALTER TABLE [dbo].[Tenant] ADD CONSTRAINT [DF_Tenant_OffersMenuText] DEFAULT('Offers')
FOR [OffersMenuText];
GO

PRINT N'Creating [dbo].[DF_AvatarPart_LastModDate]...';
GO

ALTER TABLE [dbo].[AvatarPart] ADD CONSTRAINT [DF_AvatarPart_LastModDate] DEFAULT(getdate())
FOR [LastModDate];
GO

PRINT N'Creating [dbo].[DF_AvatarPArt_LastModUser]...';
GO

ALTER TABLE [dbo].[AvatarPart] ADD CONSTRAINT [DF_AvatarPArt_LastModUser] DEFAULT('N/A')
FOR [LastModUser];
GO

PRINT N'Creating [dbo].[DF_AvatarPart_AddedDate]...';
GO

ALTER TABLE [dbo].[AvatarPart] ADD CONSTRAINT [DF_AvatarPart_AddedDate] DEFAULT(getdate())
FOR [AddedDate];
GO

PRINT N'Creating [dbo].[DF_AvatarPart_AddedUser]...';
GO

ALTER TABLE [dbo].[AvatarPart] ADD CONSTRAINT [DF_AvatarPart_AddedUser] DEFAULT('N/A')
FOR [AddedUser];
GO

PRINT N'Creating [dbo].[FK_ProgramCodes_Programs]...';
GO

ALTER TABLE [dbo].[ProgramCodes]
	WITH NOCHECK ADD CONSTRAINT [FK_ProgramCodes_Programs] FOREIGN KEY ([PID]) REFERENCES [dbo].[Programs]([PID]);
GO

PRINT N'Creating [dbo].[FK_ProgramGamePointConversion_Programs]...';
GO

ALTER TABLE [dbo].[ProgramGamePointConversion]
	WITH NOCHECK ADD CONSTRAINT [FK_ProgramGamePointConversion_Programs] FOREIGN KEY ([PGID]) REFERENCES [dbo].[Programs]([PID]);
GO

PRINT N'Altering [dbo].[fx_IsFinisher]...';
GO

ALTER FUNCTION [dbo].[fx_IsFinisher] (
	@PID INT,
	@ProgID INT
	)
RETURNS BIT
AS
BEGIN
	DECLARE @ret BIT
	DECLARE @GameCompletionPoints INT
	DECLARE @UserPoints INT

	IF (
			@PID IS NULL
			OR @ProgID IS NULL
			OR @ProgID = 0
			)
	BEGIN
		SET @ret = 0
	END
	ELSE
	BEGIN
		SELECT @GameCompletionPoints = IsNull(CompletionPoints, 0)
		FROM Programs
		WHERE PID = @ProgID

		/*
		if (select ProgramGameID from Programs where PID = @ProgID) = 0
		begin
			select @GameCompletionPoints = IsNull(CompletionPoints,0) from Programs where PID = @ProgID
		end
		else
		begin

			select @GameCompletionPoints = isnull(SUM(isnull(pgl.PointNumber,0)),0)
			from ProgramGame pg
					left join ProgramGameLevel pgl
						on pg.PGID = pgl.PGID
					left join Programs p
						on p.ProgramGameID = pg.PGID
			where
				p.PID = @ProgID
		end
		*/
		SELECT @UserPoints = isnull(SUM(isnull(NumPoints, 0)), 0)
		FROM PatronPoints
		WHERE PID = @PID

		SELECT @ret = CASE 
				WHEN @UserPoints < @GameCompletionPoints
					OR @GameCompletionPoints = 0
					THEN 0
				ELSE 1
				END
	END

	RETURN @ret
END
GO

PRINT N'Creating [dbo].[fx_PatronBadgeCount]...';
GO

/* =============================================
-- Return a count of how many badges Patron ID 
-- @PID has out of the comma-separated text 
-- list in @BadgeList
-- ============================================= */
CREATE FUNCTION [dbo].[fx_PatronBadgeCount] (
	@PID INT,
	@BadgeList NVARCHAR(4000)
	)
RETURNS INT
AS
BEGIN
	DECLARE @return INT

	SELECT @return = COUNT(DISTINCT BadgeID)
	FROM PatronBadges
	WHERE PID = @PID
		AND BadgeID IN (
			SELECT *
			FROM fnSplitBigInt(@BadgeList)
			)

	RETURN @return
END
GO

PRINT N'Refreshing [dbo].[ProgramGameCummulativePoints]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[ProgramGameCummulativePoints]';
GO

PRINT N'Refreshing [dbo].[rpt_GamePlayStats1]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_GamePlayStats1]';
GO

PRINT N'Altering [dbo].[app_Award_GetPatronQualifyingAwards]...';
GO

ALTER PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwards] @PID INT = 0
AS
SELECT award.*,
	patron.PID,
	patron.ProgID,
	patron.PrimaryLibrary,
	patron.District,
	patron.SchoolName,
	patron.TotalGoal,
	patron.Points
FROM Award award
INNER JOIN (
	SELECT pt.PID,
		pt.progID,
		pt.PrimaryLibrary,
		pt.District,
		pt.SchoolName,
		isnull(pt.GoalCache, - 1) AS TotalGoal,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		pt.TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS patron ON patron.TenID = award.TenID
	AND (
		award.ProgramID = patron.ProgID
		OR award.ProgramID = 0
		)
	AND (
		award.BranchID = patron.PrimaryLibrary
		OR award.BranchID = 0
		)
	AND (
		award.District = patron.District
		OR award.District = ''
		)
	AND (
		award.SchoolName = patron.SchoolName
		OR award.SchoolName = ''
		)
	AND (award.NumPoints <= patron.Points)
	AND (
		TotalGoal < 1
		OR award.GoalPercent <= (patron.points * 100) / TotalGoal
		)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronBadgeCount(patron.PID, BadgeList) >= award.BadgesAchieved
		)
GO

PRINT N'Altering [dbo].[app_Award_GetPatronQualifyingAwardsWTenant]...';
GO

ALTER PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwardsWTenant] @PID INT = 0,
	@TenID INT = 1
AS
SELECT award.*,
	patron.PID,
	patron.ProgID,
	patron.PrimaryLibrary,
	patron.District,
	patron.SchoolName,
	patron.TotalGoal,
	patron.Points
FROM Award award
INNER JOIN (
	SELECT pt.PID,
		pt.progID,
		pt.PrimaryLibrary,
		pt.District,
		pt.SchoolName,
		isnull(pt.GoalCache, - 1) AS TotalGoal,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		@TenID AS TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS patron ON patron.TenID = award.TenID
	AND (
		award.ProgramID = patron.ProgID
		OR award.ProgramID = 0
		)
	AND (
		award.BranchID = patron.PrimaryLibrary
		OR award.BranchID = 0
		)
	AND (
		award.District = patron.District
		OR award.District = ''
		)
	AND (
		award.SchoolName = patron.SchoolName
		OR award.SchoolName = ''
		)
	AND (award.NumPoints <= patron.Points)
	AND (
		TotalGoal < 1
		OR award.GoalPercent <= (patron.points * 100) / TotalGoal
		)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronBadgeCount(patron.PID, BadgeList) >= award.BadgesAchieved
		)
GO

PRINT N'Altering [dbo].[app_Award_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_Award_Insert] (
	@AwardName VARCHAR(80),
	@BadgeID INT,
	@NumPoints INT,
	@BranchID INT,
	@ProgramID INT,
	@District VARCHAR(50),
	@SchoolName VARCHAR(50),
	@BadgeList VARCHAR(500),
	@BadgesAchieved INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@GoalPercent INT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@AID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Award (
		AwardName,
		BadgeID,
		NumPoints,
		BranchID,
		ProgramID,
		District,
		SchoolName,
		BadgesAchieved,
		BadgeList,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		GoalPercent,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@AwardName,
		@BadgeID,
		@NumPoints,
		@BranchID,
		@ProgramID,
		@District,
		@SchoolName,
		@BadgesAchieved,
		@BadgeList,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@GoalPercent,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @AID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Award_Update]...';
GO

ALTER PROCEDURE [dbo].[app_Award_Update] (
	@AID INT,
	@AwardName VARCHAR(80),
	@BadgeID INT,
	@NumPoints INT,
	@BranchID INT,
	@ProgramID INT,
	@District VARCHAR(50),
	@SchoolName VARCHAR(50),
	@BadgesAchieved INT,
	@BadgeList VARCHAR(500),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@GoalPercent INT = 0,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = ''
	)
AS
UPDATE Award
SET AwardName = @AwardName,
	BadgeID = @BadgeID,
	NumPoints = @NumPoints,
	BranchID = @BranchID,
	ProgramID = @ProgramID,
	District = @District,
	SchoolName = @SchoolName,
	BadgesAchieved = @BadgesAchieved,
	BadgeList = @BadgeList,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	GoalPercent = @GoalPercent,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE AID = @AID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Badge_GetBadgeGallery]...';
GO

------------------------------------------------------------------
ALTER PROCEDURE [dbo].[app_Badge_GetBadgeGallery] @TenID INT,
	@A INT = 0,
	@B INT = 0,
	@C INT = 0,
	@L INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT BID,
	UserName AS NAME
FROM Badge b
WHERE TenID = @TenID
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeAgeGrp A
			WHERE b.BID = A.BID
				AND A.CID = @A
			)
		OR @A = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeBranch B
			WHERE b.BID = B.BID
				AND B.CID = @B
			)
		OR @B = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeCategory C
			WHERE b.BID = C.BID
				AND C.CID = @C
			)
		OR @C = 0
		)
	AND (
		b.BID IN (
			SELECT BID
			FROM BadgeLocation L
			WHERE b.BID = L.BID
				AND L.CID = @l
			)
		OR @L = 0
		)
	AND HiddenFromPublic != 1
ORDER BY b.UserName
GO

PRINT N'Altering [dbo].[app_Badge_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_Badge_Insert] (
	@AdminName VARCHAR(50),
	@UserName VARCHAR(50),
	@GenNotificationFlag BIT,
	@NotificationSubject VARCHAR(150),
	@NotificationBody TEXT,
	@CustomEarnedMessage TEXT,
	@IncludesPhysicalPrizeFlag BIT,
	@PhysicalPrizeName VARCHAR(50),
	@AssignProgramPrizeCode BIT,
	@PCNotificationSubject VARCHAR(150),
	@PCNotificationBody TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@HiddenFromPublic BIT,
	@BID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Badge (
		AdminName,
		UserName,
		GenNotificationFlag,
		NotificationSubject,
		NotificationBody,
		CustomEarnedMessage,
		IncludesPhysicalPrizeFlag,
		PhysicalPrizeName,
		AssignProgramPrizeCode,
		PCNotificationSubject,
		PCNotificationBody,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		HiddenFromPublic
		)
	VALUES (
		@AdminName,
		@UserName,
		@GenNotificationFlag,
		@NotificationSubject,
		@NotificationBody,
		@CustomEarnedMessage,
		@IncludesPhysicalPrizeFlag,
		@PhysicalPrizeName,
		@AssignProgramPrizeCode,
		@PCNotificationSubject,
		@PCNotificationBody,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@HiddenFromPublic
		)

	SELECT @BID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Badge_Update]...';
GO

ALTER PROCEDURE [dbo].[app_Badge_Update] (
	@BID INT,
	@AdminName VARCHAR(50),
	@UserName VARCHAR(50),
	@GenNotificationFlag BIT,
	@NotificationSubject VARCHAR(150),
	@NotificationBody TEXT,
	@CustomEarnedMessage TEXT,
	@IncludesPhysicalPrizeFlag BIT,
	@PhysicalPrizeName VARCHAR(50),
	@AssignProgramPrizeCode BIT,
	@PCNotificationSubject VARCHAR(150),
	@PCNotificationBody TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@HiddenFromPublic BIT
	)
AS
UPDATE Badge
SET AdminName = @AdminName,
	UserName = @UserName,
	GenNotificationFlag = @GenNotificationFlag,
	NotificationSubject = @NotificationSubject,
	NotificationBody = @NotificationBody,
	CustomEarnedMessage = @CustomEarnedMessage,
	IncludesPhysicalPrizeFlag = @IncludesPhysicalPrizeFlag,
	PhysicalPrizeName = @PhysicalPrizeName,
	AssignProgramPrizeCode = @AssignProgramPrizeCode,
	PCNotificationSubject = @PCNotificationSubject,
	PCNotificationBody = @PCNotificationBody,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	HiddenFromPublic = @HiddenFromPublic
WHERE BID = @BID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Badge_GetBadgeBookLists]...';
GO

------------------------------------------------------------------
ALTER PROCEDURE [dbo].[app_Badge_GetBadgeBookLists] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = LTRIM(RTRIM(STUFF((
					SELECT ', ' + p.ListName
					FROM BookList p
					WHERE p.TenID = @TenID
						AND p.AwardBadgeID = @BID
					GROUP BY p.ListName
					ORDER BY p.ListName
					FOR XML PATH('')
					), 1, 1, '')))
GO

PRINT N'Altering [dbo].[app_BookList_GetForDisplay]...';
GO

ALTER PROCEDURE [dbo].[app_BookList_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL,
	@SearchText NVARCHAR(255) = NULL
AS
-- variables for user information
DECLARE @ProgramId INT,
	@PatronTenant INT

-- load user information, Program and Tenant
SELECT @ProgramId = isnull(ProgID, 0),
	@PatronTenant = isnull([TenID], 0)
FROM Patron
WHERE PID = @PID

-- if no tenant was supplied, use the patron's tenant
IF (@TenID IS NULL)
BEGIN
	SET @TenID = @PatronTenant
END

-- temporary storage for unique BLIDs
CREATE TABLE #temp (BLID INT)

-- get BLIDs that are not associated with a program, associated with the user's program, and match the search text (if provided)
INSERT INTO #temp
SELECT DISTINCT bl.[BLID]
FROM BookList bl
LEFT OUTER JOIN [BookListBooks] blb ON blb.[BLID] = bl.[BLID]
WHERE (
		bl.[ProgID] = 0
		OR bl.[ProgID] = @ProgramId
		)
	AND (
		@SearchText IS NULL
		OR (
			bl.[ListName] LIKE @SearchText
			OR bl.[Description] LIKE @SearchText
			OR blb.[Title] LIKE @SearchText
			OR blb.[Author] LIKE @SearchText
			)
		)
	AND bl.[TenID] = @TenID

-- final organization of booklist
SELECT bl.*,
	(
		SELECT count(*)
		FROM [PatronBookLists] pbl
		WHERE pbl.[blid] = bl.[blid]
			AND pbl.[pid] = @pid
			AND pbl.[HasReadFlag] = 1
		) AS NumBooksCompleted
FROM #temp t
LEFT JOIN dbo.BookList bl ON bl.BLID = t.BLID
WHERE t.BLID IN (
		SELECT DISTINCT [BLID]
		FROM [BookListBooks]
		)
ORDER BY bl.[ListName]

DROP TABLE #temp
GO

PRINT N'Altering [dbo].[app_BookListBooks_GetForDisplay]...';
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForDisplay]    Script Date: 01/05/2015 14:43:20 ******/
ALTER PROCEDURE [dbo].[app_BookListBooks_GetForDisplay] @PID INT = 0
AS
--declare @PID int dbo.BookList
--select @PID = 100000
DECLARE @Lit1 INT
DECLARE @Lit2 INT,
	@ProgramId INT,
	@BranchId INT
DECLARE @TenID INT

SELECT @Lit1 = isnull(LiteracyLevel1, 0),
	@Lit2 = isnull(LiteracyLevel2, ''),
	@ProgramId = isnull(ProgID, 0),
	@BranchId = 0,
	@TenID = TenID
FROM Patron
WHERE PID = @PID

----------------------------------------------------------
--select @Age, @Zip, @Age-36, @ProgramId, @BranchId
--select  o.*
--from Offer o
----------------------------------------------------------
CREATE TABLE #temp (
	BLID INT,
	ListName VARCHAR(50),
	Description TEXT
	)

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LiteracyLevel1 > 0
	AND @Lit1 = LiteracyLevel1
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LiteracyLevel2 > 0
	AND @Lit2 = LiteracyLevel2
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE ProgID > 0
	AND ProgID = @ProgramId
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LibraryID > 0
	AND LibraryID = @BranchId
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE ProgID = 0
	AND LiteracyLevel1 = 0
	AND LiteracyLevel2 = 0
	AND TenID = @TenID

SELECT DISTINCT BLID
INTO #temp1
FROM #temp

DROP TABLE #temp

SELECT ROW_NUMBER() OVER (
		ORDER BY bl.BLID
		) AS Rank,
	bl.*
FROM #temp1 t
LEFT JOIN dbo.BookList bl ON bl.BLID = t.BLID
GO

PRINT N'Altering [dbo].[app_SurveyResults_GetExport]...';
GO

ALTER PROCEDURE [dbo].[app_SurveyResults_GetExport] @SID INT = NULL,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL,
	@SchoolID INT = NULL
AS
-- declare @SID int
-- declare @SourceType varchar(250)
-- declare @SourceID int
-- select @SID = 1,@SourceType= null,@SourceID = null
CREATE TABLE #Results (
	SRID INT,
	Username VARCHAR(50) NULL,
	FirstName VARCHAR(50) NULL,
	LastName VARCHAR(50) NULL,
	SchoolName VARCHAR(50) NULL,
	Source VARCHAR(250) NULL,
	SourceName VARCHAR(250) NULL
	)

INSERT INTO #Results
SELECT r.SRID,
	p.Username,
	p.FirstName,
	p.LastName,
	isNull(c.Code, ''),
	r.Source,
	CASE [Source]
		WHEN 'Program Pre-Test'
			THEN isnull((
						SELECT AdminName
						FROM Programs
						WHERE PID = SourceID
						), 'N/A')
		WHEN 'Program Post-Test'
			THEN isnull((
						SELECT AdminName
						FROM Programs
						WHERE PID = SourceID
						), 'N/A')
		WHEN 'Game'
			THEN isnull((
						SELECT AdminName
						FROM Minigame
						WHERE MGID = SourceID
						), 'N/A')
		WHEN 'Reading List'
			THEN isnull((
						SELECT ListName
						FROM BookList
						WHERE BLID = SourceID
						), 'N/A')
		WHEN 'Event'
			THEN isnull((
						SELECT EventTitle
						FROM Event
						WHERE EID = SourceID
						), 'N/A')
		ELSE 'NA'
		END [SourceName]
FROM SurveyResults r
INNER JOIN Patron p ON r.PID = p.PID
LEFT JOIN Code c ON p.SchoolName = c.CID
WHERE r.SID = @SID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
	AND (
		p.SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)

SELECT DISTINCT a.QID,
	a.SQMLID,
	a.QType,
	q.QNumber
INTO #T1
FROM dbo.SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
INNER JOIN SurveyQuestion q ON a.QID = q.QID
WHERE r.SID = @SID
ORDER BY q.QNumber

DECLARE @NumColumnSets INT
DECLARE @RunningCounter INT

SELECT @NumColumnSets = COUNT(*)
FROM #T1

SELECT @RunningCounter = 1

DECLARE @SQL1 VARCHAR(8000)

SELECT @SQL1 = 'alter table #Results Add '

WHILE @RunningCounter <= @NumColumnSets
BEGIN
	SELECT @SQL1 = @SQL1 + ' AnswerChoices' + Convert(VARCHAR, @RunningCounter) + ' text null ' + ', FreeFormOrOther' + Convert(VARCHAR, @RunningCounter) + ' text null, '

	SELECT @RunningCounter = @RunningCounter + 1
END

SELECT @SQL1 = substring(@SQL1, 1, len(@SQL1) - 1)

PRINT @SQL1

EXEC (@SQL1)

DECLARE @ChoiceAnswerText VARCHAR(8000)
DECLARE @SRID INT
DECLARE @SAID INT

DECLARE db_cursor CURSOR
FOR
SELECT SRID
FROM #Results

OPEN db_cursor

FETCH NEXT
FROM db_cursor
INTO @SRID

WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE db_cursor2 CURSOR
	FOR
	SELECT SAID
	FROM dbo.SurveyAnswers a
	INNER JOIN SurveyQuestion q ON a.QID = q.QID
	WHERE a.SRID = @SRID
	ORDER BY q.QNumber

	SELECT @RunningCounter = 1

	OPEN db_cursor2

	FETCH NEXT
	FROM db_cursor2
	INTO @SAID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @SQL1 = 'Update #Results Set AnswerChoices' + CONVERT(VARCHAR, @RunningCounter) + ' = (select replace(convert(varchar(8000), ChoiceAnswerText), ''~|~'', '' AND '') from dbo.SurveyAnswers where SAID = ' + CONVERT(VARCHAR, @SAID) + '), ' + 'FreeFormOrOther' + CONVERT(VARCHAR, @RunningCounter) + ' = (select replace(convert(varchar(8000), ClarificationText), ''~|~'', '' AND '') + ' + '   convert(varchar(8000), FreeFormAnswer) from dbo.SurveyAnswers where SAID = ' + CONVERT(VARCHAR, @SAID) + ') where SRID = ' + CONVERT(VARCHAR, @SRID)

		PRINT @SQL1

		EXEC (@SQL1)

		SELECT @RunningCounter = @RunningCounter + 1

		FETCH NEXT
		FROM db_cursor2
		INTO @SAID
	END

	CLOSE db_cursor2

	DEALLOCATE db_cursor2

	FETCH NEXT
	FROM db_cursor
	INTO @SRID
END

CLOSE db_cursor

DEALLOCATE db_cursor

ALTER TABLE #Results

DROP COLUMN SRID

SELECT *
FROM #Results

DROP TABLE #Results

DROP TABLE #T1
GO

PRINT N'Altering [dbo].[app_Badge_GetBadgeBranches]...';
GO

ALTER PROCEDURE [dbo].[app_Badge_GetBadgeBranches] @BID INT,
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @CTID INT

SELECT @CTID = CTID
FROM dbo.CodeType
WHERE TenID = @TenID
	AND CodeTypeName = 'Branch'

SELECT @BID AS BID,
	c.CID,
	c.Code AS NAME, --c.CTID,
	CASE 
		WHEN bb.BID IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.BadgeBranch bb
RIGHT JOIN Code c ON bb.CID = c.CID
	AND (
		bb.BID = @BID
		OR bb.BID IS NULL
		)
WHERE c.TenID = @TenID
	AND c.CTID = @CTID
ORDER BY c.Code
GO

PRINT N'Altering [dbo].[app_Code_Insert]...';
GO

/****** Object:  StoredProcedure [dbo].[app_Code_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
ALTER PROCEDURE [dbo].[app_Code_Insert] (
	@CTID INT,
	@Code VARCHAR(255),
	@Description VARCHAR(255),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@CID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Code (
		CTID,
		Code,
		Description,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@CTID,
		@Code,
		@Description,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @CID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Code_Update]...';
GO

/****** Object:  StoredProcedure [dbo].[app_Code_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
ALTER PROCEDURE [dbo].[app_Code_Update] (
	@CID INT,
	@CTID INT,
	@Code VARCHAR(255),
	@Description VARCHAR(255),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = ''
	)
AS
UPDATE Code
SET CTID = @CTID,
	Code = @Code,
	Description = @Description,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE CID = @CID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Event_GetAll]...';
GO

--Create the Select Proc
ALTER PROCEDURE [dbo].[app_Event_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT Code
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
	AND HiddenFromPublic != 1
ORDER BY EventDate DESC
GO

PRINT N'Altering [dbo].[app_Event_GetUpcomingDisplay]...';
GO

ALTER PROCEDURE [dbo].[app_Event_GetUpcomingDisplay] @startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@systemID INT = 0,
	@branchID INT = 0,
	@searchText NVARCHAR(255) = NULL,
	@TenID INT = NULL
AS
SELECT e.*,
	c.[Code] AS [Branch],
	lc.[BranchLink],
	lc.[BranchAddress],
	lc.[BranchTelephone]
FROM [Event] e
LEFT OUTER JOIN [Code] c ON e.[BranchID] = c.[CID]
LEFT OUTER JOIN [LibraryCrosswalk] lc ON e.[BranchID] = lc.[BranchID]
WHERE (
		e.[BranchID] = @branchID
		OR @branchID = 0
		)
	AND (
		lc.[DistrictID] = @systemID
		OR @systemID = 0
		)
	AND (
		e.[EventDate] >= @startDate
		OR (
			e.[EndDate] IS NOT NULL
			AND e.[EndDate] >= @startDate
			)
		OR @startDate IS NULL
		)
	AND (
		e.[EventDate] <= @endDate
		OR (
			e.[EndDate] IS NOT NULL
			AND e.[EndDate] <= @endDate
			)
		OR @endDate IS NULL
		)
	AND (
		dateadd(d, 1, e.[EventDate]) >= GETDATE()
		OR (
			dateadd(d, 1, e.[EndDate]) >= GETDATE()
			AND e.[EndDate] IS NOT NULL
			)
		)
	AND (
		e.[TenID] = @TenID
		OR @TenID IS NULL
		)
	AND e.[HiddenFromPublic] != 1
	AND (
		(
			e.[EventTitle] LIKE @searchText
			OR e.[HTML] LIKE @searchText
			)
		OR @searchText IS NULL
		)
ORDER BY e.[EventDate] ASC,
	e.[EventTitle]
GO

PRINT N'Altering [dbo].[app_LibraryCrosswalk_GetAll]...';
GO

ALTER PROCEDURE [dbo].[app_LibraryCrosswalk_GetAll] @TenID INT = NULL
AS
DECLARE @Libraries TABLE (
	CID INT NOT NULL,
	Code VARCHAR(50) NOT NULL
	)

INSERT INTO @Libraries
SELECT c.CID,
	c.Code
FROM Code c
INNER JOIN CodeType t ON c.CTID = t.CTID
WHERE t.CodeTypeName = 'Branch'
	AND (
		t.TenID = @TenID
		OR @TenID IS NULL
		)

DELETE
FROM [LibraryCrosswalk]
WHERE BranchID NOT IN (
		SELECT CID
		FROM @Libraries
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)

SELECT isnull(w.ID, 0) AS ID,
	isnull(l.CID, 0) AS BranchID,
	isnull(w.DistrictID, 0) AS DistrictID,
	isnull(w.City, '') AS City,
	isnull(w.BranchLink, '') AS BranchLink,
	isnull(w.BranchAddress, '') AS BranchAddress,
	isnull(w.BranchTelephone, '') AS BranchTelephone
FROM [LibraryCrosswalk] w
RIGHT JOIN @Libraries l ON w.BranchID = l.CID
ORDER BY l.Code
GO

PRINT N'Altering [dbo].[app_CodeType_Insert]...';
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
ALTER PROCEDURE [dbo].[app_CodeType_Insert] (
	@isSystem BIT,
	@CodeTypeName VARCHAR(255),
	@Description TEXT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@CTID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO CodeType (
		isSystem,
		CodeTypeName,
		Description,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@isSystem,
		@CodeTypeName,
		@Description,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @CTID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_CodeType_Update]...';
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
ALTER PROCEDURE [dbo].[app_CodeType_Update] (
	@CTID INT,
	@isSystem BIT,
	@CodeTypeName VARCHAR(255),
	@Description TEXT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = ''
	)
AS
UPDATE CodeType
SET isSystem = @isSystem,
	CodeTypeName = @CodeTypeName,
	Description = @Description,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE CTID = @CTID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Badge_GetBadgeEvents]...';
GO

------------------------------------------------------------------
ALTER PROCEDURE [dbo].[app_Badge_GetBadgeEvents] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = LTRIM(RTRIM(STUFF((
					SELECT ', ' + e.EventTitle
					FROM Event e
					WHERE e.TenID = @TenID
						AND e.BadgeID = @BID
						AND e.HiddenFromPublic != 1
					GROUP BY e.EventTitle
					ORDER BY e.EventTitle
					FOR XML PATH('')
					), 1, 1, '')))
GO

PRINT N'Altering [dbo].[app_Event_GetEventsByEventCode]...';
GO

ALTER PROCEDURE [dbo].[app_Event_GetEventsByEventCode] (
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT *
	FROM Event
	WHERE CAST(GETDATE() AS DATE) >= CAST(EventDate AS DATE)
		AND SecretCode = @Key
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
END
GO

PRINT N'Altering [dbo].[app_Event_InitTenant]...';
GO

ALTER PROCEDURE [dbo].[app_Event_InitTenant] @src INT,
	@dst INT
AS
INSERT INTO Event (
	EventTitle,
	EventDate,
	EventTime,
	HTML,
	SecretCode,
	NumberPoints,
	BadgeID,
	BranchID,
	Custom1,
	Custom2,
	Custom3,
	LastModDate,
	LastModUser,
	AddedDate,
	AddedUser,
	TenID,
	FldInt1,
	FldInt2
	--,FldInt3
	,
	FldBit1,
	FldBit2,
	FldBit3,
	FldText1,
	FldText2,
	FldText3,
	EndDate,
	EndTime,
	ShortDescription,
	FldInt3,
	ExternalLinkToEvent,
	HiddenFromPublic
	)
OUTPUT 'event',
	@dst,
	[inserted].FldInt3,
	GETDATE(),
	[inserted].[EID]
INTO TenantInitData(IntitType, DestTID, SrcPK, DateCreated, DstPK)
SELECT e.EventTitle,
	e.EventDate,
	e.EventTime,
	e.HTML,
	e.SecretCode,
	e.NumberPoints,
	0 -- CANT DO BADGE
	,
	0 -- CANT DO BRANCH
	,
	e.Custom1,
	e.Custom2,
	e.Custom3,
	e.LastModDate,
	e.LastModUser,
	e.AddedDate,
	'SYSADMIN',
	@dst,
	e.FldInt1,
	e.FldInt2
	--,e.FldInt3
	,
	e.FldBit1,
	e.FldBit2,
	e.FldBit3,
	e.FldText1,
	e.FldText2,
	e.FldText3,
	e.EndDate,
	e.EndTime,
	e.ShortDescription,
	e.EID,
	e.ExternalLinkToEvent,
	e.HiddenFromPublic
FROM Event e
WHERE e.TenID = @src
	AND e.EID NOT IN (
		SELECT SrcPK
		FROM TenantInitData
		WHERE IntitType = 'event'
			AND DestTID = @dst
		)
GO

PRINT N'Altering [dbo].[app_Event_Insert]...';
GO

--Create the Insert Proc
ALTER PROCEDURE [dbo].[app_Event_Insert] (
	@EventTitle VARCHAR(150),
	@EventDate DATETIME,
	@EventTime VARCHAR(15),
	@HTML TEXT,
	@SecretCode VARCHAR(50),
	@NumberPoints INT,
	@BadgeID INT,
	@BranchID INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@ShortDescription TEXT,
	@EndDate DATETIME,
	@EndTime VARCHAR(50),
	@ExternalLinkToEvent NVARCHAR(255),
	@HiddenFromPublic BIT,
	@EID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Event (
		EventTitle,
		EventDate,
		EventTime,
		HTML,
		SecretCode,
		NumberPoints,
		BadgeID,
		BranchID,
		Custom1,
		Custom2,
		Custom3,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		ShortDescription,
		EndDate,
		EndTime,
		ExternalLinkToEvent,
		HiddenFromPublic
		)
	VALUES (
		@EventTitle,
		@EventDate,
		@EventTime,
		@HTML,
		@SecretCode,
		@NumberPoints,
		@BadgeID,
		@BranchID,
		@Custom1,
		@Custom2,
		@Custom3,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@ShortDescription,
		@EndDate,
		@EndTime,
		@ExternalLinkToEvent,
		@HiddenFromPublic
		)

	SELECT @EID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Event_Update]...';
GO

--Create the Update Proc
ALTER PROCEDURE [dbo].[app_Event_Update] (
	@EID INT,
	@EventTitle VARCHAR(150),
	@EventDate DATETIME,
	@EventTime VARCHAR(15),
	@HTML TEXT,
	@SecretCode VARCHAR(50),
	@NumberPoints INT,
	@BadgeID INT,
	@BranchID INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@ShortDescription TEXT,
	@EndDate DATETIME,
	@EndTime VARCHAR(50),
	@ExternalLinkToEvent NVARCHAR(255),
	@HiddenFromPublic BIT
	)
AS
UPDATE Event
SET EventTitle = @EventTitle,
	EventDate = @EventDate,
	EventTime = @EventTime,
	HTML = @HTML,
	SecretCode = @SecretCode,
	NumberPoints = @NumberPoints,
	BadgeID = @BadgeID,
	BranchID = @BranchID,
	Custom1 = @Custom1,
	Custom2 = @Custom2,
	Custom3 = @Custom3,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	ShortDescription = @ShortDescription,
	EndDate = @EndDate,
	EndTime = @EndTime,
	ExternalLinkToEvent = @ExternalLinkToEvent,
	HiddenFromPublic = @HiddenFromPublic
WHERE EID = @EID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_LibraryCrosswalk_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_LibraryCrosswalk_Insert] (
	@BranchID INT,
	@DistrictID INT,
	@City VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@BranchLink NVARCHAR(255) = '',
	@BranchAddress NVARCHAR(255) = '',
	@BranchTelephone NVARCHAR(255) = '',
	@ID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO LibraryCrosswalk (
		BranchID,
		DistrictID,
		City,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		BranchLink,
		BranchAddress,
		BranchTelephone
		)
	VALUES (
		@BranchID,
		@DistrictID,
		@City,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@BranchLink,
		@BranchAddress,
		@BranchTelephone
		)

	SELECT @ID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_LibraryCrosswalk_Update]...';
GO

ALTER PROCEDURE [dbo].[app_LibraryCrosswalk_Update] (
	@ID INT,
	@BranchID INT,
	@DistrictID INT,
	@City VARCHAR(50),
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@BranchLink NVARCHAR(255) = '',
	@BranchAddress NVARCHAR(255) = '',
	@BranchTelephone NVARCHAR(255) = ''
	)
AS
UPDATE LibraryCrosswalk
SET BranchID = @BranchID,
	DistrictID = @DistrictID,
	City = @City,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	BranchLink = @BranchLink,
	BranchAddress = @BranchAddress,
	BranchTelephone = @BranchTelephone
WHERE ID = @ID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Programs_GetProgramMinigames]...';
GO

ALTER PROCEDURE [dbo].[app_Programs_GetProgramMinigames] @LevelIDs VARCHAR(1000) = '',
	@WhichMG INT = 0,
	@DefaultMG INT = 0
AS
IF @WhichMG = 0
	SELECT DISTINCT x.MGID,
		x.GameName
	FROM (
		SELECT mg.MGID,
			mg.GameName,
			- 1 AS LevelNumber
		FROM Minigame mg
		WHERE mg.MGID = @DefaultMG
		
		UNION
		
		SELECT mg.MGID,
			mg.GameName,
			pg.LevelNumber
		FROM Minigame mg
		INNER JOIN dbo.ProgramGameLevel pg ON mg.MGID = pg.Minigame1ID
		WHERE pg.PGLID IN (
				SELECT *
				FROM [dbo].[fnSplitBigInt](@LevelIDs)
				)
			--order by LevelNumber
		) AS x
ELSE
	SELECT DISTINCT x.MGID,
		x.GameName
	FROM (
		SELECT mg.MGID,
			mg.GameName,
			- 1 AS LevelNumber
		FROM Minigame mg
		WHERE mg.MGID = @DefaultMG
		
		UNION
		
		SELECT mg.MGID,
			mg.GameName,
			pg.LevelNumber
		FROM Minigame mg
		INNER JOIN dbo.ProgramGameLevel pg ON mg.MGID = pg.Minigame2ID
		WHERE pg.PGLID IN (
				SELECT *
				FROM [dbo].[fnSplitBigInt](@LevelIDs)
				)
			--order by LevelNumber
		) AS x
		/*
-- deprecated when added the default board game minigames
		select mg.*
			from Minigame mg join dbo.ProgramGameLevel pg
				on mg.MGID = pg.Minigame2ID
			where pg.PGLID in
					(select * from [dbo].[fnSplitBigInt](@LevelIDs))
		order by pg.LevelNumber
*/
GO

PRINT N'Altering [dbo].[app_Offers_GetForDisplay]...';
GO

ALTER PROCEDURE [dbo].[app_Offers_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL
AS
--declare @PID int
--select @PID = 100000
IF (@TenID IS NULL)
	SELECT @TenID = TenID
	FROM Patron
	WHERE PID = @PID

DECLARE @Zip VARCHAR(20)
DECLARE @Age INT,
	@ProgramId INT,
	@BranchId INT

SELECT @Age = isnull(Age, 0),
	@Zip = isnull(ZipCode, ''),
	@ProgramId = isnull(ProgID, 0),
	@BranchId = isnull(PrimaryLibrary, 0)
FROM Patron
WHERE PID = @PID

----------------------------------------------------------
--select @Age, @Zip, @Age-36, @ProgramId, @BranchId
--select  o.*
--from Offer o
----------------------------------------------------------
SELECT *
INTO #temp
FROM Offer
WHERE TenID = @TenID
	AND Offer.isEnabled = 1
	AND (
		Offer.MaxImpressions = 0
		OR Offer.MaxImpressions > Offer.TotalImpressions
		)

DELETE
FROM #temp
WHERE AgeStart > 0
	AND AgeEnd = 0
	AND @Age < AgeStart

DELETE
FROM #temp
WHERE AgeEnd > 0
	AND AgeStart = 0
	AND @Age > AgeEnd

DELETE
FROM #temp
WHERE AgeEnd > 0
	AND AgeStart > 0
	AND (
		@Age < AgeStart
		OR @Age > AgeEnd
		)

DELETE
FROM #temp
WHERE ProgramId <> 0
	AND ProgramId <> @ProgramId

IF @BranchId <> 0
	DELETE
	FROM #temp
	WHERE BranchId <> 0
		AND BranchId <> @BranchId

IF @Zip <> ''
	DELETE
	FROM #temp
	WHERE ZipCode <> ''
		AND ZipCode <> left(@Zip, 5)

SELECT ROW_NUMBER() OVER (
		ORDER BY OID
		) AS Rank,
	*
FROM #temp
GO

PRINT N'Altering [dbo].[app_Patron_GetPatronForEdit]...';
GO

ALTER PROCEDURE [dbo].[app_Patron_GetPatronForEdit] @PID INT = 0,
	@TenID INT = NULL
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT isNull(p.[PID], 0) AS PID,
	isNull(p.[IsMasterAccount], 0) AS IsMasterAccount,
	isNull(p.MasterAcctPID, 0) AS [MasterAcctPID],
	isNull(p.Username, '') AS [Username],
	isNull(p.Password, '') AS [Password],
	isNull(p.DOB, NULL) AS [DOB],
	isNull(p.Age, '') AS [Age],
	isNull(p.SchoolGrade, '') AS [SchoolGrade],
	isNull(p.ProgID, 0) AS [ProgID],
	isNull(p.FirstName, '') AS [FirstName],
	isNull(p.MiddleName, '') AS [MiddleName],
	isNull(p.LastName, '') AS [LastName],
	isNull(p.Gender, '') AS [Gender],
	isNull(p.EmailAddress, '') AS [EmailAddress],
	isNull(p.PhoneNumber, '') AS [PhoneNumber],
	isNull(p.StreetAddress1, '') AS [StreetAddress1],
	isNull(p.StreetAddress2, '') AS [StreetAddress2],
	isNull(p.City, '') AS [City],
	isNull(p.STATE, '') AS [State],
	isNull(p.ZipCode, '') AS [ZipCode],
	isNull(p.Country, '') AS [Country],
	isNull(p.County, '') AS [County],
	isNull(p.ParentGuardianFirstName, '') AS [ParentGuardianFirstName],
	isNull(p.ParentGuardianLastName, '') AS [ParentGuardianLastName],
	isNull(p.ParentGuardianMiddleName, '') AS [ParentGuardianMiddleName],
	isNull(p.PrimaryLibrary, 0) AS [PrimaryLibrary],
	isNull(p.LibraryCard, '') AS [LibraryCard],
	isNull(p.SchoolName, '') AS [SchoolName],
	isNull(p.District, '') AS [District],
	isNull(p.Teacher, '') AS [Teacher],
	isNull(p.GroupTeamName, '') AS [GroupTeamName],
	isNull(p.SchoolType, '') AS [SchoolType],
	isNull(p.LiteracyLevel1, '') AS [LiteracyLevel1],
	isNull(p.LiteracyLevel2, '') AS [LiteracyLevel2],
	isNull(p.ParentPermFlag, 0) AS [ParentPermFlag],
	isNull(p.Over18Flag, 0) AS [Over18Flag],
	isNull(p.ShareFlag, 0) AS [ShareFlag],
	isNull(p.TermsOfUseflag, 0) AS [TermsOfUseflag],
	isNull(p.Custom1, '') AS [Custom1],
	isNull(p.Custom2, '') AS [Custom2],
	isNull(p.Custom3, '') AS [Custom3],
	isNull(p.Custom4, '') AS [Custom4],
	isNull(p.Custom5, '') AS [Custom5],
	isNull(p.RegistrationDate, NULL) AS [RegistrationDate],
	isNull(p.SDistrict, 0) AS [SDistrict],
	isNull(p.Goal, 0) AS [Goal],
	isNull(p.AvatarState, '') AS [AvatarState],
	isNull(p.GoalCache, '') AS [GoalCache],
	rs.*
FROM dbo.Patron p
RIGHT JOIN RegistrationSettings rs ON p.PID = @PID
WHERE rs.TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Patron_GetSubAccountList]...';
GO

/****** Object:  StoredProcedure [dbo].[app_Patron_GetSubAccountList]    Script Date: 01/05/2015 14:43:23 ******/
ALTER PROCEDURE [dbo].[app_Patron_GetSubAccountList] @PID INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT subs.*,
	pg.AdminName AS Program
FROM dbo.Patron subs
INNER JOIN dbo.Patron mast ON subs.MasterAcctPID = mast.PID
	AND mast.PID = @PID
	AND mast.IsMasterAccount = 1
LEFT JOIN Programs pg ON subs.ProgID = pg.PID
ORDER BY [FirstName],
	[LastName],
	[Username]
GO

PRINT N'Altering [dbo].[app_Patron_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_Patron_Insert] (
	@IsMasterAccount BIT,
	@MasterAcctPID INT,
	@Username VARCHAR(50),
	@Password VARCHAR(255),
	@DOB DATETIME,
	@Age INT,
	@SchoolGrade VARCHAR(5),
	@ProgID INT,
	@FirstName VARCHAR(50),
	@MiddleName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@StreetAddress1 VARCHAR(80),
	@StreetAddress2 VARCHAR(80),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@Country VARCHAR(50),
	@County VARCHAR(50),
	@ParentGuardianFirstName VARCHAR(50),
	@ParentGuardianLastName VARCHAR(50),
	@ParentGuardianMiddleName VARCHAR(50),
	@PrimaryLibrary INT,
	@LibraryCard VARCHAR(20),
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@ParentPermFlag BIT,
	@Over18Flag BIT,
	@ShareFlag BIT,
	@TermsOfUseflag BIT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@SDistrict INT,
	@Goal INT,
	@AvatarState VARCHAR(50) = '',
	@GoalCache INT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@Score1 INT = 0,
	@Score2 INT = 0,
	@Score1Pct DECIMAL(18, 2) = 0,
	@Score2Pct DECIMAL(18, 2) = 0,
	@Score1Date DATETIME,
	@Score2Date DATETIME,
	@PID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Patron (
		IsMasterAccount,
		MasterAcctPID,
		Username,
		Password,
		DOB,
		Age,
		SchoolGrade,
		ProgID,
		FirstName,
		MiddleName,
		LastName,
		Gender,
		EmailAddress,
		PhoneNumber,
		StreetAddress1,
		StreetAddress2,
		City,
		STATE,
		ZipCode,
		Country,
		County,
		ParentGuardianFirstName,
		ParentGuardianLastName,
		ParentGuardianMiddleName,
		PrimaryLibrary,
		LibraryCard,
		SchoolName,
		District,
		Teacher,
		GroupTeamName,
		SchoolType,
		LiteracyLevel1,
		LiteracyLevel2,
		ParentPermFlag,
		Over18Flag,
		ShareFlag,
		TermsOfUseflag,
		Custom1,
		Custom2,
		Custom3,
		Custom4,
		Custom5,
		SDistrict,
		Goal,
		AvatarState,
		GoalCache,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		Score1,
		Score2,
		Score1Pct,
		Score2Pct,
		Score1Date,
		Score2Date
		)
	VALUES (
		@IsMasterAccount,
		@MasterAcctPID,
		@Username,
		@Password,
		@DOB,
		@Age,
		@SchoolGrade,
		@ProgID,
		@FirstName,
		@MiddleName,
		@LastName,
		@Gender,
		@EmailAddress,
		@PhoneNumber,
		@StreetAddress1,
		@StreetAddress2,
		@City,
		@State,
		@ZipCode,
		@Country,
		@County,
		@ParentGuardianFirstName,
		@ParentGuardianLastName,
		@ParentGuardianMiddleName,
		@PrimaryLibrary,
		@LibraryCard,
		@SchoolName,
		@District,
		@Teacher,
		@GroupTeamName,
		@SchoolType,
		@LiteracyLevel1,
		@LiteracyLevel2,
		@ParentPermFlag,
		@Over18Flag,
		@ShareFlag,
		@TermsOfUseflag,
		@Custom1,
		@Custom2,
		@Custom3,
		@Custom4,
		@Custom5,
		@SDistrict,
		@Goal,
		@AvatarState,
		@GoalCache,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@Score1,
		@Score2,
		@Score1Pct,
		@Score2Pct,
		@Score1Date,
		@Score2Date
		)

	SELECT @PID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Patron_Update]...';
GO

ALTER PROCEDURE [dbo].[app_Patron_Update] (
	@PID INT,
	@IsMasterAccount BIT,
	@MasterAcctPID INT,
	@Username VARCHAR(50),
	@Password VARCHAR(255),
	@DOB DATETIME,
	@Age INT,
	@SchoolGrade VARCHAR(5),
	@ProgID INT,
	@FirstName VARCHAR(50),
	@MiddleName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@StreetAddress1 VARCHAR(80),
	@StreetAddress2 VARCHAR(80),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@Country VARCHAR(50),
	@County VARCHAR(50),
	@ParentGuardianFirstName VARCHAR(50),
	@ParentGuardianLastName VARCHAR(50),
	@ParentGuardianMiddleName VARCHAR(50),
	@PrimaryLibrary INT,
	@LibraryCard VARCHAR(20),
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@ParentPermFlag BIT,
	@Over18Flag BIT,
	@ShareFlag BIT,
	@TermsOfUseflag BIT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@SDistrict INT,
	@Goal INT,
	@AvatarState VARCHAR(50),
	@GoalCache INT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@Score1 INT = 0,
	@Score2 INT = 0,
	@Score1Pct DECIMAL(18, 2) = 0,
	@Score2Pct DECIMAL(18, 2) = 0,
	@Score1Date DATETIME,
	@Score2Date DATETIME
	)
AS
UPDATE Patron
SET IsMasterAccount = @IsMasterAccount,
	MasterAcctPID = @MasterAcctPID,
	Username = @Username,
	Password = @Password,
	DOB = @DOB,
	Age = @Age,
	SchoolGrade = @SchoolGrade,
	ProgID = @ProgID,
	FirstName = @FirstName,
	MiddleName = @MiddleName,
	LastName = @LastName,
	Gender = @Gender,
	EmailAddress = @EmailAddress,
	PhoneNumber = @PhoneNumber,
	StreetAddress1 = @StreetAddress1,
	StreetAddress2 = @StreetAddress2,
	City = @City,
	STATE = @State,
	ZipCode = @ZipCode,
	Country = @Country,
	County = @County,
	ParentGuardianFirstName = @ParentGuardianFirstName,
	ParentGuardianLastName = @ParentGuardianLastName,
	ParentGuardianMiddleName = @ParentGuardianMiddleName,
	PrimaryLibrary = @PrimaryLibrary,
	LibraryCard = @LibraryCard,
	SchoolName = @SchoolName,
	District = @District,
	Teacher = @Teacher,
	GroupTeamName = @GroupTeamName,
	SchoolType = @SchoolType,
	LiteracyLevel1 = @LiteracyLevel1,
	LiteracyLevel2 = @LiteracyLevel2,
	ParentPermFlag = @ParentPermFlag,
	Over18Flag = @Over18Flag,
	ShareFlag = @ShareFlag,
	TermsOfUseflag = @TermsOfUseflag,
	Custom1 = @Custom1,
	Custom2 = @Custom2,
	Custom3 = @Custom3,
	Custom4 = @Custom4,
	Custom5 = @Custom5,
	SDistrict = @SDistrict,
	Goal = @Goal,
	AvatarState = @AvatarState,
	GoalCache = @GoalCache,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	Score1 = @Score1,
	Score2 = @Score2,
	Score1Pct = @Score1Pct,
	Score2Pct = @Score2Pct,
	Score1Date = @Score1Date,
	Score2Date = @Score2Date
WHERE PID = @PID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_PatronPoints_GetProgramLeaderboard]...';
GO

ALTER PROCEDURE [dbo].[app_PatronPoints_GetProgramLeaderboard] @ProgId INT = 0,
	@TenID INT = NULL
AS
IF @TenID IS NULL
	SELECT @TenID = TenID
	FROM Programs
	WHERE PID = @ProgId

SELECT TOP 10 pp.PID,
	isnull(SUM(isnull(convert(BIGINT, NumPoints), 0)), 0) AS TotalPoints,
	p.Username
INTO #TempLB
FROM PatronPoints pp
INNER JOIN Patron p ON pp.PID = p.PID
	AND p.TenID = @TenID
WHERE p.ProgID = @ProgId
GROUP BY pp.PID,
	p.Username
ORDER BY TotalPoints DESC

UPDATE #TempLB
SET TotalPoints = 20000000
WHERE TotalPoints > 20000000

SELECT PID,
	Username,
	CONVERT(INT, TotalPoints) AS TotalPoints,
	ROW_NUMBER() OVER (
		ORDER BY TotalPoints DESC
		) AS Rank
FROM #TempLB
ORDER BY TotalPoints DESC
GO

PRINT N'Altering [dbo].[app_PrizeDrawing_GetAllWinners]...';
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetAllWinners]    Script Date: 01/05/2015 14:43:24 ******/
ALTER PROCEDURE [dbo].[app_PrizeDrawing_GetAllWinners] @PDID INT = 0
AS
SELECT pdw.*,
	p.Username,
	p.FirstName,
	p.LastName,
	p.PID
FROM dbo.PrizeDrawingWinners pdw
LEFT JOIN Patron p ON pdw.PatronID = p.PID
WHERE PDID = @PDID
ORDER BY PDID DESC
GO

PRINT N'Altering [dbo].[app_Programs_GetDefaultProgramForAgeAndGrade]...';
GO

ALTER PROCEDURE [dbo].[app_Programs_GetDefaultProgramForAgeAndGrade] @Age INT = - 1,
	@Grade INT = - 1,
	@TenID INT = NULL
AS
DECLARE @ID INT

SELECT PID,
	Porder,
	MaxAge,
	MaxGrade,
	TabName
INTO #Temp
FROM [Programs]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)

IF (
		@Grade = - 1
		AND @Age >= 0
		)
BEGIN
	SELECT TOP 1 @ID = PID
	FROM #Temp
	WHERE MaxAge >= @Age
	ORDER BY MaxAge ASC,
		POrder ASC
		--select @ID
END
ELSE IF (
		@Grade > 0
		AND @Age = 0
		)
BEGIN
	SELECT TOP 1 @ID = PID
	FROM #Temp
	WHERE MaxGrade >= @Grade
	ORDER BY MaxGrade ASC,
		POrder ASC
		--select @ID
END
ELSE
BEGIN
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC
		--SELECT @ID
END

IF (@ID IS NULL)
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC

SELECT @ID
GO

PRINT N'Altering [dbo].[app_Programs_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_Programs_Insert] (
	@AdminName VARCHAR(50),
	@Title VARCHAR(50),
	@TabName VARCHAR(20),
	@POrder INT,
	@IsActive BIT,
	@IsHidden BIT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@MaxAge INT,
	@MaxGrade INT,
	@LoggingStart DATETIME,
	@LoggingEnd DATETIME,
	@ParentalConsentFlag BIT,
	@ParentalConsentText TEXT,
	@PatronReviewFlag BIT,
	@RequireBookDetails BIT,
	@LogoutURL VARCHAR(150),
	@ProgramGameID INT,
	@HTML1 TEXT,
	@HTML2 TEXT,
	@HTML3 TEXT,
	@HTML4 TEXT,
	@HTML5 TEXT,
	@HTML6 TEXT,
	@BannerImage VARCHAR(150),
	@RegistrationBadgeID INT,
	@CompletionPoints INT = 0,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@PreTestID INT = 0,
	@PostTestID INT = 0,
	@PreTestMandatory INT = 0,
	@PretestEndDate DATETIME,
	@PostTestStartDate DATETIME,
	@GoalDefault INT = 0,
	@GoalMin INT = 0,
	@GoalMax INT = 0,
	@GoalIntervalId INT = 0,
	@HideSchoolInRegistration BIT = 0,
	@PID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Programs (
		AdminName,
		Title,
		TabName,
		POrder,
		IsActive,
		IsHidden,
		StartDate,
		EndDate,
		MaxAge,
		MaxGrade,
		LoggingStart,
		LoggingEnd,
		ParentalConsentFlag,
		ParentalConsentText,
		PatronReviewFlag,
		RequirebookDetails,
		LogoutURL,
		ProgramGameID,
		HTML1,
		HTML2,
		HTML3,
		HTML4,
		HTML5,
		HTML6,
		BannerImage,
		RegistrationBadgeID,
		CompletionPoints,
		LastModUser,
		AddedDate,
		AddedUser,
		LastModDate,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		PreTestID,
		PostTestID,
		PreTestMandatory,
		PretestEndDate,
		PostTestStartDate,
		GoalDefault,
		GoalMin,
		GoalMax,
		GoalIntervalId,
		HideSchoolInRegistration
		)
	VALUES (
		@AdminName,
		@Title,
		@TabName,
		(
			SELECT isnull(Max(POrder), 0) + 1
			FROM Programs
			),
		@IsActive,
		@IsHidden,
		@StartDate,
		@EndDate,
		@MaxAge,
		@MaxGrade,
		@LoggingStart,
		@LoggingEnd,
		@ParentalConsentFlag,
		@ParentalConsentText,
		@PatronReviewFlag,
		@RequireBookDetails,
		@LogoutURL,
		@ProgramGameID,
		@HTML1,
		@HTML2,
		@HTML3,
		@HTML4,
		@HTML5,
		@HTML6,
		@BannerImage,
		@RegistrationBadgeID,
		@CompletionPoints,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@LastModDate,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@PreTestID,
		@PostTestID,
		@PreTestMandatory,
		@PretestEndDate,
		@PostTestStartDate,
		@GoalDefault,
		@GoalMin,
		@GoalMax,
		@GoalIntervalId,
		@HideSchoolInRegistration
		)

	SELECT @PID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Programs_Update]...';
GO

ALTER PROCEDURE [dbo].[app_Programs_Update] (
	@PID INT,
	@AdminName VARCHAR(50),
	@Title VARCHAR(50),
	@TabName VARCHAR(20),
	@POrder INT,
	@IsActive BIT,
	@IsHidden BIT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@MaxAge INT,
	@MaxGrade INT,
	@LoggingStart DATETIME,
	@LoggingEnd DATETIME,
	@ParentalConsentFlag BIT,
	@ParentalConsentText TEXT,
	@PatronReviewFlag BIT,
	@RequireBookDetails BIT,
	@LogoutURL VARCHAR(150),
	@ProgramGameID INT,
	@HTML1 TEXT,
	@HTML2 TEXT,
	@HTML3 TEXT,
	@HTML4 TEXT,
	@HTML5 TEXT,
	@HTML6 TEXT,
	@BannerImage VARCHAR(150),
	@RegistrationBadgeID INT,
	@CompletionPoints INT = 0,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@PreTestID INT = 0,
	@PostTestID INT = 0,
	@PreTestMandatory INT = 0,
	@PretestEndDate DATETIME,
	@PostTestStartDate DATETIME,
	@GoalDefault INT = 0,
	@GoalMin INT = 0,
	@GoalMax INT = 0,
	@GoalIntervalId INT = 0,
	@HideSchoolInRegistration BIT = 0
	)
AS
UPDATE Programs
SET AdminName = @AdminName,
	Title = @Title,
	TabName = @TabName,
	POrder = @POrder,
	IsActive = @IsActive,
	IsHidden = @IsHidden,
	StartDate = @StartDate,
	EndDate = @EndDate,
	MaxAge = @MaxAge,
	MaxGrade = @MaxGrade,
	LoggingStart = @LoggingStart,
	LoggingEnd = @LoggingEnd,
	ParentalConsentFlag = @ParentalConsentFlag,
	ParentalConsentText = @ParentalConsentText,
	PatronReviewFlag = @PatronReviewFlag,
	RequireBookDetails = @RequireBookDetails,
	LogoutURL = @LogoutURL,
	ProgramGameID = @ProgramGameID,
	HTML1 = @HTML1,
	HTML2 = @HTML2,
	HTML3 = @HTML3,
	HTML4 = @HTML4,
	HTML5 = @HTML5,
	HTML6 = @HTML6,
	BannerImage = @BannerImage,
	RegistrationBadgeID = @RegistrationBadgeID,
	CompletionPoints = @CompletionPoints,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LastModDate = @LastModDate,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	PreTestID = @PreTestID,
	PostTestID = @PostTestID,
	PreTestMandatory = @PreTestMandatory,
	PretestEndDate = @PretestEndDate,
	PostTestStartDate = @PostTestStartDate,
	GoalDefault = @GoalDefault,
	GoalMin = @GoalMin,
	GoalMax = @GoalMax,
	GoalIntervalId = @GoalIntervalId,
	HideSchoolInRegistration = @HideSchoolInRegistration
WHERE PID = @PID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_RegistrationSettings_Insert]...';
GO

ALTER PROCEDURE [dbo].[app_RegistrationSettings_Insert] (
	@Literacy1Label VARCHAR(50),
	@Literacy2Label VARCHAR(50),
	@DOB_Prompt BIT,
	@Age_Prompt BIT,
	@SchoolGrade_Prompt BIT,
	@FirstName_Prompt BIT,
	@MiddleName_Prompt BIT,
	@LastName_Prompt BIT,
	@Gender_Prompt BIT,
	@EmailAddress_Prompt BIT,
	@PhoneNumber_Prompt BIT,
	@StreetAddress1_Prompt BIT,
	@StreetAddress2_Prompt BIT,
	@City_Prompt BIT,
	@State_Prompt BIT,
	@ZipCode_Prompt BIT,
	@Country_Prompt BIT,
	@County_Prompt BIT,
	@ParentGuardianFirstName_Prompt BIT,
	@ParentGuardianLastName_Prompt BIT,
	@ParentGuardianMiddleName_Prompt BIT,
	@PrimaryLibrary_Prompt BIT,
	@LibraryCard_Prompt BIT,
	@SchoolName_Prompt BIT,
	@District_Prompt BIT,
	@Teacher_Prompt BIT,
	@GroupTeamName_Prompt BIT,
	@SchoolType_Prompt BIT,
	@LiteracyLevel1_Prompt BIT,
	@LiteracyLevel2_Prompt BIT,
	@ParentPermFlag_Prompt BIT,
	@Over18Flag_Prompt BIT,
	@ShareFlag_Prompt BIT,
	@TermsOfUseflag_Prompt BIT,
	@Custom1_Prompt BIT,
	@Custom2_Prompt BIT,
	@Custom3_Prompt BIT,
	@Custom4_Prompt BIT,
	@Custom5_Prompt BIT,
	@DOB_Req BIT,
	@Age_Req BIT,
	@SchoolGrade_Req BIT,
	@FirstName_Req BIT,
	@MiddleName_Req BIT,
	@LastName_Req BIT,
	@Gender_Req BIT,
	@EmailAddress_Req BIT,
	@PhoneNumber_Req BIT,
	@StreetAddress1_Req BIT,
	@StreetAddress2_Req BIT,
	@City_Req BIT,
	@State_Req BIT,
	@ZipCode_Req BIT,
	@Country_Req BIT,
	@County_Req BIT,
	@ParentGuardianFirstName_Req BIT,
	@ParentGuardianLastName_Req BIT,
	@ParentGuardianMiddleName_Req BIT,
	@PrimaryLibrary_Req BIT,
	@LibraryCard_Req BIT,
	@SchoolName_Req BIT,
	@District_Req BIT,
	@Teacher_Req BIT,
	@GroupTeamName_Req BIT,
	@SchoolType_Req BIT,
	@LiteracyLevel1_Req BIT,
	@LiteracyLevel2_Req BIT,
	@ParentPermFlag_Req BIT,
	@Over18Flag_Req BIT,
	@ShareFlag_Req BIT,
	@TermsOfUseflag_Req BIT,
	@Custom1_Req BIT,
	@Custom2_Req BIT,
	@Custom3_Req BIT,
	@Custom4_Req BIT,
	@Custom5_Req BIT,
	@DOB_Show BIT,
	@Age_Show BIT,
	@SchoolGrade_Show BIT,
	@FirstName_Show BIT,
	@MiddleName_Show BIT,
	@LastName_Show BIT,
	@Gender_Show BIT,
	@EmailAddress_Show BIT,
	@PhoneNumber_Show BIT,
	@StreetAddress1_Show BIT,
	@StreetAddress2_Show BIT,
	@City_Show BIT,
	@State_Show BIT,
	@ZipCode_Show BIT,
	@Country_Show BIT,
	@County_Show BIT,
	@ParentGuardianFirstName_Show BIT,
	@ParentGuardianLastName_Show BIT,
	@ParentGuardianMiddleName_Show BIT,
	@PrimaryLibrary_Show BIT,
	@LibraryCard_Show BIT,
	@SchoolName_Show BIT,
	@District_Show BIT,
	@Teacher_Show BIT,
	@GroupTeamName_Show BIT,
	@SchoolType_Show BIT,
	@LiteracyLevel1_Show BIT,
	@LiteracyLevel2_Show BIT,
	@ParentPermFlag_Show BIT,
	@Over18Flag_Show BIT,
	@ShareFlag_Show BIT,
	@TermsOfUseflag_Show BIT,
	@Custom1_Show BIT,
	@Custom2_Show BIT,
	@Custom3_Show BIT,
	@Custom4_Show BIT,
	@Custom5_Show BIT,
	@DOB_Edit BIT,
	@Age_Edit BIT,
	@SchoolGrade_Edit BIT,
	@FirstName_Edit BIT,
	@MiddleName_Edit BIT,
	@LastName_Edit BIT,
	@Gender_Edit BIT,
	@EmailAddress_Edit BIT,
	@PhoneNumber_Edit BIT,
	@StreetAddress1_Edit BIT,
	@StreetAddress2_Edit BIT,
	@City_Edit BIT,
	@State_Edit BIT,
	@ZipCode_Edit BIT,
	@Country_Edit BIT,
	@County_Edit BIT,
	@ParentGuardianFirstName_Edit BIT,
	@ParentGuardianLastName_Edit BIT,
	@ParentGuardianMiddleName_Edit BIT,
	@PrimaryLibrary_Edit BIT,
	@LibraryCard_Edit BIT,
	@SchoolName_Edit BIT,
	@District_Edit BIT,
	@Teacher_Edit BIT,
	@GroupTeamName_Edit BIT,
	@SchoolType_Edit BIT,
	@LiteracyLevel1_Edit BIT,
	@LiteracyLevel2_Edit BIT,
	@ParentPermFlag_Edit BIT,
	@Over18Flag_Edit BIT,
	@ShareFlag_Edit BIT,
	@TermsOfUseflag_Edit BIT,
	@Custom1_Edit BIT,
	@Custom2_Edit BIT,
	@Custom3_Edit BIT,
	@Custom4_Edit BIT,
	@Custom5_Edit BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict_Prompt BIT,
	@SDistrict_Req BIT,
	@SDistrict_Show BIT,
	@SDistrict_Edit BIT,
	@Goal_Prompt BIT,
	@Goal_Req BIT,
	@Goal_Show BIT,
	@Goal_Edit BIT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@RID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO RegistrationSettings (
		Literacy1Label,
		Literacy2Label,
		DOB_Prompt,
		Age_Prompt,
		SchoolGrade_Prompt,
		FirstName_Prompt,
		MiddleName_Prompt,
		LastName_Prompt,
		Gender_Prompt,
		EmailAddress_Prompt,
		PhoneNumber_Prompt,
		StreetAddress1_Prompt,
		StreetAddress2_Prompt,
		City_Prompt,
		State_Prompt,
		ZipCode_Prompt,
		Country_Prompt,
		County_Prompt,
		ParentGuardianFirstName_Prompt,
		ParentGuardianLastName_Prompt,
		ParentGuardianMiddleName_Prompt,
		PrimaryLibrary_Prompt,
		LibraryCard_Prompt,
		SchoolName_Prompt,
		District_Prompt,
		Teacher_Prompt,
		GroupTeamName_Prompt,
		SchoolType_Prompt,
		LiteracyLevel1_Prompt,
		LiteracyLevel2_Prompt,
		ParentPermFlag_Prompt,
		Over18Flag_Prompt,
		ShareFlag_Prompt,
		TermsOfUseflag_Prompt,
		Custom1_Prompt,
		Custom2_Prompt,
		Custom3_Prompt,
		Custom4_Prompt,
		Custom5_Prompt,
		DOB_Req,
		Age_Req,
		SchoolGrade_Req,
		FirstName_Req,
		MiddleName_Req,
		LastName_Req,
		Gender_Req,
		EmailAddress_Req,
		PhoneNumber_Req,
		StreetAddress1_Req,
		StreetAddress2_Req,
		City_Req,
		State_Req,
		ZipCode_Req,
		Country_Req,
		County_Req,
		ParentGuardianFirstName_Req,
		ParentGuardianLastName_Req,
		ParentGuardianMiddleName_Req,
		PrimaryLibrary_Req,
		LibraryCard_Req,
		SchoolName_Req,
		District_Req,
		Teacher_Req,
		GroupTeamName_Req,
		SchoolType_Req,
		LiteracyLevel1_Req,
		LiteracyLevel2_Req,
		ParentPermFlag_Req,
		Over18Flag_Req,
		ShareFlag_Req,
		TermsOfUseflag_Req,
		Custom1_Req,
		Custom2_Req,
		Custom3_Req,
		Custom4_Req,
		Custom5_Req,
		DOB_Show,
		Age_Show,
		SchoolGrade_Show,
		FirstName_Show,
		MiddleName_Show,
		LastName_Show,
		Gender_Show,
		EmailAddress_Show,
		PhoneNumber_Show,
		StreetAddress1_Show,
		StreetAddress2_Show,
		City_Show,
		State_Show,
		ZipCode_Show,
		Country_Show,
		County_Show,
		ParentGuardianFirstName_Show,
		ParentGuardianLastName_Show,
		ParentGuardianMiddleName_Show,
		PrimaryLibrary_Show,
		LibraryCard_Show,
		SchoolName_Show,
		District_Show,
		Teacher_Show,
		GroupTeamName_Show,
		SchoolType_Show,
		LiteracyLevel1_Show,
		LiteracyLevel2_Show,
		ParentPermFlag_Show,
		Over18Flag_Show,
		ShareFlag_Show,
		TermsOfUseflag_Show,
		Custom1_Show,
		Custom2_Show,
		Custom3_Show,
		Custom4_Show,
		Custom5_Show,
		DOB_Edit,
		Age_Edit,
		SchoolGrade_Edit,
		FirstName_Edit,
		MiddleName_Edit,
		LastName_Edit,
		Gender_Edit,
		EmailAddress_Edit,
		PhoneNumber_Edit,
		StreetAddress1_Edit,
		StreetAddress2_Edit,
		City_Edit,
		State_Edit,
		ZipCode_Edit,
		Country_Edit,
		County_Edit,
		ParentGuardianFirstName_Edit,
		ParentGuardianLastName_Edit,
		ParentGuardianMiddleName_Edit,
		PrimaryLibrary_Edit,
		LibraryCard_Edit,
		SchoolName_Edit,
		District_Edit,
		Teacher_Edit,
		GroupTeamName_Edit,
		SchoolType_Edit,
		LiteracyLevel1_Edit,
		LiteracyLevel2_Edit,
		ParentPermFlag_Edit,
		Over18Flag_Edit,
		ShareFlag_Edit,
		TermsOfUseflag_Edit,
		Custom1_Edit,
		Custom2_Edit,
		Custom3_Edit,
		Custom4_Edit,
		Custom5_Edit,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		SDistrict_Prompt,
		SDistrict_Req,
		SDistrict_Show,
		SDistrict_Edit,
		Goal_Prompt,
		Goal_Req,
		Goal_Show,
		Goal_Edit,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@Literacy1Label,
		@Literacy2Label,
		@DOB_Prompt,
		@Age_Prompt,
		@SchoolGrade_Prompt,
		@FirstName_Prompt,
		@MiddleName_Prompt,
		@LastName_Prompt,
		@Gender_Prompt,
		@EmailAddress_Prompt,
		@PhoneNumber_Prompt,
		@StreetAddress1_Prompt,
		@StreetAddress2_Prompt,
		@City_Prompt,
		@State_Prompt,
		@ZipCode_Prompt,
		@Country_Prompt,
		@County_Prompt,
		@ParentGuardianFirstName_Prompt,
		@ParentGuardianLastName_Prompt,
		@ParentGuardianMiddleName_Prompt,
		@PrimaryLibrary_Prompt,
		@LibraryCard_Prompt,
		@SchoolName_Prompt,
		@District_Prompt,
		@Teacher_Prompt,
		@GroupTeamName_Prompt,
		@SchoolType_Prompt,
		@LiteracyLevel1_Prompt,
		@LiteracyLevel2_Prompt,
		@ParentPermFlag_Prompt,
		@Over18Flag_Prompt,
		@ShareFlag_Prompt,
		@TermsOfUseflag_Prompt,
		@Custom1_Prompt,
		@Custom2_Prompt,
		@Custom3_Prompt,
		@Custom4_Prompt,
		@Custom5_Prompt,
		@DOB_Req,
		@Age_Req,
		@SchoolGrade_Req,
		@FirstName_Req,
		@MiddleName_Req,
		@LastName_Req,
		@Gender_Req,
		@EmailAddress_Req,
		@PhoneNumber_Req,
		@StreetAddress1_Req,
		@StreetAddress2_Req,
		@City_Req,
		@State_Req,
		@ZipCode_Req,
		@Country_Req,
		@County_Req,
		@ParentGuardianFirstName_Req,
		@ParentGuardianLastName_Req,
		@ParentGuardianMiddleName_Req,
		@PrimaryLibrary_Req,
		@LibraryCard_Req,
		@SchoolName_Req,
		@District_Req,
		@Teacher_Req,
		@GroupTeamName_Req,
		@SchoolType_Req,
		@LiteracyLevel1_Req,
		@LiteracyLevel2_Req,
		@ParentPermFlag_Req,
		@Over18Flag_Req,
		@ShareFlag_Req,
		@TermsOfUseflag_Req,
		@Custom1_Req,
		@Custom2_Req,
		@Custom3_Req,
		@Custom4_Req,
		@Custom5_Req,
		@DOB_Show,
		@Age_Show,
		@SchoolGrade_Show,
		@FirstName_Show,
		@MiddleName_Show,
		@LastName_Show,
		@Gender_Show,
		@EmailAddress_Show,
		@PhoneNumber_Show,
		@StreetAddress1_Show,
		@StreetAddress2_Show,
		@City_Show,
		@State_Show,
		@ZipCode_Show,
		@Country_Show,
		@County_Show,
		@ParentGuardianFirstName_Show,
		@ParentGuardianLastName_Show,
		@ParentGuardianMiddleName_Show,
		@PrimaryLibrary_Show,
		@LibraryCard_Show,
		@SchoolName_Show,
		@District_Show,
		@Teacher_Show,
		@GroupTeamName_Show,
		@SchoolType_Show,
		@LiteracyLevel1_Show,
		@LiteracyLevel2_Show,
		@ParentPermFlag_Show,
		@Over18Flag_Show,
		@ShareFlag_Show,
		@TermsOfUseflag_Show,
		@Custom1_Show,
		@Custom2_Show,
		@Custom3_Show,
		@Custom4_Show,
		@Custom5_Show,
		@DOB_Edit,
		@Age_Edit,
		@SchoolGrade_Edit,
		@FirstName_Edit,
		@MiddleName_Edit,
		@LastName_Edit,
		@Gender_Edit,
		@EmailAddress_Edit,
		@PhoneNumber_Edit,
		@StreetAddress1_Edit,
		@StreetAddress2_Edit,
		@City_Edit,
		@State_Edit,
		@ZipCode_Edit,
		@Country_Edit,
		@County_Edit,
		@ParentGuardianFirstName_Edit,
		@ParentGuardianLastName_Edit,
		@ParentGuardianMiddleName_Edit,
		@PrimaryLibrary_Edit,
		@LibraryCard_Edit,
		@SchoolName_Edit,
		@District_Edit,
		@Teacher_Edit,
		@GroupTeamName_Edit,
		@SchoolType_Edit,
		@LiteracyLevel1_Edit,
		@LiteracyLevel2_Edit,
		@ParentPermFlag_Edit,
		@Over18Flag_Edit,
		@ShareFlag_Edit,
		@TermsOfUseflag_Edit,
		@Custom1_Edit,
		@Custom2_Edit,
		@Custom3_Edit,
		@Custom4_Edit,
		@Custom5_Edit,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@SDistrict_Prompt,
		@SDistrict_Req,
		@SDistrict_Show,
		@SDistrict_Edit,
		@Goal_Prompt,
		@Goal_Req,
		@Goal_Show,
		@Goal_Edit,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @RID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_RegistrationSettings_Update]...';
GO

ALTER PROCEDURE [dbo].[app_RegistrationSettings_Update] (
	@RID INT,
	@Literacy1Label VARCHAR(50),
	@Literacy2Label VARCHAR(50),
	@DOB_Prompt BIT,
	@Age_Prompt BIT,
	@SchoolGrade_Prompt BIT,
	@FirstName_Prompt BIT,
	@MiddleName_Prompt BIT,
	@LastName_Prompt BIT,
	@Gender_Prompt BIT,
	@EmailAddress_Prompt BIT,
	@PhoneNumber_Prompt BIT,
	@StreetAddress1_Prompt BIT,
	@StreetAddress2_Prompt BIT,
	@City_Prompt BIT,
	@State_Prompt BIT,
	@ZipCode_Prompt BIT,
	@Country_Prompt BIT,
	@County_Prompt BIT,
	@ParentGuardianFirstName_Prompt BIT,
	@ParentGuardianLastName_Prompt BIT,
	@ParentGuardianMiddleName_Prompt BIT,
	@PrimaryLibrary_Prompt BIT,
	@LibraryCard_Prompt BIT,
	@SchoolName_Prompt BIT,
	@District_Prompt BIT,
	@Teacher_Prompt BIT,
	@GroupTeamName_Prompt BIT,
	@SchoolType_Prompt BIT,
	@LiteracyLevel1_Prompt BIT,
	@LiteracyLevel2_Prompt BIT,
	@ParentPermFlag_Prompt BIT,
	@Over18Flag_Prompt BIT,
	@ShareFlag_Prompt BIT,
	@TermsOfUseflag_Prompt BIT,
	@Custom1_Prompt BIT,
	@Custom2_Prompt BIT,
	@Custom3_Prompt BIT,
	@Custom4_Prompt BIT,
	@Custom5_Prompt BIT,
	@DOB_Req BIT,
	@Age_Req BIT,
	@SchoolGrade_Req BIT,
	@FirstName_Req BIT,
	@MiddleName_Req BIT,
	@LastName_Req BIT,
	@Gender_Req BIT,
	@EmailAddress_Req BIT,
	@PhoneNumber_Req BIT,
	@StreetAddress1_Req BIT,
	@StreetAddress2_Req BIT,
	@City_Req BIT,
	@State_Req BIT,
	@ZipCode_Req BIT,
	@Country_Req BIT,
	@County_Req BIT,
	@ParentGuardianFirstName_Req BIT,
	@ParentGuardianLastName_Req BIT,
	@ParentGuardianMiddleName_Req BIT,
	@PrimaryLibrary_Req BIT,
	@LibraryCard_Req BIT,
	@SchoolName_Req BIT,
	@District_Req BIT,
	@Teacher_Req BIT,
	@GroupTeamName_Req BIT,
	@SchoolType_Req BIT,
	@LiteracyLevel1_Req BIT,
	@LiteracyLevel2_Req BIT,
	@ParentPermFlag_Req BIT,
	@Over18Flag_Req BIT,
	@ShareFlag_Req BIT,
	@TermsOfUseflag_Req BIT,
	@Custom1_Req BIT,
	@Custom2_Req BIT,
	@Custom3_Req BIT,
	@Custom4_Req BIT,
	@Custom5_Req BIT,
	@DOB_Show BIT,
	@Age_Show BIT,
	@SchoolGrade_Show BIT,
	@FirstName_Show BIT,
	@MiddleName_Show BIT,
	@LastName_Show BIT,
	@Gender_Show BIT,
	@EmailAddress_Show BIT,
	@PhoneNumber_Show BIT,
	@StreetAddress1_Show BIT,
	@StreetAddress2_Show BIT,
	@City_Show BIT,
	@State_Show BIT,
	@ZipCode_Show BIT,
	@Country_Show BIT,
	@County_Show BIT,
	@ParentGuardianFirstName_Show BIT,
	@ParentGuardianLastName_Show BIT,
	@ParentGuardianMiddleName_Show BIT,
	@PrimaryLibrary_Show BIT,
	@LibraryCard_Show BIT,
	@SchoolName_Show BIT,
	@District_Show BIT,
	@Teacher_Show BIT,
	@GroupTeamName_Show BIT,
	@SchoolType_Show BIT,
	@LiteracyLevel1_Show BIT,
	@LiteracyLevel2_Show BIT,
	@ParentPermFlag_Show BIT,
	@Over18Flag_Show BIT,
	@ShareFlag_Show BIT,
	@TermsOfUseflag_Show BIT,
	@Custom1_Show BIT,
	@Custom2_Show BIT,
	@Custom3_Show BIT,
	@Custom4_Show BIT,
	@Custom5_Show BIT,
	@DOB_Edit BIT,
	@Age_Edit BIT,
	@SchoolGrade_Edit BIT,
	@FirstName_Edit BIT,
	@MiddleName_Edit BIT,
	@LastName_Edit BIT,
	@Gender_Edit BIT,
	@EmailAddress_Edit BIT,
	@PhoneNumber_Edit BIT,
	@StreetAddress1_Edit BIT,
	@StreetAddress2_Edit BIT,
	@City_Edit BIT,
	@State_Edit BIT,
	@ZipCode_Edit BIT,
	@Country_Edit BIT,
	@County_Edit BIT,
	@ParentGuardianFirstName_Edit BIT,
	@ParentGuardianLastName_Edit BIT,
	@ParentGuardianMiddleName_Edit BIT,
	@PrimaryLibrary_Edit BIT,
	@LibraryCard_Edit BIT,
	@SchoolName_Edit BIT,
	@District_Edit BIT,
	@Teacher_Edit BIT,
	@GroupTeamName_Edit BIT,
	@SchoolType_Edit BIT,
	@LiteracyLevel1_Edit BIT,
	@LiteracyLevel2_Edit BIT,
	@ParentPermFlag_Edit BIT,
	@Over18Flag_Edit BIT,
	@ShareFlag_Edit BIT,
	@TermsOfUseflag_Edit BIT,
	@Custom1_Edit BIT,
	@Custom2_Edit BIT,
	@Custom3_Edit BIT,
	@Custom4_Edit BIT,
	@Custom5_Edit BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict_Prompt BIT,
	@SDistrict_Req BIT,
	@SDistrict_Show BIT,
	@SDistrict_Edit BIT,
	@Goal_Prompt BIT,
	@Goal_Req BIT,
	@Goal_Show BIT,
	@Goal_Edit BIT,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = ''
	)
AS
UPDATE RegistrationSettings
SET Literacy1Label = @Literacy1Label,
	Literacy2Label = @Literacy2Label,
	DOB_Prompt = @DOB_Prompt,
	Age_Prompt = @Age_Prompt,
	SchoolGrade_Prompt = @SchoolGrade_Prompt,
	FirstName_Prompt = @FirstName_Prompt,
	MiddleName_Prompt = @MiddleName_Prompt,
	LastName_Prompt = @LastName_Prompt,
	Gender_Prompt = @Gender_Prompt,
	EmailAddress_Prompt = @EmailAddress_Prompt,
	PhoneNumber_Prompt = @PhoneNumber_Prompt,
	StreetAddress1_Prompt = @StreetAddress1_Prompt,
	StreetAddress2_Prompt = @StreetAddress2_Prompt,
	City_Prompt = @City_Prompt,
	State_Prompt = @State_Prompt,
	ZipCode_Prompt = @ZipCode_Prompt,
	Country_Prompt = @Country_Prompt,
	County_Prompt = @County_Prompt,
	ParentGuardianFirstName_Prompt = @ParentGuardianFirstName_Prompt,
	ParentGuardianLastName_Prompt = @ParentGuardianLastName_Prompt,
	ParentGuardianMiddleName_Prompt = @ParentGuardianMiddleName_Prompt,
	PrimaryLibrary_Prompt = @PrimaryLibrary_Prompt,
	LibraryCard_Prompt = @LibraryCard_Prompt,
	SchoolName_Prompt = @SchoolName_Prompt,
	District_Prompt = @District_Prompt,
	Teacher_Prompt = @Teacher_Prompt,
	GroupTeamName_Prompt = @GroupTeamName_Prompt,
	SchoolType_Prompt = @SchoolType_Prompt,
	LiteracyLevel1_Prompt = @LiteracyLevel1_Prompt,
	LiteracyLevel2_Prompt = @LiteracyLevel2_Prompt,
	ParentPermFlag_Prompt = @ParentPermFlag_Prompt,
	Over18Flag_Prompt = @Over18Flag_Prompt,
	ShareFlag_Prompt = @ShareFlag_Prompt,
	TermsOfUseflag_Prompt = @TermsOfUseflag_Prompt,
	Custom1_Prompt = @Custom1_Prompt,
	Custom2_Prompt = @Custom2_Prompt,
	Custom3_Prompt = @Custom3_Prompt,
	Custom4_Prompt = @Custom4_Prompt,
	Custom5_Prompt = @Custom5_Prompt,
	DOB_Req = @DOB_Req,
	Age_Req = @Age_Req,
	SchoolGrade_Req = @SchoolGrade_Req,
	FirstName_Req = @FirstName_Req,
	MiddleName_Req = @MiddleName_Req,
	LastName_Req = @LastName_Req,
	Gender_Req = @Gender_Req,
	EmailAddress_Req = @EmailAddress_Req,
	PhoneNumber_Req = @PhoneNumber_Req,
	StreetAddress1_Req = @StreetAddress1_Req,
	StreetAddress2_Req = @StreetAddress2_Req,
	City_Req = @City_Req,
	State_Req = @State_Req,
	ZipCode_Req = @ZipCode_Req,
	Country_Req = @Country_Req,
	County_Req = @County_Req,
	ParentGuardianFirstName_Req = @ParentGuardianFirstName_Req,
	ParentGuardianLastName_Req = @ParentGuardianLastName_Req,
	ParentGuardianMiddleName_Req = @ParentGuardianMiddleName_Req,
	PrimaryLibrary_Req = @PrimaryLibrary_Req,
	LibraryCard_Req = @LibraryCard_Req,
	SchoolName_Req = @SchoolName_Req,
	District_Req = @District_Req,
	Teacher_Req = @Teacher_Req,
	GroupTeamName_Req = @GroupTeamName_Req,
	SchoolType_Req = @SchoolType_Req,
	LiteracyLevel1_Req = @LiteracyLevel1_Req,
	LiteracyLevel2_Req = @LiteracyLevel2_Req,
	ParentPermFlag_Req = @ParentPermFlag_Req,
	Over18Flag_Req = @Over18Flag_Req,
	ShareFlag_Req = @ShareFlag_Req,
	TermsOfUseflag_Req = @TermsOfUseflag_Req,
	Custom1_Req = @Custom1_Req,
	Custom2_Req = @Custom2_Req,
	Custom3_Req = @Custom3_Req,
	Custom4_Req = @Custom4_Req,
	Custom5_Req = @Custom5_Req,
	DOB_Show = @DOB_Show,
	Age_Show = @Age_Show,
	SchoolGrade_Show = @SchoolGrade_Show,
	FirstName_Show = @FirstName_Show,
	MiddleName_Show = @MiddleName_Show,
	LastName_Show = @LastName_Show,
	Gender_Show = @Gender_Show,
	EmailAddress_Show = @EmailAddress_Show,
	PhoneNumber_Show = @PhoneNumber_Show,
	StreetAddress1_Show = @StreetAddress1_Show,
	StreetAddress2_Show = @StreetAddress2_Show,
	City_Show = @City_Show,
	State_Show = @State_Show,
	ZipCode_Show = @ZipCode_Show,
	Country_Show = @Country_Show,
	County_Show = @County_Show,
	ParentGuardianFirstName_Show = @ParentGuardianFirstName_Show,
	ParentGuardianLastName_Show = @ParentGuardianLastName_Show,
	ParentGuardianMiddleName_Show = @ParentGuardianMiddleName_Show,
	PrimaryLibrary_Show = @PrimaryLibrary_Show,
	LibraryCard_Show = @LibraryCard_Show,
	SchoolName_Show = @SchoolName_Show,
	District_Show = @District_Show,
	Teacher_Show = @Teacher_Show,
	GroupTeamName_Show = @GroupTeamName_Show,
	SchoolType_Show = @SchoolType_Show,
	LiteracyLevel1_Show = @LiteracyLevel1_Show,
	LiteracyLevel2_Show = @LiteracyLevel2_Show,
	ParentPermFlag_Show = @ParentPermFlag_Show,
	Over18Flag_Show = @Over18Flag_Show,
	ShareFlag_Show = @ShareFlag_Show,
	TermsOfUseflag_Show = @TermsOfUseflag_Show,
	Custom1_Show = @Custom1_Show,
	Custom2_Show = @Custom2_Show,
	Custom3_Show = @Custom3_Show,
	Custom4_Show = @Custom4_Show,
	Custom5_Show = @Custom5_Show,
	DOB_Edit = @DOB_Edit,
	Age_Edit = @Age_Edit,
	SchoolGrade_Edit = @SchoolGrade_Edit,
	FirstName_Edit = @FirstName_Edit,
	MiddleName_Edit = @MiddleName_Edit,
	LastName_Edit = @LastName_Edit,
	Gender_Edit = @Gender_Edit,
	EmailAddress_Edit = @EmailAddress_Edit,
	PhoneNumber_Edit = @PhoneNumber_Edit,
	StreetAddress1_Edit = @StreetAddress1_Edit,
	StreetAddress2_Edit = @StreetAddress2_Edit,
	City_Edit = @City_Edit,
	State_Edit = @State_Edit,
	ZipCode_Edit = @ZipCode_Edit,
	Country_Edit = @Country_Edit,
	County_Edit = @County_Edit,
	ParentGuardianFirstName_Edit = @ParentGuardianFirstName_Edit,
	ParentGuardianLastName_Edit = @ParentGuardianLastName_Edit,
	ParentGuardianMiddleName_Edit = @ParentGuardianMiddleName_Edit,
	PrimaryLibrary_Edit = @PrimaryLibrary_Edit,
	LibraryCard_Edit = @LibraryCard_Edit,
	SchoolName_Edit = @SchoolName_Edit,
	District_Edit = @District_Edit,
	Teacher_Edit = @Teacher_Edit,
	GroupTeamName_Edit = @GroupTeamName_Edit,
	SchoolType_Edit = @SchoolType_Edit,
	LiteracyLevel1_Edit = @LiteracyLevel1_Edit,
	LiteracyLevel2_Edit = @LiteracyLevel2_Edit,
	ParentPermFlag_Edit = @ParentPermFlag_Edit,
	Over18Flag_Edit = @Over18Flag_Edit,
	ShareFlag_Edit = @ShareFlag_Edit,
	TermsOfUseflag_Edit = @TermsOfUseflag_Edit,
	Custom1_Edit = @Custom1_Edit,
	Custom2_Edit = @Custom2_Edit,
	Custom3_Edit = @Custom3_Edit,
	Custom4_Edit = @Custom4_Edit,
	Custom5_Edit = @Custom5_Edit,
	SDistrict_Prompt = @SDistrict_Prompt,
	SDistrict_Req = @SDistrict_Req,
	SDistrict_Show = @SDistrict_Show,
	SDistrict_Edit = @SDistrict_Edit,
	Goal_Prompt = @Goal_Prompt,
	Goal_Req = @Goal_Req,
	Goal_Show = @Goal_Show,
	Goal_Edit = @Goal_Edit,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LastModDate = @LastModDate,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE RID = @RID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_Survey_Insert]...';
GO

--Create the Insert Proc
ALTER PROCEDURE [dbo].[app_Survey_Insert] (
	@Name VARCHAR(50),
	@LongName VARCHAR(150),
	@Description TEXT,
	@Preamble TEXT,
	@Status INT,
	@TakenCount INT,
	@PatronCount INT,
	@CanBeScored BIT,
	@TenID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@BadgeId INT,
	@SID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Survey (
		NAME,
		LongName,
		Description,
		Preamble,
		STATUS,
		TakenCount,
		PatronCount,
		CanBeScored,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		BadgeId
		)
	VALUES (
		@Name,
		@LongName,
		@Description,
		@Preamble,
		@Status,
		@TakenCount,
		@PatronCount,
		@CanBeScored,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@BadgeId
		)

	SELECT @SID = SCOPE_IDENTITY()
END
GO

PRINT N'Altering [dbo].[app_Survey_Update]...';
GO

--Create the Update Proc
ALTER PROCEDURE [dbo].[app_Survey_Update] (
	@SID INT,
	@Name VARCHAR(50),
	@LongName VARCHAR(150),
	@Description TEXT,
	@Preamble TEXT,
	@Status INT,
	@TakenCount INT,
	@PatronCount INT,
	@CanBeScored BIT,
	@TenID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@BadgeId INT
	)
AS
UPDATE Survey
SET NAME = @Name,
	LongName = @LongName,
	Description = @Description,
	Preamble = @Preamble,
	STATUS = @Status,
	TakenCount = @TakenCount,
	PatronCount = @PatronCount,
	CanBeScored = @CanBeScored,
	TenID = @TenID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3,
	BadgeId = @BadgeId
WHERE SID = @SID
	AND TenID = @TenID
GO

PRINT N'Altering [dbo].[app_MGMatchingGame_GetRandomPlayItems]...';
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetRandomPlayItems]    Script Date: 01/05/2015 14:43:21 ******/
ALTER PROCEDURE [dbo].[app_MGMatchingGame_GetRandomPlayItems] @MAGID INT,
	@NumItems INT,
	@Difficulty INT
AS
DECLARE @SQL VARCHAR(8000)

CREATE TABLE #Temp1 (
	[ID] UNIQUEIDENTIFIER,
	[MAGTID] [int],
	[MAGID] [int],
	[MGID] [int],
	[Tile1UseMedium] [bit],
	[Tile1UseHard] [bit],
	[Tile2UseMedium] [bit],
	[Tile2UseHard] [bit],
	[Tile3UseMedium] [bit],
	[Tile3UseHard] [bit]
	)

CREATE TABLE #Temp2 (
	[MAGTID] [int],
	TileImage VARCHAR(255)
	)

SELECT @SQL = 'insert into #Temp1
	select top ' + convert(VARCHAR, @NumItems) + ' NEWID() as ID,
		[MAGTID], [MAGID], [MGID], [Tile1UseMedium], [Tile1UseHard], [Tile2UseMedium], [Tile2UseHard], [Tile3UseMedium],[Tile3UseHard]   from  dbo.MGMatchingGameTiles Where MAGID = ' + convert(VARCHAR, @MAGID) + '  order by id'

EXEC (@SQL)

--select * from #Temp1
INSERT INTO #Temp2
SELECT MAGTID,
	('t1_' + CONVERT(VARCHAR, MAGTID) + '.png') AS TileImage
FROM #Temp1

INSERT INTO #Temp2
SELECT MAGTID,
	(
		CASE 
			WHEN @Difficulty = 1
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 2
				AND Tile1UseMedium = 1
				AND Tile2UseMedium = 0
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 2
				AND Tile2UseMedium = 1
				THEN 't2_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile2UseHard = 0
				AND Tile3UseHard = 0
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile2UseHard = 1
				AND Tile3UseHard = 0
				THEN 't2_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile3UseHard = 1
				THEN 't3_' + CONVERT(VARCHAR, MAGTID) + '.png'
			END
		) AS TileImage
FROM #Temp1

SELECT NEWID() AS ID,
	*
FROM #Temp2
ORDER BY ID

DROP TABLE #Temp1

DROP TABLE #Temp2
GO

PRINT N'Altering [dbo].[GetPatronsPaged]...';
GO

ALTER PROCEDURE [dbo].[GetPatronsPaged] (
	@startRowIndex INT = 0,
	@maximumRows INT = 0,
	@sortString VARCHAR(200) = 'p.PID desc',
	@searchFirstName VARCHAR(50) = '',
	@searchLastName VARCHAR(50) = '',
	@searchUsername VARCHAR(50) = '',
	@searchEmail VARCHAR(128) = '',
	@searchDOB DATETIME = NULL,
	@searchProgram INT = 0,
	@searchGender VARCHAR(2) = '',
	@searchLibraryId INT = 0,
	@searchLibraryDistrictId INT = 0,
	@TenID INT = NULL
	)
AS
DECLARE @SQL1 VARCHAR(8000)

IF LEN(@sortString) = 0
	SET @sortString = 'p.PID'

DECLARE @Filter VARCHAR(8000)

SELECT @Filter = ''

IF @searchFirstName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' FirstName like ''%' + replace(@searchFirstName, '''', '''''') + '%'' '

IF @searchLastName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' LastName like ''%' + replace(@searchLastName, '''', '''''') + '%'' '

IF @searchUsername <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Username like ''%' + replace(@searchUsername, '''', '''''') + '%'' '

IF @searchEmail <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' EmailAddress like ''%' + replace(@searchEmail, '''', '''''') + '%'' '

IF @searchDOB IS NOT NULL
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' DOB = ''' + convert(VARCHAR, @searchDOB, 101) + ''' '

IF @searchProgram <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' ProgID = ' + CONVERT(VARCHAR, @searchProgram) + ' '

IF @searchGender <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Gender like ''%' + @searchGender + '%'' '

IF @searchLibraryId <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' PrimaryLibrary = ' + CONVERT(VARCHAR, @searchLibraryId) + ' '

IF @searchLibraryDistrictId <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' District = ' + CONVERT(VARCHAR, @searchLibraryDistrictId) + ' '

SELECT @Filter = @Filter + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND '
		END + ' p.TenID = ' + convert(VARCHAR, @TenID) + ' '

SELECT @SQL1 = 'SELECT  PID, FirstName, LastName, DOB, Username, EmailAddress, Gender, Program, ProgId, Branch
FROM
(
Select p.*, c.[Code] as [Branch], pg.AdminName as Program
, ROW_NUMBER() OVER (ORDER BY ' + @sortString + ' ) AS RowRank
FROM Patron p
left outer join [Code] c on p.[PrimaryLibrary] = c.[CID]
left outer join Programs pg
on p.ProgID = pg.PID
WHERE (c.[CTID] = 1 OR c.[CTID] is NULL)
' + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND ' + @Filter
		END + '
) AS p
WHERE RowRank > ' + convert(VARCHAR, @startRowIndex) + ' AND RowRank <= (' + convert(VARCHAR, @startRowIndex) + ' + ' + convert(VARCHAR, @maximumRows) + ') ' + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND ' + @Filter
		END

--select @SQL1
EXEC (@SQL1)
GO

PRINT N'Altering [dbo].[GetTotalPatrons]...';
GO

ALTER PROCEDURE [dbo].[GetTotalPatrons] (
	@startRowIndex INT = 0,
	@maximumRows INT = 0,
	@sortString VARCHAR(200) = 'p.PID desc',
	@searchFirstName VARCHAR(50) = '',
	@searchLastName VARCHAR(50) = '',
	@searchUsername VARCHAR(50) = '',
	@searchEmail VARCHAR(128) = '',
	@searchDOB DATETIME = NULL,
	@searchProgram INT = 0,
	@searchGender VARCHAR(2) = '',
	@searchLibraryId INT = 0,
	@searchLibraryDistrictId INT = 0,
	@TenID INT = NULL
	)
AS
DECLARE @SQL1 VARCHAR(8000)

IF LEN(@sortString) = 0
	SET @sortString = 'p.PID'

DECLARE @Filter VARCHAR(8000)

SELECT @Filter = ''

IF @searchFirstName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' FirstName like ''%' + replace(@searchFirstName, '''', '''''') + '%'' '

IF @searchLastName <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' LastName like ''%' + replace(@searchLastName, '''', '''''') + '%'' '

IF @searchUsername <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Username like ''%' + replace(@searchUsername, '''', '''''') + '%'' '

IF @searchEmail <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' EmailAddress like ''%' + replace(@searchEmail, '''', '''''') + '%'' '

IF @searchDOB IS NOT NULL
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' DOB = ''' + convert(VARCHAR, @searchDOB, 101) + ''' '

IF @searchDOB IS NOT NULL
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' DOB = ''' + convert(VARCHAR, @searchDOB, 101) + ''' '

IF @searchProgram <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' ProgID = ' + CONVERT(VARCHAR, @searchProgram) + ' '

IF @searchGender <> ''
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' Gender like ''%' + @searchGender + '%'' '

IF @searchLibraryId <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' PrimaryLibrary = ' + CONVERT(VARCHAR, @searchLibraryId) + ' '

IF @searchLibraryDistrictId <> 0
	SELECT @Filter = @Filter + CASE len(@Filter)
			WHEN 0
				THEN ''
			ELSE ' AND '
			END + ' District = ' + CONVERT(VARCHAR, @searchLibraryDistrictId) + ' '

SELECT @Filter = @Filter + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND '
		END + ' TenID = ' + convert(VARCHAR, @TenID) + ' '

SELECT @SQL1 = 'SELECT  count(*)
FROM Patron p ' + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' WHERE ' + @Filter
		END

EXEC (@SQL1)
	--select @SQL1
GO

PRINT N'Creating [dbo].[app_AvatarPart_Delete]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_Delete] @APID INT,
	@TenID INT = NULL
AS
DELETE
FROM [AvatarPart]
WHERE APID = @APID
	AND TenID = @TenID
GO

PRINT N'Creating [dbo].[app_AvatarPart_GetAll]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [AvatarPart]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY Ordering
GO

PRINT N'Creating [dbo].[app_AvatarPart_GetByID]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_GetByID] @APID INT
AS
SELECT *
FROM [AvatarPart]
WHERE APID = @APID
GO

PRINT N'Creating [dbo].[app_AvatarPart_GetQualifiedByPatron]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_GetQualifiedByPatron] @PID INT = NULL
AS
SELECT a.*
FROM [PatronBadges] pb
INNER JOIN AvatarPart a ON pb.BadgeID = a.BadgeID
WHERE pb.PID = @PID

UNION ALL

SELECT a.*
FROM [AvatarPart] a
WHERE a.BadgeID = - 1
ORDER BY Ordering
GO

PRINT N'Creating [dbo].[app_AvatarPart_Insert]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_Insert] (
	@Name VARCHAR(50),
	@Gender VARCHAR(1),
	@ComponentID INT = 0,
	@BadgeID INT = 0,
	@Ordering INT = 0,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@APID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO AvatarPart (
		NAME,
		Gender,
		ComponentID,
		BadgeID,
		Ordering,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID
		)
	VALUES (
		@Name,
		@Gender,
		@ComponentID,
		@BadgeID,
		@Ordering,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID
		)

	SELECT @APID = SCOPE_IDENTITY()

	SELECT @APID
END
GO

PRINT N'Creating [dbo].[app_AvatarPart_Update]...';
GO

CREATE PROCEDURE [dbo].[app_AvatarPart_Update] (
	@Name VARCHAR(50),
	@Gender VARCHAR(1),
	@ComponentID INT = 0,
	@BadgeID INT = 0,
	@Ordering INT = 0,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@TenID INT = 0,
	@APID INT OUTPUT
	)
AS
UPDATE AvatarPart
SET NAME = @Name,
	Gender = @Gender,
	ComponentID = @ComponentID,
	BadgeID = @BadgeID,
	Ordering = @Ordering,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	TenID = @TenID
WHERE APID = @APID
	AND TenID = @TenID
GO

PRINT N'Creating [dbo].[app_Award_Filter]...';
GO

CREATE PROCEDURE [dbo].[app_Award_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	SELECT a.*,
		ISNULL(b.[AdminName], '') AS [BadgeName],
		ISNULL(cbranch.[Code], '') AS [Branch],
		ISNULL(p.[AdminName], '') AS [Program],
		ISNULL(cdistrict.[Code], '') AS [DistrictName],
		ISNULL(cschool.[Code], '') AS [SchName]
	FROM [Award] a
	LEFT JOIN [Badge] b ON b.[BID] = a.[BadgeID]
	LEFT JOIN [Code] cbranch ON cbranch.[CID] = a.[BranchID]
	LEFT JOIN [Programs] p ON p.[PID] = a.[ProgramID]
	LEFT JOIN [Code] cdistrict ON cdistrict.[CID] = a.[District]
	LEFT JOIN [Code] cschool ON cschool.[CID] = a.[SchoolName]
	WHERE (
			a.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				a.[AwardName] LIKE @SearchText
				OR a.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			a.[BranchID] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY a.[AwardName]
END
GO

PRINT N'Creating [dbo].[app_Badge_Filter]...';
GO

CREATE PROCEDURE [dbo].[app_Badge_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	IF @BranchId IS NULL
	BEGIN
		SELECT b.*
		FROM [Badge] b
		WHERE (
				b.[TenID] = @TenID
				OR @TenID IS NULL
				)
			AND (
				(
					b.[AdminName] LIKE @SearchText
					OR b.[UserName] LIKE @SearchText
					OR b.[AddedUser] LIKE @SearchText
					)
				OR @SearchText IS NULL
				)
		ORDER BY b.[AdminName]
	END
	ELSE
	BEGIN
		SELECT b.*
		FROM [Badge] b
		LEFT OUTER JOIN [BadgeBranch] bb ON b.[BID] = bb.[BID]
		WHERE (
				b.[TenID] = @TenID
				OR @TenID IS NULL
				)
			AND (
				(
					b.[AdminName] LIKE @SearchText
					OR b.[UserName] LIKE @SearchText
					OR b.[AddedUser] LIKE @SearchText
					)
				OR @SearchText IS NULL
				)
			AND (
				bb.[CID] = @BranchID
				OR @BranchID IS NULL
				)
		ORDER BY b.[AdminName]
	END
END
GO

PRINT N'Creating [dbo].[app_Badge_GetBadgeGoals]...';
GO

CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGoals] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.AwardName
			ELSE @List + ', ' + p.AwardName
			END, '')
FROM Award p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
	AND p.GoalPercent > 0
GROUP BY p.AwardName
GO

PRINT N'Creating [dbo].[app_Badge_GetVisibleCount]...';
GO

CREATE PROCEDURE [dbo].[app_Badge_GetVisibleCount] @TenID INT = NULL
AS
SELECT COUNT(BID)
FROM [Badge]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
	AND [HiddenFromPublic] != 1
GO

PRINT N'Creating [dbo].[app_BookList_Filter]...';
GO

CREATE PROCEDURE [dbo].[app_BookList_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	SELECT bl.*,
		(
			SELECT COUNT(BLBID)
			FROM [BookListBooks] blb
			WHERE blb.[BLID] = bl.[BLID]
			) AS TotalTasks,
		ISNULL(p.[AdminName], '') AS [ProgName],
		ISNULL(c.[Code], '') AS [Library]
	FROM [BookList] bl
	LEFT JOIN [Code] c ON bl.[LibraryID] = c.[CID]
	LEFT JOIN [Programs] p ON bl.[TenID] = p.[TenID]
		AND bl.[ProgID] = p.[PID]
	WHERE (
			bl.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				bl.[ListName] LIKE @SearchText
				OR bl.[AdminName] LIKE @SearchText
				OR bl.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			bl.[LibraryID] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY bl.[AdminName]
END
GO

PRINT N'Creating [dbo].[app_Event_Export]...';
GO

/****** Object:  StoredProcedure [dbo].[app_Event_Export]    Script Date: 3/14/2016 11:11:41 ******/
CREATE PROCEDURE [dbo].[app_Event_Export] @TenID INT = NULL
AS
SELECT e.[EventTitle] AS [Name],
	e.[EventDate] AS [Date],
	e.[HTML] AS [Description],
	e.[SecretCode],
	e.[NumberPoints] AS [PointsEarned],
	e.[ExternalLinkToEvent] AS [Link],
	e.[HiddenFromPublic],
	c.[Code] AS [Branch]
FROM [Event] e
LEFT OUTER JOIN [Code] c ON e.[BranchID] = c.[CID]
WHERE (
		e.[TenID] = @TenID
		OR @TenID IS NULL
		)
ORDER BY e.[EventDate]
GO

PRINT N'Creating [dbo].[app_Event_Filter]...';
GO

CREATE PROCEDURE [dbo].[app_Event_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	SELECT e.*,
		c.[Code] AS [Branch]
	FROM [Event] e
	LEFT JOIN [Code] c ON e.[BranchID] = c.[CID]
	WHERE (
			e.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				e.[EventTitle] LIKE @SearchText
				OR e.[HTML] LIKE @SearchText
				OR e.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			e.[BranchId] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY [EventTitle]
END
GO

PRINT N'Creating [dbo].[app_LibraryCrosswalk_Export]...';
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Export] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT bc.[Code] AS [Branch],
		dc.[Code] AS [LibraryDistrict],
		lcw.[BranchLink] AS [Link],
		lcw.[BranchAddress] AS [Address],
		lcw.[BranchTelephone] AS [Telephone]
	FROM librarycrosswalk lcw
	INNER JOIN code bc ON lcw.[BranchId] = bc.[CID]
	INNER JOIN code dc ON lcw.[DistrictId] = dc.[CID]
	WHERE lcw.[TenID] = @TenID
	ORDER BY dc.[Code],
		bc.[Code]
END
GO

IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = object_id(N'[dbo].[app_Notifications_GetAllToOrFromPatron]')
			AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
BEGIN
	DROP PROCEDURE [dbo].[app_Notifications_GetAllToOrFromPatron]
END
GO

PRINT N'Creating [dbo].[app_Notifications_GetAllToOrFromPatron]...';
GO

CREATE PROCEDURE [dbo].[app_Notifications_GetAllToOrFromPatron] @PID INT
AS
SELECT n.*,
	isnull(p1.Username, 'System') AS ToUsername,
	isnull(p1.FirstName, '') AS ToFirstName,
	isnull(p1.LastName, '') AS ToLastName,
	isnull(p2.Username, 'System') AS FromUsername,
	isnull(p2.FirstName, '') AS FromFirstName,
	isnull(p2.LastName, '') AS FromLastName
FROM [Notifications] n
LEFT JOIN Patron p1 ON n.PID_To = p1.pid
LEFT JOIN Patron p2 ON n.PID_From = p2.pid
WHERE (
		PID_To = @PID
		OR PID_From = @PID
		)
ORDER BY AddedDate DESC
GO

PRINT N'Creating [dbo].[app_ProgramCodes_GetAllByTenantId]...';
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllByTenantId] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(pc.[PCID]) [CodeCount]
	FROM [ProgramCodes] pc
	INNER JOIN [Programs] p ON pc.[PID] = p.[PID]
		AND p.[TenID] = @TenID
END
GO

PRINT N'Creating [dbo].[app_SchoolCrosswalk_Export]...';
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Export] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT s.[Code] AS [SchoolName],
		st.[Code] AS [SchoolType],
		d.[Code] AS [DistrictName],
		NULLIF(scw.[MinGrade], 0) AS [MinGrade],
		NULLIF(scw.[MaxGrade], 0) AS [MaxGrade],
		NULLIF(scw.[MinAge], 0) AS [MinAge],
		NULLIF(scw.[MaxAge], 0) AS [MaxAge]
	FROM [schoolcrosswalk] scw
	INNER JOIN [code] s ON scw.[SchoolID] = s.[CID]
	INNER JOIN [code] st ON scw.[SchTypeID] = st.[CID]
	INNER JOIN [code] d ON scw.[DistrictID] = d.[CID]
	WHERE scw.[TenID] = @TenID
	ORDER BY d.[Code],
		st.[Code],
		s.[Code]
END
GO

PRINT N'Creating [dbo].[rpt_ProgramByBranch]...';
GO

CREATE PROCEDURE [dbo].[rpt_ProgramByBranch] @TenID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT pgm.[TabName] AS [Program],
		count(p.[pid]) AS [Signups],
		sum(CASE [dbo].[fx_IsFinisher2](p.PID, p.ProgID, NULL)
				WHEN 1
					THEN 1
				ELSE 0
				END) AS [Achievers],
		pgm.[CompletionPoints] AS [Achiever Points]
	FROM [Programs] pgm
	LEFT OUTER JOIN [patron] p ON p.[ProgID] = pgm.[PID]
	WHERE p.[TenId] = @TenID
	GROUP BY pgm.[TabName],
		pgm.[PID],
		pgm.[CompletionPoints]
	ORDER BY pgm.[PID]

	DECLARE @ProgramId INT

	DECLARE PGM_CURSOR CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT [PID]
	FROM [Programs]
	ORDER BY [PID]

	OPEN PGM_CURSOR

	FETCH NEXT
	FROM PGM_CURSOR
	INTO @ProgramId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT coalesce(s.[Description], 'No System') AS [Library System],
			coalesce(b.[description], 'No Branch') AS [Library],
			count(p.[pid]) AS [Signups],
			sum(CASE [dbo].[fx_IsFinisher2](p.PID, p.ProgID, NULL)
					WHEN 1
						THEN 1
					ELSE 0
					END) AS [Achievers]
		FROM [code] b
		INNER JOIN [librarycrosswalk] lxw ON lxw.[BranchId] = b.[CID]
		INNER JOIN [code] s ON lxw.[DistrictId] = s.[CID]
			AND s.[CTID] = 2
		LEFT OUTER JOIN [patron] p ON p.[PrimaryLibrary] = b.[CID]
			AND p.[ProgID] = @ProgramId
			AND p.[TenID] = @TenID
		WHERE b.[CTID] = 1
		GROUP BY s.[Description],
			b.[description]
		ORDER BY s.[Description],
			b.[description]

		FETCH NEXT
		FROM PGM_CURSOR
		INTO @ProgramId
	END

	CLOSE PGM_CURSOR

	DEALLOCATE PGM_CURSOR
END
GO

PRINT N'Creating [dbo].[rpt_TenantStatusReport]...';
GO

CREATE PROCEDURE [dbo].[rpt_TenantStatusReport] @TenID INT,
	@BranchId INT = NULL,
	@DistrictId INT = NULL,
	@ProgramId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RegisteredPatrons INT,
		@PointsEarned INT,
		@PointsEarnedReading INT,
		@ChallengesCompleted INT,
		@SecretCodesRedeemed INT,
		@AdventuresCompleted INT,
		@BadgesAwarded INT,
		@RedeemedProgramCodes INT
	DECLARE @Branches TABLE ([BranchId] INT)
	DECLARE @HasBranches BIT = 0

	IF @BranchId IS NULL
		AND @DistrictId IS NOT NULL
	BEGIN
		INSERT INTO @Branches
		SELECT [BranchId]
		FROM [LibraryCrosswalk]
		WHERE [DistrictID] = @DistrictId

		SET @HasBranches = 1
	END
	ELSE IF @BranchId IS NOT NULL
	BEGIN
		INSERT INTO @Branches
		VALUES (@BranchId)

		SET @HasBranches = 1
	END

	SELECT @RegisteredPatrons = count(PID)
	FROM [Patron] p
	WHERE p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @PointsEarned = sum(pp.[NumPoints]),
		@PointsEarnedReading = sum(CASE pp.[AwardReasonCd]
				WHEN 0
					THEN 1
				ELSE 0
				END),
		@ChallengesCompleted = sum(CASE pp.[IsBookList]
				WHEN 1
					THEN 1
				ELSE 0
				END),
		@SecretCodesRedeemed = sum(CASE pp.[isEvent]
				WHEN 1
					THEN 1
				ELSE 0
				END),
		@AdventuresCompleted = sum(CASE pp.[AwardReasonCd]
				WHEN 4
					THEN 1
				ELSE 0
				END)
	FROM [patronpoints] pp
	INNER JOIN [patron] p ON p.[pid] = pp.[pid]
		AND p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @BadgesAwarded = count(pbid)
	FROM [patronbadges] pb
	INNER JOIN [patron] p ON p.[pid] = pb.[pid]
		AND p.[tenid] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)

	SELECT @RedeemedProgramCodes = count(pc.[PCID])
	FROM [ProgramCodes] pc
	INNER JOIN [Patron] p ON p.[PID] = pc.[PatronId]
		AND p.[TenID] = @TenID
		AND (
			p.[ProgID] = @ProgramId
			OR @ProgramId IS NULL
			)
		AND (
			p.[PrimaryLibrary] IN (
				SELECT [BranchId]
				FROM @Branches
				)
			OR @HasBranches = 0
			)
	WHERE pc.[isUsed] = 1

	SELECT @RegisteredPatrons AS RegisteredPatrons,
		COALESCE(@PointsEarned, 0) AS PointsEarned,
		COALESCE(@PointsEarnedReading, 0) AS PointsEarnedReading,
		COALESCE(@ChallengesCompleted, 0) AS ChallengesCompleted,
		COALESCE(@SecretCodesRedeemed, 0) AS SecretCodesRedeemed,
		COALESCE(@AdventuresCompleted, 0) AS AdventuresCompleted,
		@BadgesAwarded AS BadgesAwarded,
		@RedeemedProgramCodes AS RedeemedProgramCodes
END
GO

PRINT N'Refreshing [dbo].[app_Award_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Award_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Award_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Award_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Award_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Award_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeReading]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeReading]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetEnrollmentPrograms]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetEnrollmentPrograms]';
GO

PRINT N'Refreshing [dbo].[app_Award_GetBadgeListMembership]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Award_GetBadgeListMembership]';
GO

PRINT N'Refreshing [dbo].[app_Badge_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetList]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetList]';
GO

PRINT N'Refreshing [dbo].[app_PatronBadges_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronBadges_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_PatronPrizes_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronPrizes_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_BookList_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookList_Delete]';
GO

PRINT N'Refreshing [dbo].[app_BookList_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookList_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_BookList_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookList_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_BookList_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookList_Insert]';
GO

PRINT N'Refreshing [dbo].[app_BookList_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookList_Update]';
GO

PRINT N'Refreshing [dbo].[app_PatronPoints_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronPoints_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetAllComplete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetAllComplete]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetSources]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetSources]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_Delete]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_GetForPatronDisplay]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_GetForPatronDisplay]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_Insert]';
GO

PRINT N'Refreshing [dbo].[app_BookListBooks_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_BookListBooks_Update]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeAgeGroups]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeAgeGroups]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeCategories]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeCategories]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeLocations]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeLocations]';
GO

PRINT N'Refreshing [dbo].[app_Badge_UpdateBadgeAgeGroups]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_UpdateBadgeAgeGroups]';
GO

PRINT N'Refreshing [dbo].[app_Badge_UpdateBadgeBranches]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_UpdateBadgeBranches]';
GO

PRINT N'Refreshing [dbo].[app_Badge_UpdateBadgeCategories]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_UpdateBadgeCategories]';
GO

PRINT N'Refreshing [dbo].[app_Badge_UpdateBadgeLocations]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_UpdateBadgeLocations]';
GO

PRINT N'Refreshing [dbo].[app_Code_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetAllTypeID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetAllTypeID]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetAllTypeName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetAllTypeName]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Event_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Event_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Event_GetEventList]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Event_GetEventList]';
GO

PRINT N'Refreshing [dbo].[app_LibraryCrosswalk_GetFilteredBranchDDValues]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_LibraryCrosswalk_GetFilteredBranchDDValues]';
GO

PRINT N'Refreshing [dbo].[app_LibraryCrosswalk_GetFilteredDistrictDDValues]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_LibraryCrosswalk_GetFilteredDistrictDDValues]';
GO

PRINT N'Refreshing [dbo].[app_Offer_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Offer_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_PrizeTemplate_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PrizeTemplate_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SchoolCrosswalk_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SchoolCrosswalk_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SchoolCrosswalk_GetFilteredSchoolDDValues]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SchoolCrosswalk_GetFilteredSchoolDDValues]';
GO

PRINT N'Refreshing [dbo].[rpt_PatronActivity]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_PatronActivity]';
GO

PRINT N'Refreshing [dbo].[app_CodeType_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_CodeType_Delete]';
GO

PRINT N'Refreshing [dbo].[app_CodeType_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_CodeType_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_CodeType_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_CodeType_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeEventIDS]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeEventIDS]';
GO

PRINT N'Refreshing [dbo].[app_Event_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Event_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Event_GetEventCountByEventCode]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Event_GetEventCountByEventCode]';
GO

PRINT N'Refreshing [dbo].[app_LibraryCrosswalk_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_LibraryCrosswalk_Delete]';
GO

PRINT N'Refreshing [dbo].[app_LibraryCrosswalk_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_LibraryCrosswalk_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_LibraryCrosswalk_GetByLibraryID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_LibraryCrosswalk_GetByLibraryID]';
GO

PRINT N'Refreshing [dbo].[app_Badge_GetBadgeGames]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Badge_GetBadgeGames]';
GO

PRINT N'Refreshing [dbo].[app_MGChooseAdv_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGChooseAdv_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGCodeBreaker_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGCodeBreaker_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGHiddenPic_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGHiddenPic_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGHiddenPicBk_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGHiddenPicBk_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGMatchingGame_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGMatchingGame_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGMixAndMatch_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGMixAndMatch_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGOnlineBook_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGOnlineBook_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_MGWordMatch_GetByIDWithParent]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_MGWordMatch_GetByIDWithParent]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_GetList]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_GetList]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_Insert]';
GO

PRINT N'Refreshing [dbo].[app_Minigame_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Minigame_Update]';
GO

PRINT N'Refreshing [dbo].[rpt_MiniGameStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_MiniGameStats]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_GetAllFromPatron]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_GetAllFromPatron]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_GetAllToPatron]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_GetAllToPatron]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_GetAllUnreadToPatron]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_GetAllUnreadToPatron]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_Insert]';
GO

PRINT N'Refreshing [dbo].[app_Notifications_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Notifications_Update]';
GO

PRINT N'Refreshing [dbo].[app_Patron_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Offer_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Offer_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Offer_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Offer_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Offer_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Offer_Insert]';
GO

PRINT N'Refreshing [dbo].[app_Offer_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Offer_Update]';
GO

PRINT N'Refreshing [dbo].[app_Patron_CheckIfFinisher]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_CheckIfFinisher]';
GO

PRINT N'Refreshing [dbo].[rpt_DashboardStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_DashboardStats]';
GO

PRINT N'Refreshing [dbo].[rpt_FinisherStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_FinisherStats]';
GO

PRINT N'Refreshing [dbo].[rpt_ReadingActivityReport]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_ReadingActivityReport]';
GO

PRINT N'Refreshing [dbo].[rpt_TenantReport]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_TenantReport]';
GO

PRINT N'Refreshing [dbo].[rpt_TenantSummaryReport]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_TenantSummaryReport]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetAllLibrarySystems]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetAllLibrarySystems]';
GO

PRINT N'Refreshing [dbo].[app_Code_GetAllSchools]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Code_GetAllSchools]';
GO

PRINT N'Refreshing [dbo].[app_Patron_CanManageSubAccount]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_CanManageSubAccount]';
GO

PRINT N'Refreshing [dbo].[app_Patron_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Patron_GetByEmail]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_GetByEmail]';
GO

PRINT N'Refreshing [dbo].[app_Patron_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Patron_GetByUsername]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_GetByUsername]';
GO

PRINT N'Refreshing [dbo].[app_Patron_GetScoreRank]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Patron_GetScoreRank]';
GO

PRINT N'Refreshing [dbo].[app_PrizeDrawing_DrawWinners]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PrizeDrawing_DrawWinners]';
GO

PRINT N'Refreshing [dbo].[app_ProgramCodes_GetExportList]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramCodes_GetExportList]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetAllActive]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetAllActive]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetAllOrdered]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetAllOrdered]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetAllTabs]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetAllTabs]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetAllStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetAllStats]';
GO

PRINT N'Refreshing [dbo].[rpt_GameLevelStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_GameLevelStats]';
GO

PRINT N'Refreshing [dbo].[rpt_PatronFilter]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_PatronFilter]';
GO

PRINT N'Refreshing [dbo].[rpt_PatronFilter_Expanded]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_PatronFilter_Expanded]';
GO

PRINT N'Refreshing [dbo].[rpt_PrizeRecipients]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_PrizeRecipients]';
GO

PRINT N'Refreshing [dbo].[rpt_RegistrationStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[rpt_RegistrationStats]';
GO

PRINT N'Refreshing [dbo].[app_PatronReadingLog_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReadingLog_Delete]';
GO

PRINT N'Refreshing [dbo].[app_PatronReadingLog_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReadingLog_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_PatronReadingLog_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReadingLog_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_PatronReadingLog_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReadingLog_Insert]';
GO

PRINT N'Refreshing [dbo].[app_PatronReadingLog_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReadingLog_Update]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_Delete]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_GetByLogID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_GetByLogID]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_Insert]';
GO

PRINT N'Refreshing [dbo].[app_PatronReview_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_PatronReview_Update]';
GO

PRINT N'Refreshing [dbo].[app_ProgramGame_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramGame_Delete]';
GO

PRINT N'Refreshing [dbo].[app_ProgramGame_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramGame_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_ProgramGame_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramGame_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_ProgramGame_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramGame_Insert]';
GO

PRINT N'Refreshing [dbo].[app_ProgramGame_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ProgramGame_Update]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Programs_GetDefaultProgramID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_GetDefaultProgramID]';
GO

PRINT N'Refreshing [dbo].[app_Programs_Reorder]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_Reorder]';
GO

PRINT N'Refreshing [dbo].[app_Survey_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Survey_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetByProgramID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetByProgramID]';
GO

PRINT N'Refreshing [dbo].[app_RegistrationSettings_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_RegistrationSettings_Delete]';
GO

PRINT N'Refreshing [dbo].[app_RegistrationSettings_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_RegistrationSettings_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_RegistrationSettings_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_RegistrationSettings_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_ReportTemplate_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ReportTemplate_Delete]';
GO

PRINT N'Refreshing [dbo].[app_ReportTemplate_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ReportTemplate_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_ReportTemplate_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ReportTemplate_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_ReportTemplate_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ReportTemplate_Insert]';
GO

PRINT N'Refreshing [dbo].[app_ReportTemplate_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_ReportTemplate_Update]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_DeleteAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_DeleteAll]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_Insert]';
GO

PRINT N'Refreshing [dbo].[app_SentEmailLog_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SentEmailLog_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_Delete]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_DeleteAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_DeleteAll]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_DeleteByPrimaryKey]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_DeleteByPrimaryKey]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_Get]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_Get]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_GetAll]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_GetByPrimaryKey]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_GetByPrimaryKey]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_Insert]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroups_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroups_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetAllPermissions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetAllPermissions]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetAllPermissionsAUDIT]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetAllPermissionsAUDIT]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetGroups]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetGroups]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetGroupsFlagged]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetGroupsFlagged]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetGroupsNonMembers]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetGroupsNonMembers]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_UpdateGroups]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_UpdateGroups]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetPermissions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetPermissions]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetPermissionsNotGranted]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetPermissionsNotGranted]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetUsers]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetUsers]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetUsersFlagged]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetUsersFlagged]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetUsersNonMembers]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetUsersNonMembers]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_ResetPassword]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_ResetPassword]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_Delete]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_DeleteByModule]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_DeleteByModule]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_Get]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_Get]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_GetByModule]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_GetByModule]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_GetByModuleName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_GetByModuleName]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_GetByName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_GetByName]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_Insert]';
GO

PRINT N'Refreshing [dbo].[cbspSRPPermissionsMaster_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPPermissionsMaster_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetSpecialUserPermissions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetSpecialUserPermissions]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_UpdateSpecialUserPermissions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_UpdateSpecialUserPermissions]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_GetPermissionsFlagged]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_GetPermissionsFlagged]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_UpdatePermissions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_UpdatePermissions]';
GO

PRINT N'Refreshing [dbo].[app_SRPReport_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPReport_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SRPReport_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPReport_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SRPReport_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPReport_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SRPReport_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPReport_Insert]';
GO

PRINT N'Refreshing [dbo].[app_SRPReport_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPReport_Update]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_GetByName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_GetByName]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_Insert]';
GO

PRINT N'Refreshing [dbo].[app_SRPSettings_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SRPSettings_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_DeleteAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_DeleteAll]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_DeleteByPrimaryKey]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_DeleteByPrimaryKey]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_GetAll]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_GetByName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_GetByName]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_GetByPrimaryKey]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_GetByPrimaryKey]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_Insert]';
GO

PRINT N'Refreshing [dbo].[cbspSRPSettings_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPSettings_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_Delete]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_EmailExists]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_EmailExists]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_Get]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_Get]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetActiveSessions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetActiveSessions]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetAll]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetByUsername]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetByUsername]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetLoginHistory]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetLoginHistory]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetLoginNow]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetLoginNow]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_GetLoginNowTenID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_GetLoginNowTenID]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_Insert]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_Login]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_Login]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_Update]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUser_UsernameExists]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUser_UsernameExists]';
GO

PRINT N'Refreshing [dbo].[cbspSRPUserGroups_UpdateUsers]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[cbspSRPUserGroups_UpdateUsers]';
GO

PRINT N'Refreshing [dbo].[app_Survey_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Survey_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Survey_GetAllFinalized]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Survey_GetAllFinalized]';
GO

PRINT N'Refreshing [dbo].[app_Survey_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Survey_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_GetAllExpanded]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_GetAllExpanded]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_Insert]';
GO

PRINT N'Refreshing [dbo].[app_SurveyAnswers_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyAnswers_Update]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetQClarifications]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetQClarifications]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetQFreeForm]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetQFreeForm]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetQStats]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetQStats]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetQStatsMedium]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetQStatsMedium]';
GO

PRINT N'Refreshing [dbo].[app_SurveyResults_GetQStatsSimple]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyResults_GetQStatsSimple]';
GO

PRINT N'Refreshing [dbo].[app_Survey_GetNumQuestions]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Survey_GetNumQuestions]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_GetPageFromQNum]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_GetPageFromQNum]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_Insert]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_Reorder]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_Reorder]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_Update]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetAll]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetAll]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetAllActive]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetAllActive]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetByDomainName]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetByDomainName]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetByID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetByID]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_GetMasterID]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_GetMasterID]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_Insert]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_Insert]';
GO

PRINT N'Refreshing [dbo].[app_Tenant_Update]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Tenant_Update]';
GO

PRINT N'Refreshing [dbo].[app_Programs_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_Delete]';
GO

PRINT N'Refreshing [dbo].[app_Programs_MoveDn]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_MoveDn]';
GO

PRINT N'Refreshing [dbo].[app_Programs_MoveUp]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_Programs_MoveUp]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_Delete]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_Delete]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_MoveDn]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_MoveDn]';
GO

PRINT N'Refreshing [dbo].[app_SurveyQuestion_MoveUp]...';
GO

EXECUTE sp_refreshsqlmodule N'[dbo].[app_SurveyQuestion_MoveUp]';
GO

-- Refactoring step to update target server with deployed transaction logs
SET NOCOUNT ON

IF OBJECT_ID(N'dbo.__RefactorLog') IS NULL
BEGIN
	CREATE TABLE [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)

	EXEC sp_addextendedproperty N'microsoft_database_tools_support',
		N'refactoring log',
		N'schema',
		N'dbo',
		N'table',
		N'__RefactorLog'
END
GO

SET NOCOUNT ON

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = '7db41d23-646b-46fb-9977-cd876db5a480'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('7db41d23-646b-46fb-9977-cd876db5a480')

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = 'bc8c806a-29fa-4231-be7f-3fda3b8f72e5'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('bc8c806a-29fa-4231-be7f-3fda3b8f72e5')

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = '1541420b-11fe-45f9-9cd0-a01fdb163d74'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('1541420b-11fe-45f9-9cd0-a01fdb163d74')

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = '791f345f-2f92-40ea-93f7-2c888b34449d'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('791f345f-2f92-40ea-93f7-2c888b34449d')

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = 'e0ab93b7-ff5e-429d-9bc8-4ae13bfff833'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('e0ab93b7-ff5e-429d-9bc8-4ae13bfff833')

IF NOT EXISTS (
		SELECT OperationKey
		FROM [dbo].[__RefactorLog]
		WHERE OperationKey = '9da23137-7445-412e-859d-f1a2eaea41e9'
		)
	INSERT INTO [dbo].[__RefactorLog] (OperationKey)
	VALUES ('9da23137-7445-412e-859d-f1a2eaea41e9')
GO


GO

PRINT N'Checking existing data against newly created constraints';
GO

SET NOCOUNT ON

ALTER TABLE [dbo].[ProgramCodes]
	WITH CHECK CHECK CONSTRAINT [FK_ProgramCodes_Programs];

ALTER TABLE [dbo].[ProgramGamePointConversion]
	WITH CHECK CHECK CONSTRAINT [FK_ProgramGamePointConversion_Programs];

PRINT N'Updating any existing Award Triggers...';
GO

SET NOCOUNT ON

UPDATE [dbo].[Award]
SET [BadgesAchieved] = CASE 
		WHEN len(rtrim(ltrim([BadgeList]))) = 0
			THEN NULL
		ELSE len(rtrim(ltrim([BadgeList]))) - len(replace(rtrim(ltrim([BadgeList])), ',', '')) + 1
		END
GO

PRINT N'Updating Programs...';
GO

SET NOCOUNT ON

UPDATE [dbo].[Programs]
SET [RequireBookDetails] = 0
GO

PRINT N'Updating Events...';

SET NOCOUNT ON

UPDATE [Event]
SET [HiddenFromPublic] = 0;
GO

PRINT N'Updating Badges...';

SET NOCOUNT ON

UPDATE [Badge]
SET [HiddenFromPublic] = 0;
GO

PRINT N'Updating Registration Settings...';
GO

SET NOCOUNT ON

UPDATE [RegistrationSettings]
SET [Goal_Prompt] = 0,
	[Goal_Req] = 0,
	[Goal_Show] = 0,
	[Goal_Edit] = 0
GO

PRINT N'Updating Permissions...';
GO

SET NOCOUNT ON

UPDATE [SRPPermissionsMaster]
SET [PermissionName] = 'Modify Program Permission'
WHERE [PermissionID] = 2200;

UPDATE [SRPPermissionsMaster]
SET [PermissionName] = 'Challenges Management'
WHERE [PermissionID] = 4400;

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5150,
	'Patron Notifications',
	'Allows access notifications to specific patrons.',
	NULL
	)

INSERT INTO SRPPermissionsMaster (
	[PermissionID],
	[PermissionName],
	[PermissionDesc],
	[MODID]
	)
VALUES (
	5400,
	'Import/Export Management',
	'Allows access to import/export of program management items.',
	NULL
	)

INSERT INTO [SRPGroupPermissions] (
	[GID],
	[PermissionID],
	[AddedDate],
	[AddedUser]
	)
VALUES (
	1000,
	5150,
	GETDATE(),
	'sysadmin'
	);

INSERT INTO [SRPGroupPermissions] (
	[GID],
	[PermissionID],
	[AddedDate],
	[AddedUser]
	)
VALUES (
	1000,
	5400,
	GETDATE(),
	'sysadmin'
	);

PRINT N'Updating Settings...';
GO

SET NOCOUNT ON

DELETE
FROM [SRPSettings]
WHERE [Name] = 'DupEvtCodes';
GO

SET NOCOUNT ON

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
	'CRLoginHtml',
	'<p class="lead">For information on setting up your summer reading program, you can visit the <a href="http://manual.greatreadingadventure.com/" target="_blank">manual</a>.</p>',
	'Text',
	'TextBox',
	0,
	'CR Login HTML',
	'HTML to show CR users upon login',
	'',
	'<p class="lead">For information on setting up your summer reading program, you can visit the <a href="http://manual.greatreadingadventure.com/" target="_blank">manual</a>.</p>',
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

PRINT N'Updating Avatars...'
GO

SET NOCOUNT ON
SET IDENTITY_INSERT [dbo].[AvatarPart] ON
GO

SET NOCOUNT ON

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	1,
	N'Bottom1',
	N'O',
	0,
	- 1,
	0,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	2,
	N'Bottom2',
	N'O',
	0,
	- 1,
	1,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	3,
	N'Bottom3',
	N'O',
	0,
	- 1,
	2,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	4,
	N'Bottom4',
	N'O',
	0,
	- 1,
	3,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	5,
	N'Bottom5',
	N'O',
	0,
	- 1,
	4,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	6,
	N'Top1',
	N'O',
	1,
	- 1,
	0,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	7,
	N'Top2',
	N'O',
	1,
	- 1,
	1,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	8,
	N'Top3',
	N'O',
	1,
	- 1,
	2,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	9,
	N'Top4',
	N'O',
	1,
	- 1,
	3,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	10,
	N'Top5',
	N'O',
	1,
	- 1,
	4,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	11,
	N'Top6',
	N'O',
	1,
	- 1,
	5,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	12,
	N'Top7',
	N'O',
	1,
	- 1,
	6,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	13,
	N'Top8',
	N'O',
	1,
	- 1,
	7,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	14,
	N'BoyHead',
	N'O',
	2,
	- 1,
	0,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	15,
	N'GirlHead',
	N'O',
	2,
	- 1,
	1,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	16,
	N'Man1Head',
	N'O',
	2,
	- 1,
	2,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	17,
	N'Man2Head',
	N'O',
	2,
	- 1,
	3,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	18,
	N'Man3Head',
	N'O',
	2,
	- 1,
	4,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	19,
	N'Woman1Head',
	N'O',
	2,
	- 1,
	5,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	20,
	N'Woman2Head',
	N'O',
	2,
	- 1,
	6,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)

INSERT [dbo].[AvatarPart] (
	[APID],
	[Name],
	[Gender],
	[ComponentID],
	[BadgeID],
	[Ordering],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
	[TenID]
	)
VALUES (
	21,
	N'Woman3Head',
	N'O',
	2,
	- 1,
	7,
	GETDATE(),
	N'sysadmin',
	GETDATE(),
	N'sysadmin',
	1
	)
GO

SET IDENTITY_INSERT [dbo].[AvatarPart] OFF
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
	SET NOCOUNT ON

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
		1,
		0,
		'Upgraded database'
		)
END
GO

COMMIT TRAN UPDATEGRA;

PRINT N'Update complete.';
GO



