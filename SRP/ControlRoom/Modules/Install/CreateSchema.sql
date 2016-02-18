/****** Object:  StoredProcedure [dbo].[app_Avatar_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Avatar_Delete] @AID INT,
	@TenID INT = NULL
AS
DELETE
FROM [Avatar]
WHERE AID = @AID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Avatar_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Avatar_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Avatar]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Avatar_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Avatar_GetByID] @AID INT
AS
SELECT *
FROM [Avatar]
WHERE AID = @AID
GO
/****** Object:  StoredProcedure [dbo].[app_Avatar_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Avatar_Insert] (
	@Name VARCHAR(50),
	@Gender VARCHAR(1),
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
	@AID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Avatar (
		NAME,
		Gender,
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
		FldText3
		)
	VALUES (
		@Name,
		@Gender,
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
		@FldText3
		)

	SELECT @AID = SCOPE_IDENTITY()

	SELECT @AID
END
GO
/****** Object:  StoredProcedure [dbo].[app_Avatar_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Avatar_Update] (
	@AID INT,
	@Name VARCHAR(50),
	@Gender VARCHAR(1),
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
	@FldText3 TEXT = ''
	)
AS
UPDATE Avatar
SET NAME = @Name,
	Gender = @Gender,
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
	FldText3 = @FldText3
WHERE AID = @AID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Award_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_Delete] @AID INT
AS
DELETE
FROM [Award]
WHERE AID = @AID
GO
/****** Object:  StoredProcedure [dbo].[app_Award_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_GetAll] @TenID INT = NULL
AS
SELECT *,
	isnull((
			SELECT AdminName
			FROM Badge
			WHERE BID = BadgeId
			), '') AS BadgeName,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = BranchID
			), '') AS Branch,
	isnull((
			SELECT AdminName
			FROM Programs
			WHERE PID = ProgramId
			), '') AS Program,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = District
			), '') AS DistrictName,
	isnull((
			SELECT Code
			FROM Code
			WHERE CID = SchoolName
			), '') AS SchName
FROM [Award]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AwardName
GO
/****** Object:  StoredProcedure [dbo].[app_Award_GetBadgeListMembership]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_GetBadgeListMembership] @BadgeList VARCHAR(500) = '',
	@TenID INT
AS
SELECT BID,
	AdminName,
	CASE 
		WHEN CHARINDEX(CONVERT(VARCHAR(10), BID) + ',', ',' + @BadgeList + ',') > 0
			THEN 1
		ELSE 0
		END AS isMember
FROM Badge
WHERE TenID = @TenID
ORDER BY AdminName,
	BID
GO
/****** Object:  StoredProcedure [dbo].[app_Award_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_GetByID] @AID INT
AS
SELECT *
FROM [Award]
WHERE AID = @AID
GO
/****** Object:  StoredProcedure [dbo].[app_Award_GetPatronQualifyingAwards]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwards] @PID INT = 0
AS
SELECT a.*,
	p.PID,
	ProgID,
	PrimaryLibrary,
	p.District,
	p.SchoolName,
	Points
FROM Award a
INNER JOIN (
	SELECT PID,
		ProgID,
		PrimaryLibrary,
		District,
		SchoolName,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS p ON p.TenID = a.TenID
	AND (
		a.ProgramID = p.ProgID
		OR a.ProgramID = 0
		)
	AND (
		a.BranchID = p.PrimaryLibrary
		OR a.BranchID = 0
		)
	AND (
		a.District = p.District
		OR a.District = ''
		)
	AND (
		a.SchoolName = p.SchoolName
		OR a.SchoolName = ''
		)
	AND (a.NumPoints <= p.Points)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronHasAllBadgesInList(p.PID, BadgeList) = 1
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Award_GetPatronQualifyingAwardsWTenant]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_GetPatronQualifyingAwardsWTenant] @PID INT = 0,
	@TenID INT = 1
AS
SELECT a.*,
	p.PID,
	ProgID,
	PrimaryLibrary,
	p.District,
	p.SchoolName,
	Points
FROM Award a
INNER JOIN (
	SELECT PID,
		ProgID,
		PrimaryLibrary,
		District,
		SchoolName,
		isnull((
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0)
				FROM PatronPoints pp
				WHERE pp.PID = pt.PID
				), 0) AS Points,
		@TenID AS TenID
	FROM Patron pt
	WHERE pt.PID = @PID
	) AS p ON p.TenID = a.TenID
	AND (
		a.ProgramID = p.ProgID
		OR a.ProgramID = 0
		)
	AND (
		a.BranchID = p.PrimaryLibrary
		OR a.BranchID = 0
		)
	AND (
		a.District = p.District
		OR a.District = ''
		)
	AND (
		a.SchoolName = p.SchoolName
		OR a.SchoolName = ''
		)
	AND (a.NumPoints <= p.Points)
	AND (
		BadgeList = ''
		OR dbo.fx_PatronHasAllBadgesInList(p.PID, BadgeList) = 1
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Award_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_Insert] (
	@AwardName VARCHAR(80),
	@BadgeID INT,
	@NumPoints INT,
	@BranchID INT,
	@ProgramID INT,
	@District VARCHAR(50),
	@SchoolName VARCHAR(50),
	@BadgeList VARCHAR(500),
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
		BadgeList,
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
		@BadgeList,
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
		@FldText3
		)

	SELECT @AID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Award_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Award_Update] (
	@AID INT,
	@AwardName VARCHAR(80),
	@BadgeID INT,
	@NumPoints INT,
	@BranchID INT,
	@ProgramID INT,
	@District VARCHAR(50),
	@SchoolName VARCHAR(50),
	@BadgeList VARCHAR(500),
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
	BadgeList = @BadgeList,
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
	FldText3 = @FldText3
WHERE AID = @AID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_Delete] @BID INT
AS
DELETE
FROM BadgeAgeGrp
WHERE BID = @BID

DELETE
FROM BadgeLocation
WHERE BID = @BID

DELETE
FROM BadgeCategory
WHERE BID = @BID

DELETE
FROM BadgeBranch
WHERE BID = @BID

DELETE
FROM [Badge]
WHERE BID = @BID
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Badge]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AdminName
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeAgeGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeAgeGroups] @BID INT,
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @CTID INT

SELECT @CTID = CTID
FROM dbo.CodeType
WHERE TenID = @TenID
	AND CodeTypeName = 'Badge Age Group'

SELECT @BID AS BID,
	c.CID,
	c.Code AS NAME, --c.CTID,
	CASE 
		WHEN bb.BID IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM (
	SELECT *
	FROM dbo.BadgeAgeGrp
	WHERE BID = @BID
	) AS bb
RIGHT JOIN (
	SELECT *
	FROM Code
	WHERE TenID = @TenID
		AND CTID = @CTID
	) AS c ON bb.CID = c.CID
ORDER BY c.Code
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeBookLists]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeBookLists] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.ListName
			ELSE @List + ', ' + p.ListName
			END, '')
FROM BookList p
WHERE p.TenID = @TenID
	AND p.AwardBadgeID = @BID
ORDER BY p.ListName
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeBranches]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_GetBadgeBranches] @BID INT,
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
RIGHT OUTER JOIN Code c ON bb.CID = c.CID
	AND (bb.BID = @BID OR bb.BID IS NULL)
WHERE c.TenID = @TenID
	AND c.CTID = @CTID
ORDER BY c.Code

GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeCategories]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeCategories] @BID INT,
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @CTID INT

SELECT @CTID = CTID
FROM dbo.CodeType
WHERE TenID = @TenID
	AND CodeTypeName = 'Badge Category'

SELECT @BID AS BID,
	c.CID,
	c.Code AS NAME, --c.CTID,
	CASE 
		WHEN bb.BID IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM (
	SELECT *
	FROM dbo.BadgeCategory
	WHERE BID = @BID
	) AS bb
RIGHT JOIN (
	SELECT *
	FROM Code
	WHERE TenID = @TenID
		AND CTID = @CTID
	) AS c ON bb.CID = c.CID
ORDER BY c.Code
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeEventIDS]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_GetBadgeEventIDS] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN convert(VARCHAR, EID)
			ELSE @List + ', ' + convert(VARCHAR, EID)
			END, '')
FROM Event p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
ORDER BY p.EventTitle
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeEvents]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeEvents] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.EventTitle
			ELSE @List + ', ' + p.EventTitle
			END, '')
FROM Event p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
ORDER BY p.EventTitle
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeGallery]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGallery] @TenID INT,
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
ORDER BY b.UserName
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeGames]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGames] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.GameName
			ELSE @List + ', ' + p.GameName
			END, '')
FROM Minigame p
WHERE p.TenID = @TenID
	AND p.AwardedBadgeID = @BID
ORDER BY p.GameName
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeLocations]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeLocations] @BID INT,
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @CTID INT

SELECT @CTID = CTID
FROM dbo.CodeType
WHERE TenID = @TenID
	AND CodeTypeName = 'Badge Location'

SELECT @BID AS BID,
	c.CID,
	c.Code AS NAME, --c.CTID,
	CASE 
		WHEN bb.BID IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM (
	SELECT *
	FROM dbo.BadgeLocation
	WHERE BID = @BID
	) AS bb
RIGHT JOIN (
	SELECT *
	FROM Code
	WHERE TenID = @TenID
		AND CTID = @CTID
	) AS c ON bb.CID = c.CID
ORDER BY c.Code
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetBadgeReading]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeReading] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN g.GameName
			ELSE @List + ', ' + g.GameName
			END, '')
FROM dbo.ProgramGameLevel p
INNER JOIN ProgramGame g ON g.PGID = p.PGID
WHERE g.TenID = @TenID
	AND p.AwardBadgeID = @BID
	OR AwardBadgeIDBonus = @BID

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.AwardName
			ELSE @List + ', ' + p.AwardName
			END, '')
FROM Award p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
	AND p.NumPoints > 0
GROUP BY p.AwardName
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_GetByID] @BID INT
AS
SELECT *
FROM [Badge]
WHERE BID = @BID
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetEnrollmentPrograms]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetEnrollmentPrograms] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.TabName
			ELSE @List + ', ' + p.TabName
			END, '')
FROM Programs p
WHERE p.TenID = @TenID
	AND p.RegistrationBadgeID = @BID
ORDER BY p.POrder

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN r.TabName
			ELSE @List + ', ' + r.TabName
			END, '')
FROM Award p
INNER JOIN Programs r ON ProgramID = PID
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
ORDER BY r.POrder
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_GetList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_GetList] @ids VARCHAR(255) = ''
AS
SELECT *
FROM Badge
WHERE BID IN (
		(
			SELECT *
			FROM [dbo].[fnSplitBigInt](@ids)
			)
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_Insert] (
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
		FldText3
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
		@FldText3
		)

	SELECT @BID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Badge_Update] (
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
	@FldText3 TEXT = ''
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
	FldText3 = @FldText3
WHERE BID = @BID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_UpdateBadgeAgeGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_UpdateBadgeAgeGroups] @BID INT,
	@TenID INT,
	@CID_LIST VARCHAR(4000)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.BadgeAgeGrp
WHERE BID = @BID
	AND CID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)

INSERT INTO dbo.BadgeAgeGrp
SELECT @BID,
	CID
FROM dbo.Code
WHERE CID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)
	AND CID NOT IN (
		SELECT CID
		FROM dbo.BadgeAgeGrp
		WHERE BID = @BID
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_UpdateBadgeBranches]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_UpdateBadgeBranches] @BID INT,
	@TenID INT,
	@CID_LIST VARCHAR(4000)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.BadgeBranch
WHERE BID = @BID
	AND CID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)

INSERT INTO dbo.BadgeBranch
SELECT @BID,
	CID
FROM dbo.Code
WHERE CID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)
	AND CID NOT IN (
		SELECT CID
		FROM dbo.BadgeBranch
		WHERE BID = @BID
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_UpdateBadgeCategories]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_UpdateBadgeCategories] @BID INT,
	@TenID INT,
	@CID_LIST VARCHAR(4000)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.BadgeCategory
WHERE BID = @BID
	AND CID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)

INSERT INTO dbo.BadgeCategory
SELECT @BID,
	CID
FROM dbo.Code
WHERE CID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)
	AND CID NOT IN (
		SELECT CID
		FROM dbo.BadgeCategory
		WHERE BID = @BID
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Badge_UpdateBadgeLocations]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_UpdateBadgeLocations] @BID INT,
	@TenID INT,
	@CID_LIST VARCHAR(4000)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.BadgeLocation
WHERE BID = @BID
	AND CID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)

INSERT INTO dbo.BadgeLocation
SELECT @BID,
	CID
FROM dbo.Code
WHERE CID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@CID_LIST)
		)
	AND CID NOT IN (
		SELECT CID
		FROM dbo.BadgeLocation
		WHERE BID = @BID
		)
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_BookList_Delete] @BLID INT,
	@TenID INT = NULL
AS
DELETE
FROM BookListBooks
WHERE BLID = @BLID
	AND TenID = @TenID

DELETE
FROM BookList
WHERE BLID = @BLID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_BookList_GetAll] @TenID INT = NULL
AS
SELECT bl.*,
	isnull(p.AdminName, '') AS ProgName,
	isnull(c.Code, '') AS Library
FROM [BookList] bl
LEFT JOIN Programs p ON bl.ProgID = p.PID
	AND bl.TenID = p.TenID
LEFT JOIN Code c ON bl.LibraryID = c.cid
WHERE bl.TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookList_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_BookList_GetByID] @BLID INT
AS
SELECT *
FROM [BookList]
WHERE BLID = @BLID
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_GetForDisplay]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_BookList_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL
AS
--declare @PID int dbo.BookList
--select @PID = 100000
IF (@TenID IS NULL)
	SELECT @TenID = TenID
	FROM Patron
	WHERE PID = @PID

DECLARE @Lit1 INT
DECLARE @Lit2 INT,
	@ProgramId INT,
	@BranchId INT

SELECT @Lit1 = isnull(LiteracyLevel1, 0),
	@Lit2 = isnull(LiteracyLevel2, ''),
	@ProgramId = isnull(ProgID, 0),
	@BranchId = isnull(PrimaryLibrary, 0)
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
WHERE LibraryID = 0
	AND ProgID = 0
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
	bl.*,
	(select count(*) from [PatronBookLists] pbl WHERE pbl.[blid] = bl.[blid] AND pbl.[pid] = @pid AND pbl.[HasReadFlag] = 1) as NumBooksCompleted
FROM #temp1 t
LEFT JOIN dbo.BookList bl ON bl.BLID = t.BLID
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookList_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_BookList_Insert] (
	@AdminName VARCHAR(50),
	@ListName VARCHAR(50),
	@AdminDescription TEXT,
	@Description TEXT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@ProgID INT,
	@LibraryID INT,
	@AwardBadgeID INT,
	@AwardPoints INT,
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
	@NumBooksToComplete INT = 0,
	@BLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO BookList (
		AdminName,
		ListName,
		AdminDescription,
		Description,
		LiteracyLevel1,
		LiteracyLevel2,
		ProgID,
		LibraryID,
		AwardBadgeID,
		AwardPoints,
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
		NumBooksToComplete
		)
	VALUES (
		@AdminName,
		@ListName,
		@AdminDescription,
		@Description,
		@LiteracyLevel1,
		@LiteracyLevel2,
		@ProgID,
		@LibraryID,
		@AwardBadgeID,
		@AwardPoints,
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
		@NumBooksToComplete
		)

	SELECT @BLID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_BookList_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookList_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_BookList_Update] (
	@BLID INT,
	@AdminName VARCHAR(50),
	@ListName VARCHAR(50),
	@AdminDescription TEXT,
	@Description TEXT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@ProgID INT,
	@LibraryID INT,
	@AwardBadgeID INT,
	@AwardPoints INT,
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
	@NumBooksToComplete INT = 0
	)
AS
UPDATE BookList
SET AdminName = @AdminName,
	ListName = @ListName,
	AdminDescription = @AdminDescription,
	Description = @Description,
	LiteracyLevel1 = @LiteracyLevel1,
	LiteracyLevel2 = @LiteracyLevel2,
	ProgID = @ProgID,
	LibraryID = @LibraryID,
	AwardBadgeID = @AwardBadgeID,
	AwardPoints = @AwardPoints,
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
	NumBooksToComplete = @NumBooksToComplete
WHERE BLID = @BLID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Delete] @BLBID INT,
	@TenID INT = NULL
AS
DELETE
FROM [BookListBooks]
WHERE BLBID = @BLBID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetAll]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_GetAll] @BLID INT
AS
SELECT *
FROM [BookListBooks]
WHERE BLID = @BLID
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_GetByID] @BLBID INT
AS
SELECT *
FROM [BookListBooks]
WHERE BLBID = @BLBID
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForDisplay]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForDisplay]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_BookListBooks_GetForDisplay] @PID INT = 0
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
	@BranchId = isnull(PrimaryLibrary, 0),
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
WHERE LibraryID = 0
	AND ProgID = 0
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
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForPatronDisplay]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_GetForPatronDisplay]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_BookListBooks_GetForPatronDisplay] @BLID INT = 0,
	@PID INT = 0
AS
SELECT isnull(p.HasReadFlag, 0) AS HasRead,
	isnull(p.PBLBID, 0) AS PBLBID,
	b.*
FROM BookListBooks b
LEFT JOIN PatronBookLists p ON b.BLBID = p.BLBID
	AND b.BLID = p.BLID
	AND p.PID = @PID
WHERE b.BLID = @BLID
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Insert] (
	@BLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@ISBN VARCHAR(50),
	@URL VARCHAR(150),
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
	@BLBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO BookListBooks (
		BLID,
		Author,
		Title,
		ISBN,
		URL,
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
		FldText3
		)
	VALUES (
		@BLID,
		@Author,
		@Title,
		@ISBN,
		@URL,
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
		@FldText3
		)

	SELECT @BLBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_BookListBooks_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_BookListBooks_Update] (
	@BLBID INT,
	@BLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@ISBN VARCHAR(50),
	@URL VARCHAR(150),
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
	@FldText3 TEXT = ''
	)
AS
UPDATE BookListBooks
SET BLID = @BLID,
	Author = @Author,
	Title = @Title,
	ISBN = @ISBN,
	URL = @URL,
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
	FldText3 = @FldText3
WHERE BLBID = @BLBID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Code_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Code_Delete] @CID INT
AS
DELETE
FROM [Code]
WHERE CID = @CID
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Code_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Code]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllLibrarySystems]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_GetAllLibrarySystems]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_Code_GetAllLibrarySystems] @TenID INT = NULL
AS
SELECT DISTINCT rtrim(ltrim(District)) AS LibSys
FROM Patron
WHERE rtrim(ltrim(District)) <> ''
	AND District IS NOT NULL
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY rtrim(ltrim(District))
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllSchools]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_GetAllSchools]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_Code_GetAllSchools] @TenID INT = NULL
AS
SELECT DISTINCT rtrim(ltrim(SchoolName)) AS School
FROM Patron
WHERE rtrim(ltrim(SchoolName)) <> ''
	AND SchoolName IS NOT NULL
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY rtrim(ltrim(SchoolName))
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllTypeID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_GetAllTypeID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Code_GetAllTypeID] @ID INT
AS
SELECT *
FROM [Code]
WHERE CTID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllTypeName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Code_GetAllTypeName] @name VARCHAR(50),
	@TenID INT = NULL
AS
SELECT *
FROM [Code]
WHERE CTID = (
		SELECT CTID
		FROM dbo.CodeType
		WHERE CodeTypeName = @name
			AND (
				TenID = @TenID
				OR @TenID IS NULL
				)
		)
ORDER BY Code
GO
/****** Object:  StoredProcedure [dbo].[app_Code_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Code_GetByID] @CID INT
AS
SELECT *
FROM [Code]
WHERE CID = @CID
GO
/****** Object:  StoredProcedure [dbo].[app_Code_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Code_Insert] (
	@CTID INT,
	@Code VARCHAR(25),
	@Description VARCHAR(80),
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
/****** Object:  StoredProcedure [dbo].[app_Code_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Code_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Code_Update] (
	@CID INT,
	@CTID INT,
	@Code VARCHAR(25),
	@Description VARCHAR(80),
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
/****** Object:  StoredProcedure [dbo].[app_CodeType_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_CodeType_Delete] @CTID INT
AS
DELETE
FROM [CodeType]
WHERE CTID = @CTID
GO
/****** Object:  StoredProcedure [dbo].[app_CodeType_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_GetAll]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CodeType_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CodeType]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_CodeType_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CodeType_GetByID] @CTID INT
AS
SELECT *
FROM [CodeType]
WHERE CTID = @CTID
GO
/****** Object:  StoredProcedure [dbo].[app_CodeType_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_Insert]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_CodeType_Insert] (
	@isSystem BIT,
	@CodeTypeName VARCHAR(50),
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
/****** Object:  StoredProcedure [dbo].[app_CodeType_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CodeType_Update]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_CodeType_Update] (
	@CTID INT,
	@isSystem BIT,
	@CodeTypeName VARCHAR(50),
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
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_Delete] @TenID INT
AS
DELETE
FROM [CustomEventFields]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CustomEventFields]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_GetByID]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_GetByID] @TenID INT
AS
SELECT *
FROM [CustomEventFields]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_Insert] (
	@Use1 BIT,
	@Label1 VARCHAR(50),
	@DDValues1 VARCHAR(50),
	@Use2 BIT,
	@Use3 BIT,
	@Label2 VARCHAR(50),
	@Label3 VARCHAR(50),
	@DDValues2 VARCHAR(50),
	@DDValues3 VARCHAR(50),
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
	@CID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO CustomEventFields (
		Use1,
		Label1,
		DDValues1,
		Use2,
		Use3,
		Label2,
		Label3,
		DDValues2,
		DDValues3,
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
		FldText3
		)
	VALUES (
		@Use1,
		@Label1,
		@DDValues1,
		@Use2,
		@Use3,
		@Label2,
		@Label3,
		@DDValues2,
		@DDValues3,
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
		@FldText3
		)

	SELECT @CID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_CustomEventFields_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_CustomEventFields_Update] (
	@CID INT,
	@Use1 BIT,
	@Label1 VARCHAR(50),
	@DDValues1 VARCHAR(50),
	@Use2 BIT,
	@Use3 BIT,
	@Label2 VARCHAR(50),
	@Label3 VARCHAR(50),
	@DDValues2 VARCHAR(50),
	@DDValues3 VARCHAR(50),
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
	@FldText3 TEXT = ''
	)
AS
UPDATE CustomEventFields
SET Use1 = @Use1,
	Label1 = @Label1,
	DDValues1 = @DDValues1,
	Use2 = @Use2,
	Use3 = @Use3,
	Label2 = @Label2,
	Label3 = @Label3,
	DDValues2 = @DDValues2,
	DDValues3 = @DDValues3,
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
	FldText3 = @FldText3
WHERE CID = @CID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_Delete]    Script Date: 01/05/2015 14:43:20 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_Delete] @CID INT
AS
DELETE
FROM [CustomRegistrationFields]
WHERE CID = @CID
GO
/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [CustomRegistrationFields]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_GetByID] @TenID INT
AS
SELECT *
FROM [CustomRegistrationFields]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_Insert] (
	@Use1 BIT,
	@Label1 VARCHAR(50),
	@DDValues1 VARCHAR(50),
	@Use2 BIT,
	@Use3 BIT,
	@Use4 BIT,
	@Use5 BIT,
	@Label2 VARCHAR(50),
	@Label3 VARCHAR(50),
	@Label4 VARCHAR(50),
	@Label5 VARCHAR(50),
	@DDValues2 VARCHAR(50),
	@DDValues3 VARCHAR(50),
	@DDValues4 VARCHAR(50),
	@DDValues5 VARCHAR(50),
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
	@CID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO CustomRegistrationFields (
		Use1,
		Label1,
		DDValues1,
		Use2,
		Use3,
		Use4,
		Use5,
		Label2,
		Label3,
		Label4,
		Label5,
		DDValues2,
		DDValues3,
		DDValues4,
		DDValues5,
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
		FldText3
		)
	VALUES (
		@Use1,
		@Label1,
		@DDValues1,
		@Use2,
		@Use3,
		@Use4,
		@Use5,
		@Label2,
		@Label3,
		@Label4,
		@Label5,
		@DDValues2,
		@DDValues3,
		@DDValues4,
		@DDValues5,
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
		@FldText3
		)

	SELECT @CID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_CustomRegistrationFields_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_CustomRegistrationFields_Update] (
	@CID INT,
	@Use1 BIT,
	@Label1 VARCHAR(50),
	@DDValues1 VARCHAR(50),
	@Use2 BIT,
	@Use3 BIT,
	@Use4 BIT,
	@Use5 BIT,
	@Label2 VARCHAR(50),
	@Label3 VARCHAR(50),
	@Label4 VARCHAR(50),
	@Label5 VARCHAR(50),
	@DDValues2 VARCHAR(50),
	@DDValues3 VARCHAR(50),
	@DDValues4 VARCHAR(50),
	@DDValues5 VARCHAR(50),
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
	@FldText3 TEXT = ''
	)
AS
UPDATE CustomRegistrationFields
SET Use1 = @Use1,
	Label1 = @Label1,
	DDValues1 = @DDValues1,
	Use2 = @Use2,
	Use3 = @Use3,
	Use4 = @Use4,
	Use5 = @Use5,
	Label2 = @Label2,
	Label3 = @Label3,
	Label4 = @Label4,
	Label5 = @Label5,
	DDValues2 = @DDValues2,
	DDValues3 = @DDValues3,
	DDValues4 = @DDValues4,
	DDValues5 = @DDValues5,
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
	FldText3 = @FldText3
WHERE CID = @CID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Event_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Event_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Event_Delete] @EID INT
AS
DELETE
FROM [Event]
WHERE EID = @EID
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetAdminSearch]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_GetAdminSearch] @startDate DATETIME,
	@endDate DATETIME,
	@branchID INT,
	@TenID INT = NULL
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
	AND (
		BranchID = @branchID
		OR @branchID = 0
		)
	AND (
		EventDate >= @startDate
		OR @startDate IS NULL
		)
	AND (
		EventDate <= @endDate
		OR @endDate IS NULL
		)
ORDER BY EventDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Event_GetAll] @TenID INT = NULL
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
ORDER BY EventDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Event_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Event_GetByID] @EID INT
AS
SELECT *,
	(
		SELECT Code
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE EID = @EID
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetEventCountByEventCode]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_GetEventCountByEventCode] (
	@EID INT = NULL,
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT COUNT(*) AS NumCodes
	FROM Event
	WHERE SecretCode = @Key
		AND @key != ''
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
		AND (
			EID != @EID
			OR @EID IS NULL
			)
END
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetEventList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_GetEventList] @List VARCHAR(2000) = '',
	@TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Now DATETIME

SELECT @Now = GETDATE()

SELECT CASE 
		WHEN EventDate <= @Now
			AND EndDate IS NULL
			THEN 'Expired '
		WHEN EndDate IS NOT NULL
			AND EndDate <= @Now
			THEN 'Expired '
		WHEN EventDate <= @Now
			AND EndDate >= @Now
			THEN ''
		WHEN EventDate >= @Now
			THEN ''
		ELSE ''
		END AS Expired,
	*,
	isnull(Code, '') AS Branch
FROM Event
LEFT JOIN Code ON BranchID = CID
WHERE Event.TenID = @TenID
	AND EID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@List)
		)
ORDER BY Expired,
	EventDate
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetEventsByEventCode]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_GetEventsByEventCode] (
	@startDate DATETIME,
	@endDate DATETIME,
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT *
	FROM Event
	WHERE EventDate BETWEEN @startDate
			AND @endDate
		AND SecretCode = @Key
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
END
GO
/****** Object:  StoredProcedure [dbo].[app_Event_GetUpcomingDisplay]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_GetUpcomingDisplay] @startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@branchID INT = 0,
	@TenID INT = NULL
AS
SELECT *,
	(
		SELECT [Description]
		FROM dbo.Code
		WHERE CID = BranchID
		) AS Branch
FROM [Event]
WHERE (
		BranchID = @branchID
		OR @branchID = 0
		)
	AND (
		EventDate >= @startDate
		OR (
			EndDate IS NOT NULL
			AND EndDate >= @startDate
			)
		OR @startDate IS NULL
		)
	AND (
		EventDate <= @endDate
		OR (
			EndDate IS NOT NULL
			AND EndDate <= @endDate
			)
		OR @endDate IS NULL
		)
	AND (
		dateadd(d, 1, EventDate) >= GETDATE()
		OR (
			dateadd(d, 1, EndDate) >= GETDATE()
			AND EndDate IS NOT NULL
			)
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY EventDate ASC,
	EventTitle
GO
/****** Object:  StoredProcedure [dbo].[app_Event_InitTenant]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Event_InitTenant] @src INT,
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
	FldInt3
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
	e.EID
FROM Event e
WHERE e.TenID = @src
	AND e.EID NOT IN (
		SELECT SrcPK
		FROM TenantInitData
		WHERE IntitType = 'event'
			AND DestTID = @dst
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Event_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Event_Insert] (
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
		EndTime
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
		@EndTime
		)

	SELECT @EID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Event_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Event_Update] (
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
	@EndTime VARCHAR(50)
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
	EndTime = @EndTime
WHERE EID = @EID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Delete] @GPSID INT
AS
DELETE
FROM [GamePlayStats]
WHERE GPSID = @GPSID
GO
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_GetAll]
AS
SELECT *
FROM [GamePlayStats]
GO
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_GetByID] @GPSID INT
AS
SELECT *
FROM [GamePlayStats]
WHERE GPSID = @GPSID
GO
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Insert] (
	@PID INT,
	@MGID INT,
	@MGType INT,
	@Started DATETIME,
	@Difficulty VARCHAR(50),
	@CompletedPlay BIT,
	@Completed DATETIME,
	@GPSID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO GamePlayStats (
		PID,
		MGID,
		MGType,
		Started,
		Difficulty,
		CompletedPlay,
		Completed
		)
	VALUES (
		@PID,
		@MGID,
		@MGType,
		@Started,
		@Difficulty,
		@CompletedPlay,
		@Completed
		)

	SELECT @GPSID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_GamePlayStats_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_GamePlayStats_Update] (
	@GPSID INT,
	@PID INT,
	@MGID INT,
	@MGType INT,
	@Started DATETIME,
	@Difficulty VARCHAR(50),
	@CompletedPlay BIT,
	@Completed DATETIME
	)
AS
UPDATE GamePlayStats
SET PID = @PID,
	MGID = @MGID,
	MGType = @MGType,
	Started = @Started,
	Difficulty = @Difficulty,
	CompletedPlay = @CompletedPlay,
	Completed = @Completed
WHERE GPSID = @GPSID
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Delete] @ID INT
AS
DELETE
FROM [LibraryCrosswalk]
WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetAll] @TenID INT = NULL
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
	isnull(w.City, '') AS City
FROM [LibraryCrosswalk] w
RIGHT JOIN @Libraries l ON w.BranchID = l.CID
ORDER BY l.Code
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetByID] @ID INT
AS
SELECT *
FROM [LibraryCrosswalk]
WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_GetByLibraryID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetByLibraryID] @ID INT = 0
AS
SELECT *
FROM LibraryCrosswalk
WHERE BranchID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_GetFilteredBranchDDValues]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetFilteredBranchDDValues] @DistrictID INT = 0,
	@City VARCHAR(50) = '',
	@TenID INT = NULL
AS
SELECT DISTINCT BranchID AS CID,
	c.Code AS Code, c.[Description] as [Description]
FROM LibraryCrosswalk w
INNER JOIN Code c ON w.BranchID = c.CID
WHERE (
		DistrictID = @DistrictID
		OR @DistrictID IS NULL
		OR @DistrictID = 0
		)
	AND (
		City = @City
		OR @City IS NULL
		OR @City = ''
		)
	AND (
		w.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY [Description]
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_GetFilteredDistrictDDValues]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetFilteredDistrictDDValues] @City VARCHAR(50) = '',
	@TenID INT = NULL
AS
SELECT DISTINCT DistrictID AS CID,
	c.Code AS Code, c.[Description] as [Description]
FROM LibraryCrosswalk w
INNER JOIN Code c ON w.DistrictID = c.CID
WHERE (
		City = @City
		OR @City IS NULL
		OR @City = ''
		)
	AND (
		w.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY [Description]
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Insert] (
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
		FldText3
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
		@FldText3
		)

	SELECT @ID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_LibraryCrosswalk_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Update] (
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
	@FldText3 TEXT = ''
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
	FldText3 = @FldText3
WHERE ID = @ID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Delete] @CAID INT
AS
DELETE
FROM [MGChooseAdv]
WHERE CAID = @CAID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetAll]
AS
SELECT *
FROM [MGChooseAdv]
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetByID] @CAID INT
AS
SELECT *
FROM [MGChooseAdv]
WHERE CAID = @CAID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetByIDWithParent] @MGID INT
AS
SELECT mg.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGChooseAdv mg
INNER JOIN dbo.Minigame g ON mg.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdv_GetByMGID] @MGID INT
AS
SELECT *
FROM MGChooseAdv
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CAID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGChooseAdv (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CAID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Update] (
	@CAID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGChooseAdv
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CAID = @CAID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Delete]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Delete] @CASID INT
AS
DECLARE @MGID INT

SELECT @MGID = MGID
FROM MGChooseAdvSlides
WHERE CASID = @CASID

DELETE
FROM MGChooseAdvSlides
WHERE CASID = @CASID

EXEC app_MGChooseAdvSlides_Reorder @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGChooseAdvSlides
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAllByDifficulty]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetAllByDifficulty]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetAllByDifficulty] @MGID INT = 0,
	@Diff INT = 1
AS
SELECT *,
	(
		SELECT MAX(StepNumber)
		FROM MGChooseAdvSlides
		WHERE MGID = @MGID
			AND Difficulty = @Diff
		) AS MAX
FROM MGChooseAdvSlides
WHERE MGID = @MGID
	AND Difficulty = @Diff
ORDER BY StepNumber
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetByID] @CASID INT
AS
SELECT *
FROM [MGChooseAdvSlides]
WHERE CASID = @CASID
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetPlaySlide]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_GetPlaySlide]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_GetPlaySlide] @CAID INT,
	@Step INT = 1,
	@Difficulty INT = 1
AS
SELECT *
FROM [MGChooseAdvSlides]
WHERE CAID = @CAID
	AND StepNumber = @Step
	AND Difficulty = @Difficulty
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Insert] (
	@CAID INT,
	@MGID INT,
	@Difficulty INT = 1,
	@StepNumber INT,
	@SlideText TEXT,
	@FirstImageGoToStep INT,
	@SecondImageGoToStep INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CASID INT OUTPUT
	)
AS
BEGIN
	SELECT @StepNumber = isnull((
				SELECT isnull(MAX(StepNumber), 0)
				FROM MGChooseAdvSlides
				WHERE CAID = @CAID
					AND Difficulty = @Difficulty
				), 0) + 1

	INSERT INTO MGChooseAdvSlides (
		CAID,
		MGID,
		Difficulty,
		StepNumber,
		SlideText,
		FirstImageGoToStep,
		SecondImageGoToStep,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@CAID,
		@MGID,
		@Difficulty,
		@StepNumber,
		@SlideText,
		@FirstImageGoToStep,
		@SecondImageGoToStep,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CASID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_MoveDn]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_MoveDn] @CASID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@MGID INT,
	@Diff INT,
	@CAID INT

SELECT @CurrentRecordLocation = StepNumber,
	@CAID = CAID,
	@MGID = MGID,
	@Diff = Difficulty
FROM MGChooseAdvSlides
WHERE CASID = @CASID

EXEC [dbo].[app_MGChooseAdvSlides_Reorder] @MGID

IF @CurrentRecordLocation < (
		SELECT MAX(StepNumber)
		FROM MGChooseAdvSlides
		WHERE MGID = @MGID
			AND Difficulty = @Diff
		)
BEGIN
	SELECT @NextRecordID = CASID
	FROM MGChooseAdvSlides
	WHERE StepNumber = (@CurrentRecordLocation + 1)
		AND MGID = @MGID
		AND Difficulty = @Diff

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation + 1
	WHERE CASID = @CASID

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation
	WHERE CASID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_MoveUp]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_MoveUp] @CASID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@MGID INT,
	@Diff INT,
	@CAID INT

SELECT @CurrentRecordLocation = StepNumber,
	@CAID = CAID,
	@MGID = MGID,
	@Diff = Difficulty
FROM MGChooseAdvSlides
WHERE CASID = @CASID

EXEC [dbo].[app_MGChooseAdvSlides_Reorder] @MGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = CASID
	FROM MGChooseAdvSlides
	WHERE StepNumber = (@CurrentRecordLocation - 1)
		AND MGID = @MGID
		AND Difficulty = @Diff

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation - 1
	WHERE CASID = @CASID

	UPDATE MGChooseAdvSlides
	SET StepNumber = @CurrentRecordLocation
	WHERE CASID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Reorder]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Reorder] @MGID INT
AS
UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 1
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 1

UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 2
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 2

UPDATE MGChooseAdvSlides
SET StepNumber = rowNumber
FROM MGChooseAdvSlides
INNER JOIN (
	SELECT CASID,
		StepNumber,
		MGID,
		Difficulty,
		row_number() OVER (
			ORDER BY StepNumber ASC
			) AS rowNumber
	FROM MGChooseAdvSlides
	WHERE MGID = @MGID
		AND Difficulty = 3
	) drRowNumbers ON drRowNumbers.CASID = MGChooseAdvSlides.CASID
	AND drRowNumbers.MGID = @MGID
	AND drRowNumbers.Difficulty = 3
GO
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdvSlides_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdvSlides_Update] (
	@CASID INT,
	@CAID INT,
	@MGID INT,
	@Difficulty INT,
	@StepNumber INT,
	@SlideText TEXT,
	@FirstImageGoToStep INT,
	@SecondImageGoToStep INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGChooseAdvSlides
SET CAID = @CAID,
	MGID = @MGID,
	Difficulty = @Difficulty,
	StepNumber = @StepNumber,
	SlideText = @SlideText,
	FirstImageGoToStep = @FirstImageGoToStep,
	SecondImageGoToStep = @SecondImageGoToStep,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CASID = @CASID
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Delete] @CBID INT
AS
DELETE
FROM [MGCodeBreaker]
WHERE CBID = @CBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetAll]
AS
SELECT *
FROM [MGCodeBreaker]
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetByID] @CBID INT
AS
SELECT *
FROM [MGCodeBreaker]
WHERE CBID = @CBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetByIDWithParent] @MGID INT
AS
SELECT cb.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGCodeBreaker cb
INNER JOIN dbo.Minigame g ON cb.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetByMGID] @MGID INT
AS
SELECT *
FROM [MGCodeBreaker]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Insert] (
	@MGID INT,
	@EasyString VARCHAR(250),
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@MediumString VARCHAR(250),
	@HardString VARCHAR(250),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGCodeBreaker (
		MGID,
		EasyString,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		MediumString,
		HardString,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EasyString,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@MediumString,
		@HardString,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Update] (
	@CBID INT,
	@MGID INT,
	@EasyString VARCHAR(250),
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@MediumString VARCHAR(250),
	@HardString VARCHAR(250),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGCodeBreaker
SET MGID = @MGID,
	EasyString = @EasyString,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	MediumString = @MediumString,
	HardString = @HardString,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE CBID = @CBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Delete] @HPID INT
AS
DELETE
FROM [MGHiddenPic]
WHERE HPID = @HPID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetAll]
AS
SELECT *
FROM [MGHiddenPic]
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByID] @HPID INT
AS
SELECT *
FROM [MGHiddenPic]
WHERE HPID = @HPID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByIDWithParent] @MGID INT
AS
SELECT mg.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGHiddenPic mg
INNER JOIN dbo.Minigame g ON mg.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByMGID] @MGID INT
AS
SELECT *
FROM MGHiddenPic
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetRandomBK]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_GetRandomBK]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetRandomBK] @HPID INT,
	@HPBID INT OUTPUT
AS
DECLARE @a INT

SELECT TOP 1 NEWID() AS id,
	HPBID
INTO #tmp
FROM dbo.MGHiddenPicBk
WHERE HPID = @HPID
ORDER BY id

SELECT @HPBID = HPBID
FROM #tmp

RETURN @a
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@EasyDictionary TEXT,
	@MediumDictionary TEXT,
	@HardDictionary TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@HPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGHiddenPic (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		EasyDictionary,
		MediumDictionary,
		HardDictionary,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@EasyDictionary,
		@MediumDictionary,
		@HardDictionary,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @HPID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Update] (
	@HPID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@EasyDictionary TEXT,
	@MediumDictionary TEXT,
	@HardDictionary TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGHiddenPic
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	EasyDictionary = @EasyDictionary,
	MediumDictionary = @MediumDictionary,
	HardDictionary = @HardDictionary,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE HPID = @HPID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Delete] @HPBID INT
AS
DELETE
FROM [MGHiddenPicBk]
WHERE HPBID = @HPBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGHiddenPicBk
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetByID] @HPBID INT
AS
SELECT *
FROM [MGHiddenPicBk]
WHERE HPBID = @HPBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetByIDWithParent] @MGID INT
AS
SELECT mg.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGHiddenPicBk mg
INNER JOIN dbo.Minigame g ON mg.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_GetByMGID] @MGID INT
AS
SELECT *
FROM MGHiddenPicBk
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Insert] (
	@HPID INT,
	@MGID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@HPBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGHiddenPicBk (
		HPID,
		MGID,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@HPID,
		@MGID,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @HPBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Update]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Update] (
	@HPBID INT,
	@HPID INT,
	@MGID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGHiddenPicBk
SET HPID = @HPID,
	MGID = @MGID,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE HPBID = @HPBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Delete]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Delete] @MAGID INT
AS
DELETE
FROM [MGMatchingGame]
WHERE MAGID = @MAGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetAll]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetAll]
AS
SELECT *
FROM [MGMatchingGame]
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByID]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetByID] @MAGID INT
AS
SELECT *
FROM [MGMatchingGame]
WHERE MAGID = @MAGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetByIDWithParent] @MGID INT
AS
SELECT mg.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGMatchingGame mg
INNER JOIN dbo.Minigame g ON mg.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetByMGID]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetByMGID] @MGID INT
AS
SELECT *
FROM MGMatchingGame
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetRandomPlayItems]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetRandomPlayItems]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetRandomPlayItems] @MAGID INT,
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
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EasyGameSize INT,
	@MediumGameSize INT,
	@HardGameSize INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MAGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMatchingGame (
		MGID,
		CorrectRoundsToWinCount,
		EasyGameSize,
		MediumGameSize,
		HardGameSize,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@CorrectRoundsToWinCount,
		@EasyGameSize,
		@MediumGameSize,
		@HardGameSize,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MAGID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Update] (
	@MAGID INT,
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EasyGameSize INT,
	@MediumGameSize INT,
	@HardGameSize INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMatchingGame
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	EasyGameSize = @EasyGameSize,
	MediumGameSize = @MediumGameSize,
	HardGameSize = @HardGameSize,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MAGID = @MAGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Delete] @MAGTID INT
AS
DELETE
FROM [MGMatchingGameTiles]
WHERE MAGTID = @MAGTID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_GetAll] @MGID INT = 0
AS
SELECT *
FROM MGMatchingGameTiles
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_GetByID] @MAGTID INT
AS
SELECT *
FROM [MGMatchingGameTiles]
WHERE MAGTID = @MAGTID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Insert] (
	@MAGID INT,
	@MGID INT,
	@Tile1UseMedium BIT,
	@Tile1UseHard BIT,
	@Tile2UseMedium BIT,
	@Tile2UseHard BIT,
	@Tile3UseMedium BIT,
	@Tile3UseHard BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MAGTID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMatchingGameTiles (
		MAGID,
		MGID,
		Tile1UseMedium,
		Tile1UseHard,
		Tile2UseMedium,
		Tile2UseHard,
		Tile3UseMedium,
		Tile3UseHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MAGID,
		@MGID,
		@Tile1UseMedium,
		@Tile1UseHard,
		@Tile2UseMedium,
		@Tile2UseHard,
		@Tile3UseMedium,
		@Tile3UseHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MAGTID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMatchingGameTiles_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGameTiles_Update] (
	@MAGTID INT,
	@MAGID INT,
	@MGID INT,
	@Tile1UseMedium BIT,
	@Tile1UseHard BIT,
	@Tile2UseMedium BIT,
	@Tile2UseHard BIT,
	@Tile3UseMedium BIT,
	@Tile3UseHard BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMatchingGameTiles
SET MAGID = @MAGID,
	MGID = @MGID,
	Tile1UseMedium = @Tile1UseMedium,
	Tile1UseHard = @Tile1UseHard,
	Tile2UseMedium = @Tile2UseMedium,
	Tile2UseHard = @Tile2UseHard,
	Tile3UseMedium = @Tile3UseMedium,
	Tile3UseHard = @Tile3UseHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MAGTID = @MAGTID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Delete] @MMID INT
AS
DELETE
FROM [MGMixAndMatch]
WHERE MMID = @MMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetAll]
AS
SELECT *
FROM [MGMixAndMatch]
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetByID] @MMID INT
AS
SELECT *
FROM [MGMixAndMatch]
WHERE MMID = @MMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetByIDWithParent] @MGID INT
AS
SELECT mm.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGMixAndMatch mm
INNER JOIN dbo.Minigame g ON mm.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_GetByMGID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetByMGID] @MGID INT
AS
SELECT *
FROM [MGMixAndMatch]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MMID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMixAndMatch (
		MGID,
		CorrectRoundsToWinCount,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@CorrectRoundsToWinCount,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MMID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Update] (
	@MMID INT,
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMixAndMatch
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MMID = @MMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_Delete] @MMIID INT
AS
DELETE
FROM [MGMixAndMatchItems]
WHERE MMIID = @MMIID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetAll] @MGID INT = 0
AS
SELECT *
FROM [MGMixAndMatchItems]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetByID] @MMIID INT
AS
SELECT *
FROM [MGMixAndMatchItems]
WHERE MMIID = @MMIID
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetRandom3]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_GetRandom3]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_GetRandom3] @MMID INT
AS
SELECT TOP 3 NEWID() AS id,
	*
FROM dbo.MGMixAndMatchItems
WHERE MMID = @MMID
ORDER BY id
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_Insert] (
	@MMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MMIID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMixAndMatchItems (
		MMID,
		MGID,
		ItemImage,
		EasyLabel,
		MediumLabel,
		HardLabel,
		AudioEasy,
		AudioMedium,
		AudioHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MMID,
		@MGID,
		@ItemImage,
		@EasyLabel,
		@MediumLabel,
		@HardLabel,
		@AudioEasy,
		@AudioMedium,
		@AudioHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MMIID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_Update] (
	@MMIID INT,
	@MMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMixAndMatchItems
SET MMID = @MMID,
	MGID = @MGID,
	ItemImage = @ItemImage,
	EasyLabel = @EasyLabel,
	MediumLabel = @MediumLabel,
	HardLabel = @HardLabel,
	AudioEasy = @AudioEasy,
	AudioMedium = @AudioMedium,
	AudioHard = @AudioHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MMIID = @MMIID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Delete] @OBID INT
AS
DELETE
FROM [MGOnlineBook]
WHERE OBID = @OBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetAll]
AS
SELECT *
FROM [MGOnlineBook]
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetByID] @OBID INT
AS
SELECT *
FROM [MGOnlineBook]
WHERE OBID = @OBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetByIDWithParent] @MGID INT
AS
SELECT ob.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM [MGOnlineBook] ob
INNER JOIN dbo.Minigame g ON ob.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetByMGID] @MGID INT
AS
SELECT *
FROM [MGOnlineBook]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@OBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGOnlineBook (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @OBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBook_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBook_Update] (
	@OBID INT,
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGOnlineBook
SET MGID = @MGID,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE OBID = @OBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Delete] @OBPGID INT
AS
DECLARE @MGID INT;

SELECT @MGID = MGID
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID

DELETE
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID

EXEC app_MGOnlineBookPages_Reorder @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetAll] @MGID INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(PageNumber), 1)
		FROM [MGOnlineBookPages]
		WHERE MGID = @MGID
		) AS MAX
FROM [MGOnlineBookPages]
WHERE MGID = @MGID
ORDER BY PageNumber
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetByID] @OBPGID INT
AS
SELECT *
FROM [MGOnlineBookPages]
WHERE OBPGID = @OBPGID
ORDER BY PageNumber
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_GetByPage]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_GetByPage] @Page INT,
	@OBID INT
AS
SELECT *
FROM [MGOnlineBookPages]
WHERE PageNumber = @Page
	AND OBID = @OBID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Insert] (
	@OBID INT,
	@MGID INT,
	@PageNumber INT,
	@TextEasy TEXT,
	@TextMedium TEXT,
	@TextHard TEXT,
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@OBPGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGOnlineBookPages (
		OBID,
		MGID,
		PageNumber,
		TextEasy,
		TextMedium,
		TextHard,
		AudioEasy,
		AudioMedium,
		AudioHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@OBID,
		@MGID,
		(
			SELECT isnull(Max(PageNumber), 0) + 1
			FROM MGOnlineBookPages
			WHERE MGID = @MGID
			),
		@TextEasy,
		@TextMedium,
		@TextHard,
		@AudioEasy,
		@AudioMedium,
		@AudioHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @OBPGID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_MoveDn] @OBPGID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@MGID INT

SELECT @CurrentRecordLocation = PageNumber,
	@MGID = MGID
FROM MGOnlineBookPages
WHERE OBPGID = @OBPGID

EXEC [dbo].[app_MGOnlineBookPages_Reorder] @MGID

IF @CurrentRecordLocation < (
		SELECT MAX(PageNumber)
		FROM MGOnlineBookPages
		WHERE MGID = @MGID
		)
BEGIN
	SELECT @NextRecordID = OBPGID
	FROM MGOnlineBookPages
	WHERE PageNumber = (@CurrentRecordLocation + 1)
		AND MGID = @MGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation + 1
	WHERE OBPGID = @OBPGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation
	WHERE OBPGID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_MoveUp] @OBPGID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@MGID INT

SELECT @CurrentRecordLocation = PageNumber,
	@MGID = MGID
FROM MGOnlineBookPages
WHERE OBPGID = @OBPGID

EXEC [dbo].[app_MGOnlineBookPages_Reorder] @MGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = OBPGID
	FROM MGOnlineBookPages
	WHERE PageNumber = (@CurrentRecordLocation - 1)
		AND MGID = @MGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation - 1
	WHERE OBPGID = @OBPGID

	UPDATE MGOnlineBookPages
	SET PageNumber = @CurrentRecordLocation
	WHERE OBPGID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Reorder] @MGID INT
AS
UPDATE MGOnlineBookPages
SET PageNumber = rowNumber
FROM MGOnlineBookPages
INNER JOIN (
	SELECT OBPGID,
		PageNumber,
		row_number() OVER (
			ORDER BY PageNumber ASC
			) AS rowNumber
	FROM MGOnlineBookPages
	WHERE MGID = @MGID
	) drRowNumbers ON drRowNumbers.OBPGID = MGOnlineBookPages.OBPGID
	AND MGID = @MGID
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGOnlineBookPages_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGOnlineBookPages_Update] (
	@OBPGID INT,
	@OBID INT,
	@MGID INT,
	@PageNumber INT,
	@TextEasy TEXT,
	@TextMedium TEXT,
	@TextHard TEXT,
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGOnlineBookPages
SET OBID = @OBID,
	MGID = @MGID,
	PageNumber = @PageNumber,
	TextEasy = @TextEasy,
	TextMedium = @TextMedium,
	TextHard = @TextHard,
	AudioEasy = @AudioEasy,
	AudioMedium = @AudioMedium,
	AudioHard = @AudioHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE OBPGID = @OBPGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Delete] @WMID INT
AS
DELETE
FROM [MGWordMatch]
WHERE WMID = @WMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetAll]
AS
SELECT *
FROM [MGWordMatch]
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByID] @WMID INT
AS
SELECT *
FROM [MGWordMatch]
WHERE WMID = @WMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByIDWithParent]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByIDWithParent] @MGID INT
AS
SELECT mm.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGWordMatch mm
INNER JOIN dbo.Minigame g ON mm.MGID = g.MGID
WHERE g.MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetByMGID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByMGID] @MGID INT
AS
SELECT *
FROM [MGWordMatch]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetRandomX]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_GetRandomX]    Script Date: 01/05/2015 14:43:22 ******/
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetRandomX] @WMID INT,
	@Num INT = 3
AS
DECLARE @SQL VARCHAR(8000)

SELECT @SQL = 'select top ' + convert(VARCHAR, @Num) + ' NEWID() as id, * from  dbo.MGWordMatchItems Where WMID = ' + convert(VARCHAR, @WMID) + '  order by id'

EXEC (@SQL)
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@NumOptionsToChooseFrom INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@WMID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGWordMatch (
		MGID,
		CorrectRoundsToWinCount,
		NumOptionsToChooseFrom,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@CorrectRoundsToWinCount,
		@NumOptionsToChooseFrom,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @WMID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Update] (
	@WMID INT,
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@NumOptionsToChooseFrom INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGWordMatch
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	NumOptionsToChooseFrom = @NumOptionsToChooseFrom,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE WMID = @WMID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Delete]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_Delete] @WMIID INT
AS
DELETE
FROM [MGWordMatchItems]
WHERE WMIID = @WMIID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetAll]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_GetAll] @MGID INT
AS
SELECT *
FROM [MGWordMatchItems]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_GetByID]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_GetByID] @WMIID INT
AS
SELECT *
FROM [MGWordMatchItems]
WHERE WMIID = @WMIID
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_Insert] (
	@WMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@WMIID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGWordMatchItems (
		WMID,
		MGID,
		ItemImage,
		EasyLabel,
		MediumLabel,
		HardLabel,
		AudioEasy,
		AudioMedium,
		AudioHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@WMID,
		@MGID,
		@ItemImage,
		@EasyLabel,
		@MediumLabel,
		@HardLabel,
		@AudioEasy,
		@AudioMedium,
		@AudioHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @WMIID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_MGWordMatchItems_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGWordMatchItems_Update] (
	@WMIID INT,
	@WMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGWordMatchItems
SET WMID = @WMID,
	MGID = @MGID,
	ItemImage = @ItemImage,
	EasyLabel = @EasyLabel,
	MediumLabel = @MediumLabel,
	HardLabel = @HardLabel,
	AudioEasy = @AudioEasy,
	AudioMedium = @AudioMedium,
	AudioHard = @AudioHard,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE WMIID = @WMIID
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Minigame_Delete] @MGID INT
AS
DELETE
FROM dbo.MGOnlineBookPages
WHERE MGID = @MGID

DELETE
FROM dbo.MGOnlineBook
WHERE MGID = @MGID

DELETE
FROM dbo.MGMixAndMatchItems
WHERE MGID = @MGID

DELETE
FROM dbo.MGMixAndMatch
WHERE MGID = @MGID

DELETE
FROM dbo.MGCodeBreakerKey
WHERE MGID = @MGID

DELETE
FROM dbo.MGCodeBreaker
WHERE MGID = @MGID

DELETE
FROM dbo.MGMatchingGameTiles
WHERE MGID = @MGID

DELETE
FROM dbo.MGMatchingGame
WHERE MGID = @MGID

DELETE
FROM dbo.MGHiddenPicBk
WHERE MGID = @MGID

DELETE
FROM dbo.MGHiddenPic
WHERE MGID = @MGID

DELETE
FROM dbo.MGChooseAdvSlides
WHERE MGID = @MGID

DELETE
FROM dbo.MGChooseAdv
WHERE MGID = @MGID

DELETE
FROM dbo.MGWordMatchItems
WHERE MGID = @MGID

DELETE
FROM dbo.MGWordMatch
WHERE MGID = @MGID

DELETE
FROM [Minigame]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Minigame_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Minigame]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Minigame_GetByID] @MGID INT
AS
SELECT *
FROM [Minigame]
WHERE MGID = @MGID
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_GetList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Minigame_GetList] @IDList VARCHAR(1000) = ''
AS
BEGIN
	DECLARE @tmp AS TABLE (
		Num INT identity,
		Value INT
		)

	INSERT INTO @tmp (value)
	SELECT Value
	FROM [dbo].[fnSplitBigInt](@IDList)

	--select * from @tmp
	SELECT m.*
	FROM Minigame m
	INNER JOIN @tmp t ON m.MGID = t.Value
	ORDER BY Num DESC
END
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Minigame_Insert] (
	@MiniGameType INT,
	@MiniGameTypeName VARCHAR(50),
	@AdminName VARCHAR(50),
	@GameName VARCHAR(50),
	@isActive BIT,
	@NumberPoints INT,
	@AwardedBadgeID INT,
	@Acknowledgements TEXT,
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
	@MGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Minigame (
		MiniGameType,
		MiniGameTypeName,
		AdminName,
		GameName,
		isActive,
		NumberPoints,
		AwardedBadgeID,
		Acknowledgements,
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
		FldText3
		)
	VALUES (
		@MiniGameType,
		@MiniGameTypeName,
		@AdminName,
		@GameName,
		@isActive,
		@NumberPoints,
		@AwardedBadgeID,
		@Acknowledgements,
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
		@FldText3
		)

	SELECT @MGID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Minigame_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Minigame_Update] (
	@MGID INT,
	@MiniGameType INT,
	@MiniGameTypeName VARCHAR(50),
	@AdminName VARCHAR(50),
	@GameName VARCHAR(50),
	@isActive BIT,
	@NumberPoints INT,
	@AwardedBadgeID INT,
	@Acknowledgements TEXT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE Minigame
SET MiniGameType = @MiniGameType,
	MiniGameTypeName = @MiniGameTypeName,
	AdminName = @AdminName,
	GameName = @GameName,
	isActive = @isActive,
	NumberPoints = @NumberPoints,
	AwardedBadgeID = @AwardedBadgeID,
	Acknowledgements = @Acknowledgements,
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
	FldText3 = @FldText3
WHERE MGID = @MGID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Notifications_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Notifications_Delete] @NID INT
AS
DELETE
FROM [Notifications]
WHERE NID = @NID
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Notifications_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Notifications]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AddedDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllFromPatron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllFromPatron]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetAllFromPatron] @PID INT
AS
SELECT *
FROM [Notifications]
WHERE PID_From = @PID
ORDER BY AddedDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllToOrFromPatron]    Script Date: 2/4/2016 13:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
WHERE (PID_To = @PID OR PID_From = @PID)
ORDER BY AddedDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllToPatron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetAllToPatron] @PID INT,
	@TenID INT = NULL
AS
SELECT n.*,
	isnull(p1.Username, 'System') AS ToUsername,
	isnull(p1.FirstName, 'System') AS ToFistName,
	isnull(p1.LastName, 'System') AS ToLastName,
	isnull(p2.Username, 'System') AS FromUsername,
	isnull(p2.FirstName, 'System') AS FromFistName,
	isnull(p2.LastName, 'System') AS FromLastName
FROM [Notifications] n
LEFT JOIN Patron p1 ON n.PID_To = p1.pid
LEFT JOIN Patron p2 ON n.PID_From = p2.pid
WHERE PID_To = @PID
	AND n.TenID = @TenID
ORDER BY AddedDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetAllUnreadToPatron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetAllUnreadToPatron] @PID INT,
	@TenID INT = NULL
AS
SELECT n.*,
	isnull(p1.Username, 'System') AS ToUsername,
	isnull(p1.FirstName, 'System') AS ToFistName,
	isnull(p1.LastName, 'System') AS ToLastName,
	isnull(p2.Username, 'System') AS FromUsername,
	isnull(p2.FirstName, 'System') AS FromFistName,
	isnull(p2.LastName, 'System') AS FromLastName
FROM [Notifications] n
LEFT JOIN Patron p1 ON n.PID_To = p1.pid
LEFT JOIN Patron p2 ON n.PID_From = p2.pid
WHERE PID_To = @PID
	AND n.TenID = @TenID
	AND n.isUnread = 1
ORDER BY AddedDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Notifications_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetByID] @NID INT
AS
SELECT *
FROM [Notifications]
WHERE NID = @NID
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Notifications_Insert] (
	@PID_To INT,
	@PID_From INT,
	@isQuestion BIT,
	@Subject VARCHAR(150),
	@Body TEXT,
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
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
	@isUnread BIT = 0,
	@NID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Notifications (
		PID_To,
		PID_From,
		isQuestion,
		Subject,
		Body,
		AddedDate,
		AddedUser,
		LastModDate,
		LastModUser,
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
		isUnread
		)
	VALUES (
		@PID_To,
		@PID_From,
		@isQuestion,
		@Subject,
		@Body,
		@AddedDate,
		@AddedUser,
		@LastModDate,
		@LastModUser,
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
		@isUnread
		)

	SELECT @NID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Notifications_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Notifications_Update] (
	@NID INT,
	@PID_To INT,
	@PID_From INT,
	@isQuestion BIT,
	@Subject VARCHAR(150),
	@Body TEXT,
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
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
	@isUnread BIT = 0
	)
AS
UPDATE Notifications
SET PID_To = @PID_To,
	PID_From = @PID_From,
	isQuestion = @isQuestion,
	Subject = @Subject,
	Body = @Body,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
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
	isUnread = @isUnread
WHERE NID = @NID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Offer_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Offer_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Offer_Delete] @OID INT
AS
DELETE
FROM [Offer]
WHERE OID = @OID
GO
/****** Object:  StoredProcedure [dbo].[app_Offer_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Offer_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT AdminName
		FROM dbo.Programs p
		WHERE o.ProgramId = p.PID
		) AS Program,
	(
		SELECT Code
		FROM dbo.Code c
		WHERE o.BranchId = c.CID
		) AS Branch
FROM [Offer] o
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Offer_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Offer_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Offer_GetByID] @OID INT
AS
SELECT *
FROM [Offer]
WHERE OID = @OID
GO
/****** Object:  StoredProcedure [dbo].[app_Offer_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Offer_Insert] (
	@isEnabled BIT,
	@AdminName VARCHAR(50),
	@Title VARCHAR(150),
	@ExternalRedirectFlag BIT,
	@RedirectURL VARCHAR(150),
	@MaxImpressions INT,
	@TotalImpressions INT,
	@SerialPrefix VARCHAR(50),
	@ZipCode VARCHAR(5),
	@AgeStart INT,
	@AgeEnd INT,
	@ProgramId INT,
	@BranchId INT,
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
	@OID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Offer (
		isEnabled,
		AdminName,
		Title,
		ExternalRedirectFlag,
		RedirectURL,
		MaxImpressions,
		TotalImpressions,
		SerialPrefix,
		ZipCode,
		AgeStart,
		AgeEnd,
		ProgramId,
		BranchId,
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
		FldText3
		)
	VALUES (
		@isEnabled,
		@AdminName,
		@Title,
		@ExternalRedirectFlag,
		@RedirectURL,
		@MaxImpressions,
		@TotalImpressions,
		@SerialPrefix,
		@ZipCode,
		@AgeStart,
		@AgeEnd,
		@ProgramId,
		@BranchId,
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
		@FldText3
		)

	SELECT @OID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Offer_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Offer_Update] (
	@OID INT,
	@isEnabled BIT,
	@AdminName VARCHAR(50),
	@Title VARCHAR(150),
	@ExternalRedirectFlag BIT,
	@RedirectURL VARCHAR(150),
	@MaxImpressions INT,
	@TotalImpressions INT,
	@SerialPrefix VARCHAR(50),
	@ZipCode VARCHAR(5),
	@AgeStart INT,
	@AgeEnd INT,
	@ProgramId INT,
	@BranchId INT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE Offer
SET isEnabled = @isEnabled,
	AdminName = @AdminName,
	Title = @Title,
	ExternalRedirectFlag = @ExternalRedirectFlag,
	RedirectURL = @RedirectURL,
	MaxImpressions = @MaxImpressions,
	TotalImpressions = @TotalImpressions,
	SerialPrefix = @SerialPrefix,
	ZipCode = @ZipCode,
	AgeStart = @AgeStart,
	AgeEnd = @AgeEnd,
	ProgramId = @ProgramId,
	BranchId = @BranchId,
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
	FldText3 = @FldText3
WHERE OID = @OID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Offers_GetForDisplay]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Offers_GetForDisplay] @PID INT = 0,
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
/****** Object:  StoredProcedure [dbo].[app_Patron_CanManageSubAccount]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Patron_CanManageSubAccount]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_CanManageSubAccount] @MainAccount INT = 0,
	@SubAccount INT = 0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Count INT,
	@UID INT

SELECT @Count = isnull(Count(*), 0)
FROM dbo.Patron
WHERE PID = @SubAccount
	AND MasterAcctPID = @MainAccount
GROUP BY PID

IF @Count = 0
	OR @Count IS NULL
BEGIN
	SELECT 0
END
ELSE
BEGIN
	SELECT 1
END

RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_CheckIfFinisher]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_CheckIfFinisher] @PID INT = NULL
AS
SELECT isnull(dbo.fx_IsFinisher(p.PID, p.ProgID), 0) AS IsFinisher
FROM Patron p
WHERE p.PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_Delete] @PID INT
AS
DELETE
FROM dbo.Notifications
WHERE PID_To = @PID
	OR PID_From = @PID

DELETE
FROM dbo.PatronBadges
WHERE PID = @PID

DELETE
FROM dbo.PatronBookLists
WHERE PID = @PID

DELETE
FROM dbo.PatronPoints
WHERE PID = @PID

DELETE
FROM dbo.PatronPrizes
WHERE PID = @PID

DELETE
FROM dbo.PatronReadingLog
WHERE PID = @PID

DELETE
FROM dbo.PatronReview
WHERE PID = @PID

DELETE
FROM dbo.PatronRewardCodes
WHERE PID = @PID

DELETE
FROM dbo.PrizeDrawingWinners
WHERE PatronID = @PID

UPDATE Patron
SET MasterAcctPID = 0
WHERE MasterAcctPID = @PID

DELETE
FROM [Patron]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Patron_GetAll] @TenID INT = NULL
AS
SELECT p.*,
	pg.AdminName AS Program
FROM [Patron] p
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE (
		p.TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetByEmail]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Patron_GetByEmail]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_GetByEmail] @Email VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM Patron
WHERE EmailAddress = @Email
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Patron_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Patron_GetByID] @PID INT
AS
SELECT *
FROM [Patron]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetByUsername]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Patron_GetByUsername] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM Patron
WHERE Username = @Username
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetPatronForEdit]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_GetPatronForEdit] @PID INT = 0,
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
	isNull(p.AvatarID, 0) AS [AvatarID],
	isNull(p.RegistrationDate, NULL) AS [RegistrationDate],
	ISNULL(p.SDistrict, 0) AS SDistrict,
	rs.*
FROM dbo.Patron p
RIGHT JOIN RegistrationSettings rs ON p.PID = @PID
WHERE rs.TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetScoreRank]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_GetScoreRank] @PID INT,
	@WhichScore INT = 1
AS
IF @WhichScore = 1
BEGIN
	SELECT convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
					AND p1.Score1 <= p.Score1
				) - 1) * 100.00 / convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
				)) AS Percentile,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
			) AS T,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
				AND p1.Score1 <= p.Score1
			) - 1 AS R,
		Score1 AS Score
	FROM Patron p
	WHERE p.PID = @PID
END
ELSE
BEGIN
	SELECT convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
					AND p1.Score2 <= p.Score2
				) - 1) * 100.00 / convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
				)) AS Percentile,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
			) AS T,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
				AND p1.Score2 <= p.Score2
			) - 1 AS R,
		Score2 AS Score
	FROM Patron p
	WHERE p.PID = @PID
END
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_GetSubAccountList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Patron_GetSubAccountList]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_Patron_GetSubAccountList] @PID INT = 0
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
	--order BY subs.PID desc
GO
/****** Object:  StoredProcedure [dbo].[app_Patron_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_Insert] (
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
	@AvatarID INT,
	@SDistrict INT,
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
		AvatarID,
		SDistrict,
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
		@AvatarID,
		@SDistrict,
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
/****** Object:  StoredProcedure [dbo].[app_Patron_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Patron_Update] (
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
	@AvatarID INT,
	@SDistrict INT,
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
	AvatarID = @AvatarID,
	SDistrict = @SDistrict,
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
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Delete] @PBID INT
AS
DELETE
FROM [PatronBadges]
WHERE PBID = @PBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_PatronBadges_GetAll] @PID INT = 0
AS
SELECT ROW_NUMBER() OVER (
		ORDER BY DateEarned,
			PBID
		) AS Rank,
	pb.*,
	b.UserName AS Title
FROM [PatronBadges] pb
LEFT JOIN Badge b ON pb.BadgeID = b.BID
WHERE PID = @PID
ORDER BY Rank DESC
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBadges_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBadges_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_GetByID] @PBID INT
AS
SELECT *
FROM [PatronBadges]
WHERE PBID = @PBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Insert] (
	@PID INT,
	@BadgeID INT,
	@DateEarned DATETIME,
	@PBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronBadges (
		PID,
		BadgeID,
		DateEarned
		)
	VALUES (
		@PID,
		@BadgeID,
		@DateEarned
		)

	SELECT @PBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Update] (
	@PBID INT,
	@PID INT,
	@BadgeID INT,
	@DateEarned DATETIME
	)
AS
UPDATE PatronBadges
SET PID = @PID,
	BadgeID = @BadgeID,
	DateEarned = @DateEarned
WHERE PBID = @PBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Delete] @PBLBID INT
AS
DELETE
FROM [PatronBookLists]
WHERE PBLBID = @PBLBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetAll]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_GetAll]
AS
SELECT *
FROM [PatronBookLists]
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_GetByID] @PBLBID INT
AS
SELECT *
FROM [PatronBookLists]
WHERE PBLBID = @PBLBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Insert] (
	@PID INT,
	@BLBID INT,
	@BLID INT,
	@HasReadFlag BIT,
	@LastModDate DATETIME,
	@PBLBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronBookLists (
		PID,
		BLBID,
		BLID,
		HasReadFlag,
		LastModDate
		)
	VALUES (
		@PID,
		@BLBID,
		@BLID,
		@HasReadFlag,
		@LastModDate
		)

	SELECT @PBLBID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Update] (
	@PBLBID INT,
	@PID INT,
	@BLBID INT,
	@BLID INT,
	@HasReadFlag BIT,
	@LastModDate DATETIME
	)
AS
UPDATE PatronBookLists
SET PID = @PID,
	BLBID = @BLBID,
	BLID = @BLID,
	HasReadFlag = @HasReadFlag,
	LastModDate = @LastModDate
WHERE PBLBID = @PBLBID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Delete] @PPID INT
AS
DELETE
FROM [PatronPoints]
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_GetAll] @PID INT = 0
AS
SELECT pp.*,
	ISNULL(pl.Author, '') AS Author,
	ISNULL(pl.Title, '') AS Title,
	ISNULL(pr.Review, '') AS Review,
	ISNULL(isApproved, 0) AS isApproved,
	ISNULL(ev.EventTitle, '') AS EventTitle,
	ISNULL(bl.ListName, '') AS ListName,
	ISNULL(mg.GameName, '') AS GameName,
	ISNULL(pr.PRID, 0) AS PRID,
	ISNULL(pl.ReadingTypeLabel, '') ReadingType,
	ISNULL(pl.ReadingAmount, '') ReadingAmount
FROM [PatronPoints] pp
LEFT JOIN [PatronReadingLog] pl ON pl.PRLID = pp.LogID
LEFT JOIN [PatronReview] pr ON pl.PRLID = pr.PRLID
LEFT JOIN [Event] ev ON pp.EventID = ev.EID
LEFT JOIN [BookList] bl ON pp.BookListID = bl.BLID
LEFT JOIN [Minigame] mg ON pp.GameLevelActivityID = mg.MGID
WHERE @PID = pp.PID
ORDER BY AwardDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_GetByID] @PPID INT
AS
SELECT *
FROM [PatronPoints]
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetLastPatronEntryID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_PatronPoints_GetLastPatronEntryID] @PID INT
AS
SELECT isnull(MAX(PPID), 0)
FROM [PatronPoints]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsBookList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsBookList]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsBookList] (
	@PID INT,
	@BLID INT = 0
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND BookListID = @BLID
		AND AwardReasonCd = 2
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByKeyword]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByKeyword]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsByKeyword] (
	@PID INT,
	@Key VARCHAR(50) = ''
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND EventCode = @Key
		AND AwardReasonCd = 1
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByMGID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByMGID]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsByMGID] (
	@PID INT,
	@MGID INT = 0
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND GameLevelActivityID = @MGID
		AND AwardReasonCd = 4
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetProgramLeaderboard]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_PatronPoints_GetProgramLeaderboard] @ProgId INT = 0,
	@TenID INT = NULL
AS
IF @TenID IS NULL
	SELECT @TenID = TenID
	FROM Programs
	WHERE PID = @ProgId

SELECT TOP 10 pp.PID,
	isnull(SUM(isnull(convert(BIGINT, NumPoints), 0)), 0) AS TotalPoints,
	p.Username,
	p.AvatarID
INTO #TempLB
FROM PatronPoints pp
INNER JOIN Patron p ON pp.PID = p.PID
	AND p.TenID = @TenID
WHERE p.ProgID = @ProgId
GROUP BY pp.PID,
	p.Username,
	p.AvatarID
ORDER BY TotalPoints DESC

UPDATE #TempLB
SET TotalPoints = 20000000
WHERE TotalPoints > 20000000

SELECT PID,
	Username,
	AvatarID,
	CONVERT(INT, TotalPoints) AS TotalPoints,
	ROW_NUMBER() OVER (
		ORDER BY TotalPoints DESC
		) AS Rank
FROM #TempLB
ORDER BY TotalPoints DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetTotalPatronPoints]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetTotalPatronPoints]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_GetTotalPatronPoints] (@PID INT)
AS
BEGIN
	IF (
			EXISTS (
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
				FROM PatronPoints
				WHERE PID = @PID
				)
			)
		SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
		FROM PatronPoints
		WHERE PID = @PID
	ELSE
		SELECT 0 AS TotalPoints
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetTotalPatronPointsOnDate]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_PatronPoints_GetTotalPatronPointsOnDate] (
	@PID INT,
	@Date DATETIME = NULL
	)
AS
BEGIN
	IF @Date IS NULL
		SELECT @Date = GETDATE()

	IF (
			EXISTS (
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
				FROM PatronPoints
				WHERE PID = @PID
				)
			)
		--And convert(varchar, AwardDate, 101) = convert(varchar, @Date, 101)
		SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
		FROM PatronPoints
		WHERE PID = @PID
			AND convert(VARCHAR, AwardDate, 101) = convert(VARCHAR, @Date, 101)
	ELSE
		SELECT 0 AS TotalPoints
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Insert] (
	@PID INT,
	@NumPoints INT,
	@AwardDate DATETIME,
	@AwardReason VARCHAR(50),
	@AwardReasonCd INT,
	@BadgeAwardedFlag BIT,
	@BadgeID INT,
	@PBID INT,
	@isReading BIT,
	@LogID INT,
	@isEvent BIT,
	@EventID INT,
	@EventCode VARCHAR(50),
	@isBookList BIT,
	@BookListID INT,
	@isGame BIT,
	@isGameLevelActivity BIT,
	@GameID INT,
	@GameLevel INT,
	@GameLevelID INT,
	@GameLevelActivityID INT,
	@PPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronPoints (
		PID,
		NumPoints,
		AwardDate,
		AwardReason,
		AwardReasonCd,
		BadgeAwardedFlag,
		BadgeID,
		PBID,
		isReading,
		LogID,
		isEvent,
		EventID,
		EventCode,
		isBookList,
		BookListID,
		isGame,
		isGameLevelActivity,
		GameID,
		GameLevel,
		GameLevelID,
		GameLevelActivityID
		)
	VALUES (
		@PID,
		@NumPoints,
		@AwardDate,
		@AwardReason,
		@AwardReasonCd,
		@BadgeAwardedFlag,
		@BadgeID,
		@PBID,
		@isReading,
		@LogID,
		@isEvent,
		@EventID,
		@EventCode,
		@isBookList,
		@BookListID,
		@isGame,
		@isGameLevelActivity,
		@GameID,
		@GameLevel,
		@GameLevelID,
		@GameLevelActivityID
		)

	SELECT @PPID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Update] (
	@PPID INT,
	@PID INT,
	@NumPoints INT,
	@AwardDate DATETIME,
	@AwardReason VARCHAR(50),
	@AwardReasonCd INT,
	@BadgeAwardedFlag BIT,
	@BadgeID INT,
	@PBID INT,
	@isReading BIT,
	@LogID INT,
	@isEvent BIT,
	@EventID INT,
	@EventCode VARCHAR(50),
	@isBookList BIT,
	@BookListID INT,
	@isGame BIT,
	@isGameLevelActivity BIT,
	@GameID INT,
	@GameLevel INT,
	@GameLevelID INT,
	@GameLevelActivityID INT
	)
AS
UPDATE PatronPoints
SET PID = @PID,
	NumPoints = @NumPoints,
	AwardDate = @AwardDate,
	AwardReason = @AwardReason,
	AwardReasonCd = @AwardReasonCd,
	BadgeAwardedFlag = @BadgeAwardedFlag,
	BadgeID = @BadgeID,
	PBID = @PBID,
	isReading = @isReading,
	LogID = @LogID,
	isEvent = @isEvent,
	EventID = @EventID,
	EventCode = @EventCode,
	isBookList = @isBookList,
	BookListID = @BookListID,
	isGame = @isGame,
	isGameLevelActivity = @isGameLevelActivity,
	GameID = @GameID,
	GameLevel = @GameLevel,
	GameLevelID = @GameLevelID,
	GameLevelActivityID = @GameLevelActivityID
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Delete]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_Delete] @PPID INT
AS
DELETE
FROM [PatronPrizes]
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetAll]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetAll] @PID INT
AS
SELECT pp.*,
	ISNULL(b.AdminName, '') AS Badge
FROM [PatronPrizes] pp
LEFT JOIN Badge b ON pp.BadgeID = b.BID
WHERE pp.PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByDrawingID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByDrawingID]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetByDrawingID] @DrawingID INT
AS
SELECT *
FROM [PatronPrizes]
WHERE DrawingID = @DrawingID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetByID]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetByID] @PPID INT
AS
SELECT *
FROM [PatronPrizes]
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_Insert] (
	@PID INT,
	@PrizeSource INT,
	@BadgeID INT,
	@DrawingID INT,
	@PrizeName VARCHAR(50),
	@RedeemedFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronPrizes (
		PID,
		PrizeSource,
		BadgeID,
		DrawingID,
		PrizeName,
		RedeemedFlag,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PID,
		@PrizeSource,
		@BadgeID,
		@DrawingID,
		@PrizeName,
		@RedeemedFlag,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PPID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_Update] (
	@PPID INT,
	@PID INT,
	@PrizeSource INT,
	@BadgeID INT,
	@DrawingID INT,
	@PrizeName VARCHAR(50),
	@RedeemedFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PatronPrizes
SET PID = @PID,
	PrizeSource = @PrizeSource,
	BadgeID = @BadgeID,
	DrawingID = @DrawingID,
	PrizeName = @PrizeName,
	RedeemedFlag = @RedeemedFlag,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PPID = @PPID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Delete] @PRLID INT
AS
DELETE
FROM [PatronReadingLog]
WHERE PRLID = @PRLID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_GetAll] @PID INT = 0
AS
SELECT *
FROM [PatronReadingLog]
WHERE @PID = PID
ORDER BY LoggingDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_GetByID] @PRLID INT
AS
SELECT *
FROM [PatronReadingLog]
WHERE PRLID = @PRLID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Insert] (
	@PID INT,
	@ReadingType INT,
	@ReadingTypeLabel VARCHAR(50),
	@ReadingAmount INT,
	@ReadingPoints INT,
	@LoggingDate VARCHAR(50),
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@HasReview BIT,
	@ReviewID INT,
	@PRLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronReadingLog (
		PID,
		ReadingType,
		ReadingTypeLabel,
		ReadingAmount,
		ReadingPoints,
		LoggingDate,
		Author,
		Title,
		HasReview,
		ReviewID
		)
	VALUES (
		@PID,
		@ReadingType,
		@ReadingTypeLabel,
		@ReadingAmount,
		@ReadingPoints,
		@LoggingDate,
		@Author,
		@Title,
		@HasReview,
		@ReviewID
		)

	SELECT @PRLID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Update] (
	@PRLID INT,
	@PID INT,
	@ReadingType INT,
	@ReadingTypeLabel VARCHAR(50),
	@ReadingAmount INT,
	@ReadingPoints INT,
	@LoggingDate VARCHAR(50),
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@HasReview BIT,
	@ReviewID INT
	)
AS
UPDATE PatronReadingLog
SET PID = @PID,
	ReadingType = @ReadingType,
	ReadingTypeLabel = @ReadingTypeLabel,
	ReadingAmount = @ReadingAmount,
	ReadingPoints = @ReadingPoints,
	LoggingDate = @LoggingDate,
	Author = @Author,
	Title = @Title,
	HasReview = @HasReview,
	ReviewID = @ReviewID
WHERE PRLID = @PRLID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Delete] @PRID INT
AS
DELETE
FROM [PatronReview]
WHERE PRID = @PRID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetAll] @pid INT = 0
AS
SELECT *
FROM [PatronReview]
WHERE @PID = PID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetByID] @PRID INT
AS
SELECT *
FROM [PatronReview]
WHERE PRID = @PRID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByLogID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_GetByLogID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronReview_GetByLogID] @PRLID INT
AS
SELECT *
FROM [PatronReview]
WHERE PRLID = @PRLID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Insert] (
	@PID INT,
	@PRLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@Review TEXT,
	@isApproved BIT,
	@ReviewDate DATETIME,
	@ApprovalDate DATETIME,
	@ApprovedBy VARCHAR(50),
	@PRID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronReview (
		PID,
		PRLID,
		Author,
		Title,
		Review,
		isApproved,
		ReviewDate,
		ApprovalDate,
		ApprovedBy
		)
	VALUES (
		@PID,
		@PRLID,
		@Author,
		@Title,
		@Review,
		@isApproved,
		@ReviewDate,
		@ApprovalDate,
		@ApprovedBy
		)

	SELECT @PRID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronReview_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Update] (
	@PRID INT,
	@PID INT,
	@PRLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@Review TEXT,
	@isApproved BIT,
	@ReviewDate DATETIME,
	@ApprovalDate DATETIME,
	@ApprovedBy VARCHAR(50)
	)
AS
UPDATE PatronReview
SET PID = @PID,
	PRLID = @PRLID,
	Author = @Author,
	Title = @Title,
	Review = @Review,
	isApproved = @isApproved,
	ReviewDate = @ReviewDate,
	ApprovalDate = @ApprovalDate,
	ApprovedBy = @ApprovedBy
WHERE PRID = @PRID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Delete] @PRCID INT
AS
DELETE
FROM [PatronRewardCodes]
WHERE PRCID = @PRCID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_GetAll] @PID INT
AS
SELECT *
FROM [PatronRewardCodes]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_GetByID] @PRCID INT
AS
SELECT *
FROM [PatronRewardCodes]
WHERE PRCID = @PRCID
GO
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Insert] (
	@PID INT,
	@BadgeID INT,
	@ProgID INT,
	@RewardCode VARCHAR(100),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PRCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronRewardCodes (
		PID,
		BadgeID,
		ProgID,
		RewardCode,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PID,
		@BadgeID,
		@ProgID,
		@RewardCode,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PRCID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PatronRewardCodes_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronRewardCodes_Update] (
	@PRCID INT,
	@PID INT,
	@BadgeID INT,
	@ProgID INT,
	@RewardCode VARCHAR(100),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PatronRewardCodes
SET PID = @PID,
	BadgeID = @BadgeID,
	ProgID = @ProgID,
	RewardCode = @RewardCode,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PRCID = @PRCID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_Delete] @PDID INT
AS
DELETE
FROM [PrizeDrawing]
WHERE PDID = @PDID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_DrawWinners]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_PrizeDrawing_DrawWinners] @PDID INT = 0,
	@NumWinners INT = 1,
	@Additional INT = 0
AS
DECLARE @TID INT
DECLARE @TenID INT

SELECT @TID = TID,
	@TenID = TenID
FROM dbo.PrizeDrawing
WHERE PDID = @PDID

DECLARE @Gender VARCHAR(1),
	@SchoolName VARCHAR(50)
DECLARE @ProgID INT,
	@PrimaryLibrary INT
DECLARE @IncPrevWinnersFlag BIT
DECLARE @MinP INT,
	@MaxP INT,
	@MinR INT,
	@MaxR INT

SELECT @ProgID = ProgID,
	@PrimaryLibrary = PrimaryLibrary,
	@Gender = Gender,
	@SchoolName = SchoolName,
	@IncPrevWinnersFlag = IncPrevWinnersFlag,
	@MinP = MinPoints,
	@MaxP = MaxPoints,
	@MinR = MinReviews,
	@MaxR = MaxReviews
FROM dbo.PrizeTemplate
WHERE TID = @TID

SELECT PID,
	p.ProgID,
	p.PrimaryLibrary,
	p.SchoolName,
	p.Gender,
	isnull((
			SELECT SUM(NumPoints)
			FROM dbo.PatronPoints pp
			WHERE pp.PID = p.PID
				AND (
					AwardDate >= t.LogDateStart
					OR t.LogDateStart IS NULL
					)
				AND (
					AwardDate <= t.LogDateEnd
					OR t.LogDateEnd IS NULL
					)
			), 0) AS PatronPoints,
	isnull((
			SELECT count(PRID)
			FROM dbo.PatronReview pr
			WHERE pr.PID = p.PID
				AND (
					ReviewDate >= t.ReviewDateStart
					OR t.ReviewDateStart IS NULL
					)
				AND (
					ReviewDate <= t.ReviewDateEnd
					OR t.ReviewDateEnd IS NULL
					)
			), 0) AS PatronReviews,
	NEWID() AS Random
INTO #TEMP
FROM Patron p
INNER JOIN dbo.PrizeTemplate t ON t.TID = @TID
WHERE p.TenID = @TenID

IF (@ProgID <> 0)
	DELETE
	FROM #TEMP
	WHERE ProgID <> @ProgID

IF (@PrimaryLibrary <> 0)
	DELETE
	FROM #TEMP
	WHERE PrimaryLibrary <> @PrimaryLibrary

IF (@Gender <> '')
	DELETE
	FROM #TEMP
	WHERE Gender <> @Gender

IF (@SchoolName <> '')
	DELETE
	FROM #TEMP
	WHERE SchoolName <> @SchoolName

IF (@MinP <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronPoints < @MinP

IF (@MaxP <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronPoints > @MaxP

IF (@MinR <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronReviews < @MinR

IF (@MaxR <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronReviews > @MaxR

IF (@IncPrevWinnersFlag = 0)
	DELETE
	FROM #TEMP
	WHERE PID IN (
			SELECT DISTINCT PatronID
			FROM dbo.PrizeDrawingWinners
			)

IF (@Additional = 1)
	DELETE
	FROM #TEMP
	WHERE PID IN (
			SELECT DISTINCT PatronID
			FROM dbo.PrizeDrawingWinners
			WHERE PDID = @PDID
			)

INSERT INTO PrizeDrawingWinners (
	PDID,
	PatronID,
	NotificationID,
	PrizePickedUpFlag,
	LastModDate,
	LastModUser,
	AddedDate,
	AddedUser
	)
SELECT TOP (@NumWinners) @PDID,
	PID,
	0,
	0,
	GETDATE(),
	'N/A',
	GETDATE(),
	'N/A'
FROM #TEMP
ORDER BY Random

SELECT *
FROM PrizeDrawingWinners
WHERE PDID = @PDID
	AND NotificationID = 0
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetAll] @TenID INT = NULL
AS
SELECT pd.*,
	t.TName
FROM [PrizeDrawing] pd
LEFT JOIN PrizeTemplate t ON pd.TID = t.TID
	AND pd.TenID = t.TenID
WHERE pd.TenID = @TenID
ORDER BY PDID DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetAllWinners]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetAllWinners]    Script Date: 01/05/2015 14:43:24 ******/
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetAllWinners] @PDID INT = 0
AS
SELECT pdw.*,
	p.Username,
	p.FirstName,
	p.LastName
FROM dbo.PrizeDrawingWinners pdw
LEFT JOIN Patron p ON pdw.PatronID = p.PID
WHERE PDID = @PDID
ORDER BY PDID DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_GetByID] @PDID INT
AS
SELECT *
FROM [PrizeDrawing]
WHERE PDID = @PDID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_Insert] (
	@PrizeName VARCHAR(250),
	@TID INT,
	@DrawingDateTime DATETIME,
	@NumWinners INT,
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
	@PDID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PrizeDrawing (
		PrizeName,
		TID,
		DrawingDateTime,
		NumWinners,
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
		FldText3
		)
	VALUES (
		@PrizeName,
		@TID,
		@DrawingDateTime,
		@NumWinners,
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
		@FldText3
		)

	SELECT @PDID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawing_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawing_Update] (
	@PDID INT,
	@PrizeName VARCHAR(250),
	@TID INT,
	@DrawingDateTime DATETIME,
	@NumWinners INT,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE PrizeDrawing
SET PrizeName = @PrizeName,
	TID = @TID,
	DrawingDateTime = @DrawingDateTime,
	NumWinners = @NumWinners,
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
	FldText3 = @FldText3
WHERE PDID = @PDID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Delete] @PDWID INT
AS
DELETE
FROM [PrizeDrawingWinners]
WHERE PDWID = @PDWID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_GetAll]
AS
SELECT *
FROM [PrizeDrawingWinners]
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_GetByID] @PDWID INT
AS
SELECT *
FROM [PrizeDrawingWinners]
WHERE PDWID = @PDWID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Insert] (
	@PDID INT,
	@PatronID INT,
	@NotificationID INT,
	@PrizePickedUpFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PDWID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PrizeDrawingWinners (
		PDID,
		PatronID,
		NotificationID,
		PrizePickedUpFlag,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PDID,
		@PatronID,
		@NotificationID,
		@PrizePickedUpFlag,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PDWID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Update] (
	@PDWID INT,
	@PDID INT,
	@PatronID INT,
	@NotificationID INT,
	@PrizePickedUpFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PrizeDrawingWinners
SET PDID = @PDID,
	PatronID = @PatronID,
	NotificationID = @NotificationID,
	PrizePickedUpFlag = @PrizePickedUpFlag,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PDWID = @PDWID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_Delete] @TID INT
AS
DELETE
FROM [PrizeTemplate]
WHERE TID = @TID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_GetAll] @TenID INT
AS
SELECT t.*,
	ISNULL(p.AdminName, '') AS ProgName,
	ISNULL(c.Code, '') AS Library
FROM [PrizeTemplate] t
LEFT JOIN Programs p ON t.ProgID = p.PID
LEFT JOIN Code c ON t.PrimaryLibrary = c.CID
WHERE (
		t.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY TID DESC
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_GetByID] @TID INT
AS
SELECT *
FROM [PrizeTemplate]
WHERE TID = @TID
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_Insert] (
	@TName VARCHAR(150),
	@NumPrizes INT,
	@IncPrevWinnersFlag BIT,
	@SendNotificationFlag BIT,
	@NotificationSubject VARCHAR(250),
	@NotificationMessage TEXT,
	@ProgID INT,
	@Gender VARCHAR(1),
	@SchoolName VARCHAR(50),
	@PrimaryLibrary INT,
	@MinPoints INT,
	@MaxPoints INT,
	@LogDateStart DATETIME,
	@LogDateEnd DATETIME,
	@MinReviews INT,
	@MaxReviews INT,
	@ReviewDateStart DATETIME,
	@ReviewDateEnd DATETIME,
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
	@TID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PrizeTemplate (
		TName,
		NumPrizes,
		IncPrevWinnersFlag,
		SendNotificationFlag,
		NotificationSubject,
		NotificationMessage,
		ProgID,
		Gender,
		SchoolName,
		PrimaryLibrary,
		MinPoints,
		MaxPoints,
		LogDateStart,
		LogDateEnd,
		MinReviews,
		MaxReviews,
		ReviewDateStart,
		ReviewDateEnd,
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
		FldText3
		)
	VALUES (
		@TName,
		@NumPrizes,
		@IncPrevWinnersFlag,
		@SendNotificationFlag,
		@NotificationSubject,
		@NotificationMessage,
		@ProgID,
		@Gender,
		@SchoolName,
		@PrimaryLibrary,
		@MinPoints,
		@MaxPoints,
		@LogDateStart,
		@LogDateEnd,
		@MinReviews,
		@MaxReviews,
		@ReviewDateStart,
		@ReviewDateEnd,
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
		@FldText3
		)

	SELECT @TID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_PrizeTemplate_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PrizeTemplate_Update] (
	@TID INT,
	@TName VARCHAR(150),
	@NumPrizes INT,
	@IncPrevWinnersFlag BIT,
	@SendNotificationFlag BIT,
	@NotificationSubject VARCHAR(250),
	@NotificationMessage TEXT,
	@ProgID INT,
	@Gender VARCHAR(1),
	@SchoolName VARCHAR(50),
	@PrimaryLibrary INT,
	@MinPoints INT,
	@MaxPoints INT,
	@LogDateStart DATETIME,
	@LogDateEnd DATETIME,
	@MinReviews INT,
	@MaxReviews INT,
	@ReviewDateStart DATETIME,
	@ReviewDateEnd DATETIME,
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
	@FldText3 TEXT = ''
	)
AS
UPDATE PrizeTemplate
SET TName = @TName,
	NumPrizes = @NumPrizes,
	IncPrevWinnersFlag = @IncPrevWinnersFlag,
	SendNotificationFlag = @SendNotificationFlag,
	NotificationSubject = @NotificationSubject,
	NotificationMessage = @NotificationMessage,
	ProgID = @ProgID,
	Gender = @Gender,
	SchoolName = @SchoolName,
	PrimaryLibrary = @PrimaryLibrary,
	MinPoints = @MinPoints,
	MaxPoints = @MaxPoints,
	LogDateStart = @LogDateStart,
	LogDateEnd = @LogDateEnd,
	MinReviews = @MinReviews,
	MaxReviews = @MaxReviews,
	ReviewDateStart = @ReviewDateStart,
	ReviewDateEnd = @ReviewDateEnd,
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
	FldText3 = @FldText3
WHERE TID = @TID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_AssignCodeForPatron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_AssignCodeForPatron] (
	@PID INT,
	@PatronId INT
	)
AS
DECLARE @PCID INT

SELECT TOP 1 @PCID = PCID
FROM ProgramCodes
WHERE PID = @PID
	AND isUsed = 0
ORDER BY PCID

UPDATE ProgramCodes
SET isUsed = 1,
	DateUsed = GETDATE(),
	PatronId = @PatronId
WHERE PCID = @PCID

SELECT *,
	0
FROM ProgramCodes
WHERE PCID = @PCID

UNION

SELECT *,
	1
FROM ProgramCodes
WHERE PCID = @PCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_Delete] @PCID INT
AS
DELETE
FROM [ProgramCodes]
WHERE PCID = @PCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Generate]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_Generate] @start INT = 1,
	@end INT = 10000,
	@PID INT = 1
AS
-- generate x rows/numbers
--DECLARE @start INT = 1;
--DECLARE @end INT = 10000;
WITH numbers
AS (
	SELECT @start AS Number,
		NEWID() AS Code
	WHERE Substring(CONVERT(VARCHAR(36), NEWID()), 4, 20) NOT IN (
			SELECT ShortCode
			FROM ProgramCodes
			)
	
	UNION ALL
	
	SELECT number + 1,
		NEWID()
	FROM numbers
	WHERE number < @end
	)
INSERT INTO ProgramCodes (
	PID,
	CodeNumber,
	CodeValue,
	isUsed,
	DateCreated,
	DateUsed,
	PatronId,
	ShortCode
	)
SELECT @PID,
	Number,
	Code,
	0,
	GETDATE(),
	NULL,
	0,
	Substring(CONVERT(VARCHAR(36), Code), 4, 20)
FROM numbers
OPTION (MAXRECURSION 0)
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAll]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAll]
AS
SELECT *
FROM [ProgramCodes]
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllByProgram]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllByProgram]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllByProgram] @PID INT
AS
SELECT *
FROM [ProgramCodes]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllForPatron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetAllForPatron]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllForPatron] @PID INT
AS
SELECT *
FROM [ProgramCodes]
WHERE PatronId = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetByID] @PCID INT
AS
SELECT *
FROM [ProgramCodes]
WHERE PCID = @PCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_GetExportList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_GetExportList] @PID INT = 1
AS
SELECT CodeNumber AS "Code Number",
	ShortCode AS "Code Value",
	CASE isUsed
		WHEN 1
			THEN 'Yes'
		ELSE 'No'
		END AS "Code Was Assigned",
	DateUsed AS "Date Used",
	p.FirstName AS "Assigned to First Name",
	p.LastName AS "Assigned to Last Name",
	p.Username AS "Assigned to Username"
FROM ProgramCodes pc
LEFT JOIN Patron p ON pc.PatronId = p.PID
WHERE pc.PID = @PID
ORDER BY PCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_Insert] (
	@PID INT,
	@CodeNumber INT,
	@CodeValue UNIQUEIDENTIFIER,
	@ShortCode VARCHAR(20) = '',
	@isUsed BIT,
	@DateCreated DATETIME,
	@DateUsed DATETIME,
	@PatronId INT,
	@PCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramCodes (
		PID,
		CodeNumber,
		CodeValue,
		ShortCode,
		isUsed,
		DateCreated,
		DateUsed,
		PatronId
		)
	VALUES (
		@PID,
		@CodeNumber,
		@CodeValue,
		@ShortCode,
		@isUsed,
		@DateCreated,
		@DateUsed,
		@PatronId
		)

	SELECT @PCID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Stats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_Stats] @PID INT = 1
AS
SELECT isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
			), 0) AS TotalCodes,
	isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 1
			), 0) AS UsedCodes,
	isnull((
			SELECT COUNT(*)
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 0
			), 0) AS RemainingCodes
	--,isnull((select Top 1 convert(varchar(64),CodeValue) from ProgramCodes where PID = @PID and isUsed=1 order by PCID desc),'') as LastUsedCode
	,
	isnull((
			SELECT TOP 1 ShortCode
			FROM ProgramCodes
			WHERE PID = @PID
				AND isUsed = 1
			ORDER BY PCID DESC
			), '') AS LastUsedCode
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramCodes_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramCodes_Update] (
	@PCID INT,
	@PID INT,
	@CodeNumber INT,
	@CodeValue UNIQUEIDENTIFIER,
	@ShortCode VARCHAR(20) = '',
	@isUsed BIT,
	@DateCreated DATETIME,
	@DateUsed DATETIME,
	@PatronId INT
	)
AS
UPDATE ProgramCodes
SET PID = @PID,
	CodeNumber = @CodeNumber,
	CodeValue = @CodeValue,
	ShortCode = @ShortCode,
	isUsed = @isUsed,
	DateCreated = @DateCreated,
	DateUsed = @DateUsed,
	PatronId = @PatronId
WHERE PCID = @PCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGame_Delete]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_Delete] @PGID INT
AS
DELETE ProgramGameLevel
WHERE PGID = @PGID

DELETE
FROM [ProgramGame]
WHERE PGID = @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [ProgramGame]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGame_GetByID]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_GetByID] @PGID INT
AS
SELECT *
FROM [ProgramGame]
WHERE PGID = @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_Insert] (
	@GameName VARCHAR(50),
	@MapImage VARCHAR(50),
	@BonusMapImage VARCHAR(50),
	@BoardWidth INT,
	@BoardHeight INT,
	@BonusLevelPointMultiplier MONEY,
	@LevelCompleteImage VARCHAR(50),
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
	@Minigame1ID INT = 0,
	@Minigame2ID INT = 0,
	@PGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGame (
		GameName,
		MapImage,
		BonusMapImage,
		BoardWidth,
		BoardHeight,
		BonusLevelPointMultiplier,
		LevelCompleteImage,
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
		Minigame1ID,
		Minigame2ID
		)
	VALUES (
		@GameName,
		@MapImage,
		@BonusMapImage,
		@BoardWidth,
		@BoardHeight,
		@BonusLevelPointMultiplier,
		@LevelCompleteImage,
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
		@Minigame1ID,
		@Minigame2ID
		)

	SELECT @PGID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGame_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_Update] (
	@PGID INT,
	@GameName VARCHAR(50),
	@MapImage VARCHAR(50),
	@BonusMapImage VARCHAR(50),
	@BoardWidth INT,
	@BoardHeight INT,
	@BonusLevelPointMultiplier MONEY,
	@LevelCompleteImage VARCHAR(50),
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
	@Minigame1ID INT = 0,
	@Minigame2ID INT = 0
	)
AS
UPDATE ProgramGame
SET GameName = @GameName,
	MapImage = @MapImage,
	BonusMapImage = @BonusMapImage,
	BoardWidth = @BoardWidth,
	BoardHeight = @BoardHeight,
	BonusLevelPointMultiplier = @BonusLevelPointMultiplier,
	LevelCompleteImage = @LevelCompleteImage,
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
	Minigame1ID = @Minigame1ID,
	Minigame2ID = @Minigame2ID
WHERE PGID = @PGID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Delete]    Script Date: 01/05/2015 14:43:24 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Delete] @PGLID INT
AS
DECLARE @PGID INT

SELECT @PGID = PGID
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID

DELETE
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID

EXEC [app_ProgramGameLevel_Reorder] @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_GetAll] @PGID INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(LevelNumber), 1)
		FROM [ProgramGameLevel]
		WHERE PGID = @PGID
		) AS MAX
FROM [ProgramGameLevel]
WHERE PGID = @PGID
ORDER BY LevelNumber
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_GetByID] @PGLID INT
AS
SELECT *
FROM [ProgramGameLevel]
WHERE PGLID = @PGLID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Insert]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Insert] (
	@PGID INT,
	@LevelNumber INT,
	@LocationX INT,
	@LocationY INT,
	@PointNumber INT,
	@Minigame1ID INT,
	@Minigame2ID INT,
	@AwardBadgeID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LocationXBonus INT,
	@LocationYBonus INT,
	@Minigame1IDBonus INT,
	@Minigame2IDBonus INT,
	@AwardBadgeIDBonus INT,
	@PGLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGameLevel (
		PGID,
		LevelNumber,
		LocationX,
		LocationY,
		PointNumber,
		Minigame1ID,
		Minigame2ID,
		AwardBadgeID,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		LocationXBonus,
		LocationYBonus,
		Minigame1IDBonus,
		Minigame2IDBonus,
		AwardBadgeIDBonus
		)
	VALUES (
		@PGID,
		(
			SELECT isnull(Max(LevelNumber), 0) + 1
			FROM ProgramGameLevel
			WHERE PGID = @PGID
			),
		@LocationX,
		@LocationY,
		@PointNumber,
		@Minigame1ID,
		@Minigame2ID,
		@AwardBadgeID,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@LocationXBonus,
		@LocationYBonus,
		@Minigame1IDBonus,
		@Minigame2IDBonus,
		@AwardBadgeIDBonus
		)

	SELECT @PGLID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveDn]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_MoveDn] @PGLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@PGID INT

SELECT @CurrentRecordLocation = LevelNumber,
	@PGID = PGID
FROM ProgramGameLevel
WHERE PGLID = @PGLID

EXEC [dbo].[app_ProgramGameLevel_Reorder] @PGID

IF @CurrentRecordLocation < (
		SELECT MAX(LevelNumber)
		FROM ProgramGameLevel
		WHERE PGID = @PGID
		)
BEGIN
	SELECT @NextRecordID = PGLID
	FROM ProgramGameLevel
	WHERE LevelNumber = (@CurrentRecordLocation + 1)
		AND PGID = @PGID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation + 1
	WHERE PGLID = @PGLID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation
	WHERE PGLID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_MoveUp]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_MoveUp] @PGLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@PGID INT

SELECT @CurrentRecordLocation = LevelNumber,
	@PGID = PGID
FROM ProgramGameLevel
WHERE PGLID = @PGLID

EXEC [dbo].[app_ProgramGameLevel_Reorder] @PGID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = PGLID
	FROM ProgramGameLevel
	WHERE LevelNumber = (@CurrentRecordLocation - 1)
		AND PGID = @PGID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation - 1
	WHERE PGLID = @PGLID

	UPDATE ProgramGameLevel
	SET LevelNumber = @CurrentRecordLocation
	WHERE PGLID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Reorder] @PGID INT
AS
UPDATE ProgramGameLevel
SET LevelNumber = rowNumber
FROM ProgramGameLevel
INNER JOIN (
	SELECT PGLID,
		LevelNumber,
		row_number() OVER (
			ORDER BY LevelNumber ASC
			) AS rowNumber
	FROM ProgramGameLevel
	WHERE PGID = @PGID
	) drRowNumbers ON drRowNumbers.PGLID = ProgramGameLevel.PGLID
	AND PGID = @PGID
WHERE PGID = @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Update]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Update] (
	@PGLID INT,
	@PGID INT,
	@LevelNumber INT,
	@LocationX INT,
	@LocationY INT,
	@PointNumber INT,
	@Minigame1ID INT,
	@Minigame2ID INT,
	@AwardBadgeID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LocationXBonus INT,
	@LocationYBonus INT,
	@Minigame1IDBonus INT,
	@Minigame2IDBonus INT,
	@AwardBadgeIDBonus INT
	)
AS
UPDATE ProgramGameLevel
SET PGID = @PGID,
	LevelNumber = @LevelNumber,
	LocationX = @LocationX,
	LocationY = @LocationY,
	PointNumber = @PointNumber,
	Minigame1ID = @Minigame1ID,
	Minigame2ID = @Minigame2ID,
	AwardBadgeID = @AwardBadgeID,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LocationXBonus = @LocationXBonus,
	LocationYBonus = @LocationYBonus,
	Minigame1IDBonus = @Minigame1IDBonus,
	Minigame2IDBonus = @Minigame2IDBonus,
	AwardBadgeIDBonus = @AwardBadgeIDBonus
WHERE PGLID = @PGLID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Delete]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Delete] @PGCID INT
AS
DELETE
FROM [ProgramGamePointConversion]
WHERE PGCID = @PGCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetAll] @PGID INT = 0
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE PGID = @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByActivityType]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByActivityType]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetByActivityType] @PGID INT,
	@ActivityTypeID INT
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE ActivityTypeId = @ActivityTypeID
	AND PGID = @PGID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_GetByID] @PGCID INT
AS
SELECT *
FROM [ProgramGamePointConversion]
WHERE PGCID = @PGCID
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Insert]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Insert] (
	@PGID INT,
	@ActivityTypeId INT,
	@ActivityCount INT,
	@PointCount INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PGCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGamePointConversion (
		PGID,
		ActivityTypeId,
		ActivityCount,
		PointCount,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PGID,
		@ActivityTypeId,
		@ActivityCount,
		@PointCount,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PGCID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ProgramGamePointConversion_Update]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_ProgramGamePointConversion_Update] (
	@PGCID INT,
	@PGID INT,
	@ActivityTypeId INT,
	@ActivityCount INT,
	@PointCount INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE ProgramGamePointConversion
SET PGID = @PGID,
	ActivityTypeId = @ActivityTypeId,
	ActivityCount = @ActivityCount,
	PointCount = @PointCount,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PGCID = @PGCID
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Programs_Delete] @PID INT,
	@PatronProgram INT = 0,
	@PrizeProgram INT = 0,
	@OfferProgram INT = 0,
	@BookListProgram INT = 0
AS
UPDATE Patron
SET ProgID = @PatronProgram
WHERE ProgID = @PID

UPDATE PrizeTemplate
SET ProgID = @PatronProgram
WHERE ProgID = @PID

UPDATE Offer
SET ProgramId = @PatronProgram
WHERE ProgramId = @PID

UPDATE BookList
SET ProgID = @PatronProgram
WHERE ProgID = @PID

DELETE
FROM ProgramCodes
WHERE PID = @PID

DELETE
FROM ProgramGamePointConversion
WHERE PGID = @PID

--DELETE from ProgramCodes where PID = @PID
DECLARE @TenID INT

SELECT @TenID = TenID
FROM Programs
WHERE PID = @PID

DELETE
FROM [Programs]
WHERE PID = @PID

EXEC [app_Programs_Reorder] @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Programs_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount
FROM [Programs]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetAllActive]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_GetAllActive] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount,
	(
		SELECT isnull(Max(POrder), 1)
		FROM Programs
		) AS MAX
FROM [Programs]
WHERE IsActive = 1
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetAllOrdered]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Programs_GetAllOrdered] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount,
	(
		SELECT isnull(Max(POrder), 1)
		FROM Programs
		) AS MAX
FROM [Programs]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetAllTabs]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_GetAllTabs] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount,
	(
		SELECT isnull(Max(POrder), 1)
		FROM Programs
		) AS MAX
FROM [Programs]
WHERE IsActive = 1
	AND IsHidden = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_Programs_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Programs_GetByID] @PID INT
AS
SELECT *
FROM [Programs]
WHERE PID = @PID
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetDefaultProgramForAgeAndGrade]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_GetDefaultProgramForAgeAndGrade] @Age INT = - 1,
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
END
ELSE
BEGIN
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC
END

IF (@ID IS NULL)
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC

select @ID
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetDefaultProgramID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_GetDefaultProgramID] @TenID INT = NULL
AS
DECLARE @ID INT

SELECT TOP 1 @ID = PID
FROM [Programs]
WHERE IsActive = 1
	AND IsHidden = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC

SELECT @ID

RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_GetProgramMinigames]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_GetProgramMinigames] @LevelIDs VARCHAR(1000) = '',
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
/****** Object:  StoredProcedure [dbo].[app_Programs_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_Insert] (
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
		PostTestStartDate
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
		@PostTestStartDate
		)

	SELECT @PID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_MoveDn] @PID INT
AS
DECLARE @TenID INT

SELECT @TenID = TenID
FROM Programs
WHERE PID = @PID

EXEC [dbo].[app_Programs_Reorder] @TenID

DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT

SELECT @CurrentRecordLocation = POrder
FROM Programs
WHERE PID = @PID

IF @CurrentRecordLocation < (
		SELECT MAX(POrder)
		FROM Programs
		WHERE TenID = @TenID
		)
BEGIN
	SELECT @NextRecordID = PID
	FROM Programs
	WHERE POrder = (@CurrentRecordLocation + 1)
		AND TenID = @TenID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation + 1
	WHERE PID = @PID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation
	WHERE PID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_MoveUp] @PID INT
AS
DECLARE @TenID INT

SELECT @TenID = TenID
FROM Programs
WHERE PID = @PID

EXEC [dbo].[app_Programs_Reorder] @TenID

DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT

SELECT @CurrentRecordLocation = POrder
FROM Programs
WHERE PID = @PID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = PID
	FROM Programs
	WHERE POrder = (@CurrentRecordLocation - 1)
		AND TenID = @TenID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation - 1
	WHERE PID = @PID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation
	WHERE PID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_Reorder] @TenID INT
AS
UPDATE Programs
SET POrder = rowNumber
FROM Programs
INNER JOIN (
	SELECT PID,
		POrder,
		row_number() OVER (
			ORDER BY POrder ASC
			) AS rowNumber
	FROM Programs
	WHERE TenID = @TenID
	) drRowNumbers ON drRowNumbers.PID = Programs.PID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Programs_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Programs_Update] (
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
	@PostTestStartDate DATETIME
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
	PostTestStartDate = @PostTestStartDate
WHERE PID = @PID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_Delete]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_Delete] @RID INT
AS
DELETE
FROM [RegistrationSettings]
WHERE RID = @RID
GO
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [RegistrationSettings]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_GetByID]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_RegistrationSettings_GetByID] @TenID INT
AS
SELECT *
FROM [RegistrationSettings]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_RegistrationSettings_Insert] (
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
/****** Object:  StoredProcedure [dbo].[app_RegistrationSettings_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_RegistrationSettings_Update] (
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
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_Delete] @RTID INT
AS
DELETE
FROM [ReportTemplate]
WHERE RTID = @RTID
GO
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_GetAll] @TenID INT = NULL
AS
SELECT RTID,
	ReportName
FROM [ReportTemplate]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_ReportTemplate_GetByID] @RTID INT
AS
SELECT *
FROM [ReportTemplate]
WHERE RTID = @RTID
GO
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ReportTemplate_Insert] (
	@ProgId INT,
	@ReportName VARCHAR(150),
	@DisplayFilters BIT,
	@DOBFrom DATETIME,
	@DOBTo DATETIME,
	@AgeFrom INT,
	@AgeTo INT,
	@SchoolGrade VARCHAR(5),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@County VARCHAR(50),
	@PrimaryLibrary INT,
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@RegistrationDateStart DATETIME,
	@RegistrationDateEnd DATETIME,
	@PointsMin INT,
	@PointsMax INT,
	@PointsStart DATETIME,
	@PointsEnd DATETIME,
	@EventCode VARCHAR(50),
	@EarnedBadge INT,
	@PhysicalPrizeEarned VARCHAR(50),
	@PhysicalPrizeRedeemed BIT,
	@PhysicalPrizeStartDate DATETIME,
	@PhysicalPrizeEndDate DATETIME,
	@ReviewsMin INT,
	@ReviewsMax INT,
	@ReviewTitle VARCHAR(150),
	@ReviewAuthor VARCHAR(100),
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@RandomDrawingName VARCHAR(50),
	@RandomDrawingNum INT,
	@RandomDrawingStartDate DATETIME,
	@RandomDrawingEndDate DATETIME,
	@HasBeenDrawn BIT,
	@HasRedeemend BIT,
	@PIDInc BIT,
	@UsernameInc BIT,
	@DOBInc BIT,
	@AgeInc BIT,
	@SchoolGradeInc BIT,
	@FirstNameInc BIT,
	@LastNameInc BIT,
	@GenderInc BIT,
	@EmailAddressInc BIT,
	@PhoneNumberInc BIT,
	@CityInc BIT,
	@StateInc BIT,
	@ZipCodeInc BIT,
	@CountyInc BIT,
	@PrimaryLibraryInc BIT,
	@SchoolNameInc BIT,
	@DistrictInc BIT,
	@TeacherInc BIT,
	@GroupTeamNameInc BIT,
	@SchoolTypeInc BIT,
	@LiteracyLevel1Inc BIT,
	@LiteracyLevel2Inc BIT,
	@Custom1Inc BIT,
	@Custom2Inc BIT,
	@Custom3Inc BIT,
	@Custom4Inc BIT,
	@Custom5Inc BIT,
	@RegistrationDateInc BIT,
	@PointsInc BIT,
	@EarnedBadgeInc BIT,
	@PhysicalPrizeNameInc BIT,
	@PhysicalPrizeDateInc BIT,
	@NumReviewsInc BIT,
	@ReviewAuthorInc BIT,
	@ReviewTitleInc BIT,
	@ReviewDateInc BIT,
	@RandomDrawingNameInc BIT,
	@RandomDrawingNumInc BIT,
	@RandomDrawingDateInc BIT,
	@HasBeenDrawnInc BIT,
	@HasRedeemendInc BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict VARCHAR(50),
	@SDistrictInc BIT,
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
	@Score1From INT = 0,
	@Score1To INT = 0,
	@Score1PctFrom INT = 0,
	@Score1PctTo INT = 0,
	@Score2From INT = 0,
	@Score2To INT = 0,
	@Score2PctFrom INT = 0,
	@Score2PctTo INT = 0,
	@Score1Inc BIT,
	@Score2Inc BIT,
	@Score1PctInc BIT,
	@Score2PctInc BIT,
	@RTID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ReportTemplate (
		ProgId,
		ReportName,
		DisplayFilters,
		DOBFrom,
		DOBTo,
		AgeFrom,
		AgeTo,
		SchoolGrade,
		FirstName,
		LastName,
		Gender,
		EmailAddress,
		PhoneNumber,
		City,
		STATE,
		ZipCode,
		County,
		PrimaryLibrary,
		SchoolName,
		District,
		Teacher,
		GroupTeamName,
		SchoolType,
		LiteracyLevel1,
		LiteracyLevel2,
		Custom1,
		Custom2,
		Custom3,
		Custom4,
		Custom5,
		RegistrationDateStart,
		RegistrationDateEnd,
		PointsMin,
		PointsMax,
		PointsStart,
		PointsEnd,
		EventCode,
		EarnedBadge,
		PhysicalPrizeEarned,
		PhysicalPrizeRedeemed,
		PhysicalPrizeStartDate,
		PhysicalPrizeEndDate,
		ReviewsMin,
		ReviewsMax,
		ReviewTitle,
		ReviewAuthor,
		ReviewStartDate,
		ReviewEndDate,
		RandomDrawingName,
		RandomDrawingNum,
		RandomDrawingStartDate,
		RandomDrawingEndDate,
		HasBeenDrawn,
		HasRedeemend,
		PIDInc,
		UsernameInc,
		DOBInc,
		AgeInc,
		SchoolGradeInc,
		FirstNameInc,
		LastNameInc,
		GenderInc,
		EmailAddressInc,
		PhoneNumberInc,
		CityInc,
		StateInc,
		ZipCodeInc,
		CountyInc,
		PrimaryLibraryInc,
		SchoolNameInc,
		DistrictInc,
		TeacherInc,
		GroupTeamNameInc,
		SchoolTypeInc,
		LiteracyLevel1Inc,
		LiteracyLevel2Inc,
		Custom1Inc,
		Custom2Inc,
		Custom3Inc,
		Custom4Inc,
		Custom5Inc,
		RegistrationDateInc,
		PointsInc,
		EarnedBadgeInc,
		PhysicalPrizeNameInc,
		PhysicalPrizeDateInc,
		NumReviewsInc,
		ReviewAuthorInc,
		ReviewTitleInc,
		ReviewDateInc,
		RandomDrawingNameInc,
		RandomDrawingNumInc,
		RandomDrawingDateInc,
		HasBeenDrawnInc,
		HasRedeemendInc,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		SDistrict,
		SDistrictInc,
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
		Score1From,
		Score1To,
		Score1PctFrom,
		Score1PctTo,
		Score2From,
		Score2To,
		Score2PctFrom,
		Score2PctTo,
		Score1Inc,
		Score2Inc,
		Score1PctInc,
		Score2PctInc
		)
	VALUES (
		@ProgId,
		@ReportName,
		@DisplayFilters,
		@DOBFrom,
		@DOBTo,
		@AgeFrom,
		@AgeTo,
		@SchoolGrade,
		@FirstName,
		@LastName,
		@Gender,
		@EmailAddress,
		@PhoneNumber,
		@City,
		@State,
		@ZipCode,
		@County,
		@PrimaryLibrary,
		@SchoolName,
		@District,
		@Teacher,
		@GroupTeamName,
		@SchoolType,
		@LiteracyLevel1,
		@LiteracyLevel2,
		@Custom1,
		@Custom2,
		@Custom3,
		@Custom4,
		@Custom5,
		@RegistrationDateStart,
		@RegistrationDateEnd,
		@PointsMin,
		@PointsMax,
		@PointsStart,
		@PointsEnd,
		@EventCode,
		@EarnedBadge,
		@PhysicalPrizeEarned,
		@PhysicalPrizeRedeemed,
		@PhysicalPrizeStartDate,
		@PhysicalPrizeEndDate,
		@ReviewsMin,
		@ReviewsMax,
		@ReviewTitle,
		@ReviewAuthor,
		@ReviewStartDate,
		@ReviewEndDate,
		@RandomDrawingName,
		@RandomDrawingNum,
		@RandomDrawingStartDate,
		@RandomDrawingEndDate,
		@HasBeenDrawn,
		@HasRedeemend,
		@PIDInc,
		@UsernameInc,
		@DOBInc,
		@AgeInc,
		@SchoolGradeInc,
		@FirstNameInc,
		@LastNameInc,
		@GenderInc,
		@EmailAddressInc,
		@PhoneNumberInc,
		@CityInc,
		@StateInc,
		@ZipCodeInc,
		@CountyInc,
		@PrimaryLibraryInc,
		@SchoolNameInc,
		@DistrictInc,
		@TeacherInc,
		@GroupTeamNameInc,
		@SchoolTypeInc,
		@LiteracyLevel1Inc,
		@LiteracyLevel2Inc,
		@Custom1Inc,
		@Custom2Inc,
		@Custom3Inc,
		@Custom4Inc,
		@Custom5Inc,
		@RegistrationDateInc,
		@PointsInc,
		@EarnedBadgeInc,
		@PhysicalPrizeNameInc,
		@PhysicalPrizeDateInc,
		@NumReviewsInc,
		@ReviewAuthorInc,
		@ReviewTitleInc,
		@ReviewDateInc,
		@RandomDrawingNameInc,
		@RandomDrawingNumInc,
		@RandomDrawingDateInc,
		@HasBeenDrawnInc,
		@HasRedeemendInc,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@SDistrict,
		@SDistrictInc,
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
		@Score1From,
		@Score1To,
		@Score1PctFrom,
		@Score1PctTo,
		@Score2From,
		@Score2To,
		@Score2PctFrom,
		@Score2PctTo,
		@Score1Inc,
		@Score2Inc,
		@Score1PctInc,
		@Score2PctInc
		)

	SELECT @RTID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_ReportTemplate_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_ReportTemplate_Update] (
	@RTID INT,
	@ProgId INT,
	@ReportName VARCHAR(150),
	@DisplayFilters BIT,
	@DOBFrom DATETIME,
	@DOBTo DATETIME,
	@AgeFrom INT,
	@AgeTo INT,
	@SchoolGrade VARCHAR(5),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@County VARCHAR(50),
	@PrimaryLibrary INT,
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@RegistrationDateStart DATETIME,
	@RegistrationDateEnd DATETIME,
	@PointsMin INT,
	@PointsMax INT,
	@PointsStart DATETIME,
	@PointsEnd DATETIME,
	@EventCode VARCHAR(50),
	@EarnedBadge INT,
	@PhysicalPrizeEarned VARCHAR(50),
	@PhysicalPrizeRedeemed BIT,
	@PhysicalPrizeStartDate DATETIME,
	@PhysicalPrizeEndDate DATETIME,
	@ReviewsMin INT,
	@ReviewsMax INT,
	@ReviewTitle VARCHAR(150),
	@ReviewAuthor VARCHAR(100),
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@RandomDrawingName VARCHAR(50),
	@RandomDrawingNum INT,
	@RandomDrawingStartDate DATETIME,
	@RandomDrawingEndDate DATETIME,
	@HasBeenDrawn BIT,
	@HasRedeemend BIT,
	@PIDInc BIT,
	@UsernameInc BIT,
	@DOBInc BIT,
	@AgeInc BIT,
	@SchoolGradeInc BIT,
	@FirstNameInc BIT,
	@LastNameInc BIT,
	@GenderInc BIT,
	@EmailAddressInc BIT,
	@PhoneNumberInc BIT,
	@CityInc BIT,
	@StateInc BIT,
	@ZipCodeInc BIT,
	@CountyInc BIT,
	@PrimaryLibraryInc BIT,
	@SchoolNameInc BIT,
	@DistrictInc BIT,
	@TeacherInc BIT,
	@GroupTeamNameInc BIT,
	@SchoolTypeInc BIT,
	@LiteracyLevel1Inc BIT,
	@LiteracyLevel2Inc BIT,
	@Custom1Inc BIT,
	@Custom2Inc BIT,
	@Custom3Inc BIT,
	@Custom4Inc BIT,
	@Custom5Inc BIT,
	@RegistrationDateInc BIT,
	@PointsInc BIT,
	@EarnedBadgeInc BIT,
	@PhysicalPrizeNameInc BIT,
	@PhysicalPrizeDateInc BIT,
	@NumReviewsInc BIT,
	@ReviewAuthorInc BIT,
	@ReviewTitleInc BIT,
	@ReviewDateInc BIT,
	@RandomDrawingNameInc BIT,
	@RandomDrawingNumInc BIT,
	@RandomDrawingDateInc BIT,
	@HasBeenDrawnInc BIT,
	@HasRedeemendInc BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict VARCHAR(50),
	@SDistrictInc BIT,
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
	@Score1From INT = 0,
	@Score1To INT = 0,
	@Score1PctFrom INT = 0,
	@Score1PctTo INT = 0,
	@Score2From INT = 0,
	@Score2To INT = 0,
	@Score2PctFrom INT = 0,
	@Score2PctTo INT = 0,
	@Score1Inc BIT,
	@Score2Inc BIT,
	@Score1PctInc BIT,
	@Score2PctInc BIT
	)
AS
UPDATE ReportTemplate
SET ProgId = @ProgId,
	ReportName = @ReportName,
	DisplayFilters = @DisplayFilters,
	DOBFrom = @DOBFrom,
	DOBTo = @DOBTo,
	AgeFrom = @AgeFrom,
	AgeTo = @AgeTo,
	SchoolGrade = @SchoolGrade,
	FirstName = @FirstName,
	LastName = @LastName,
	Gender = @Gender,
	EmailAddress = @EmailAddress,
	PhoneNumber = @PhoneNumber,
	City = @City,
	STATE = @State,
	ZipCode = @ZipCode,
	County = @County,
	PrimaryLibrary = @PrimaryLibrary,
	SchoolName = @SchoolName,
	District = @District,
	Teacher = @Teacher,
	GroupTeamName = @GroupTeamName,
	SchoolType = @SchoolType,
	LiteracyLevel1 = @LiteracyLevel1,
	LiteracyLevel2 = @LiteracyLevel2,
	Custom1 = @Custom1,
	Custom2 = @Custom2,
	Custom3 = @Custom3,
	Custom4 = @Custom4,
	Custom5 = @Custom5,
	RegistrationDateStart = @RegistrationDateStart,
	RegistrationDateEnd = @RegistrationDateEnd,
	PointsMin = @PointsMin,
	PointsMax = @PointsMax,
	PointsStart = @PointsStart,
	PointsEnd = @PointsEnd,
	EventCode = @EventCode,
	EarnedBadge = @EarnedBadge,
	PhysicalPrizeEarned = @PhysicalPrizeEarned,
	PhysicalPrizeRedeemed = @PhysicalPrizeRedeemed,
	PhysicalPrizeStartDate = @PhysicalPrizeStartDate,
	PhysicalPrizeEndDate = @PhysicalPrizeEndDate,
	ReviewsMin = @ReviewsMin,
	ReviewsMax = @ReviewsMax,
	ReviewTitle = @ReviewTitle,
	ReviewAuthor = @ReviewAuthor,
	ReviewStartDate = @ReviewStartDate,
	ReviewEndDate = @ReviewEndDate,
	RandomDrawingName = @RandomDrawingName,
	RandomDrawingNum = @RandomDrawingNum,
	RandomDrawingStartDate = @RandomDrawingStartDate,
	RandomDrawingEndDate = @RandomDrawingEndDate,
	HasBeenDrawn = @HasBeenDrawn,
	HasRedeemend = @HasRedeemend,
	PIDInc = @PIDInc,
	UsernameInc = @UsernameInc,
	DOBInc = @DOBInc,
	AgeInc = @AgeInc,
	SchoolGradeInc = @SchoolGradeInc,
	FirstNameInc = @FirstNameInc,
	LastNameInc = @LastNameInc,
	GenderInc = @GenderInc,
	EmailAddressInc = @EmailAddressInc,
	PhoneNumberInc = @PhoneNumberInc,
	CityInc = @CityInc,
	StateInc = @StateInc,
	ZipCodeInc = @ZipCodeInc,
	CountyInc = @CountyInc,
	PrimaryLibraryInc = @PrimaryLibraryInc,
	SchoolNameInc = @SchoolNameInc,
	DistrictInc = @DistrictInc,
	TeacherInc = @TeacherInc,
	GroupTeamNameInc = @GroupTeamNameInc,
	SchoolTypeInc = @SchoolTypeInc,
	LiteracyLevel1Inc = @LiteracyLevel1Inc,
	LiteracyLevel2Inc = @LiteracyLevel2Inc,
	Custom1Inc = @Custom1Inc,
	Custom2Inc = @Custom2Inc,
	Custom3Inc = @Custom3Inc,
	Custom4Inc = @Custom4Inc,
	Custom5Inc = @Custom5Inc,
	RegistrationDateInc = @RegistrationDateInc,
	PointsInc = @PointsInc,
	EarnedBadgeInc = @EarnedBadgeInc,
	PhysicalPrizeNameInc = @PhysicalPrizeNameInc,
	PhysicalPrizeDateInc = @PhysicalPrizeDateInc,
	NumReviewsInc = @NumReviewsInc,
	ReviewAuthorInc = @ReviewAuthorInc,
	ReviewTitleInc = @ReviewTitleInc,
	ReviewDateInc = @ReviewDateInc,
	RandomDrawingNameInc = @RandomDrawingNameInc,
	RandomDrawingNumInc = @RandomDrawingNumInc,
	RandomDrawingDateInc = @RandomDrawingDateInc,
	HasBeenDrawnInc = @HasBeenDrawnInc,
	HasRedeemendInc = @HasRedeemendInc,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	SDistrict = @SDistrict,
	SDistrictInc = @SDistrictInc,
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
	Score1From = @Score1From,
	Score1To = @Score1To,
	Score1PctFrom = @Score1PctFrom,
	Score1PctTo = @Score1PctTo,
	Score2From = @Score2From,
	Score2To = @Score2To,
	Score2PctFrom = @Score2PctFrom,
	Score2PctTo = @Score2PctTo,
	Score1Inc = @Score1Inc,
	Score2Inc = @Score2Inc,
	Score1PctInc = @Score1PctInc,
	Score2PctInc = @Score2PctInc
WHERE RTID = @RTID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Delete] @ID INT
AS
DELETE
FROM [SchoolCrosswalk]
WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetAll] @TenID INT = NULL
AS
DECLARE @Schools TABLE (
	CID INT NOT NULL,
	Code VARCHAR(50) NOT NULL
	)

INSERT INTO @Schools
SELECT c.CID,
	c.Code
FROM Code c
INNER JOIN CodeType t ON c.CTID = t.CTID
WHERE t.CodeTypeName = 'School'
	AND (
		t.TenID = @TenID
		OR @TenID IS NULL
		)

DELETE
FROM [SchoolCrosswalk]
WHERE SchoolID NOT IN (
		SELECT CID
		FROM @Schools
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)

SELECT isnull(w.ID, 0) AS ID,
	isnull(l.CID, 0) AS SchoolID,
	isnull(w.SchTypeID, 0) AS SchTypeID,
	isnull(w.DistrictID, 0) AS DistrictID,
	isnull(w.City, '') AS City,
	isnull(w.MinGrade, 0) AS MinGrade,
	isnull(w.MaxGrade, 0) AS MaxGrade,
	isnull(w.MinAge, 0) AS MinAge,
	isnull(w.MaxAge, 0) AS MaxAge,
	isnull(l.Code, '') AS SchoolName,
	RANK() OVER (
		ORDER BY l.Code ASC
		) AS RANK
FROM [SchoolCrosswalk] w
RIGHT JOIN @Schools l ON w.SchoolID = l.CID
ORDER BY l.Code
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetByID] @ID INT
AS
SELECT *
FROM [SchoolCrosswalk]
WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_GetBySchoolID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetBySchoolID] @ID INT = 0
AS
SELECT *
FROM SchoolCrosswalk
WHERE SchoolID = @ID
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_GetFilteredSchoolDDValues]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetFilteredSchoolDDValues] @SchTypeID INT = 0,
	@DistrictID INT = 0,
	@City VARCHAR(50) = '',
	@Grade INT = 0,
	@Age INT = 0,
	@TenID INT = NULL
AS
SELECT DISTINCT SchoolID AS CID,
	c.Code AS Code, c.[Description] as [Description]
FROM SchoolCrosswalk w
INNER JOIN Code c ON w.SchoolID = c.CID
WHERE (
		SchTypeID = @SchTypeID
		OR @SchTypeID IS NULL
		OR @SchTypeID = 0
		)
	AND (
		DistrictID = @DistrictID
		OR @DistrictID IS NULL
		OR @DistrictID = 0
		)
	AND (
		City = @City
		OR @City IS NULL
		OR @City = ''
		)
	AND (
		(
			MinGrade <= @Grade
			AND MaxGrade >= @Grade
			)
		OR @Grade IS NULL
		OR @Grade = 0
		OR (
			MinGrade = 0
			AND MaxGrade = 0
			)
		)
	AND (
		(
			MinAge <= @Age
			AND MaxAge >= @Age
			)
		OR @Age IS NULL
		OR @Age = 0
		OR (
			MinAge = 0
			AND MaxAge = 0
			)
		)
	AND (
		w.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY [Description]
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Insert] (
	@SchoolID INT,
	@SchTypeID INT,
	@DistrictID INT,
	@City VARCHAR(50),
	@MinGrade INT,
	@MaxGrade INT,
	@MinAge INT,
	@MaxAge INT,
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
	@ID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SchoolCrosswalk (
		SchoolID,
		SchTypeID,
		DistrictID,
		City,
		MinGrade,
		MaxGrade,
		MinAge,
		MaxAge,
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
		@SchoolID,
		@SchTypeID,
		@DistrictID,
		@City,
		@MinGrade,
		@MaxGrade,
		@MinAge,
		@MaxAge,
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

	SELECT @ID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SchoolCrosswalk_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_Update] (
	@ID INT,
	@SchoolID INT,
	@SchTypeID INT,
	@DistrictID INT,
	@City VARCHAR(50),
	@MinGrade INT,
	@MaxGrade INT,
	@MinAge INT,
	@MaxAge INT,
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
UPDATE SchoolCrosswalk
SET SchoolID = @SchoolID,
	SchTypeID = @SchTypeID,
	DistrictID = @DistrictID,
	City = @City,
	MinGrade = @MinGrade,
	MaxGrade = @MaxGrade,
	MinAge = @MinAge,
	MaxAge = @MaxAge,
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
WHERE ID = @ID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Delete] @EID INT
AS
DELETE
FROM [SentEmailLog]
WHERE EID = @EID
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_DeleteAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_DeleteAll]    Script Date: 01/05/2015 14:43:26 ******/
CREATE PROCEDURE [dbo].[app_SentEmailLog_DeleteAll] @EID INT
AS
DELETE
FROM [SentEmailLog]
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_GetAll]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_GetAll] @EID INT
AS
SELECT *
FROM [SentEmailLog]
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_GetByID]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_GetByID] @EID INT
AS
SELECT *
FROM [SentEmailLog]
WHERE EID = @EID
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Insert]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Insert] (
	@SentDateTime DATETIME,
	@SentFrom VARCHAR(150),
	@SentTo VARCHAR(150),
	@Subject VARCHAR(150),
	@Body TEXT,
	@EID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SentEmailLog (
		SentDateTime,
		SentFrom,
		SentTo,
		Subject,
		Body
		)
	VALUES (
		@SentDateTime,
		@SentFrom,
		@SentTo,
		@Subject,
		@Body
		)

	SELECT @EID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SentEmailLog_Update]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SentEmailLog_Update] (
	@EID INT,
	@SentDateTime DATETIME,
	@SentFrom VARCHAR(150),
	@SentTo VARCHAR(150),
	@Subject VARCHAR(150),
	@Body TEXT
	)
AS
UPDATE SentEmailLog
SET SentDateTime = @SentDateTime,
	SentFrom = @SentFrom,
	SentTo = @SentTo,
	Subject = @Subject,
	Body = @Body
WHERE EID = @EID
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Delete] @SQCID INT
AS
DECLARE @QID INT;

SELECT @QID = QID
FROM [SQChoices]
WHERE SQCID = @SQCID

DELETE
FROM [SQChoices]
WHERE SQCID = @SQCID

EXEC app_SQChoices_Reorder @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetAll] @QID INT,
	@Echo INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(ChoiceOrder), 0)
		FROM [SQChoices]
		WHERE QID = @QID
		) AS MAX,
	@Echo AS Echo
FROM [SQChoices]
WHERE QID = @QID
ORDER BY ChoiceOrder
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_GetAllInList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetAllInList] @List VARCHAR(2000)
AS
SELECT *
FROM [SQChoices]
WHERE SQCID IN (
		SELECT *
		FROM dbo.fnSplitString(@List, ',')
		)
ORDER BY ChoiceOrder
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQChoices_GetByID] @SQCID INT
AS
SELECT *
FROM [SQChoices]
WHERE SQCID = @SQCID
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Insert] (
	@QID INT,
	@ChoiceOrder INT,
	@ChoiceText VARCHAR(50),
	@Score INT,
	@JumpToQuestion INT,
	@AskClarification BIT,
	@ClarificationRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SQCID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SQChoices (
		QID,
		ChoiceOrder,
		ChoiceText,
		Score,
		JumpToQuestion,
		AskClarification,
		ClarificationRequired,
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
		@QID,
		(
			SELECT isnull(Max(ChoiceOrder), 0) + 1
			FROM SQChoices
			WHERE QID = @QID
			),
		@ChoiceText,
		@Score,
		@JumpToQuestion,
		@AskClarification,
		@ClarificationRequired,
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

	SELECT @SQCID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQChoices_MoveDn] @SQCID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = ChoiceOrder,
	@QID = QID
FROM SQChoices
WHERE SQCID = @SQCID

EXEC [dbo].[app_SQChoices_Reorder] @QID

IF @CurrentRecordLocation < (
		SELECT MAX(ChoiceOrder)
		FROM SQChoices
		WHERE QID = @QID
		)
BEGIN
	SELECT @NextRecordID = SQCID
	FROM SQChoices
	WHERE ChoiceOrder = (@CurrentRecordLocation + 1)
		AND QID = @QID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation + 1
	WHERE SQCID = @SQCID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation
	WHERE SQCID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQChoices_MoveUp] @SQCID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = ChoiceOrder,
	@QID = QID
FROM SQChoices
WHERE SQCID = @SQCID

EXEC [dbo].[app_SQChoices_Reorder] @QID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = SQCID
	FROM SQChoices
	WHERE ChoiceOrder = (@CurrentRecordLocation - 1)
		AND QID = @QID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation - 1
	WHERE SQCID = @SQCID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation
	WHERE SQCID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQChoices_Reorder] @QID INT
AS
UPDATE SQChoices
SET ChoiceOrder = rowNumber
FROM SQChoices
INNER JOIN (
	SELECT SQCID,
		ChoiceOrder,
		row_number() OVER (
			ORDER BY ChoiceOrder ASC
			) AS rowNumber
	FROM SQChoices
	WHERE QID = @QID
	) drRowNumbers ON drRowNumbers.SQCID = SQChoices.SQCID
	AND QID = @QID
WHERE QID = @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SQChoices_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SQChoices_Update] (
	@SQCID INT,
	@QID INT,
	@ChoiceOrder INT,
	@ChoiceText VARCHAR(50),
	@Score INT,
	@JumpToQuestion INT,
	@AskClarification BIT,
	@ClarificationRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE SQChoices
SET QID = @QID
	--,ChoiceOrder =  @ChoiceOrder
	,
	ChoiceText = @ChoiceText,
	Score = @Score,
	JumpToQuestion = @JumpToQuestion,
	AskClarification = @AskClarification,
	ClarificationRequired = @ClarificationRequired,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SQCID = @SQCID
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Delete] @SQMLID INT
AS
DECLARE @QID INT;

SELECT @QID = QID
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID

DELETE
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID

EXEC app_SQMatrixLines_Reorder @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_GetAll] @QID INT
AS
SELECT *,
	(
		SELECT isnull(Max(LineOrder), 0)
		FROM [SQMatrixLines]
		WHERE QID = @QID
		) AS MAX
FROM [SQMatrixLines]
WHERE QID = @QID
ORDER BY LineOrder
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_GetByID] @SQMLID INT
AS
SELECT *
FROM [SQMatrixLines]
WHERE SQMLID = @SQMLID
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Insert] (
	@QID INT,
	@LineOrder INT,
	@LineText VARCHAR(500),
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SQMLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SQMatrixLines (
		QID,
		LineOrder,
		LineText,
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
		@QID,
		(
			SELECT isnull(Max(LineOrder), 0) + 1
			FROM SQMatrixLines
			WHERE QID = @QID
			) -- @LineOrder
		,
		@LineText,
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

	SELECT @SQMLID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQMatrixLines_MoveDn] @SQMLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = LineOrder,
	@QID = QID
FROM SQMatrixLines
WHERE SQMLID = @SQMLID

EXEC [dbo].[app_SQMatrixLines_Reorder] @QID

IF @CurrentRecordLocation < (
		SELECT MAX(LineOrder)
		FROM SQMatrixLines
		WHERE QID = @QID
		)
BEGIN
	SELECT @NextRecordID = SQMLID
	FROM SQMatrixLines
	WHERE LineOrder = (@CurrentRecordLocation + 1)
		AND QID = @QID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation + 1
	WHERE SQMLID = @SQMLID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation
	WHERE SQMLID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQMatrixLines_MoveUp] @SQMLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = LineOrder,
	@QID = QID
FROM SQMatrixLines
WHERE SQMLID = @SQMLID

EXEC [dbo].[app_SQMatrixLines_Reorder] @QID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = SQMLID
	FROM SQMatrixLines
	WHERE LineOrder = (@CurrentRecordLocation - 1)
		AND QID = @QID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation - 1
	WHERE SQMLID = @SQMLID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation
	WHERE SQMLID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SQMatrixLines_Reorder] @QID INT
AS
UPDATE SQMatrixLines
SET LineOrder = rowNumber
FROM SQMatrixLines
INNER JOIN (
	SELECT SQMLID,
		LineOrder,
		row_number() OVER (
			ORDER BY LineOrder ASC
			) AS rowNumber
	FROM SQMatrixLines
	WHERE QID = @QID
	) drRowNumbers ON drRowNumbers.SQMLID = SQMatrixLines.SQMLID
	AND QID = @QID
WHERE QID = @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SQMatrixLines_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Update] (
	@SQMLID INT,
	@QID INT,
	@LineOrder INT,
	@LineText VARCHAR(500),
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE SQMatrixLines
SET QID = @QID,
	LineOrder = @LineOrder,
	LineText = @LineText,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SQMLID = @SQMLID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPReport_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SRPReport_Delete]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SRPReport_Delete] @RID INT
AS
DELETE
FROM [SRPReport]
WHERE RID = @RID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPReport_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPReport_GetAll] @TenID INT = NULL
AS
SELECT RID,
	ReportName,
	AddedDate
FROM [SRPReport]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SRPReport_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SRPReport_GetByID]    Script Date: 01/05/2015 14:43:26 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPReport_GetByID] @RID INT
AS
SELECT *
FROM [SRPReport]
WHERE RID = @RID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPReport_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SRPReport_Insert] (
	@RTID INT,
	@ProgId INT,
	@ReportName VARCHAR(150),
	@DisplayFilters BIT,
	@ReportFormat INT,
	@DOBFrom DATETIME,
	@DOBTo DATETIME,
	@AgeFrom INT,
	@AgeTo INT,
	@SchoolGrade VARCHAR(5),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@County VARCHAR(50),
	@PrimaryLibrary INT,
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@RegistrationDateStart DATETIME,
	@RegistrationDateEnd DATETIME,
	@PointsMin INT,
	@PointsMax INT,
	@PointsStart DATETIME,
	@PointsEnd DATETIME,
	@EventCode VARCHAR(50),
	@EarnedBadge INT,
	@PhysicalPrizeEarned VARCHAR(50),
	@PhysicalPrizeRedeemed BIT,
	@PhysicalPrizeStartDate DATETIME,
	@PhysicalPrizeEndDate DATETIME,
	@ReviewsMin INT,
	@ReviewsMax INT,
	@ReviewTitle VARCHAR(150),
	@ReviewAuthor VARCHAR(100),
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@RandomDrawingName VARCHAR(50),
	@RandomDrawingNum INT,
	@RandomDrawingStartDate DATETIME,
	@RandomDrawingEndDate DATETIME,
	@HasBeenDrawn BIT,
	@HasRedeemend BIT,
	@PIDInc BIT,
	@UsernameInc BIT,
	@DOBInc BIT,
	@AgeInc BIT,
	@SchoolGradeInc BIT,
	@FirstNameInc BIT,
	@LastNameInc BIT,
	@GenderInc BIT,
	@EmailAddressInc BIT,
	@PhoneNumberInc BIT,
	@CityInc BIT,
	@StateInc BIT,
	@ZipCodeInc BIT,
	@CountyInc BIT,
	@PrimaryLibraryInc BIT,
	@SchoolNameInc BIT,
	@DistrictInc BIT,
	@TeacherInc BIT,
	@GroupTeamNameInc BIT,
	@SchoolTypeInc BIT,
	@LiteracyLevel1Inc BIT,
	@LiteracyLevel2Inc BIT,
	@Custom1Inc BIT,
	@Custom2Inc BIT,
	@Custom3Inc BIT,
	@Custom4Inc BIT,
	@Custom5Inc BIT,
	@RegistrationDateInc BIT,
	@PointsInc BIT,
	@EarnedBadgeInc BIT,
	@PhysicalPrizeNameInc BIT,
	@PhysicalPrizeDateInc BIT,
	@NumReviewsInc BIT,
	@ReviewAuthorInc BIT,
	@ReviewTitleInc BIT,
	@ReviewDateInc BIT,
	@RandomDrawingNameInc BIT,
	@RandomDrawingNumInc BIT,
	@RandomDrawingDateInc BIT,
	@HasBeenDrawnInc BIT,
	@HasRedeemendInc BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict VARCHAR(50),
	@SDistrictInc BIT,
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
	@Score1From INT = 0,
	@Score1To INT = 0,
	@Score1PctFrom INT = 0,
	@Score1PctTo INT = 0,
	@Score2From INT = 0,
	@Score2To INT = 0,
	@Score2PctFrom INT = 0,
	@Score2PctTo INT = 0,
	@Score1Inc BIT,
	@Score2Inc BIT,
	@Score1PctInc BIT,
	@Score2PctInc BIT,
	@RID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SRPReport (
		RTID,
		ProgId,
		ReportName,
		DisplayFilters,
		ReportFormat,
		DOBFrom,
		DOBTo,
		AgeFrom,
		AgeTo,
		SchoolGrade,
		FirstName,
		LastName,
		Gender,
		EmailAddress,
		PhoneNumber,
		City,
		STATE,
		ZipCode,
		County,
		PrimaryLibrary,
		SchoolName,
		District,
		Teacher,
		GroupTeamName,
		SchoolType,
		LiteracyLevel1,
		LiteracyLevel2,
		Custom1,
		Custom2,
		Custom3,
		Custom4,
		Custom5,
		RegistrationDateStart,
		RegistrationDateEnd,
		PointsMin,
		PointsMax,
		PointsStart,
		PointsEnd,
		EventCode,
		EarnedBadge,
		PhysicalPrizeEarned,
		PhysicalPrizeRedeemed,
		PhysicalPrizeStartDate,
		PhysicalPrizeEndDate,
		ReviewsMin,
		ReviewsMax,
		ReviewTitle,
		ReviewAuthor,
		ReviewStartDate,
		ReviewEndDate,
		RandomDrawingName,
		RandomDrawingNum,
		RandomDrawingStartDate,
		RandomDrawingEndDate,
		HasBeenDrawn,
		HasRedeemend,
		PIDInc,
		UsernameInc,
		DOBInc,
		AgeInc,
		SchoolGradeInc,
		FirstNameInc,
		LastNameInc,
		GenderInc,
		EmailAddressInc,
		PhoneNumberInc,
		CityInc,
		StateInc,
		ZipCodeInc,
		CountyInc,
		PrimaryLibraryInc,
		SchoolNameInc,
		DistrictInc,
		TeacherInc,
		GroupTeamNameInc,
		SchoolTypeInc,
		LiteracyLevel1Inc,
		LiteracyLevel2Inc,
		Custom1Inc,
		Custom2Inc,
		Custom3Inc,
		Custom4Inc,
		Custom5Inc,
		RegistrationDateInc,
		PointsInc,
		EarnedBadgeInc,
		PhysicalPrizeNameInc,
		PhysicalPrizeDateInc,
		NumReviewsInc,
		ReviewAuthorInc,
		ReviewTitleInc,
		ReviewDateInc,
		RandomDrawingNameInc,
		RandomDrawingNumInc,
		RandomDrawingDateInc,
		HasBeenDrawnInc,
		HasRedeemendInc,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		SDistrict,
		SDistrictInc,
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
		Score1From,
		Score1To,
		Score1PctFrom,
		Score1PctTo,
		Score2From,
		Score2To,
		Score2PctFrom,
		Score2PctTo,
		Score1Inc,
		Score2Inc,
		Score1PctInc,
		Score2PctInc
		)
	VALUES (
		@RTID,
		@ProgId,
		@ReportName,
		@DisplayFilters,
		@ReportFormat,
		@DOBFrom,
		@DOBTo,
		@AgeFrom,
		@AgeTo,
		@SchoolGrade,
		@FirstName,
		@LastName,
		@Gender,
		@EmailAddress,
		@PhoneNumber,
		@City,
		@State,
		@ZipCode,
		@County,
		@PrimaryLibrary,
		@SchoolName,
		@District,
		@Teacher,
		@GroupTeamName,
		@SchoolType,
		@LiteracyLevel1,
		@LiteracyLevel2,
		@Custom1,
		@Custom2,
		@Custom3,
		@Custom4,
		@Custom5,
		@RegistrationDateStart,
		@RegistrationDateEnd,
		@PointsMin,
		@PointsMax,
		@PointsStart,
		@PointsEnd,
		@EventCode,
		@EarnedBadge,
		@PhysicalPrizeEarned,
		@PhysicalPrizeRedeemed,
		@PhysicalPrizeStartDate,
		@PhysicalPrizeEndDate,
		@ReviewsMin,
		@ReviewsMax,
		@ReviewTitle,
		@ReviewAuthor,
		@ReviewStartDate,
		@ReviewEndDate,
		@RandomDrawingName,
		@RandomDrawingNum,
		@RandomDrawingStartDate,
		@RandomDrawingEndDate,
		@HasBeenDrawn,
		@HasRedeemend,
		@PIDInc,
		@UsernameInc,
		@DOBInc,
		@AgeInc,
		@SchoolGradeInc,
		@FirstNameInc,
		@LastNameInc,
		@GenderInc,
		@EmailAddressInc,
		@PhoneNumberInc,
		@CityInc,
		@StateInc,
		@ZipCodeInc,
		@CountyInc,
		@PrimaryLibraryInc,
		@SchoolNameInc,
		@DistrictInc,
		@TeacherInc,
		@GroupTeamNameInc,
		@SchoolTypeInc,
		@LiteracyLevel1Inc,
		@LiteracyLevel2Inc,
		@Custom1Inc,
		@Custom2Inc,
		@Custom3Inc,
		@Custom4Inc,
		@Custom5Inc,
		@RegistrationDateInc,
		@PointsInc,
		@EarnedBadgeInc,
		@PhysicalPrizeNameInc,
		@PhysicalPrizeDateInc,
		@NumReviewsInc,
		@ReviewAuthorInc,
		@ReviewTitleInc,
		@ReviewDateInc,
		@RandomDrawingNameInc,
		@RandomDrawingNumInc,
		@RandomDrawingDateInc,
		@HasBeenDrawnInc,
		@HasRedeemendInc,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@SDistrict,
		@SDistrictInc,
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
		@Score1From,
		@Score1To,
		@Score1PctFrom,
		@Score1PctTo,
		@Score2From,
		@Score2To,
		@Score2PctFrom,
		@Score2PctTo,
		@Score1Inc,
		@Score2Inc,
		@Score1PctInc,
		@Score2PctInc
		)

	SELECT @RID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SRPReport_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SRPReport_Update] (
	@RID INT,
	@RTID INT,
	@ProgId INT,
	@ReportName VARCHAR(150),
	@DisplayFilters BIT,
	@ReportFormat INT,
	@DOBFrom DATETIME,
	@DOBTo DATETIME,
	@AgeFrom INT,
	@AgeTo INT,
	@SchoolGrade VARCHAR(5),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Gender VARCHAR(1),
	@EmailAddress VARCHAR(150),
	@PhoneNumber VARCHAR(20),
	@City VARCHAR(20),
	@State VARCHAR(2),
	@ZipCode VARCHAR(10),
	@County VARCHAR(50),
	@PrimaryLibrary INT,
	@SchoolName VARCHAR(50),
	@District VARCHAR(50),
	@Teacher VARCHAR(20),
	@GroupTeamName VARCHAR(20),
	@SchoolType INT,
	@LiteracyLevel1 INT,
	@LiteracyLevel2 INT,
	@Custom1 VARCHAR(50),
	@Custom2 VARCHAR(50),
	@Custom3 VARCHAR(50),
	@Custom4 VARCHAR(50),
	@Custom5 VARCHAR(50),
	@RegistrationDateStart DATETIME,
	@RegistrationDateEnd DATETIME,
	@PointsMin INT,
	@PointsMax INT,
	@PointsStart DATETIME,
	@PointsEnd DATETIME,
	@EventCode VARCHAR(50),
	@EarnedBadge INT,
	@PhysicalPrizeEarned VARCHAR(50),
	@PhysicalPrizeRedeemed BIT,
	@PhysicalPrizeStartDate DATETIME,
	@PhysicalPrizeEndDate DATETIME,
	@ReviewsMin INT,
	@ReviewsMax INT,
	@ReviewTitle VARCHAR(150),
	@ReviewAuthor VARCHAR(100),
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@RandomDrawingName VARCHAR(50),
	@RandomDrawingNum INT,
	@RandomDrawingStartDate DATETIME,
	@RandomDrawingEndDate DATETIME,
	@HasBeenDrawn BIT,
	@HasRedeemend BIT,
	@PIDInc BIT,
	@UsernameInc BIT,
	@DOBInc BIT,
	@AgeInc BIT,
	@SchoolGradeInc BIT,
	@FirstNameInc BIT,
	@LastNameInc BIT,
	@GenderInc BIT,
	@EmailAddressInc BIT,
	@PhoneNumberInc BIT,
	@CityInc BIT,
	@StateInc BIT,
	@ZipCodeInc BIT,
	@CountyInc BIT,
	@PrimaryLibraryInc BIT,
	@SchoolNameInc BIT,
	@DistrictInc BIT,
	@TeacherInc BIT,
	@GroupTeamNameInc BIT,
	@SchoolTypeInc BIT,
	@LiteracyLevel1Inc BIT,
	@LiteracyLevel2Inc BIT,
	@Custom1Inc BIT,
	@Custom2Inc BIT,
	@Custom3Inc BIT,
	@Custom4Inc BIT,
	@Custom5Inc BIT,
	@RegistrationDateInc BIT,
	@PointsInc BIT,
	@EarnedBadgeInc BIT,
	@PhysicalPrizeNameInc BIT,
	@PhysicalPrizeDateInc BIT,
	@NumReviewsInc BIT,
	@ReviewAuthorInc BIT,
	@ReviewTitleInc BIT,
	@ReviewDateInc BIT,
	@RandomDrawingNameInc BIT,
	@RandomDrawingNumInc BIT,
	@RandomDrawingDateInc BIT,
	@HasBeenDrawnInc BIT,
	@HasRedeemendInc BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@SDistrict VARCHAR(50),
	@SDistrictInc BIT,
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
	@Score1From INT = 0,
	@Score1To INT = 0,
	@Score1PctFrom INT = 0,
	@Score1PctTo INT = 0,
	@Score2From INT = 0,
	@Score2To INT = 0,
	@Score2PctFrom INT = 0,
	@Score2PctTo INT = 0,
	@Score1Inc BIT,
	@Score2Inc BIT,
	@Score1PctInc BIT,
	@Score2PctInc BIT
	)
AS
UPDATE SRPReport
SET RTID = @RTID,
	ProgId = @ProgId,
	ReportName = @ReportName,
	DisplayFilters = @DisplayFilters,
	ReportFormat = @ReportFormat,
	DOBFrom = @DOBFrom,
	DOBTo = @DOBTo,
	AgeFrom = @AgeFrom,
	AgeTo = @AgeTo,
	SchoolGrade = @SchoolGrade,
	FirstName = @FirstName,
	LastName = @LastName,
	Gender = @Gender,
	EmailAddress = @EmailAddress,
	PhoneNumber = @PhoneNumber,
	City = @City,
	STATE = @State,
	ZipCode = @ZipCode,
	County = @County,
	PrimaryLibrary = @PrimaryLibrary,
	SchoolName = @SchoolName,
	District = @District,
	Teacher = @Teacher,
	GroupTeamName = @GroupTeamName,
	SchoolType = @SchoolType,
	LiteracyLevel1 = @LiteracyLevel1,
	LiteracyLevel2 = @LiteracyLevel2,
	Custom1 = @Custom1,
	Custom2 = @Custom2,
	Custom3 = @Custom3,
	Custom4 = @Custom4,
	Custom5 = @Custom5,
	RegistrationDateStart = @RegistrationDateStart,
	RegistrationDateEnd = @RegistrationDateEnd,
	PointsMin = @PointsMin,
	PointsMax = @PointsMax,
	PointsStart = @PointsStart,
	PointsEnd = @PointsEnd,
	EventCode = @EventCode,
	EarnedBadge = @EarnedBadge,
	PhysicalPrizeEarned = @PhysicalPrizeEarned,
	PhysicalPrizeRedeemed = @PhysicalPrizeRedeemed,
	PhysicalPrizeStartDate = @PhysicalPrizeStartDate,
	PhysicalPrizeEndDate = @PhysicalPrizeEndDate,
	ReviewsMin = @ReviewsMin,
	ReviewsMax = @ReviewsMax,
	ReviewTitle = @ReviewTitle,
	ReviewAuthor = @ReviewAuthor,
	ReviewStartDate = @ReviewStartDate,
	ReviewEndDate = @ReviewEndDate,
	RandomDrawingName = @RandomDrawingName,
	RandomDrawingNum = @RandomDrawingNum,
	RandomDrawingStartDate = @RandomDrawingStartDate,
	RandomDrawingEndDate = @RandomDrawingEndDate,
	HasBeenDrawn = @HasBeenDrawn,
	HasRedeemend = @HasRedeemend,
	PIDInc = @PIDInc,
	UsernameInc = @UsernameInc,
	DOBInc = @DOBInc,
	AgeInc = @AgeInc,
	SchoolGradeInc = @SchoolGradeInc,
	FirstNameInc = @FirstNameInc,
	LastNameInc = @LastNameInc,
	GenderInc = @GenderInc,
	EmailAddressInc = @EmailAddressInc,
	PhoneNumberInc = @PhoneNumberInc,
	CityInc = @CityInc,
	StateInc = @StateInc,
	ZipCodeInc = @ZipCodeInc,
	CountyInc = @CountyInc,
	PrimaryLibraryInc = @PrimaryLibraryInc,
	SchoolNameInc = @SchoolNameInc,
	DistrictInc = @DistrictInc,
	TeacherInc = @TeacherInc,
	GroupTeamNameInc = @GroupTeamNameInc,
	SchoolTypeInc = @SchoolTypeInc,
	LiteracyLevel1Inc = @LiteracyLevel1Inc,
	LiteracyLevel2Inc = @LiteracyLevel2Inc,
	Custom1Inc = @Custom1Inc,
	Custom2Inc = @Custom2Inc,
	Custom3Inc = @Custom3Inc,
	Custom4Inc = @Custom4Inc,
	Custom5Inc = @Custom5Inc,
	RegistrationDateInc = @RegistrationDateInc,
	PointsInc = @PointsInc,
	EarnedBadgeInc = @EarnedBadgeInc,
	PhysicalPrizeNameInc = @PhysicalPrizeNameInc,
	PhysicalPrizeDateInc = @PhysicalPrizeDateInc,
	NumReviewsInc = @NumReviewsInc,
	ReviewAuthorInc = @ReviewAuthorInc,
	ReviewTitleInc = @ReviewTitleInc,
	ReviewDateInc = @ReviewDateInc,
	RandomDrawingNameInc = @RandomDrawingNameInc,
	RandomDrawingNumInc = @RandomDrawingNumInc,
	RandomDrawingDateInc = @RandomDrawingDateInc,
	HasBeenDrawnInc = @HasBeenDrawnInc,
	HasRedeemendInc = @HasRedeemendInc,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	SDistrict = @SDistrict,
	SDistrictInc = @SDistrictInc,
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
	Score1From = @Score1From,
	Score1To = @Score1To,
	Score1PctFrom = @Score1PctFrom,
	Score1PctTo = @Score1PctTo,
	Score2From = @Score2From,
	Score2To = @Score2To,
	Score2PctFrom = @Score2PctFrom,
	Score2PctTo = @Score2PctTo,
	Score1Inc = @Score1Inc,
	Score2Inc = @Score2Inc,
	Score1PctInc = @Score1PctInc,
	Score2PctInc = @Score2PctInc
WHERE RID = @RID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Delete] @SID INT,
	@TenID INT = NULL
AS
DELETE
FROM [SRPSettings]
WHERE SID = @SID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [SRPSettings]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[app_SRPSettings_GetByID]    Script Date: 01/05/2015 14:43:27 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_GetByID] @SID INT
AS
SELECT *
FROM [SRPSettings]
WHERE SID = @SID
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_GetByName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SRPSettings_GetByName] @Name VARCHAR(50),
	@TenID INT = NULL
AS
SELECT *
FROM [SRPSettings]
WHERE NAME = @Name
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Insert] (
	@Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT,
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
	@SID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SRPSettings (
		NAME,
		Value,
		StorageType,
		EditType,
		ModID,
		Label,
		Description,
		ValueList,
		DefaultValue,
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
		@Name,
		@Value,
		@StorageType,
		@EditType,
		@ModID,
		@Label,
		@Description,
		@ValueList,
		@DefaultValue,
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

	SELECT @SID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SRPSettings_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SRPSettings_Update] (
	@SID INT,
	@Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT,
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
UPDATE SRPSettings
SET NAME = @Name,
	Value = @Value,
	StorageType = @StorageType,
	EditType = @EditType,
	ModID = @ModID,
	Label = @Label,
	Description = @Description,
	ValueList = @ValueList,
	DefaultValue = @DefaultValue,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SID = @SID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Survey_Delete] @SID INT
AS
DELETE
FROM SQChoices
WHERE QID IN (
		SELECT QID
		FROM SurveyQuestion
		WHERE SID = @SID
		)

DELETE
FROM SQMatrixLines
WHERE QID IN (
		SELECT QID
		FROM SurveyQuestion
		WHERE SID = @SID
		)

DELETE
FROM SurveyQuestion
WHERE SID = @SID

DELETE
FROM SurveyAnswers
WHERE SID = @SID

DELETE
FROM SurveyResults
WHERE SID = @SID

UPDATE Programs
SET PreTestID = 0
WHERE PreTestID = @SID

UPDATE Programs
SET PostTestID = 0
WHERE PostTestID = @SID

DELETE
FROM [Survey]
WHERE SID = @SID
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Survey]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_GetAllFinalized]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetAllFinalized] @TenID INT = NULL
AS
SELECT *
FROM [Survey]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
	AND STATUS = 2
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Survey_GetByID] @SID INT
AS
SELECT *
FROM [Survey]
WHERE SID = @SID
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_GetNumQuestions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Survey_GetNumQuestions] @SID INT = NULL
AS
SELECT isnull(Max(QNumber), 0) AS NumQuestions
FROM SurveyQuestion
WHERE SID = @SID
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Survey_Insert] (
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
		FldText3
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
		@FldText3
		)

	SELECT @SID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Survey_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Survey_Update] (
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
	@FldText3 TEXT
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
	FldText3 = @FldText3
WHERE SID = @SID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_Delete] @SAID INT
AS
DELETE
FROM [SurveyAnswers]
WHERE SAID = @SAID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetAll] @SRID INT
AS
SELECT a.*,
	q.QName,
	q.QText,
	q.QNumber
FROM [SurveyAnswers] a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
WHERE a.SRID = @SRID
ORDER BY q.QNumber
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_GetAllExpanded]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetAllExpanded] @SRID INT = NULL
AS
SELECT q.QText,
	a.*,
	(
		CASE a.SQMLID
			WHEN (
					SELECT MIN(SQMLID)
					FROM SurveyAnswers b
					WHERE b.SRID = @SRID
						AND b.QID = a.QID
					)
				THEN 1
			ELSE 0
			END
		) ShowQText
FROM SurveyAnswers a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
WHERE SRID = @SRID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetByID] @SAID INT
AS
SELECT *
FROM [SurveyAnswers]
WHERE SAID = @SAID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_Insert] (
	@SRID INT,
	@TenID INT,
	@PID INT,
	@SID INT,
	@QID INT,
	@SQMLID INT,
	@DateAnswered DATETIME,
	@QType INT,
	@FreeFormAnswer TEXT,
	@ClarificationText TEXT,
	@ChoiceAnswerIDs VARCHAR(2000),
	@ChoiceAnswerText TEXT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SAID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyAnswers (
		SRID,
		TenID,
		PID,
		SID,
		QID,
		SQMLID,
		DateAnswered,
		QType,
		FreeFormAnswer,
		ClarificationText,
		ChoiceAnswerIDs,
		ChoiceAnswerText,
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
		@SRID,
		@TenID,
		@PID,
		@SID,
		@QID,
		@SQMLID,
		@DateAnswered,
		@QType,
		@FreeFormAnswer,
		@ClarificationText,
		@ChoiceAnswerIDs,
		@ChoiceAnswerText,
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

	SELECT @SAID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyAnswers_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_Update] (
	@SAID INT,
	@SRID INT,
	@TenID INT,
	@PID INT,
	@SID INT,
	@QID INT,
	@SQMLID INT,
	@DateAnswered DATETIME,
	@QType INT,
	@FreeFormAnswer TEXT,
	@ClarificationText TEXT,
	@ChoiceAnswerIDs VARCHAR(2000),
	@ChoiceAnswerText TEXT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE SurveyAnswers
SET SRID = @SRID,
	TenID = @TenID,
	PID = @PID,
	SID = @SID,
	QID = @QID,
	SQMLID = @SQMLID,
	DateAnswered = @DateAnswered,
	QType = @QType,
	FreeFormAnswer = @FreeFormAnswer,
	ClarificationText = @ClarificationText,
	ChoiceAnswerIDs = @ChoiceAnswerIDs,
	ChoiceAnswerText = @ChoiceAnswerText,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SAID = @SAID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_Delete] @QID INT
AS
DECLARE @SID INT;

SELECT @SID = SID
FROM [SurveyQuestion]
WHERE QID = @QID

DELETE
FROM [SurveyQuestion]
WHERE QID = @QID

EXEC app_SurveyQuestion_Reorder @SID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetAll] @SID INT = NULL
AS
SELECT *,
	(
		SELECT isnull(Max(QNumber), 0)
		FROM SurveyQuestion
		WHERE SID = @SID
		) AS MAX
FROM [SurveyQuestion]
WHERE SID = @SID
ORDER BY QNumber
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetByID] @QID INT
AS
SELECT *
FROM [SurveyQuestion]
WHERE QID = @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_GetPageFromQNum]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetPageFromQNum] @SID INT = NULL,
	@QNum INT = 0
AS
DECLARE @StopQID INT
DECLARE @StopQNum INT

SELECT @StopQID = NULL,
	@StopQNum = NULL

SELECT TOP 1 @StopQID = QID,
	@StopQNum = QNumber
FROM [SurveyQuestion]
WHERE SID = @SID
	AND QNumber > @QNum
	AND QType IN (
		5,
		6
		)
ORDER BY QNumber

--select @StopQID, @StopQNum
SELECT *,
	(
		SELECT isnull(Max(QNumber), 0)
		FROM SurveyQuestion
		WHERE SID = @SID
		) AS MAX
FROM [SurveyQuestion]
WHERE SID = @SID
	AND (QNumber > @QNum)
	AND (
		QNumber <= @StopQNum
		OR @StopQNum IS NULL
		)
ORDER BY QNumber
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_Insert] (
	@SID INT,
	@QNumber INT,
	@QType INT,
	@QName VARCHAR(150),
	@QText TEXT,
	@DisplayControl INT,
	@DisplayDirection INT,
	@IsRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@QID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyQuestion (
		SID,
		QNumber,
		QType,
		QName,
		QText,
		DisplayControl,
		DisplayDirection,
		IsRequired,
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
		@SID,
		(
			SELECT isnull(Max(QNumber), 0) + 1
			FROM SurveyQuestion
			WHERE SID = @SID
			) --@QNumber
		,
		@QType,
		@QName,
		@QText,
		@DisplayControl,
		@DisplayDirection,
		@IsRequired,
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

	SELECT @QID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_MoveDn]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_MoveDn] @QID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@SID INT

SELECT @CurrentRecordLocation = QNumber,
	@SID = SID
FROM SurveyQuestion
WHERE QID = @QID

EXEC [dbo].[app_SurveyQuestion_Reorder] @SID

IF @CurrentRecordLocation < (
		SELECT MAX(QNumber)
		FROM SurveyQuestion
		WHERE SID = @SID
		)
BEGIN
	SELECT @NextRecordID = QID
	FROM SurveyQuestion
	WHERE QNumber = (@CurrentRecordLocation + 1)
		AND SID = @SID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation + 1
	WHERE QID = @QID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation
	WHERE QID = @NextRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_MoveUp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_MoveUp] @QID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@SID INT

SELECT @CurrentRecordLocation = QNumber,
	@SID = SID
FROM SurveyQuestion
WHERE QID = @QID

EXEC [dbo].[app_SurveyQuestion_Reorder] @SID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = QID
	FROM SurveyQuestion
	WHERE QNumber = (@CurrentRecordLocation - 1)
		AND SID = @SID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation - 1
	WHERE QID = @QID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation
	WHERE QID = @PreviousRecordID
END
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_Reorder]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyQuestion_Reorder] @SID INT
AS
UPDATE SurveyQuestion
SET QNumber = rowNumber
FROM SurveyQuestion
INNER JOIN (
	SELECT QID,
		QNumber,
		row_number() OVER (
			ORDER BY QNumber ASC
			) AS rowNumber
	FROM SurveyQuestion
	WHERE SID = @SID
	) drRowNumbers ON drRowNumbers.QID = SurveyQuestion.QID
	AND SID = @SID
WHERE SID = @SID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyQuestion_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SurveyQuestion_Update] (
	@QID INT,
	@SID INT,
	@QNumber INT,
	@QType INT,
	@QName VARCHAR(150),
	@QText TEXT,
	@DisplayControl INT,
	@DisplayDirection INT,
	@IsRequired BIT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE SurveyQuestion
SET SID = @SID,
	QNumber = @QNumber,
	QType = @QType,
	QName = @QName,
	QText = @QText,
	DisplayControl = @DisplayControl,
	DisplayDirection = @DisplayDirection,
	IsRequired = @IsRequired,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE QID = @QID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_Delete] @SRID INT
AS
DELETE
FROM dbo.SurveyAnswers
WHERE SRID = @SRID

DELETE
FROM [SurveyResults]
WHERE SRID = @SRID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetAll] @TenID INT = 0,
	@PID INT = NULL,
	@SID INT = NULL
AS
SELECT *
FROM [SurveyResults]
WHERE TenID = @TenID
	AND (
		PID = @PID
		OR @PID IS NULL
		)
	AND (
		SID = @SID
		OR @SID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetAllComplete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetAllComplete] @TenID INT = 0,
	@PID INT = NULL
AS
SELECT sr.*,
	s.NAME,
	(
		CASE Source
			WHEN 'Program Pre-Test'
				THEN isnull((
							SELECT TOP 1 AdminName
							FROM Programs
							WHERE PID = SourceID
							) + ' Program', 'N/A')
			WHEN 'Program Post-Test'
				THEN isnull((
							SELECT TOP 1 AdminName
							FROM Programs
							WHERE PID = SourceID
							) + ' Program', 'N/A')
			WHEN 'Game'
				THEN isnull((
							SELECT TOP 1 AdminName
							FROM Minigame
							WHERE MGID = SourceID
							) + ' Minigame', 'N/A')
			WHEN 'Book List'
				THEN isnull((
							SELECT TOP 1 AdminName
							FROM BookList
							WHERE BLID = SourceID
							) + ' Book List', 'N/A')
			WHEN 'Event'
				THEN isnull((
							SELECT TOP 1 NAME
							FROM Event
							WHERE EID = SourceID
							) + ' Event', 'N/A')
			WHEN 'Reading Log'
				THEN ''
			ELSE 'N/A'
			END
		) AS SourceName
FROM [SurveyResults] sr
INNER JOIN Survey s ON sr.SID = s.SID
WHERE sr.TenID = @TenID
	AND (
		PID = @PID
		OR @PID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetAllExpanded]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetAllExpanded] @TenID INT = 0,
	@PID INT = NULL,
	@SID INT = NULL
AS
SELECT *
FROM [SurveyResults]
WHERE TenID = @TenID
	AND (
		PID = @PID
		OR @PID IS NULL
		)
	AND (
		SID = @SID
		OR @SID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetAllStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetAllStats] @TenID INT = 0,
	@SID INT = NULL,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL,
	@SchoolID INT = NULL
AS
SELECT isnull(COUNT(*), 0) AS NumTotal
FROM [SurveyResults]
INNER JOIN Patron ON SurveyResults.PID = Patron.PID
WHERE SurveyResults.TenID = @TenID
	AND (
		SID = @SID
		OR @SID IS NULL
		)
	AND (
		Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		SourceID = @SourceID
		OR @SourceID IS NULL
		)
	AND (
		SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)

SELECT isnull(COUNT(DISTINCT SurveyResults.PID), 0) AS NumPatrons
FROM [SurveyResults]
INNER JOIN Patron ON SurveyResults.PID = Patron.PID
WHERE SurveyResults.TenID = @TenID
	AND (
		SID = @SID
		OR @SID IS NULL
		)
	AND (
		Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		SourceID = @SourceID
		OR @SourceID IS NULL
		)
	AND (
		SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)

SELECT DISTINCT a.SID,
	a.QID,
	a.SQMLID,
	a.QType,
	q.QNumber --, q.QText
	,
	(
		CASE a.SQMLID
			WHEN (
					SELECT MIN(SQMLID)
					FROM SurveyAnswers b
					WHERE b.QID = a.QID
					)
				THEN 1
			ELSE 0
			END
		) ShowQText,
	(
		SELECT COUNT(*)
		FROM SurveyAnswers a2
		WHERE a.SID = a2.SID
			AND a.QID = a2.QID
			AND a.SQMLID = a2.SQMLID
		) AS NumAnswers
FROM SurveyAnswers a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
INNER JOIN Patron p ON r.PID = p.PID
WHERE (
		a.SID = @SID
		OR @SID IS NULL
		)
	AND (
		p.SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)
ORDER BY q.QNumber,
	a.SID,
	a.QID,
	a.SQMLID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetByID] @SRID INT
AS
SELECT *
FROM [SurveyResults]
WHERE SRID = @SRID
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetBySurveyAndSource]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_GetBySurveyAndSource] @PID INT,
	@SID INT,
	@SrcType VARCHAR(150),
	@SrcID INT
AS
SELECT TOP 1 *
FROM [SurveyResults]
WHERE PID = @PID
	AND SID = @SID
	AND Source = @SrcType
	AND SourceID = @SrcID
ORDER BY StartDate DESC
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetExport]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetExport] @SID INT = NULL,
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
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQClarifications]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQClarifications] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@Answer VARCHAR(8000),
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
SELECT ClarificationText
FROM SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
WHERE a.SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
	AND convert(VARCHAR(8000), ChoiceAnswerText) = @Answer
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQComments]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQComments] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = NULL
AS
SELECT 1
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQFreeForm]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQFreeForm] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
SELECT FreeFormAnswer
FROM SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
WHERE a.SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
	AND convert(VARCHAR(8000), FreeFormAnswer) <> ''
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStats] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0
AS
SELECT CONVERT(VARCHAR(8000), ChoiceAnswerText) AS ChoiceAnswerText,
	COUNT(*) AS Count
FROM SurveyAnswers a
WHERE SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
GROUP BY CONVERT(VARCHAR(8000), ChoiceAnswerText)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQStatsMedium]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStatsMedium] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
SELECT REPLACE(CONVERT(VARCHAR(8000), a.ChoiceAnswerText), '~|~', ' AND ') AS ChoiceText,
	CONVERT(VARCHAR(8000), a.ChoiceAnswerText) AS ChoiceTextORIGINAL,
	COUNT(*) AS Count
FROM SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
WHERE a.SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
GROUP BY CONVERT(VARCHAR(8000), ChoiceAnswerText)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetQStatsSimple]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStatsSimple] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
DECLARE @ChoiceAnswerText VARCHAR(8000)

DECLARE db_cursor CURSOR
FOR
SELECT a.ChoiceAnswerText
FROM SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
WHERE a.SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID

OPEN db_cursor

FETCH NEXT
FROM db_cursor
INTO @ChoiceAnswerText

CREATE TABLE #Stats (Value VARCHAR(256))

WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO #Stats
	SELECT Value
	FROM dbo.fnSplitString(REPLACE(@ChoiceAnswerText, '~|~', '|'), '|')

	FETCH NEXT
	FROM db_cursor
	INTO @ChoiceAnswerText
END

CLOSE db_cursor

DEALLOCATE db_cursor

SELECT Value,
	COUNT(*) AS Count
INTO #Stats2
FROM #Stats
GROUP BY Value

DROP TABLE #Stats

SELECT c.ChoiceText,
	ISNULL(d.Count, 0) AS Count
FROM SQChoices c
LEFT JOIN #Stats2 d ON c.ChoiceText = d.Value
WHERE QID = @QID
ORDER BY c.ChoiceOrder
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_GetSources]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_SurveyResults_GetSources] @SID INT = NULL
AS
SELECT DISTINCT [Source],
	[SourceID],
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
FROM [SurveyResults]
WHERE (
		SID = @SID
		OR @SID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_Insert] (
	@TenID INT,
	@PID INT,
	@SID INT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@IsComplete BIT,
	@IsScorable BIT,
	@LastAnswered INT,
	@Score INT,
	@ScorePct DECIMAL,
	@Source VARCHAR(50),
	@SourceID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@SRID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO SurveyResults (
		TenID,
		PID,
		SID,
		StartDate,
		EndDate,
		IsComplete,
		IsScorable,
		LastAnswered,
		Score,
		ScorePct,
		Source,
		SourceID,
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
		@TenID,
		@PID,
		@SID,
		@StartDate,
		@EndDate,
		@IsComplete,
		@IsScorable,
		@LastAnswered,
		@Score,
		@ScorePct,
		@Source,
		@SourceID,
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

	SELECT @SRID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_SurveyResults_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_Update] (
	@SRID INT,
	@TenID INT,
	@PID INT,
	@SID INT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@IsComplete BIT,
	@IsScorable BIT,
	@LastAnswered INT,
	@Score INT,
	@ScorePct DECIMAL,
	@Source VARCHAR(50),
	@SourceID INT,
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE SurveyResults
SET TenID = @TenID,
	PID = @PID,
	SID = @SID,
	StartDate = @StartDate,
	EndDate = @EndDate,
	IsComplete = @IsComplete,
	IsScorable = @IsScorable,
	LastAnswered = @LastAnswered,
	Score = @Score,
	ScorePct = @ScorePct,
	Source = @Source,
	SourceID = @SourceID,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE SRID = @SRID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Tenant_Delete] @TenID INT
AS
DELETE
FROM [Tenant]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetAll]
AS
SELECT *
FROM [Tenant]
ORDER BY LandingName
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetAllActive]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetAllActive]
AS
SELECT *
FROM [Tenant]
WHERE isActiveFlag = 1
	AND isMasterFlag = 0
ORDER BY LandingName
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetByDomainName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Tenant_GetByDomainName] @Domain VARCHAR(128)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @TenID INT

SELECT @TenID = - 1

IF EXISTS (
		SELECT *
		FROM Tenant
		WHERE DomainName = @Domain
		)
	SELECT @TenID = TenID
	FROM Tenant
	WHERE DomainName = @Domain

RETURN @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetByID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Tenant_GetByID] @TenID INT
AS
SELECT *
FROM [Tenant]
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetByProgramID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Tenant_GetByProgramID] @PID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @TenID INT

SELECT @TenID = - 1

IF EXISTS (
		SELECT *
		FROM Programs
		WHERE PID = @PID
		)
	SELECT @TenID = TenID
	FROM Programs
	WHERE PID = @PID

RETURN @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_GetMasterID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_Tenant_GetMasterID]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @TenID INT

SELECT @TenID = - 1

IF EXISTS (
		SELECT TOP 1 *
		FROM Tenant
		WHERE isMasterFlag = 1
		)
	SELECT TOP 1 @TenID = TenID
	FROM Tenant
	WHERE isMasterFlag = 1
ELSE
	SELECT TOP 1 @TenID = TenID
	FROM Tenant
	WHERE isActiveFlag = 1
	ORDER BY TenID

SELECT @TenID

RETURN @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_Tenant_Insert] (
	@Name VARCHAR(150),
	@LandingName VARCHAR(50),
	@AdminName VARCHAR(50),
	@isActiveFlag BIT,
	@isMasterFlag BIT,
	@Description TEXT,
	@DomainName VARCHAR(50),
	@showNotifications BIT,
	@showOffers BIT,
	@showBadges BIT,
	@showEvents BIT,
	@NotificationsMenuText VARCHAR(50),
	@OffersMenuText VARCHAR(50),
	@BadgesMenuText VARCHAR(50),
	@EventsMenuText VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT,
	@TenID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Tenant (
		NAME,
		LandingName,
		AdminName,
		isActiveFlag,
		isMasterFlag,
		Description,
		DomainName,
		showNotifications,
		showOffers,
		showBadges,
		showEvents,
		NotificationsMenuText,
		OffersMenuText,
		BadgesMenuText,
		EventsMenuText,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
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
		@Name,
		@LandingName,
		@AdminName,
		@isActiveFlag,
		@isMasterFlag,
		@Description,
		@DomainName,
		@showNotifications,
		@showOffers,
		@showBadges,
		@showEvents,
		@NotificationsMenuText,
		@OffersMenuText,
		@BadgesMenuText,
		@EventsMenuText,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
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

	SELECT @TenID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[app_Tenant_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Create the Update Proc
CREATE PROCEDURE [dbo].[app_Tenant_Update] (
	@TenID INT,
	@Name VARCHAR(150),
	@LandingName VARCHAR(50),
	@AdminName VARCHAR(50),
	@isActiveFlag BIT,
	@isMasterFlag BIT,
	@Description TEXT,
	@DomainName VARCHAR(50),
	@showNotifications BIT,
	@showOffers BIT,
	@showBadges BIT,
	@showEvents BIT,
	@NotificationsMenuText VARCHAR(50),
	@OffersMenuText VARCHAR(50),
	@BadgesMenuText VARCHAR(50),
	@EventsMenuText VARCHAR(50),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@FldInt1 INT,
	@FldInt2 INT,
	@FldInt3 INT,
	@FldBit1 BIT,
	@FldBit2 BIT,
	@FldBit3 BIT,
	@FldText1 TEXT,
	@FldText2 TEXT,
	@FldText3 TEXT
	)
AS
UPDATE Tenant
SET NAME = @Name,
	LandingName = @LandingName,
	AdminName = @AdminName,
	isActiveFlag = @isActiveFlag,
	isMasterFlag = @isMasterFlag,
	Description = @Description,
	DomainName = @DomainName,
	showNotifications = @showNotifications,
	showOffers = @showOffers,
	showBadges = @showBadges,
	showEvents = @showEvents,
	NotificationsMenuText = @NotificationsMenuText,
	OffersMenuText = @OffersMenuText,
	BadgesMenuText = @BadgesMenuText,
	EventsMenuText = @EventsMenuText,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	FldInt1 = @FldInt1,
	FldInt2 = @FldInt2,
	FldInt3 = @FldInt3,
	FldBit1 = @FldBit1,
	FldBit2 = @FldBit2,
	FldBit3 = @FldBit3,
	FldText1 = @FldText1,
	FldText2 = @FldText2,
	FldText3 = @FldText3
WHERE TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[app_TenantInitData_GetPKbyOriginalPK]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_TenantInitData_GetPKbyOriginalPK] (
	@IntitType VARCHAR(50),
	@DestTID INT,
	@SrcPK INT
	)
AS
BEGIN
	SELECT DstPK
	FROM TenantInitData
	WHERE IntitType = @IntitType
		AND DestTID = @DestTID
		AND SrcPK = @SrcPK
END
GO
/****** Object:  StoredProcedure [dbo].[app_TenantInitData_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[app_TenantInitData_Insert] (
	@IntitType VARCHAR(50),
	@DestTID INT,
	@SrcPK INT,
	@DateCreated DATETIME,
	@DstPK INT,
	@InitID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO TenantInitData (
		IntitType,
		DestTID,
		SrcPK,
		DateCreated,
		DstPK
		)
	VALUES (
		@IntitType,
		@DestTID,
		@SrcPK,
		@DateCreated,
		@DstPK
		)

	SELECT @InitID = SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPGroups_Delete] @GID INT,
	@ActionUsername VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE
FROM dbo.SRPGroups
WHERE GID = @GID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_DeleteAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_DeleteAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes all records from the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_DeleteAll]
AS
DELETE
FROM [dbo].[SRPGroups]
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_DeleteByPrimaryKey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Deletes a record from the 'SRPGroups' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPGroups_DeleteByPrimaryKey] @GID INT,
	@TenID INT = NULL
AS
DELETE
FROM [dbo].[SRPGroups]
WHERE [GID] = @GID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Get]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Get]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPGroups_Get] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPGroups
WHERE GID = @GID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Gets all records from the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [dbo].[SRPGroups]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_GetByPrimaryKey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_GetByPrimaryKey]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets a record from the 'SRPGroups' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPGroups_GetByPrimaryKey] @GID INT
AS
SELECT *
FROM [dbo].[SRPGroups]
WHERE [GID] = @GID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Inserts a new record into the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_Insert] @GroupName VARCHAR(50),
	@GroupDescription VARCHAR(255),
	@ActionUsername VARCHAR(50),
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
AS
INSERT INTO [dbo].[SRPGroups] (
	[GroupName],
	[GroupDescription],
	[LastModDate],
	[LastModUser],
	[AddedDate],
	[AddedUser],
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
	@GroupName,
	@GroupDescription,
	GETDATE(),
	@ActionUsername,
	GETDATE(),
	@ActionUsername,
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

SELECT @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroups_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Updates a record in the 'SRPGroups' table.
CREATE PROCEDURE [dbo].[cbspSRPGroups_Update]
	-- The rest of writeable parameters
	@GroupName VARCHAR(50),
	@GroupDescription VARCHAR(255),
	@ActionUsername VARCHAR(50),
	-- Primary key parameters
	@GID INT,
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
AS
UPDATE [dbo].[SRPGroups]
SET [GroupName] = @GroupName,
	[GroupDescription] = @GroupDescription,
	[LastModDate] = GETDATE(),
	[LastModUser] = @ActionUsername,
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
WHERE [GID] = @GID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPGroupsGroups_GetSpecialUserPermissionsNotGranted] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPPermissionsMaster
WHERE dbo.SRPPermissionsMaster.PermissionID NOT IN (
		SELECT dbo.SRPPermissionsMaster.PermissionID
		FROM dbo.SRPPermissionsMaster
		INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
		INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
		WHERE dbo.SRPGroupPermissions.GID = @GID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Delete] @PermissionID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE
FROM dbo.SRPPermissionsMaster
WHERE @PermissionID = PermissionID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_DeleteByModule]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_DeleteByModule]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_DeleteByModule] @ModId INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE
FROM dbo.SRPPermissionsMaster
WHERE ModId = @ModId
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Get]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Get]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Get] @PermissionID INT = - 1
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPPermissionsMaster
WHERE PermissionID = @PermissionID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModule]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModule]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByModule] @ModID INT = - 1
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPPermissionsMaster
WHERE ModID = @ModID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModuleName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByModuleName]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByModuleName] @ModuleName VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPPermissionsMaster.*
FROM dbo.SRPModule
INNER JOIN dbo.SRPPermissionsMaster ON dbo.SRPModule.ModId = dbo.SRPPermissionsMaster.MODID
WHERE dbo.SRPModule.ModName = @ModuleName
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_GetByName]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_GetByName] @PermissionName VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPPermissionsMaster
WHERE PermissionName = @PermissionName
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Insert]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Insert] @PermissionID INT,
	@PermissionName VARCHAR(50),
	@PermissionDesc TEXT,
	@MODID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO dbo.SRPPermissionsMaster (
	PermissionID,
	PermissionName,
	PermissionDesc,
	MODID
	)
VALUES (
	@PermissionID,
	@PermissionName,
	@PermissionDesc,
	@MODID
	)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPPermissionsMaster_Update]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPPermissionsMaster_Update] @PermissionID INT,
	@PermissionName VARCHAR(50),
	@PermissionDesc TEXT,
	@MODID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPPermissionsMaster
SET PermissionName = @PermissionName,
	PermissionDesc = @PermissionDesc,
	MODID = @MODID
WHERE PermissionID = @PermissionID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes all records from the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_DeleteAll]
AS
DELETE
FROM [dbo].[SRPSettings]
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteByPrimaryKey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_DeleteByPrimaryKey]    Script Date: 01/05/2015 14:43:27 ******/
-- Deletes a record from the 'SRPSettings' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPSettings_DeleteByPrimaryKey] @SID INT
AS
DELETE
FROM [dbo].[SRPSettings]
WHERE [SID] = @SID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetAll]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets all records from the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_GetAll]
AS
SELECT *
FROM [dbo].[SRPSettings]
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetByName]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetByName]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets a record from the 'SRPSettings' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPSettings_GetByName] @Name VARCHAR(50)
AS
SELECT *
FROM [dbo].[SRPSettings]
WHERE [Name] = @Name
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetByPrimaryKey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_GetByPrimaryKey]    Script Date: 01/05/2015 14:43:27 ******/
-- Gets a record from the 'SRPSettings' table using the primary key value.
CREATE PROCEDURE [dbo].[cbspSRPSettings_GetByPrimaryKey] @SID INT
AS
SELECT *
FROM [dbo].[SRPSettings]
WHERE [SID] = @SID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Insert]    Script Date: 01/05/2015 14:43:27 ******/
-- Inserts a new record into the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_Insert] @Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT
AS
INSERT INTO [dbo].[SRPSettings] (
	[Name],
	[Value],
	[StorageType],
	[EditType],
	[ModID],
	[Label],
	[Description],
	[ValueList],
	[DefaultValue]
	)
VALUES (
	@Name,
	@Value,
	@StorageType,
	@EditType,
	@ModID,
	@Label,
	@Description,
	@ValueList,
	@DefaultValue
	)

SELECT @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPSettings_Update]    Script Date: 01/05/2015 14:43:27 ******/
-- Updates a record in the 'SRPSettings' table.
CREATE PROCEDURE [dbo].[cbspSRPSettings_Update]
	-- The rest of writeable parameters
	@Name VARCHAR(50),
	@Value TEXT,
	@StorageType VARCHAR(50),
	@EditType VARCHAR(50),
	@ModID INT,
	@Label VARCHAR(50),
	@Description VARCHAR(500),
	@ValueList VARCHAR(5000),
	@DefaultValue TEXT,
	-- Primary key parameters
	@SID INT
AS
UPDATE [dbo].[SRPSettings]
SET [Name] = @Name,
	[Value] = @Value,
	[StorageType] = @StorageType,
	[EditType] = @EditType,
	[ModID] = @ModID,
	[Label] = @Label,
	[Description] = @Description,
	[ValueList] = @ValueList,
	[DefaultValue] = @DefaultValue
WHERE [SID] = @SID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Delete]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Delete]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Delete] @UID INT,
	@ActionUsername VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET isDeleted = 1,
	DeletedDate = getdate(),
	LastModDate = getdate(),
	LastModUser = @ActionUsername
WHERE UID = @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_EmailExists]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_EmailExists]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_EmailExists] @EmailAddress VARCHAR(128)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @RowCount INT

SELECT @RowCount = Count(*)
FROM dbo.SRPUser
WHERE LOWER(EmailAddress) = LOWER(@EmailAddress)

IF @RowCount > 0
	RETURN 1
ELSE
	RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Get]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Get]    Script Date: 01/05/2015 14:43:27 ******/
/*
procedure [DAL].[Applicationuser_IO]
@Action		int			= 0
, @AuditpointID
as
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
declare @RowCount int
@@IDENTITY
@RowCount = @@ROWCOUNT
raiserror('UDE-CONCURRENCY',11,11) with SETERROR
set @intErrFlag = 11
*/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Get] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE UID = @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetActiveSessions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetActiveSessions]    Script Date: 01/05/2015 14:43:27 ******/
----------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetActiveSessions]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUserLoginHistory.UIDLH,
	dbo.SRPUserLoginHistory.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserLoginHistory.SessionsID,
	dbo.SRPUserLoginHistory.StartDateTime,
	dbo.SRPUserLoginHistory.IP,
	dbo.SRPUserLoginHistory.MachineName,
	dbo.SRPUserLoginHistory.Browser,
	dbo.SRPUserLoginHistory.EndDateTime
FROM dbo.SRPUser
INNER JOIN dbo.SRPUserLoginHistory ON dbo.SRPUser.UID = dbo.SRPUserLoginHistory.UID
WHERE dbo.SRPUserLoginHistory.EndDateTime IS NULL
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetAll] @TenID INT = NULL
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE IsDeleted = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAllPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAllPermissions]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_GetAllPermissions] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
WHERE dbo.SRPGroups.GID IN (
		SELECT GID
		FROM SRPUserGroups
		WHERE UID = @UID
		)

UNION

SELECT dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAllPermissionsAUDIT]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetAllPermissionsAUDIT]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_GetAllPermissionsAUDIT] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID AS ID,
	'G' AS Type,
	dbo.SRPGroups.GroupName AS NAME,
	GroupDescription AS Description,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPGroupPermissions.AddedDate,
	dbo.SRPGroupPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
WHERE dbo.SRPGroups.GID IN (
		SELECT GID
		FROM SRPUserGroups
		WHERE UID = @UID
		)

UNION

SELECT dbo.SRPUser.UID AS ID,
	'U' AS type,
	Firstname + ' ' + LastName AS NAME,
	'' AS Description,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPUserPermissions.AddedDate,
	dbo.SRPUserPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
ORDER BY PermissionID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetByUsername]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetByUsername]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetByUsername] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT *
FROM dbo.SRPUser
WHERE Username = @Username
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroups]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroups] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUser.UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser
FROM dbo.SRPGroups
INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroupsFlagged]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroupsFlagged] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @TenID INT

SELECT @TenID = TenID
FROM SRPUser
WHERE UID = @UID

SELECT @UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser,
	CASE 
		WHEN dbo.SRPUserGroups.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPGroups
LEFT JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
	AND dbo.SRPUserGroups.UID = @UID
WHERE UID = @UID
	OR UID IS NULL
	AND SRPGroups.TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroupsNonMembers]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetGroupsNonMembers]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetGroupsNonMembers] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPGroups
WHERE dbo.SRPGroups.GID NOT IN (
		SELECT dbo.SRPGroups.GID
		FROM dbo.SRPGroups
		INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
		INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
		WHERE dbo.SRPUserGroups.UID = @UID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetLoginHistory]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetLoginHistory]    Script Date: 01/05/2015 14:43:27 ******/
---------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetLoginHistory] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT h.*,
	u.Username,
	u.FirstName + ' ' + u.LastName AS NAME
FROM dbo.SRPUserLoginHistory h,
	dbo.SRPUser u
WHERE u.UID = @UID
	AND u.UID = h.UID
ORDER BY StartDateTime DESC
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetLoginNow]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[cbspSRPUser_GetLoginNow]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT h.*,
	u.Username,
	u.FirstName + ' ' + u.LastName AS NAME,
	t.AdminName AS Tenant
FROM dbo.SRPUserLoginHistory h,
	dbo.SRPUser u
INNER JOIN dbo.Tenant t ON u.TenID = t.TenID
WHERE EndDateTime IS NULL
	AND u.UID = h.UID
ORDER BY StartDateTime DESC
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetLoginNowTenID]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[cbspSRPUser_GetLoginNowTenID] @TenID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT h.*,
	u.Username,
	u.FirstName + ' ' + u.LastName AS NAME,
	t.AdminName AS Tenant
FROM dbo.SRPUserLoginHistory h,
	dbo.SRPUser u
INNER JOIN dbo.Tenant t ON u.TenID = t.TenID
WHERE EndDateTime IS NULL
	AND u.UID = h.UID
	AND u.TenID = @TenID
ORDER BY StartDateTime DESC
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissions]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissions] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPUser.UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPUserPermissions.AddedDate,
	dbo.SRPUserPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
WHERE dbo.SRPUser.UID = @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissionsFlagged] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPUserPermissions.AddedDate,
	dbo.SRPUserPermissions.AddedUser,
	CASE 
		WHEN dbo.SRPUserPermissions.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPPermissionsMaster
LEFT JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
	AND dbo.SRPUserPermissions.UID = @UID
WHERE UID = @UID
	OR UID IS NULL
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_GetSpecialUserPermissionsNotGranted] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @UID AS UID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPPermissionsMaster
WHERE dbo.SRPPermissionsMaster.PermissionID NOT IN (
		SELECT dbo.SRPPermissionsMaster.PermissionID
		FROM dbo.SRPPermissionsMaster
		INNER JOIN dbo.SRPUserPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPUserPermissions.PermissionID
		INNER JOIN dbo.SRPUser ON dbo.SRPUserPermissions.UID = dbo.SRPUser.UID
		WHERE dbo.SRPUserPermissions.UID = @UID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Insert]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Insert] @Username VARCHAR(50),
	@Password VARCHAR(255),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@EmailAddress VARCHAR(128),
	@Division VARCHAR(50) = NULL,
	@Department VARCHAR(50) = NULL,
	@Title VARCHAR(50) = NULL,
	@IsActive BIT = 1,
	@MustResetPassword BIT = 0,
	@IsDeleted BIT = 0,
	@ActionUsername VARCHAR(50),
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
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO dbo.SRPUser (
	Username,
	Password,
	FirstName,
	LastName,
	EmailAddress,
	Division,
	Department,
	Title,
	IsActive,
	MustResetPassword,
	IsDeleted,
	LastPasswordReset,
	DeletedDate,
	AddedDate,
	AddedUser,
	LastModDate,
	LastModUser,
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
	@Username,
	@Password,
	@FirstName,
	@LastName,
	@EmailAddress,
	@Division,
	@Department,
	@Title,
	@IsActive,
	@MustResetPassword,
	@IsDeleted,
	NULL,
	NULL,
	getdate(),
	@ActionUsername,
	getdate(),
	@ActionUsername,
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

SELECT @@IDENTITY

RETURN @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Login]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Login]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_Login] @UserName VARCHAR(50),
	@SessionId VARCHAR(128) = 'N/A',
	@IP VARCHAR(50) = 'N/A',
	@MachineName VARCHAR(50) = 'N/A',
	@Browser VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @Count INT,
	@IsActive BIT,
	@IsDeleted BIT,
	@MustResetPassword BIT,
	@UID INT

SELECT @Count = isnull(Count(*), 0),
	@UID = UID,
	@IsActive = IsActive,
	@IsDeleted = IsDeleted
FROM dbo.SRPUser
WHERE Username = @UserName
	AND IsDeleted = 0
	AND IsActive = 1
GROUP BY UID,
	IsActive,
	IsDeleted

IF @Count = 0
	OR @Count IS NULL
BEGIN
	--SELECT
	--		*
	--FROM
	--	dbo.SRPUser
	--WHERE
	--	 Username is null
	SELECT 0
END
ELSE
BEGIN
	--SELECT
	--	*
	--FROM
	--	dbo.SRPUser
	--WHERE
	--	UID = @UID
	INSERT INTO dbo.SRPUserLoginHistory (
		UID,
		SessionsID,
		StartDateTime,
		IP,
		MachineName,
		Browser,
		EndDateTime
		)
	VALUES (
		@UID,
		@SessionId,
		getdate(),
		@IP,
		@MachineName,
		@Browser,
		NULL
		)

	--exec cbspSRPUser_GetAllPermissions @UID
	SELECT 1
END

RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Logout]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Logout]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_Logout] @UID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUserLoginHistory
SET EndDateTime = getdate()
WHERE UID = @UID
	AND EndDateTime IS NULL
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_LogoutAll]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
CREATE PROCEDURE [dbo].[cbspSRPUser_LogoutAll]
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
UPDATE dbo.SRPUserLoginHistory
SET EndDateTime = getdate()
WHERE EndDateTime IS NULL
	/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_ResetPassword]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_ResetPassword]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUser_ResetPassword] @UID INT,
	@Password VARCHAR(50),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET Password = @Password,
	LastPasswordReset = getdate(),
	LastModDate = getdate(),
	LastModUser = @ActionUsername
WHERE UID = @UID

SELECT *
FROM dbo.SRPUser
WHERE UID = @UID

EXEC cbspSRPUser_GetAllPermissions @UID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_Update]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_Update] @UID INT,
	@Username VARCHAR(50),
	@Password VARCHAR(255),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@EmailAddress VARCHAR(128),
	@Division VARCHAR(50) = NULL,
	@Department VARCHAR(50) = NULL,
	@Title VARCHAR(50) = NULL,
	@IsActive BIT = 1,
	@MustResetPassword BIT = 0,
	@IsDeleted BIT = 0,
	@LastPasswordReset DATETIME = NULL,
	@ActionUsername VARCHAR(50),
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
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

UPDATE dbo.SRPUser
SET Username = @Username,
	Password = @Password,
	FirstName = @FirstName,
	LastName = @LastName,
	EmailAddress = @EmailAddress,
	Division = @Division,
	Department = @Department,
	Title = @Title,
	IsActive = @IsActive,
	MustResetPassword = @MustResetPassword,
	IsDeleted = @IsDeleted,
	LastPasswordReset = @LastPasswordReset,
	LastModDate = getdate(),
	LastModUser = @ActionUsername,
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
WHERE UID = @UID
	AND TenID = @TenID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateGroups]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UpdateGroups] @UID INT,
	@GID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserGroups
WHERE UID = @UID
	AND GID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@GID_LIST)
		)

INSERT INTO dbo.SRPUserGroups
SELECT @UID,
	GID,
	getdate(),
	@ActionUsername
FROM dbo.SRPGroups
WHERE GID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@GID_LIST)
		)
	AND GID NOT IN (
		SELECT GID
		FROM dbo.SRPUserGroups
		WHERE UID = @UID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateSpecialUserPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UpdateSpecialUserPermissions]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UpdateSpecialUserPermissions] @UID INT,
	@PermissionID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserPermissions
WHERE UID = @UID
	AND PermissionID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)

INSERT INTO dbo.SRPUserPermissions
SELECT @UID,
	PermissionID,
	getdate(),
	@ActionUsername
FROM dbo.SRPPermissionsMaster
WHERE PermissionID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)
	AND PermissionID NOT IN (
		SELECT PermissionID
		FROM dbo.SRPUserPermissions
		WHERE UID = @UID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UsernameExists]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUser_UsernameExists]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUser_UsernameExists] @Username VARCHAR(50)
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @RowCount INT

SELECT @RowCount = Count(*)
FROM dbo.SRPUser
WHERE Username = @Username

IF @RowCount > 0
	RETURN 1
ELSE
	RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissions]    Script Date: 01/05/2015 14:43:27 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetPermissions] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPGroupPermissions.AddedDate,
	dbo.SRPGroupPermissions.AddedUser
FROM dbo.SRPPermissionsMaster
INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
WHERE dbo.SRPGroups.GID = @GID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissionsFlagged]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissionsFlagged]    Script Date: 01/05/2015 14:43:27 ******/
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetPermissionsFlagged] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	dbo.SRPGroupPermissions.AddedDate,
	dbo.SRPGroupPermissions.AddedUser,
	CASE 
		WHEN dbo.SRPGroupPermissions.AddedDate IS NULL
			THEN 0
		ELSE 1
		END AS Checked
FROM dbo.SRPPermissionsMaster
LEFT JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
	AND dbo.SRPGroupPermissions.GID = @GID
WHERE GID = @GID
	OR GID IS NULL
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissionsNotGranted]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetPermissionsNotGranted]    Script Date: 01/05/2015 14:43:28 ******/
------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetPermissionsNotGranted] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPPermissionsMaster.PermissionID,
	dbo.SRPPermissionsMaster.PermissionName,
	dbo.SRPPermissionsMaster.PermissionDesc,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPPermissionsMaster
WHERE dbo.SRPPermissionsMaster.PermissionID NOT IN (
		SELECT dbo.SRPPermissionsMaster.PermissionID
		FROM dbo.SRPPermissionsMaster
		INNER JOIN dbo.SRPGroupPermissions ON dbo.SRPPermissionsMaster.PermissionID = dbo.SRPGroupPermissions.PermissionID
		INNER JOIN dbo.SRPGroups ON dbo.SRPGroupPermissions.GID = dbo.SRPGroups.GID
		WHERE dbo.SRPGroupPermissions.GID = @GID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsers]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsers]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetUsers] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPUser.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser
FROM dbo.SRPGroups
INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
WHERE dbo.SRPGroups.GID = @GID
	AND dbo.SRPUser.IsDeleted = 0
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsersFlagged]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsersFlagged]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetUsersFlagged] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @GID AS GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPGroups.GroupDescription,
	dbo.SRPUser.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	dbo.SRPUserGroups.AddedDate,
	dbo.SRPUserGroups.AddedUser,
	CASE 
		WHEN dbo.SRPUserGroups.AddedDate IS NULL
			THEN 0 --'False'
		ELSE 1 --'True'
		END AS isMember
FROM dbo.SRPUser
LEFT JOIN dbo.SRPUserGroups ON dbo.SRPUser.UID = dbo.SRPUserGroups.UID
	AND dbo.SRPUserGroups.GID = @GID
LEFT JOIN dbo.SRPGroups ON dbo.SRPGroups.GID = @GID
WHERE (
		dbo.SRPUserGroups.GID = @GID
		OR dbo.SRPUserGroups.GID IS NULL
		)
	AND dbo.SRPUser.IsDeleted = 0
	AND dbo.SRPUser.TenID = dbo.SRPGroups.TenID
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsersNonMembers]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_GetUsersNonMembers]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_GetUsersNonMembers] @GID INT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT dbo.SRPGroups.GID,
	dbo.SRPGroups.GroupName,
	dbo.SRPUser.UID,
	dbo.SRPUser.Username,
	dbo.SRPUser.FirstName,
	dbo.SRPUser.LastName,
	dbo.SRPUser.EmailAddress,
	NULL AS AddedDate,
	'N/A' AS AddedUser
FROM dbo.SRPGroups
INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
WHERE dbo.SRPGroups.GID NOT IN (
		SELECT dbo.SRPGroups.GID
		FROM dbo.SRPGroups
		INNER JOIN dbo.SRPUserGroups ON dbo.SRPGroups.GID = dbo.SRPUserGroups.GID
		INNER JOIN dbo.SRPUser ON dbo.SRPUserGroups.UID = dbo.SRPUser.UID
		WHERE dbo.SRPGroups.GID = @GID
			AND dbo.SRPUser.IsDeleted = 0
		)
	AND dbo.SRPUser.IsDeleted = 0
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdatePermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdatePermissions]    Script Date: 01/05/2015 14:43:28 ******/
------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_UpdatePermissions] @GID INT,
	@PermissionID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPGroupPermissions
WHERE GID = @GID
	AND PermissionID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)

INSERT INTO dbo.SRPGroupPermissions
SELECT @GID,
	PermissionID,
	getdate(),
	@ActionUsername
FROM dbo.SRPPermissionsMaster
WHERE PermissionID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@PermissionID_LIST)
		)
	AND PermissionID NOT IN (
		SELECT PermissionID
		FROM dbo.SRPGroupPermissions
		WHERE GID = @GID
		)
GO
/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdateUsers]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[cbspSRPUserGroups_UpdateUsers]    Script Date: 01/05/2015 14:43:28 ******/
CREATE PROCEDURE [dbo].[cbspSRPUserGroups_UpdateUsers] @GID INT,
	@UID_LIST VARCHAR(4000),
	@ActionUsername VARCHAR(50) = 'N/A'
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DELETE dbo.SRPUserGroups
WHERE GID = @GID
	AND UID NOT IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@UID_LIST)
		)

INSERT INTO dbo.SRPUserGroups
SELECT UID,
	@GID,
	getdate(),
	@ActionUsername
FROM dbo.SRPUser
WHERE UID IN (
		SELECT *
		FROM [dbo].[fnSplitBigInt](@UID_LIST)
		)
	AND UID NOT IN (
		SELECT UID
		FROM dbo.SRPUserGroups
		WHERE GID = @GID
		)
	AND IsDeleted = 0
GO
/****** Object:  StoredProcedure [dbo].[GetPatronsPaged]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPatronsPaged] (
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

SELECT @Filter = @Filter + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' AND '
		END + ' p.TenID = ' + convert(VARCHAR, @TenID) + ' '

SELECT @SQL1 = 'SELECT  PID, FirstName, LastName, DOB, Username, EmailAddress, Gender, Program, ProgId
FROM
(
Select p.*, pg.AdminName as Program
, ROW_NUMBER() OVER (ORDER BY ' + @sortString + ' ) AS RowRank
FROM Patron p left outer join Programs pg
on p.ProgID = pg.PID
' + CASE len(@Filter)
		WHEN 0
			THEN ''
		ELSE ' WHERE ' + @Filter
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
/****** Object:  StoredProcedure [dbo].[GetTotalPatrons]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTotalPatrons] (
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
/****** Object:  StoredProcedure [dbo].[rpt_DashboardStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_DashboardStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@Level INT = NULL,
	@TenID INT = NULL
AS
SELECT AdminName AS Program,
	count(*) AS RegistrantCount
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY AdminName

---------------------------------------------------------------------------------------------------------------
IF EXISTS (
		SELECT AdminName AS Program,
			isnull(count(*), 0) AS FinisherCount
		FROM Patron p
		RIGHT JOIN Programs pg ON p.ProgId = pg.PID
		WHERE p.TenID = @TenID
			AND p.ProgID > 0
			AND (
				ProgID = @ProgId
				OR @ProgId IS NULL
				)
			AND (
				PrimaryLibrary = @BranchID
				OR @BranchID IS NULL
				)
			AND (
				rtrim(ltrim(isnull(SchoolName, ''))) = @School
				OR @School IS NULL
				)
			AND (
				rtrim(ltrim(isnull(District, ''))) = @LibSys
				OR @LibSys IS NULL
				)
			AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
		GROUP BY AdminName
		)
	SELECT AdminName AS Program,
		isnull(count(*), 0) AS FinisherCount
	FROM Patron p
	RIGHT JOIN Programs pg ON p.ProgId = pg.PID
	WHERE p.TenID = @TenID
		AND p.ProgID > 0
		AND (
			ProgID = @ProgId
			OR @ProgId IS NULL
			)
		AND (
			PrimaryLibrary = @BranchID
			OR @BranchID IS NULL
			)
		AND (
			rtrim(ltrim(isnull(SchoolName, ''))) = @School
			OR @School IS NULL
			)
		AND (
			rtrim(ltrim(isnull(District, ''))) = @LibSys
			OR @LibSys IS NULL
			)
		AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
	GROUP BY AdminName
ELSE
	SELECT AdminName AS Program,
		0 AS FinisherCount
	FROM Programs pg
	WHERE pg.TenID = @TenID
		AND (
			PID = @ProgId
			OR @ProgId IS NULL
			)
	GROUP BY AdminName

---------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
DECLARE @current INT
DECLARE @ColCounter INT
DECLARE @SQL VARCHAR(7000)
DECLARE @SQL1 VARCHAR(7000)

-----------------------------------------------------------------------------------------
IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL
	DROP TABLE #Temp1

SELECT AdminName AS Program,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	count(*) AS RegistrantCount,
	- 1 AS IndexRank
INTO #temp1
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY p.ProgId,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY AdminName,
	Age

UPDATE #Temp1
SET IndexRank = a.IndexRank
FROM (
	SELECT TOP 100 PERCENT Program,
		RANK() OVER (
			ORDER BY Program
			) AS IndexRank
	FROM #Temp1
	GROUP BY Program
	ORDER BY Program
	) a
INNER JOIN #Temp1 ON a.Program = #Temp1.Program

--select 1, * from #Temp1
/*
select @ColCounter = isnull(max(x.R) ,0)
from (
select RANK () over ( order by Count(Program)  desc ) as R
from #temp1
group by Program
) as x
*/
SELECT @ColCounter = isnull(max(IndexRank), 0)
FROM #temp1

--select @ColCounter
IF OBJECT_ID('tempdb..#StatsData') IS NOT NULL
	DROP TABLE #StatsData

CREATE TABLE #StatsData (Age INT)

IF OBJECT_ID('tempdb..#ProgramLabels') IS NOT NULL
	DROP TABLE #ProgramLabels

CREATE TABLE #ProgramLabels (Label VARCHAR(50))

INSERT INTO #ProgramLabels (Label)
SELECT Program
FROM #temp1
GROUP BY IndexRank,
	Program
ORDER BY IndexRank

--select * from #ProgramLabels
IF @ColCounter > 0
BEGIN
	SELECT @SQL = 'alter table #StatsData add '

	--SELECT @SQL1 = 'alter table #ProgramLabels add '
	SELECT @current = 1 --, @maxcounter = 8

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = @SQL + 'PgmCount' + CONVERT(VARCHAR, @current) + ' int '

		SELECT @SQL1 = @SQL1 + 'PgmName' + CONVERT(VARCHAR, @current) + ' varchar(255)'

		IF @current < @ColCounter
		BEGIN
			SELECT @SQL = @SQL + ','

			SELECT @SQL1 = @SQL1 + ','
		END

		SELECT @current = @current + 1
	END

	PRINT @SQL

	--print @SQL1
	EXEC (@SQL)

	--EXEC (@SQL1)
	INSERT INTO #StatsData (Age)
	SELECT DISTINCT Age
	FROM #temp1
	ORDER BY Age

	SELECT @current = 1

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = 'update #StatsData set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = a.RegistrantCount ' + 'from #Temp1 a inner join  #StatsData on a.Age = #StatsData.Age ' + ' and a.IndexRank = ' + CONVERT(VARCHAR, @current)

		SELECT @SQL1 = 'update #StatsData set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = 0 ' + 'where PgmCount' + CONVERT(VARCHAR, @current) + ' is null '

		EXEC (@SQL)

		EXEC (@SQL1)

		PRINT @SQL

		SELECT @current = @current + 1
	END
END

SELECT *
FROM #ProgramLabels

SELECT *
FROM #StatsData

-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL
	DROP TABLE #Temp2

SELECT AdminName AS Program,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	count(*) AS FinisherCount,
	- 1 AS IndexRank
INTO #Temp2
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
GROUP BY p.ProgId,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY AdminName,
	Age

UPDATE #Temp2
SET IndexRank = a.IndexRank
FROM (
	SELECT TOP 100 PERCENT Program,
		RANK() OVER (
			ORDER BY Program
			) AS IndexRank
	FROM #Temp2
	GROUP BY Program
	ORDER BY Program
	) a
INNER JOIN #Temp2 ON a.Program = #Temp2.Program

--select * from #Temp2
/*
select @ColCounter = isnull(max(x.R) ,0)
from (
select RANK () over ( order by Count(Program)  desc ) as R
from #Temp2
group by Program
) as x
*/
SELECT @ColCounter = isnull(max(IndexRank), 0)
FROM #Temp2

--select @ColCounter
IF OBJECT_ID('tempdb..#StatsData2') IS NOT NULL
	DROP TABLE #StatsData2

CREATE TABLE #StatsData2 (Age INT)

IF OBJECT_ID('tempdb..#ProgramLabels2') IS NOT NULL
	DROP TABLE #ProgramLabels2

CREATE TABLE #ProgramLabels2 (Label VARCHAR(50))

INSERT INTO #ProgramLabels2 (Label)
SELECT Program
FROM #Temp2
GROUP BY IndexRank,
	Program
ORDER BY IndexRank

--select * from #ProgramLabels2
IF @ColCounter > 0
BEGIN
	SELECT @SQL = 'alter table #StatsData2 add '

	--SELECT @SQL1 = 'alter table #ProgramLabels2 add '
	SELECT @current = 1 --, @maxcounter = 8

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = @SQL + 'PgmCount' + CONVERT(VARCHAR, @current) + ' int '

		SELECT @SQL1 = @SQL1 + 'PgmName' + CONVERT(VARCHAR, @current) + ' varchar(255)'

		IF @current < @ColCounter
		BEGIN
			SELECT @SQL = @SQL + ','

			SELECT @SQL1 = @SQL1 + ','
		END

		SELECT @current = @current + 1
	END

	PRINT @SQL

	--print @SQL1
	EXEC (@SQL)

	--EXEC (@SQL1)
	INSERT INTO #StatsData2 (Age)
	SELECT DISTINCT Age
	FROM #Temp2
	ORDER BY Age

	SELECT @current = 1

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = 'update #StatsData2 set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = a.FinisherCount ' + 'from #Temp2 a inner join  #StatsData2 on a.Age = #StatsData2.Age ' + ' and a.IndexRank = ' + CONVERT(VARCHAR, @current)

		SELECT @SQL1 = 'update #StatsData2 set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = 0 ' + 'where PgmCount' + CONVERT(VARCHAR, @current) + ' is null '

		EXEC (@SQL)

		EXEC (@SQL1)

		PRINT @SQL

		SELECT @current = @current + 1
	END
END

SELECT *
FROM #ProgramLabels2

SELECT *
FROM #StatsData2

-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
SELECT pg.AdminName,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS MaleRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS FemaleRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS OtherRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS NARegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS TotalRegistrant
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY AdminName
ORDER BY AdminName

---------------------------------------------------------------------------------------------------------------
SELECT pg.AdminName,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS MaleFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS FemaleFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS OtherFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS NAFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS TotalFinisher
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND [dbo].[fx_IsFinisher2](Patron.PID, Pg.PID, @Level) = 1
GROUP BY AdminName
ORDER BY AdminName
GO
/****** Object:  StoredProcedure [dbo].[rpt_FinisherStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_FinisherStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT ProgID,
	pg.AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS Male,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS Female,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS Other,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'M'
				THEN 0
			WHEN 'O'
				THEN 0
			ELSE 1
			END) AS NA
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID --AND Patron.TenID = pg.TenID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND [dbo].[fx_IsFinisher](Patron.PID, Pg.PID) = 1
GROUP BY ProgID,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY ProgID,
	Age
GO
/****** Object:  StoredProcedure [dbo].[rpt_GameLevelStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_GameLevelStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

DECLARE @Levels TABLE (
	PGID INT,
	GameLevel INT,
	PointsNeeded INT,
	IsBonus BIT
	)

INSERT INTO @Levels
SELECT f.*
FROM ProgramGame pgm
CROSS APPLY dbo.ProgramGameCummulativePoints(pgm.PGID) f
WHERE pgm.TenID = @TenID

--select * from @Levels
SELECT ProgID,
	pg.AdminName,
	isnull((
			SELECT TOP 1 L.GameLevel
			FROM @Levels L
			WHERE L.PointsNeeded <= isNull((
						SELECT SUM(NumPoints)
						FROM PatronPoints pp
						WHERE pp.PID = p.PID
						), 0)
				AND L.PGID = pgm.PGID
			ORDER BY GameLevel DESC
			), 0) AS LevelAchieved
INTO #Temp
FROM Patron p
LEFT JOIN Programs pg ON ProgID = pg.PID
	AND p.TenID = pg.TenID
LEFT JOIN ProgramGame pgm ON pg.ProgramGameID = pgm.PGID
WHERE p.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
--AND [dbo].[fx_IsFinisher](p.PID, Pg.PID) = 1
--group by p.PID, pgm.PGID, p.ProgID, pg.AdminName
ORDER BY p.PID,
	p.ProgID

SELECT AdminName,
	LevelAchieved,
	COUNT(LevelAchieved) AS FinisherCount
FROM #Temp
GROUP BY ProgID,
	AdminName,
	LevelAchieved
ORDER BY AdminName,
	LevelAchieved
GO
/****** Object:  StoredProcedure [dbo].[rpt_MiniGameStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_MiniGameStats] @start DATETIME = NULL,
	@end DATETIME = NULL,
	@MGID INT = NULL,
	@TenID INT = NULL
AS
WITH stats
AS (
	SELECT DISTINCT gps.GPSID,
		gps.PID,
		p.Username,
		p.FirstName,
		p.LastName,
		p.Gender,
		p.EmailAddress,
		gps.MGID,
		g.GameName,
		g.AdminName,
		gps.MGType,
		g.MiniGameTypeName,
		gps.CompletedPlay,
		Difficulty,
		Started,
		Completed
	FROM GamePlayStats gps
	INNER JOIN Patron p ON gps.PID = p.PID
		AND p.TenID = @TenID
	INNER JOIN Minigame g ON gps.MGID = g.MGID
	)
SELECT DISTINCT PID AS "Patron ID",
	Username,
	FirstName AS "First Name",
	LastName AS "Last Name",
	Gender,
	EmailAddress AS Email,
	MGID AS "MiniGame ID",
	GameName AS "Game Name",
	AdminName AS "Administrative Name",
	MGType AS "MiniGame Type ID",
	MiniGameTypeName AS "MiniGame Type",
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Easy'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS EasyLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Easy'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS EasyLevelCompleted,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Medium'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS MediumLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Medium'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS MediumLevelCompleted,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Hard'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS HardLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Hard'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS HardLevelCompleted
FROM stats s
WHERE (
		@start IS NULL
		OR Started >= @start
		)
	AND (
		@end IS NULL
		OR Started >= @end
		)
	AND (
		@MGID IS NULL
		OR @MGID = MGID
		)
ORDER BY Username,
	FirstName,
	LastName,
	Gender,
	EmailAddress,
	MGID,
	GameName,
	AdminName,
	MGType,
	MiniGameTypeName
GO
/****** Object:  StoredProcedure [dbo].[rpt_PatronActivity]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_PatronActivity] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT pg.AdminName AS Program,
	p.Username,
	p.FirstName,
	p.LastName,
	ISNULL(c1.Code, '') AS LibraryDistrict,
	ISNULL(c2.Code, '') AS Library,
	ISNULL(c3.Code, '') AS SchoolDistrict,
	ISNULL(c4.Code, '') AS School,
	p.Age,
	p.SchoolGrade,
	CASE 
		WHEN Score1Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score1)
		ELSE 'N/A'
		END AS Score1,
	CASE 
		WHEN Score1Date IS NOT NULL
			THEN CONVERT(VARCHAR(10), Score1Date, 101)
		ELSE 'N/A'
		END AS Score1Date,
	CASE 
		WHEN Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score2)
		ELSE 'N/A'
		END AS Score2,
	CASE 
		WHEN Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(10), Score2Date, 101)
		ELSE 'N/A'
		END AS Score2Date,
	CASE 
		WHEN Score1Date IS NOT NULL
			AND Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score2 - Score1)
		ELSE 'N/A'
		END AS ScoreDifference,
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isReading = 1
			), 0) AS [# Points For Reading],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isEvent = 1
			), 0) AS [# Points For Events],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isGameLevelActivity = 1
			), 0) AS [# Points For Games],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isBookList = 1
			), 0) AS [# Points For Book Lists],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isReading = 1
			), 0) AS [# Times Logged Reading],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isEvent = 1
			), 0) AS [# Events Attended],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isGameLevelActivity = 1
			), 0) AS [# Times Logged Games],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isBookList = 1
			), 0) AS [# Book Lists Completed],
	isnull((
			SELECT COUNT(1)
			FROM PatronBadges pp
			WHERE pp.PID = p.PID
			), 0) AS [# Badges Earned],
	isnull((
			SELECT COUNT(1)
			FROM GamePlayStats pp
			WHERE pp.PID = p.PID
				AND CompletedPlay = 1
			), 0) AS [# Minigames Played]
FROM Patron p
LEFT JOIN Code c1 ON p.District = c1.CID
LEFT JOIN Code c2 ON p.PrimaryLibrary = c2.CID
LEFT JOIN Code c3 ON p.SDistrict = c3.CID
LEFT JOIN Code c4 ON p.SchoolName = c4.CID
LEFT JOIN Programs pg ON ProgID = pg.PID
	AND p.TenID = pg.TenID
WHERE p.TenID = @TenID
	AND Username IS NOT NULL
	AND p.ProgID > 0
	AND (
		p.ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		p.PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
ORDER BY AdminName,
	p.Username,
	p.FirstName,
	p.LastName
GO
/****** Object:  StoredProcedure [dbo].[rpt_PatronFilter]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_PatronFilter] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF
SET ANSI_WARNINGS OFF

SELECT DISTINCT Patron.PID
FROM Patron
WHERE TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[rpt_PatronFilter_Expanded]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_PatronFilter_Expanded] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL,
	@Points INT = 0,
	@PointType INT = - 1,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL
AS
SET ARITHABORT OFF
SET ANSI_WARNINGS OFF

SELECT DISTINCT Patron.PID
FROM Patron
WHERE TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND (
		@Points = 0
		OR (
			SELECT isNull(SUM(NumPoints), 0)
			FROM PatronPoints
			WHERE (
					AwardReasonCd = @PointType
					OR @PointType < 0
					)
				AND (
					AwardDate >= @StartDate
					OR @StartDate IS NULL
					)
				AND (
					AwardDate <= @EndDate
					OR @EndDate IS NULL
					)
				AND PatronPoints.PID = Patron.PID
			) >= @Points
		)
GO
/****** Object:  StoredProcedure [dbo].[rpt_PrizeRecipients]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_PrizeRecipients] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SELECT AdminName,
	FirstName,
	LastName,
	Username,
	EmailAddress,
	PrizeName,
	CASE RedeemedFlag
		WHEN 1
			THEN 'Yes'
		ELSE 'No'
		END PrizeRedeemed
FROM PatronPrizes r
LEFT JOIN Patron p ON p.PID = r.PID
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE p.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
--AND [dbo].[fx_IsFinisher](Patron.PID, Pg.PID) = 1
ORDER BY AdminName,
	FirstName,
	LastName,
	RedeemedFlag DESC
GO
/****** Object:  StoredProcedure [dbo].[rpt_ReadingActivityReport]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_ReadingActivityReport] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@ActivityType INT = 1,
	@TenID INT = NULL
AS
DECLARE @ActivityLabel VARCHAR(50)

SELECT @ActivityLabel = CASE @ActivityType
		WHEN 0
			THEN 'Books'
		WHEN 1
			THEN 'Pages'
		WHEN 2
			THEN 'Paragraphs'
		WHEN 3
			THEN 'Minutes'
		ELSE 'Unknown'
		END

SELECT isnull(pg.AdminName, 'N/A') AS Program,
	p.Username,
	isnull(p.FirstName, '') FirstName,
	isnull(p.LastName, '') LastName,
	isnull(convert(VARCHAR, Sum(dbo.fx_ConvertPoints(p.ProgID, isnull(l.ReadingPoints, 0), @ActivityType))), 'N/A') AS PatronActivityCount,
	@ActivityLabel AS Activity,
	p.PID AS PatronID
FROM Patron p
LEFT JOIN PatronReadingLog l ON p.PID = l.PID
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		p.ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		p.PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY p.PID,
	p.Username,
	p.FirstName,
	p.LastName,
	pg.AdminName,
	p.ProgID
ORDER BY pg.AdminName,
	p.FirstName,
	p.LastName
GO
/****** Object:  StoredProcedure [dbo].[rpt_RegistrationStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_RegistrationStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SELECT ProgID,
	pg.AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS Male,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS Female,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS Other,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'M'
				THEN 0
			WHEN 'O'
				THEN 0
			ELSE 1
			END) AS NA
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY ProgID,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY ProgID,
	Age
GO
/****** Object:  StoredProcedure [dbo].[rpt_TenantReport]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_TenantReport] @IncSummary BIT = 0
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT t.NAME AS Organization,
	ISNULL((
			SELECT COUNT(1)
			FROM Programs x
			WHERE x.TenID = t.TenID
			), 0) AS [# Programs],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND [dbo].[fx_IsFinisher](x.PID, x.ProgID) = 1
			), 0) AS [# Finishers],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND Gender = 'M'
			), 0) AS [Male Participation],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND Gender = 'F'
			), 0) AS [Female Participation],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
			), 0) AS [# Reading Points],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
			), 0) AS [# Total Points],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
			), 0) AS [# Reading Minutes]
FROM Tenant t

UNION

SELECT 'TOTAL: ',
	ISNULL((
			SELECT COUNT(1)
			FROM Programs x
			), 0) AS [# Programs],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE [dbo].[fx_IsFinisher](x.PID, x.ProgID) = 1
			), 0) AS [# Finishers],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE Gender = 'M'
			), 0) AS [Male Participation],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE Gender = 'F'
			), 0) AS [Female Participation],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE x.isReading = 1
			), 0) AS [# Reading Points],
	ISNULL((
			SELECT Sum(NumPoints)
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			), 0) AS [# Total Points],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE x.isReading = 1
			), 0) AS [# Reading Minutes]
WHERE @IncSummary = 1
GO
/****** Object:  StoredProcedure [dbo].[rpt_TenantSummaryReport]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_TenantSummaryReport] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = 0
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT ISNULL((
			SELECT Value
			FROM SRPSettings x
			WHERE x.TenID = t.TenID
				AND NAME = 'SysName'
			), 'Summer Reading Program') AS SystemName,
	ISNULL((
			SELECT COUNT(1)
			FROM Patron x
			WHERE x.TenID = t.TenID
				AND (
					x.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					x.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(x.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(x.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Patrons],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBadges x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Badges],
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN GamePlayStats x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Games Played]
	--, ISNULL((select COUNT(1) from Patron y join GamePlayStats x on y.PID = x.PID where y.TenID = t.TenID  and x.CompletedPlay  = 1),0) as [# Games Completed]
	,
	ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronBookLists x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND HasReadFlag = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Book Lists Completed],
	ISNULL((
			SELECT COUNT(1)
			FROM Programs y
			INNER JOIN ProgramCodes x ON y.PID = x.PID
			INNER JOIN Patron z ON x.PatronId = z.PID
			WHERE y.TenID = t.TenID
				AND isUsed = 1
				AND (
					z.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					z.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(z.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(z.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) + ISNULL((
			SELECT COUNT(1)
			FROM Patron y
			INNER JOIN PatronPrizes x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND RedeemedFlag = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Prizes Claimed],
	ISNULL((
			SELECT Sum(dbo.fx_ConvertPoints(y.ProgID, isnull(NumPoints, 0), 3))
			FROM Patron y
			INNER JOIN PatronPoints x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND x.isReading = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Reading Minutes],
	ISNULL((
			SELECT COUNT(DISTINCT Title)
			FROM Patron y
			INNER JOIN PatronReadingLog x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Titles Read],
	ISNULL((
			SELECT COUNT(DISTINCT Title)
			FROM Patron y
			INNER JOIN PatronReadingLog x ON y.PID = x.PID
			WHERE y.TenID = t.TenID
				AND HasReview = 1
				AND (
					y.ProgID = @ProgID
					OR @ProgId IS NULL
					)
				AND (
					y.PrimaryLibrary = @BranchID
					OR @BranchID IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.SchoolName, ''))) = @School
					OR @School IS NULL
					)
				AND (
					rtrim(ltrim(isnull(y.District, ''))) = @LibSys
					OR @LibSys IS NULL
					)
			), 0) AS [# Titles Reviewed]
FROM Tenant t
WHERE t.TenID = @TenID
	-- AND (x.ProgID = @ProgID or @ProgId is null) AND (x.PrimaryLibrary = @BranchID or @BranchID is null) AND (rtrim(ltrim(isnull(x.SchoolName,''))) = @School or @School is null) AND (rtrim(ltrim(isnull(x.District,''))) = @LibSys or @LibSys is null)
	-- AND (y.ProgID = @ProgID or @ProgId is null) AND (y.PrimaryLibrary = @BranchID or @BranchID is null) AND (rtrim(ltrim(isnull(y.SchoolName,''))) = @School or @School is null) AND (rtrim(ltrim(isnull(y.District,''))) = @LibSys or @LibSys is null)
GO
/****** Object:  StoredProcedure [dbo].[uspSplitIntegerList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[uspSplitIntegerList] @list_integers TEXT
AS
SET NOCOUNT ON

DECLARE @InputLen INT -- input text length
DECLARE @TextPos INT -- current position within input text
DECLARE @Chunk VARCHAR(8000) -- chunk within input text
DECLARE @ChunkPos INT -- current position within chunk
DECLARE @DelimPos INT -- position of delimiter
DECLARE @ChunkLen INT -- chunk length
DECLARE @DelimLen INT -- delimiter length
DECLARE @Delimiter VARCHAR(3) -- delimiter
DECLARE @ItemBegPos INT -- item starting position in text
DECLARE @ItemOrder INT -- item order in list

-- ALTER table to hold list items
-- actually their positions because we may want to scrub this list eliminating bad entries before substring is applied
CREATE TABLE #list_items (
	item_order INT,
	item_begpos INT,
	item_endpos INT
	)

-- process list
IF @list_integers IS NOT NULL
BEGIN
	-- initialize
	-- notice that this loop assumes a delimiter length of 1
	-- if the delimiter is longer we have to deal with stuff like delimiters straddling the chunk boundaries
	SET @InputLen = DATALENGTH(@list_integers)
	SET @TextPos = 1
	SET @Delimiter = ','
	SET @DelimLen = DATALENGTH(@Delimiter)
	SET @ItemBegPos = 1
	SET @ItemOrder = 1
	SET @ChunkLen = 1

	-- cycle through input processing chunks
	WHILE @TextPos <= @InputLen
		AND @ChunkLen <> 0
	BEGIN
		-- get current chunk
		SET @Chunk = SUBSTRING(@list_integers, @TextPos, 8000)
		-- setup initial variable values
		SET @ChunkPos = 1
		SET @ChunkLen = DATALENGTH(@Chunk)
		SET @DelimPos = CHARINDEX(@Delimiter, @Chunk, @ChunkPos)

		-- loop over the chunk, until the last delimiter
		WHILE @ChunkPos <= @ChunkLen
			AND @DelimPos <> 0
		BEGIN
			-- insert position
			INSERT INTO #list_items (
				item_order,
				item_begpos,
				item_endpos
				)
			VALUES (
				@ItemOrder,
				@ItemBegPos,
				(@TextPos + @DelimPos - 1) - 1
				)

			-- adjust positions
			SET @ItemOrder = @ItemOrder + 1
			SET @ItemBegPos = (@TextPos + @DelimPos - 1) + @DelimLen
			SET @ChunkPos = @DelimPos + @DelimLen
			-- find next delimiter
			SET @DelimPos = CHARINDEX(@Delimiter, @Chunk, @ChunkPos)
		END

		-- adjust positions
		SET @TextPos = @TextPos + @ChunkLen
	END

	-- handle last item
	IF @ItemBegPos <= @InputLen
	BEGIN
		-- insert position
		INSERT INTO #list_items (
			item_order,
			item_begpos,
			item_endpos
			)
		VALUES (
			@ItemOrder,
			@ItemBegPos,
			@InputLen
			)
	END

	-- delete the bad items
	DELETE
	FROM #list_items
	WHERE item_endpos < item_begpos

	-- return list items
	SELECT CAST(SUBSTRING(@list_integers, item_begpos, (item_endpos - item_begpos + 1)) AS INT) AS item_integer,
		item_order,
		item_begpos,
		item_endpos
	FROM #list_items
	WHERE ISNUMERIC(SUBSTRING(@list_integers, item_begpos, (item_endpos - item_begpos + 1))) = 1
	ORDER BY item_order
END

DROP TABLE #list_items

RETURN
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplitBigInt]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnSplitBigInt] (
	-- Add the parameters for the function here
	@input NVARCHAR(4000)
	)
RETURNS @retBigint TABLE ([Value] [bigint] NOT NULL)
AS
BEGIN
	DECLARE @bigint NVARCHAR(100)
	DECLARE @pos INT

	SET @input = LTRIM(RTRIM(@input)) + ',' -- TRIMMING THE BLANK SPACES
	SET @pos = CHARINDEX(',', @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE GIVEN STRING

	IF REPLACE(@input, ',', '') <> '' -- CHECK IF THE STRING EXIST FOR US TO SPLIT
	BEGIN
		WHILE @pos > 0
		BEGIN
			SET @bigint = LTRIM(RTRIM(LEFT(@input, @pos - 1))) -- GET THE 1ST INT VALUE TO BE INSERTED

			IF @bigint <> ''
			BEGIN
				INSERT INTO @retBigint (Value)
				VALUES (CAST(@bigint AS BIGINT))
			END

			SET @input = RIGHT(@input, LEN(@input) - @pos) -- RESETTING THE INPUT STRING BY REMOVING THE INSERTED ONES
			SET @pos = CHARINDEX(',', @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE RESETTED NEW STRING
		END
	END

	RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplitString]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnSplitString] (
	-- Add the parameters for the function here
	@input NVARCHAR(4000),
	@CharSplit VARCHAR(1) = ','
	)
RETURNS @retString TABLE ([Value] VARCHAR(255) NOT NULL)
AS
BEGIN
	DECLARE @string NVARCHAR(255)
	DECLARE @pos INT

	SET @input = LTRIM(RTRIM(@input)) + @CharSplit -- TRIMMING THE BLANK SPACES
	SET @pos = CHARINDEX(@CharSplit, @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE GIVEN STRING

	IF REPLACE(@input, @CharSplit, '') <> '' -- CHECK IF THE STRING EXIST FOR US TO SPLIT
	BEGIN
		WHILE @pos > 0
		BEGIN
			SET @string = LTRIM(RTRIM(LEFT(@input, @pos - 1))) -- GET THE 1ST INT VALUE TO BE INSERTED

			IF @string <> ''
			BEGIN
				INSERT INTO @retString (Value)
				VALUES (@string)
			END

			SET @input = RIGHT(@input, LEN(@input) - @pos) -- RESETTING THE INPUT STRING BY REMOVING THE INSERTED ONES
			SET @pos = CHARINDEX(@CharSplit, @input, 1) -- OBTAINING THE STARTING POSITION OF COMMA IN THE RESETTED NEW STRING
		END
	END

	RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[fx_ConvertPoints]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fx_ConvertPoints] (
	@ProgID INT,
	@Points INT,
	@OutReadingType INT
	)
RETURNS DECIMAL(16, 4)
AS
BEGIN
	DECLARE @ret DECIMAL(16, 4)
	DECLARE @OutActivityTypePoints INT
	DECLARE @OutActivityTypeCount INT

	SELECT @OutActivityTypePoints = - 1,
		@OutActivityTypeCount = - 1

	SELECT @OutActivityTypePoints = PointCount,
		@OutActivityTypeCount = ActivityCount
	FROM ProgramGamePointConversion
	WHERE PGID = @ProgID
		AND ActivityTypeId = @OutReadingType

	IF (
			@OutActivityTypePoints IS NULL
			OR @OutActivityTypeCount IS NULL
			)
	BEGIN
		SET @ret = NULL -- -1.00
	END
	ELSE
	BEGIN
		IF (@OutActivityTypePoints = 0)
		BEGIN
			SET @ret = NULL -- -1.00
		END
		ELSE
		BEGIN
			SET @ret = convert(DECIMAL(16, 4), convert(DECIMAL(16, 4), @Points) * convert(DECIMAL(16, 4), @OutActivityTypeCount)) / convert(DECIMAL(16, 4), @OutActivityTypePoints)
		END
	END

	RETURN @ret
END
GO
/****** Object:  UserDefinedFunction [dbo].[fx_IsFinisher]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fx_IsFinisher] (
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
/****** Object:  UserDefinedFunction [dbo].[fx_IsFinisher2]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fx_IsFinisher2] (
	@PID INT,
	@ProgID INT,
	@Level INT = NULL
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

		-- use the program level completion points unless they ask for a particular level
		IF (@Level IS NOT NULL)
		BEGIN
			IF (
					SELECT ProgramGameID
					FROM Programs
					WHERE PID = @ProgID
					) = 0
			BEGIN
				SELECT @GameCompletionPoints = IsNull(CompletionPoints, 0)
				FROM Programs
				WHERE PID = @ProgID
			END
			ELSE
			BEGIN
				SELECT @GameCompletionPoints = IsNull(SUM(isnull(pgl.PointNumber, 0)), 0)
				FROM ProgramGame pg
				LEFT JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
				LEFT JOIN Programs p ON p.ProgramGameID = pg.PGID
				WHERE p.PID = @ProgID
					AND (
						pgl.LevelNumber <= @Level
						OR @Level IS NULL
						)
			END
		END

		SELECT @UserPoints = isnull(SUM(isnull(NumPoints, 0)), 0)
		FROM PatronPoints
		WHERE PID = @PID

		SELECT @ret = CASE 
				WHEN @GameCompletionPoints > @UserPoints
					THEN 0
				ELSE 1
				END
	END

	RETURN @ret
END
GO
/****** Object:  UserDefinedFunction [dbo].[fx_IsLevelFinisher]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fx_IsLevelFinisher] (
	@PID INT,
	@ProgID INT,
	@Level INT = NULL
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
		SELECT @GameCompletionPoints = SUM(isnull(pgl.PointNumber, 0))
		FROM ProgramGame pg
		LEFT JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
		LEFT JOIN Programs p ON p.ProgramGameID = pg.PGID
		WHERE p.PID = @ProgID
			AND (
				pgl.LevelNumber <= @Level
				OR @Level IS NULL
				)

		SELECT @UserPoints = SUM(isnull(NumPoints, 0))
		FROM PatronPoints
		WHERE PID = @PID

		SELECT @ret = CASE 
				WHEN @GameCompletionPoints < @UserPoints
					THEN 0
				ELSE 1
				END
	END

	RETURN @ret
END
GO
/****** Object:  UserDefinedFunction [dbo].[fx_PatronHasAllBadgesInList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fx_PatronHasAllBadgesInList] (
	@PID INT,
	@BadgeList VARCHAR(500)
	)
RETURNS BIT
AS
BEGIN
	DECLARE @ret BIT

	SET @ret = 0

	IF (
			SELECT COUNT(DISTINCT BID)
			FROM Badge
			WHERE BID IN (
					SELECT *
					FROM fnSplitBigInt(@BadgeList)
					)
			) = (
			SELECT COUNT(DISTINCT BadgeID)
			FROM PatronBadges
			WHERE PID = @PID
				AND BadgeID IN (
					SELECT *
					FROM fnSplitBigInt(@BadgeList)
					)
			)
	BEGIN
		SET @ret = 1
	END

	RETURN @ret
END
GO
/****** Object:  UserDefinedFunction [dbo].[ProgramGameCummulativePoints]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ProgramGameCummulativePoints] (@PGID INT)
RETURNS @Levels TABLE (
	PGID INT,
	GameLevel INT,
	PointsNeeded INT,
	IsBonus BIT
	)
AS
BEGIN
	DECLARE @T TABLE (
		PGID INT,
		GameLevel INT,
		PointsNeeded INT,
		IsBonus BIT
		)

	INSERT INTO @T
	VALUES (
		0,
		0,
		0,
		0
		)

	INSERT INTO @T
	SELECT @PGID,
		LevelNumber,
		PointNumber,
		0
	FROM ProgramGame pg
	INNER JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
	WHERE pg.PGID = @PGID
	ORDER BY LevelNumber

	DECLARE @i INT,
		@max INT,
		@numLevels INT

	SELECT @i = 0,
		@max = 20

	SELECT @numLevels = COUNT(*)
	FROM ProgramGameLevel
	WHERE PGID = @PGID

	WHILE @i < @max
	BEGIN
		INSERT INTO @t
		SELECT @PGID,
			LevelNumber + (@i + 1) * @numLevels,
			PointNumber * BonusLevelPointMultiplier,
			1
		FROM ProgramGame pg
		INNER JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
		WHERE pg.PGID = @PGID
		ORDER BY LevelNumber

		SELECT @i = @i + 1
	END

	INSERT INTO @Levels
	SELECT @PGID,
		t1.GameLevel,
		+ isnull(sum(t2.PointsNeeded) + t1.PointsNeeded, 0),
		t1.IsBonus
	FROM @T t1
	LEFT JOIN @T t2 ON t1.GameLevel > t2.GameLevel
	GROUP BY t1.GameLevel,
		t1.PointsNeeded,
		t1.IsBonus

	DELETE
	FROM @Levels
	WHERE GameLevel = 0

	RETURN
END
GO
/****** Object:  Table [dbo].[Avatar]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Avatar](
	[AID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Gender] [varchar](1) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Avatar] PRIMARY KEY CLUSTERED 
(
	[AID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Award]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Award](
	[AID] [int] IDENTITY(1,1) NOT NULL,
	[AwardName] [varchar](80) NULL,
	[BadgeID] [int] NULL,
	[NumPoints] [int] NULL,
	[BranchID] [int] NULL,
	[ProgramID] [int] NULL,
	[District] [varchar](50) NULL,
	[SchoolName] [varchar](50) NULL,
	[BadgeList] [varchar](500) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
(
	[AID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Badge]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Badge](
	[BID] [int] IDENTITY(1000,1) NOT NULL,
	[AdminName] [varchar](50) NULL,
	[UserName] [varchar](50) NULL,
	[GenNotificationFlag] [bit] NULL,
	[NotificationSubject] [varchar](150) NULL,
	[NotificationBody] [text] NULL,
	[CustomEarnedMessage] [text] NULL,
	[IncludesPhysicalPrizeFlag] [bit] NULL,
	[PhysicalPrizeName] [varchar](50) NULL,
	[AssignProgramPrizeCode] [bit] NULL,
	[PCNotificationSubject] [varchar](150) NULL,
	[PCNotificationBody] [text] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Badge] PRIMARY KEY CLUSTERED 
(
	[BID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BadgeAgeGrp]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BadgeAgeGrp](
	[BID] [int] NOT NULL,
	[CID] [int] NOT NULL,
 CONSTRAINT [PK_BadgeAgeGrp] PRIMARY KEY CLUSTERED 
(
	[BID] ASC,
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BadgeBranch]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BadgeBranch](
	[BID] [int] NOT NULL,
	[CID] [int] NOT NULL,
 CONSTRAINT [PK_BadgeBranch] PRIMARY KEY CLUSTERED 
(
	[BID] ASC,
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BadgeCategory]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BadgeCategory](
	[BID] [int] NOT NULL,
	[CID] [int] NOT NULL,
 CONSTRAINT [PK_BadgeCategory] PRIMARY KEY CLUSTERED 
(
	[BID] ASC,
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BadgeLocation]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BadgeLocation](
	[BID] [int] NOT NULL,
	[CID] [int] NOT NULL,
 CONSTRAINT [PK_BadgeLocation] PRIMARY KEY CLUSTERED 
(
	[BID] ASC,
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BookList]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BookList](
	[BLID] [int] IDENTITY(1,1) NOT NULL,
	[AdminName] [varchar](50) NULL,
	[ListName] [varchar](50) NULL,
	[AdminDescription] [text] NULL,
	[Description] [text] NULL,
	[LiteracyLevel1] [int] NULL,
	[LiteracyLevel2] [int] NULL,
	[ProgID] [int] NULL,
	[LibraryID] [int] NULL,
	[AwardBadgeID] [int] NULL,
	[AwardPoints] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[NumBooksToComplete] [int] NULL,
 CONSTRAINT [PK_BookList] PRIMARY KEY CLUSTERED 
(
	[BLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BookListBooks]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BookListBooks](
	[BLBID] [int] IDENTITY(1,1) NOT NULL,
	[BLID] [int] NULL,
	[Author] [varchar](50) NULL,
	[Title] [varchar](150) NULL,
	[ISBN] [varchar](50) NULL,
	[URL] [varchar](150) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_BookListBooks] PRIMARY KEY CLUSTERED 
(
	[BLBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Code]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Code](
	[CID] [int] IDENTITY(1,1) NOT NULL,
	[CTID] [int] NULL,
	[Code] [varchar](25) NULL,
	[Description] [varchar](80) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED 
(
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CodeType]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CodeType](
	[CTID] [int] IDENTITY(1,1) NOT NULL,
	[isSystem] [bit] NULL,
	[CodeTypeName] [varchar](50) NULL,
	[Description] [text] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_CodeType] PRIMARY KEY CLUSTERED 
(
	[CTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomEventFields]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomEventFields](
	[CID] [int] IDENTITY(1,1) NOT NULL,
	[Use1] [bit] NULL,
	[Label1] [varchar](50) NULL,
	[DDValues1] [varchar](50) NULL,
	[Use2] [bit] NULL,
	[Use3] [bit] NULL,
	[Label2] [varchar](50) NULL,
	[Label3] [varchar](50) NULL,
	[DDValues2] [varchar](50) NULL,
	[DDValues3] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NOT NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_CustomEventFields] PRIMARY KEY CLUSTERED 
(
	[TenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomRegistrationFields]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomRegistrationFields](
	[CID] [int] IDENTITY(1,1) NOT NULL,
	[Use1] [bit] NULL,
	[Label1] [varchar](50) NULL,
	[DDValues1] [varchar](50) NULL,
	[Use2] [bit] NULL,
	[Use3] [bit] NULL,
	[Use4] [bit] NULL,
	[Use5] [bit] NULL,
	[Label2] [varchar](50) NULL,
	[Label3] [varchar](50) NULL,
	[Label4] [varchar](50) NULL,
	[Label5] [varchar](50) NULL,
	[DDValues2] [varchar](50) NULL,
	[DDValues3] [varchar](50) NULL,
	[DDValues4] [varchar](50) NULL,
	[DDValues5] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_CustomRegistrationFields] PRIMARY KEY CLUSTERED 
(
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Event]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Event](
	[EID] [int] IDENTITY(1,1) NOT NULL,
	[EventTitle] [varchar](150) NULL,
	[EventDate] [datetime] NULL,
	[EventTime] [varchar](15) NULL,
	[HTML] [text] NULL,
	[SecretCode] [varchar](50) NULL,
	[NumberPoints] [int] NULL,
	[BadgeID] [int] NULL,
	[BranchID] [int] NULL,
	[Custom1] [varchar](50) NULL,
	[Custom2] [varchar](50) NULL,
	[Custom3] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[EndDate] [datetime] NULL,
	[EndTime] [varchar](50) NULL,
	[ShortDescription] [text] NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[EID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GamePlayStats]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GamePlayStats](
	[GPSID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[MGID] [int] NULL,
	[MGType] [int] NULL,
	[Started] [datetime] NULL,
	[Difficulty] [varchar](50) NULL,
	[CompletedPlay] [bit] NULL,
	[Completed] [datetime] NULL,
 CONSTRAINT [PK_GamePlayStats] PRIMARY KEY CLUSTERED 
(
	[GPSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LibraryCrosswalk]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LibraryCrosswalk](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BranchID] [int] NULL,
	[DistrictID] [int] NULL,
	[City] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_LibraryCrosswalk] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGChooseAdv]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGChooseAdv](
	[CAID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGChooseAdv] PRIMARY KEY CLUSTERED 
(
	[CAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGChooseAdvSlides]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGChooseAdvSlides](
	[CASID] [int] IDENTITY(1,1) NOT NULL,
	[CAID] [int] NOT NULL,
	[MGID] [int] NULL,
	[Difficulty] [int] NULL,
	[StepNumber] [int] NULL,
	[SlideText] [text] NULL,
	[FirstImageGoToStep] [int] NULL,
	[SecondImageGoToStep] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGChooseAdvSlides] PRIMARY KEY CLUSTERED 
(
	[CASID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGCodeBreaker]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGCodeBreaker](
	[CBID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[EasyString] [varchar](250) NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[MediumString] [varchar](250) NULL,
	[HardString] [varchar](250) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGCodeBreaker] PRIMARY KEY CLUSTERED 
(
	[CBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGCodeBreakerKey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MGCodeBreakerKey](
	[CBKID] [int] IDENTITY(1,1) NOT NULL,
	[CBID] [int] NOT NULL,
	[MGID] [int] NULL,
 CONSTRAINT [PK_MGCodeBreakerKey] PRIMARY KEY CLUSTERED 
(
	[CBKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MGHiddenPic]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGHiddenPic](
	[HPID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[EasyDictionary] [text] NULL,
	[MediumDictionary] [text] NULL,
	[HardDictionary] [text] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGHiddenPic] PRIMARY KEY CLUSTERED 
(
	[HPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGHiddenPicBk]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGHiddenPicBk](
	[HPBID] [int] IDENTITY(1,1) NOT NULL,
	[HPID] [int] NOT NULL,
	[MGID] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGHiddenPicBk] PRIMARY KEY CLUSTERED 
(
	[HPBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGMatchingGame]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGMatchingGame](
	[MAGID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[CorrectRoundsToWinCount] [int] NULL,
	[EasyGameSize] [int] NULL,
	[MediumGameSize] [int] NULL,
	[HardGameSize] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGMatchingGame] PRIMARY KEY CLUSTERED 
(
	[MAGID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGMatchingGameTiles]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGMatchingGameTiles](
	[MAGTID] [int] IDENTITY(1,1) NOT NULL,
	[MAGID] [int] NOT NULL,
	[MGID] [int] NULL,
	[Tile1UseMedium] [bit] NULL,
	[Tile1UseHard] [bit] NULL,
	[Tile2UseMedium] [bit] NULL,
	[Tile2UseHard] [bit] NULL,
	[Tile3UseMedium] [bit] NULL,
	[Tile3UseHard] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGMatchingGameTiles] PRIMARY KEY CLUSTERED 
(
	[MAGTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGMixAndMatch]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGMixAndMatch](
	[MMID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[CorrectRoundsToWinCount] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGMixAndMatch] PRIMARY KEY CLUSTERED 
(
	[MMID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGMixAndMatchItems]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGMixAndMatchItems](
	[MMIID] [int] IDENTITY(1,1) NOT NULL,
	[MMID] [int] NOT NULL,
	[MGID] [int] NULL,
	[ItemImage] [varchar](150) NULL,
	[EasyLabel] [varchar](150) NULL,
	[MediumLabel] [varchar](150) NULL,
	[HardLabel] [varchar](150) NULL,
	[AudioEasy] [varchar](150) NULL,
	[AudioMedium] [varchar](150) NULL,
	[AudioHard] [varchar](150) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGMixAndMatchItems] PRIMARY KEY CLUSTERED 
(
	[MMIID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGOnlineBook]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGOnlineBook](
	[OBID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGOnlineBook] PRIMARY KEY CLUSTERED 
(
	[OBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGOnlineBookPages]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGOnlineBookPages](
	[OBPGID] [int] IDENTITY(1,1) NOT NULL,
	[OBID] [int] NULL,
	[MGID] [int] NULL,
	[PageNumber] [int] NULL,
	[TextEasy] [text] NULL,
	[TextMedium] [text] NULL,
	[TextHard] [text] NULL,
	[AudioEasy] [varchar](150) NULL,
	[AudioMedium] [varchar](150) NULL,
	[AudioHard] [varchar](150) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGOnlineBookPages] PRIMARY KEY CLUSTERED 
(
	[OBPGID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGWordMatch]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGWordMatch](
	[WMID] [int] IDENTITY(1,1) NOT NULL,
	[MGID] [int] NULL,
	[CorrectRoundsToWinCount] [int] NULL,
	[NumOptionsToChooseFrom] [int] NULL,
	[EnableMediumDifficulty] [bit] NULL,
	[EnableHardDifficulty] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_WGMixAndMatch] PRIMARY KEY CLUSTERED 
(
	[WMID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MGWordMatchItems]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MGWordMatchItems](
	[WMIID] [int] IDENTITY(1,1) NOT NULL,
	[WMID] [int] NOT NULL,
	[MGID] [int] NULL,
	[ItemImage] [varchar](150) NULL,
	[EasyLabel] [varchar](150) NULL,
	[MediumLabel] [varchar](150) NULL,
	[HardLabel] [varchar](150) NULL,
	[AudioEasy] [varchar](150) NULL,
	[AudioMedium] [varchar](150) NULL,
	[AudioHard] [varchar](150) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_MGWordMatchItems] PRIMARY KEY CLUSTERED 
(
	[WMIID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Minigame]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Minigame](
	[MGID] [int] IDENTITY(1,1) NOT NULL,
	[MiniGameType] [int] NULL,
	[MiniGameTypeName] [varchar](50) NULL,
	[AdminName] [varchar](50) NULL,
	[GameName] [varchar](50) NULL,
	[isActive] [bit] NULL,
	[NumberPoints] [int] NULL,
	[AwardedBadgeID] [int] NULL,
	[Acknowledgements] [text] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Minigame] PRIMARY KEY CLUSTERED 
(
	[MGID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Notifications](
	[NID] [int] IDENTITY(1,1) NOT NULL,
	[PID_To] [int] NULL,
	[PID_From] [int] NULL,
	[isQuestion] [bit] NULL,
	[Subject] [varchar](150) NULL,
	[Body] [text] NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[isUnread] [bit] NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[NID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Offer]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Offer](
	[OID] [int] IDENTITY(100000,1) NOT NULL,
	[isEnabled] [bit] NULL,
	[AdminName] [varchar](50) NULL,
	[Title] [varchar](150) NULL,
	[ExternalRedirectFlag] [bit] NULL,
	[RedirectURL] [varchar](150) NULL,
	[MaxImpressions] [int] NULL,
	[TotalImpressions] [int] NULL,
	[SerialPrefix] [varchar](50) NULL,
	[ZipCode] [varchar](5) NULL,
	[AgeStart] [int] NULL,
	[AgeEnd] [int] NULL,
	[ProgramId] [int] NULL,
	[BranchId] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED 
(
	[OID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Patron]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Patron](
	[PID] [int] IDENTITY(100000,1) NOT NULL,
	[IsMasterAccount] [bit] NULL,
	[MasterAcctPID] [int] NULL,
	[Username] [varchar](50) NULL,
	[Password] [varchar](255) NULL,
	[DOB] [datetime] NULL,
	[Age] [int] NULL,
	[SchoolGrade] [varchar](5) NULL,
	[ProgID] [int] NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Gender] [varchar](1) NULL,
	[EmailAddress] [varchar](150) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[StreetAddress1] [varchar](80) NULL,
	[StreetAddress2] [varchar](80) NULL,
	[City] [varchar](20) NULL,
	[State] [varchar](2) NULL,
	[ZipCode] [varchar](10) NULL,
	[Country] [varchar](50) NULL,
	[County] [varchar](50) NULL,
	[ParentGuardianFirstName] [varchar](50) NULL,
	[ParentGuardianLastName] [varchar](50) NULL,
	[ParentGuardianMiddleName] [varchar](50) NULL,
	[PrimaryLibrary] [int] NULL,
	[LibraryCard] [varchar](20) NULL,
	[SchoolName] [varchar](50) NULL,
	[District] [varchar](50) NULL,
	[Teacher] [varchar](20) NULL,
	[GroupTeamName] [varchar](20) NULL,
	[SchoolType] [int] NULL,
	[LiteracyLevel1] [int] NULL,
	[LiteracyLevel2] [int] NULL,
	[ParentPermFlag] [bit] NULL,
	[Over18Flag] [bit] NULL,
	[ShareFlag] [bit] NULL,
	[TermsOfUseflag] [bit] NULL,
	[Custom1] [varchar](50) NULL,
	[Custom2] [varchar](50) NULL,
	[Custom3] [varchar](50) NULL,
	[Custom4] [varchar](50) NULL,
	[Custom5] [varchar](50) NULL,
	[AvatarID] [int] NULL,
	[RegistrationDate] [datetime] NULL,
	[SDistrict] [int] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[Score1] [int] NULL,
	[Score2] [int] NULL,
	[Score1Pct] [decimal](18, 2) NULL,
	[Score2Pct] [decimal](18, 2) NULL,
	[Score1Date] [datetime] NULL,
	[Score2Date] [datetime] NULL,
 CONSTRAINT [PK_Patron] PRIMARY KEY CLUSTERED 
(
	[PID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatronBadges]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PatronBadges](
	[PBID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[BadgeID] [int] NULL,
	[DateEarned] [datetime] NULL,
 CONSTRAINT [PK_PatronBadges] PRIMARY KEY CLUSTERED 
(
	[PBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PatronBookLists]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PatronBookLists](
	[PBLBID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[BLBID] [int] NOT NULL,
	[BLID] [int] NULL,
	[HasReadFlag] [bit] NULL,
	[LastModDate] [datetime] NULL,
 CONSTRAINT [PK_PatronBookLists] PRIMARY KEY CLUSTERED 
(
	[PBLBID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PatronPoints]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatronPoints](
	[PPID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[NumPoints] [int] NULL,
	[AwardDate] [datetime] NULL,
	[AwardReason] [varchar](50) NULL,
	[AwardReasonCd] [int] NULL,
	[BadgeAwardedFlag] [bit] NULL,
	[BadgeID] [int] NULL,
	[PBID] [int] NULL,
	[isReading] [bit] NULL,
	[LogID] [int] NULL,
	[isEvent] [bit] NULL,
	[EventID] [int] NULL,
	[EventCode] [varchar](50) NULL,
	[isBookList] [bit] NULL,
	[BookListID] [int] NULL,
	[isGame] [bit] NULL,
	[isGameLevelActivity] [bit] NULL,
	[GameID] [int] NULL,
	[GameLevel] [int] NULL,
	[GameLevelID] [int] NULL,
	[GameLevelActivityID] [int] NULL,
 CONSTRAINT [PK_PatronPoints] PRIMARY KEY CLUSTERED 
(
	[PPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatronPrizes]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatronPrizes](
	[PPID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[PrizeSource] [int] NULL,
	[BadgeID] [int] NULL,
	[DrawingID] [int] NULL,
	[PrizeName] [varchar](50) NULL,
	[RedeemedFlag] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_PatronPrizes] PRIMARY KEY CLUSTERED 
(
	[PPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatronReadingLog]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatronReadingLog](
	[PRLID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[ReadingType] [int] NULL,
	[ReadingTypeLabel] [varchar](50) NULL,
	[ReadingAmount] [int] NULL,
	[ReadingPoints] [int] NULL,
	[LoggingDate] [varchar](50) NULL,
	[Author] [nvarchar](50) NULL,
	[Title] [nvarchar](150) NULL,
	[HasReview] [bit] NULL,
	[ReviewID] [int] NULL,
 CONSTRAINT [PK_PatronReadingLog] PRIMARY KEY CLUSTERED 
(
	[PRLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatronRecovery]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PatronRecovery](
	[Token] [nvarchar](50) NOT NULL,
	[PID] [int] NOT NULL,
	[Generated] [datetime] NOT NULL,
 CONSTRAINT [PK_PatronRecovery] PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PatronReview]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatronReview](
	[PRID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[PRLID] [int] NULL,
	[Author] [varchar](50) NULL,
	[Title] [varchar](150) NULL,
	[Review] [text] NULL,
	[isApproved] [bit] NULL,
	[ReviewDate] [datetime] NULL,
	[ApprovalDate] [datetime] NULL,
	[ApprovedBy] [varchar](50) NULL,
 CONSTRAINT [PK_PatronReview] PRIMARY KEY CLUSTERED 
(
	[PRID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatronRewardCodes]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatronRewardCodes](
	[PRCID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[BadgeID] [int] NULL,
	[ProgID] [int] NULL,
	[RewardCode] [varchar](100) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_PatronRewardCodes] PRIMARY KEY CLUSTERED 
(
	[PRCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrizeDrawing]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrizeDrawing](
	[PDID] [int] IDENTITY(1,1) NOT NULL,
	[PrizeName] [varchar](250) NULL,
	[TID] [int] NULL,
	[DrawingDateTime] [datetime] NULL,
	[NumWinners] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_PrizeDrawing] PRIMARY KEY CLUSTERED 
(
	[PDID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrizeDrawingWinners]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrizeDrawingWinners](
	[PDWID] [int] IDENTITY(1,1) NOT NULL,
	[PDID] [int] NULL,
	[PatronID] [int] NULL,
	[NotificationID] [int] NULL,
	[PrizePickedUpFlag] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_PrizeDrawingWinners] PRIMARY KEY CLUSTERED 
(
	[PDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrizeTemplate]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrizeTemplate](
	[TID] [int] IDENTITY(1,1) NOT NULL,
	[TName] [varchar](150) NULL,
	[NumPrizes] [int] NULL,
	[IncPrevWinnersFlag] [bit] NULL,
	[SendNotificationFlag] [bit] NULL,
	[NotificationSubject] [varchar](250) NULL,
	[NotificationMessage] [text] NULL,
	[ProgID] [int] NULL,
	[Gender] [varchar](1) NULL,
	[SchoolName] [varchar](50) NULL,
	[PrimaryLibrary] [int] NULL,
	[MinPoints] [int] NULL,
	[MaxPoints] [int] NULL,
	[LogDateStart] [datetime] NULL,
	[LogDateEnd] [datetime] NULL,
	[MinReviews] [int] NULL,
	[MaxReviews] [int] NULL,
	[ReviewDateStart] [datetime] NULL,
	[ReviewDateEnd] [datetime] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_PrizeTemplate] PRIMARY KEY CLUSTERED 
(
	[TID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProgramCodes]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProgramCodes](
	[PCID] [int] IDENTITY(1,1) NOT NULL,
	[PID] [int] NULL,
	[CodeNumber] [int] NULL,
	[CodeValue] [uniqueidentifier] NULL,
	[isUsed] [bit] NULL,
	[DateCreated] [datetime] NULL,
	[DateUsed] [datetime] NULL,
	[PatronId] [int] NULL,
	[ShortCode] [varchar](20) NULL,
 CONSTRAINT [PK_ProgramCodes] PRIMARY KEY CLUSTERED 
(
	[PCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProgramGame]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProgramGame](
	[PGID] [int] IDENTITY(1,1) NOT NULL,
	[GameName] [varchar](50) NULL,
	[MapImage] [varchar](50) NULL,
	[BonusMapImage] [varchar](50) NULL,
	[BoardWidth] [int] NULL,
	[BoardHeight] [int] NULL,
	[BonusLevelPointMultiplier] [money] NULL,
	[LevelCompleteImage] [varchar](50) NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[Minigame1ID] [int] NULL,
	[Minigame2ID] [int] NULL,
 CONSTRAINT [PK_ProgramGame] PRIMARY KEY CLUSTERED 
(
	[PGID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProgramGameLevel]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProgramGameLevel](
	[PGLID] [int] IDENTITY(1,1) NOT NULL,
	[PGID] [int] NULL,
	[LevelNumber] [int] NULL,
	[LocationX] [int] NULL,
	[LocationY] [int] NULL,
	[LocationXBonus] [int] NULL,
	[LocationYBonus] [int] NULL,
	[PointNumber] [int] NULL,
	[Minigame1ID] [int] NULL,
	[Minigame2ID] [int] NULL,
	[AwardBadgeID] [int] NULL,
	[Minigame1IDBonus] [int] NULL,
	[Minigame2IDBonus] [int] NULL,
	[AwardBadgeIDBonus] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_ProgramGameLevel] PRIMARY KEY CLUSTERED 
(
	[PGLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProgramGamePointConversion]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProgramGamePointConversion](
	[PGCID] [int] IDENTITY(1,1) NOT NULL,
	[PGID] [int] NULL,
	[ActivityTypeId] [int] NULL,
	[ActivityCount] [int] NULL,
	[PointCount] [int] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_ProgramGamePointConversion] PRIMARY KEY CLUSTERED 
(
	[PGCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Programs]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Programs](
	[PID] [int] IDENTITY(1,1) NOT NULL,
	[AdminName] [varchar](50) NULL,
	[Title] [varchar](50) NULL,
	[TabName] [varchar](20) NULL,
	[POrder] [int] NULL,
	[IsActive] [bit] NULL,
	[IsHidden] [bit] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[MaxAge] [int] NULL,
	[MaxGrade] [int] NULL,
	[LoggingStart] [datetime] NULL,
	[LoggingEnd] [datetime] NULL,
	[ParentalConsentFlag] [bit] NULL,
	[ParentalConsentText] [text] NULL,
	[PatronReviewFlag] [bit] NULL,
	[LogoutURL] [varchar](150) NULL,
	[ProgramGameID] [int] NULL,
	[HTML1] [text] NULL,
	[HTML2] [text] NULL,
	[HTML3] [text] NULL,
	[HTML4] [text] NULL,
	[HTML5] [text] NULL,
	[HTML6] [text] NULL,
	[BannerImage] [varchar](150) NULL,
	[RegistrationBadgeID] [int] NULL,
	[CompletionPoints] [int] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[PreTestID] [int] NULL,
	[PostTestID] [int] NULL,
	[PreTestMandatory] [bit] NULL,
	[PretestEndDate] [datetime] NULL,
	[PostTestStartDate] [datetime] NULL,
 CONSTRAINT [PK_Programs] PRIMARY KEY CLUSTERED 
(
	[PID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RegistrationSettings]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RegistrationSettings](
	[RID] [int] IDENTITY(1,1) NOT NULL,
	[Literacy1Label] [varchar](50) NULL,
	[Literacy2Label] [varchar](50) NULL,
	[DOB_Prompt] [bit] NULL,
	[Age_Prompt] [bit] NULL,
	[SchoolGrade_Prompt] [bit] NULL,
	[FirstName_Prompt] [bit] NULL,
	[MiddleName_Prompt] [bit] NULL,
	[LastName_Prompt] [bit] NULL,
	[Gender_Prompt] [bit] NULL,
	[EmailAddress_Prompt] [bit] NULL,
	[PhoneNumber_Prompt] [bit] NULL,
	[StreetAddress1_Prompt] [bit] NULL,
	[StreetAddress2_Prompt] [bit] NULL,
	[City_Prompt] [bit] NULL,
	[State_Prompt] [bit] NULL,
	[ZipCode_Prompt] [bit] NULL,
	[Country_Prompt] [bit] NULL,
	[County_Prompt] [bit] NULL,
	[ParentGuardianFirstName_Prompt] [bit] NULL,
	[ParentGuardianLastName_Prompt] [bit] NULL,
	[ParentGuardianMiddleName_Prompt] [bit] NULL,
	[PrimaryLibrary_Prompt] [bit] NULL,
	[LibraryCard_Prompt] [bit] NULL,
	[SchoolName_Prompt] [bit] NULL,
	[District_Prompt] [bit] NULL,
	[Teacher_Prompt] [bit] NULL,
	[GroupTeamName_Prompt] [bit] NULL,
	[SchoolType_Prompt] [bit] NULL,
	[LiteracyLevel1_Prompt] [bit] NULL,
	[LiteracyLevel2_Prompt] [bit] NULL,
	[ParentPermFlag_Prompt] [bit] NULL,
	[Over18Flag_Prompt] [bit] NULL,
	[ShareFlag_Prompt] [bit] NULL,
	[TermsOfUseflag_Prompt] [bit] NULL,
	[Custom1_Prompt] [bit] NULL,
	[Custom2_Prompt] [bit] NULL,
	[Custom3_Prompt] [bit] NULL,
	[Custom4_Prompt] [bit] NULL,
	[Custom5_Prompt] [bit] NULL,
	[DOB_Req] [bit] NULL,
	[Age_Req] [bit] NULL,
	[SchoolGrade_Req] [bit] NULL,
	[FirstName_Req] [bit] NULL,
	[MiddleName_Req] [bit] NULL,
	[LastName_Req] [bit] NULL,
	[Gender_Req] [bit] NULL,
	[EmailAddress_Req] [bit] NULL,
	[PhoneNumber_Req] [bit] NULL,
	[StreetAddress1_Req] [bit] NULL,
	[StreetAddress2_Req] [bit] NULL,
	[City_Req] [bit] NULL,
	[State_Req] [bit] NULL,
	[ZipCode_Req] [bit] NULL,
	[Country_Req] [bit] NULL,
	[County_Req] [bit] NULL,
	[ParentGuardianFirstName_Req] [bit] NULL,
	[ParentGuardianLastName_Req] [bit] NULL,
	[ParentGuardianMiddleName_Req] [bit] NULL,
	[PrimaryLibrary_Req] [bit] NULL,
	[LibraryCard_Req] [bit] NULL,
	[SchoolName_Req] [bit] NULL,
	[District_Req] [bit] NULL,
	[Teacher_Req] [bit] NULL,
	[GroupTeamName_Req] [bit] NULL,
	[SchoolType_Req] [bit] NULL,
	[LiteracyLevel1_Req] [bit] NULL,
	[LiteracyLevel2_Req] [bit] NULL,
	[ParentPermFlag_Req] [bit] NULL,
	[Over18Flag_Req] [bit] NULL,
	[ShareFlag_Req] [bit] NULL,
	[TermsOfUseflag_Req] [bit] NULL,
	[Custom1_Req] [bit] NULL,
	[Custom2_Req] [bit] NULL,
	[Custom3_Req] [bit] NULL,
	[Custom4_Req] [bit] NULL,
	[Custom5_Req] [bit] NULL,
	[DOB_Show] [bit] NULL,
	[Age_Show] [bit] NULL,
	[SchoolGrade_Show] [bit] NULL,
	[FirstName_Show] [bit] NULL,
	[MiddleName_Show] [bit] NULL,
	[LastName_Show] [bit] NULL,
	[Gender_Show] [bit] NULL,
	[EmailAddress_Show] [bit] NULL,
	[PhoneNumber_Show] [bit] NULL,
	[StreetAddress1_Show] [bit] NULL,
	[StreetAddress2_Show] [bit] NULL,
	[City_Show] [bit] NULL,
	[State_Show] [bit] NULL,
	[ZipCode_Show] [bit] NULL,
	[Country_Show] [bit] NULL,
	[County_Show] [bit] NULL,
	[ParentGuardianFirstName_Show] [bit] NULL,
	[ParentGuardianLastName_Show] [bit] NULL,
	[ParentGuardianMiddleName_Show] [bit] NULL,
	[PrimaryLibrary_Show] [bit] NULL,
	[LibraryCard_Show] [bit] NULL,
	[SchoolName_Show] [bit] NULL,
	[District_Show] [bit] NULL,
	[Teacher_Show] [bit] NULL,
	[GroupTeamName_Show] [bit] NULL,
	[SchoolType_Show] [bit] NULL,
	[LiteracyLevel1_Show] [bit] NULL,
	[LiteracyLevel2_Show] [bit] NULL,
	[ParentPermFlag_Show] [bit] NULL,
	[Over18Flag_Show] [bit] NULL,
	[ShareFlag_Show] [bit] NULL,
	[TermsOfUseflag_Show] [bit] NULL,
	[Custom1_Show] [bit] NULL,
	[Custom2_Show] [bit] NULL,
	[Custom3_Show] [bit] NULL,
	[Custom4_Show] [bit] NULL,
	[Custom5_Show] [bit] NULL,
	[DOB_Edit] [bit] NULL,
	[Age_Edit] [bit] NULL,
	[SchoolGrade_Edit] [bit] NULL,
	[FirstName_Edit] [bit] NULL,
	[MiddleName_Edit] [bit] NULL,
	[LastName_Edit] [bit] NULL,
	[Gender_Edit] [bit] NULL,
	[EmailAddress_Edit] [bit] NULL,
	[PhoneNumber_Edit] [bit] NULL,
	[StreetAddress1_Edit] [bit] NULL,
	[StreetAddress2_Edit] [bit] NULL,
	[City_Edit] [bit] NULL,
	[State_Edit] [bit] NULL,
	[ZipCode_Edit] [bit] NULL,
	[Country_Edit] [bit] NULL,
	[County_Edit] [bit] NULL,
	[ParentGuardianFirstName_Edit] [bit] NULL,
	[ParentGuardianLastName_Edit] [bit] NULL,
	[ParentGuardianMiddleName_Edit] [bit] NULL,
	[PrimaryLibrary_Edit] [bit] NULL,
	[LibraryCard_Edit] [bit] NULL,
	[SchoolName_Edit] [bit] NULL,
	[District_Edit] [bit] NULL,
	[Teacher_Edit] [bit] NULL,
	[GroupTeamName_Edit] [bit] NULL,
	[SchoolType_Edit] [bit] NULL,
	[LiteracyLevel1_Edit] [bit] NULL,
	[LiteracyLevel2_Edit] [bit] NULL,
	[ParentPermFlag_Edit] [bit] NULL,
	[Over18Flag_Edit] [bit] NULL,
	[ShareFlag_Edit] [bit] NULL,
	[TermsOfUseflag_Edit] [bit] NULL,
	[Custom1_Edit] [bit] NULL,
	[Custom2_Edit] [bit] NULL,
	[Custom3_Edit] [bit] NULL,
	[Custom4_Edit] [bit] NULL,
	[Custom5_Edit] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[SDistrict_Prompt] [bit] NULL,
	[SDistrict_Req] [bit] NULL,
	[SDistrict_Show] [bit] NULL,
	[SDistrict_Edit] [bit] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_RegistrationSettings] PRIMARY KEY CLUSTERED 
(
	[RID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportTemplate]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportTemplate](
	[RTID] [int] IDENTITY(1,1) NOT NULL,
	[ProgId] [int] NULL,
	[ReportName] [varchar](150) NULL,
	[DisplayFilters] [bit] NULL,
	[DOBFrom] [datetime] NULL,
	[DOBTo] [datetime] NULL,
	[AgeFrom] [int] NULL,
	[AgeTo] [int] NULL,
	[SchoolGrade] [varchar](5) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Gender] [varchar](1) NULL,
	[EmailAddress] [varchar](150) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[City] [varchar](20) NULL,
	[State] [varchar](2) NULL,
	[ZipCode] [varchar](10) NULL,
	[County] [varchar](50) NULL,
	[PrimaryLibrary] [int] NULL,
	[SchoolName] [varchar](50) NULL,
	[District] [varchar](50) NULL,
	[Teacher] [varchar](20) NULL,
	[GroupTeamName] [varchar](20) NULL,
	[SchoolType] [int] NULL,
	[LiteracyLevel1] [int] NULL,
	[LiteracyLevel2] [int] NULL,
	[Custom1] [varchar](50) NULL,
	[Custom2] [varchar](50) NULL,
	[Custom3] [varchar](50) NULL,
	[Custom4] [varchar](50) NULL,
	[Custom5] [varchar](50) NULL,
	[RegistrationDateStart] [datetime] NULL,
	[RegistrationDateEnd] [datetime] NULL,
	[PointsMin] [int] NULL,
	[PointsMax] [int] NULL,
	[PointsStart] [datetime] NULL,
	[PointsEnd] [datetime] NULL,
	[EventCode] [varchar](50) NULL,
	[EarnedBadge] [int] NULL,
	[PhysicalPrizeEarned] [varchar](50) NULL,
	[PhysicalPrizeRedeemed] [bit] NULL,
	[PhysicalPrizeStartDate] [datetime] NULL,
	[PhysicalPrizeEndDate] [datetime] NULL,
	[ReviewsMin] [int] NULL,
	[ReviewsMax] [int] NULL,
	[ReviewTitle] [varchar](150) NULL,
	[ReviewAuthor] [varchar](100) NULL,
	[ReviewStartDate] [datetime] NULL,
	[ReviewEndDate] [datetime] NULL,
	[RandomDrawingName] [varchar](50) NULL,
	[RandomDrawingNum] [int] NULL,
	[RandomDrawingStartDate] [datetime] NULL,
	[RandomDrawingEndDate] [datetime] NULL,
	[HasBeenDrawn] [bit] NULL,
	[HasRedeemend] [bit] NULL,
	[PIDInc] [bit] NULL,
	[UsernameInc] [bit] NULL,
	[DOBInc] [bit] NULL,
	[AgeInc] [bit] NULL,
	[SchoolGradeInc] [bit] NULL,
	[FirstNameInc] [bit] NULL,
	[LastNameInc] [bit] NULL,
	[GenderInc] [bit] NULL,
	[EmailAddressInc] [bit] NULL,
	[PhoneNumberInc] [bit] NULL,
	[CityInc] [bit] NULL,
	[StateInc] [bit] NULL,
	[ZipCodeInc] [bit] NULL,
	[CountyInc] [bit] NULL,
	[PrimaryLibraryInc] [bit] NULL,
	[SchoolNameInc] [bit] NULL,
	[DistrictInc] [bit] NULL,
	[TeacherInc] [bit] NULL,
	[GroupTeamNameInc] [bit] NULL,
	[SchoolTypeInc] [bit] NULL,
	[LiteracyLevel1Inc] [bit] NULL,
	[LiteracyLevel2Inc] [bit] NULL,
	[Custom1Inc] [bit] NULL,
	[Custom2Inc] [bit] NULL,
	[Custom3Inc] [bit] NULL,
	[Custom4Inc] [bit] NULL,
	[Custom5Inc] [bit] NULL,
	[RegistrationDateInc] [bit] NULL,
	[PointsInc] [bit] NULL,
	[EarnedBadgeInc] [bit] NULL,
	[PhysicalPrizeNameInc] [bit] NOT NULL,
	[PhysicalPrizeDateInc] [bit] NULL,
	[NumReviewsInc] [bit] NULL,
	[ReviewAuthorInc] [bit] NULL,
	[ReviewTitleInc] [bit] NULL,
	[ReviewDateInc] [bit] NULL,
	[RandomDrawingNameInc] [bit] NULL,
	[RandomDrawingNumInc] [bit] NULL,
	[RandomDrawingDateInc] [bit] NULL,
	[HasBeenDrawnInc] [bit] NULL,
	[HasRedeemendInc] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[SDistrict] [varchar](50) NULL,
	[SDistrictInc] [bit] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[Score1From] [int] NULL,
	[Score1To] [int] NULL,
	[Score1PctFrom] [int] NULL,
	[Score1PctTo] [int] NULL,
	[Score2From] [int] NULL,
	[Score2To] [int] NULL,
	[Score2PctFrom] [int] NULL,
	[Score2PctTo] [int] NULL,
	[Score1Inc] [bit] NULL,
	[Score2Inc] [bit] NULL,
	[Score1PctInc] [bit] NULL,
	[Score2PctInc] [bit] NULL,
 CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED 
(
	[RTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SchoolCrosswalk]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SchoolCrosswalk](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolID] [int] NULL,
	[SchTypeID] [int] NULL,
	[DistrictID] [int] NULL,
	[City] [varchar](50) NULL,
	[MinGrade] [int] NULL,
	[MaxGrade] [int] NULL,
	[MinAge] [int] NULL,
	[MaxAge] [int] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SchoolCrosswalk] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SentEmailLog]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SentEmailLog](
	[EID] [int] IDENTITY(1,1) NOT NULL,
	[SentDateTime] [datetime] NULL,
	[SentFrom] [varchar](150) NULL,
	[SentTo] [varchar](150) NULL,
	[Subject] [varchar](150) NULL,
	[Body] [text] NULL,
 CONSTRAINT [PK_SentEmailLog] PRIMARY KEY CLUSTERED 
(
	[EID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SQChoices]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SQChoices](
	[SQCID] [int] IDENTITY(1,1) NOT NULL,
	[QID] [int] NULL,
	[ChoiceOrder] [int] NULL,
	[ChoiceText] [varchar](50) NULL,
	[Score] [int] NULL,
	[JumpToQuestion] [int] NULL,
	[AskClarification] [bit] NULL,
	[ClarificationRequired] [bit] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SQChoices] PRIMARY KEY CLUSTERED 
(
	[SQCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SQMatrixLines]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SQMatrixLines](
	[SQMLID] [int] IDENTITY(1,1) NOT NULL,
	[QID] [int] NULL,
	[LineOrder] [int] NULL,
	[LineText] [varchar](500) NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SQMatrixLines] PRIMARY KEY CLUSTERED 
(
	[SQMLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPGroupPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPGroupPermissions](
	[GID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_GroupPermissions] PRIMARY KEY CLUSTERED 
(
	[GID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPGroups](
	[GID] [int] IDENTITY(1000,1) NOT NULL,
	[GroupName] [varchar](50) NULL,
	[GroupDescription] [varchar](255) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SRPGroups] PRIMARY KEY CLUSTERED 
(
	[GID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPPermissionsMaster]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPPermissionsMaster](
	[PermissionID] [int] NOT NULL,
	[PermissionName] [varchar](50) NULL,
	[PermissionDesc] [varchar](2000) NULL,
	[MODID] [int] NULL,
 CONSTRAINT [PK_SRPPermissionsMaster] PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPRecovery]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SRPRecovery](
	[Token] [nvarchar](50) NOT NULL,
	[UID] [int] NOT NULL,
	[Generated] [datetime] NOT NULL,
 CONSTRAINT [PK_SRPRecovery] PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SRPReport]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPReport](
	[RID] [int] IDENTITY(1,1) NOT NULL,
	[RTID] [int] NOT NULL,
	[ProgId] [int] NULL,
	[ReportName] [varchar](150) NULL,
	[DisplayFilters] [bit] NULL,
	[ReportFormat] [int] NULL,
	[DOBFrom] [datetime] NULL,
	[DOBTo] [datetime] NULL,
	[AgeFrom] [int] NULL,
	[AgeTo] [int] NULL,
	[SchoolGrade] [varchar](5) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Gender] [varchar](1) NULL,
	[EmailAddress] [varchar](150) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[City] [varchar](20) NULL,
	[State] [varchar](2) NULL,
	[ZipCode] [varchar](10) NULL,
	[County] [varchar](50) NULL,
	[PrimaryLibrary] [int] NULL,
	[SchoolName] [varchar](50) NULL,
	[District] [varchar](50) NULL,
	[Teacher] [varchar](20) NULL,
	[GroupTeamName] [varchar](20) NULL,
	[SchoolType] [int] NULL,
	[LiteracyLevel1] [int] NULL,
	[LiteracyLevel2] [int] NULL,
	[Custom1] [varchar](50) NULL,
	[Custom2] [varchar](50) NULL,
	[Custom3] [varchar](50) NULL,
	[Custom4] [varchar](50) NULL,
	[Custom5] [varchar](50) NULL,
	[RegistrationDateStart] [datetime] NULL,
	[RegistrationDateEnd] [datetime] NULL,
	[PointsMin] [int] NULL,
	[PointsMax] [int] NULL,
	[PointsStart] [datetime] NULL,
	[PointsEnd] [datetime] NULL,
	[EventCode] [varchar](50) NULL,
	[EarnedBadge] [int] NULL,
	[PhysicalPrizeEarned] [varchar](50) NULL,
	[PhysicalPrizeRedeemed] [bit] NULL,
	[PhysicalPrizeStartDate] [datetime] NULL,
	[PhysicalPrizeEndDate] [datetime] NULL,
	[ReviewsMin] [int] NULL,
	[ReviewsMax] [int] NULL,
	[ReviewTitle] [varchar](150) NULL,
	[ReviewAuthor] [varchar](100) NULL,
	[ReviewStartDate] [datetime] NULL,
	[ReviewEndDate] [datetime] NULL,
	[RandomDrawingName] [varchar](50) NULL,
	[RandomDrawingNum] [int] NULL,
	[RandomDrawingStartDate] [datetime] NULL,
	[RandomDrawingEndDate] [datetime] NULL,
	[HasBeenDrawn] [bit] NULL,
	[HasRedeemend] [bit] NULL,
	[PIDInc] [bit] NULL,
	[UsernameInc] [bit] NULL,
	[DOBInc] [bit] NULL,
	[AgeInc] [bit] NULL,
	[SchoolGradeInc] [bit] NULL,
	[FirstNameInc] [bit] NULL,
	[LastNameInc] [bit] NULL,
	[GenderInc] [bit] NULL,
	[EmailAddressInc] [bit] NULL,
	[PhoneNumberInc] [bit] NULL,
	[CityInc] [bit] NULL,
	[StateInc] [bit] NULL,
	[ZipCodeInc] [bit] NULL,
	[CountyInc] [bit] NULL,
	[PrimaryLibraryInc] [bit] NULL,
	[SchoolNameInc] [bit] NULL,
	[DistrictInc] [bit] NULL,
	[TeacherInc] [bit] NULL,
	[GroupTeamNameInc] [bit] NULL,
	[SchoolTypeInc] [bit] NULL,
	[LiteracyLevel1Inc] [bit] NULL,
	[LiteracyLevel2Inc] [bit] NULL,
	[Custom1Inc] [bit] NULL,
	[Custom2Inc] [bit] NULL,
	[Custom3Inc] [bit] NULL,
	[Custom4Inc] [bit] NULL,
	[Custom5Inc] [bit] NULL,
	[RegistrationDateInc] [bit] NULL,
	[PointsInc] [bit] NULL,
	[EarnedBadgeInc] [bit] NULL,
	[PhysicalPrizeNameInc] [bit] NOT NULL,
	[PhysicalPrizeDateInc] [bit] NULL,
	[NumReviewsInc] [bit] NULL,
	[ReviewAuthorInc] [bit] NULL,
	[ReviewTitleInc] [bit] NULL,
	[ReviewDateInc] [bit] NULL,
	[RandomDrawingNameInc] [bit] NULL,
	[RandomDrawingNumInc] [bit] NULL,
	[RandomDrawingDateInc] [bit] NULL,
	[HasBeenDrawnInc] [bit] NULL,
	[HasRedeemendInc] [bit] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[SDistrict] [varchar](50) NULL,
	[SDistrictInc] [bit] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
	[Score1From] [int] NULL,
	[Score1To] [int] NULL,
	[Score1PctFrom] [int] NULL,
	[Score1PctTo] [int] NULL,
	[Score2From] [int] NULL,
	[Score2To] [int] NULL,
	[Score2PctFrom] [int] NULL,
	[Score2PctTo] [int] NULL,
	[Score1Inc] [bit] NULL,
	[Score2Inc] [bit] NULL,
	[Score1PctInc] [bit] NULL,
	[Score2PctInc] [bit] NULL,
 CONSTRAINT [PK_SRPReport] PRIMARY KEY CLUSTERED 
(
	[RID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPSettings]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPSettings](
	[SID] [int] IDENTITY(1000,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Value] [text] NULL,
	[StorageType] [varchar](50) NULL,
	[EditType] [varchar](50) NULL,
	[ModID] [int] NULL,
	[Label] [varchar](50) NULL,
	[Description] [varchar](500) NULL,
	[ValueList] [varchar](5000) NULL,
	[DefaultValue] [text] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SRPSettings] PRIMARY KEY CLUSTERED 
(
	[SID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPUser]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPUser](
	[UID] [int] IDENTITY(1000,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[EmailAddress] [varchar](128) NOT NULL,
	[Division] [varchar](50) NULL,
	[Department] [varchar](50) NULL,
	[Title] [varchar](50) NULL,
	[IsActive] [bit] NULL,
	[MustResetPassword] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[LastPasswordReset] [datetime] NULL,
	[DeletedDate] [datetime] NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SRPUser] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPUserGroups]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPUserGroups](
	[UID] [int] NOT NULL,
	[GID] [int] NOT NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_SRPUserGroups] PRIMARY KEY CLUSTERED 
(
	[UID] ASC,
	[GID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPUserLoginHistory]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPUserLoginHistory](
	[UIDLH] [int] IDENTITY(1000,1) NOT NULL,
	[UID] [nchar](10) NULL,
	[SessionsID] [varchar](128) NULL,
	[StartDateTime] [datetime] NULL,
	[IP] [varchar](50) NULL,
	[MachineName] [varchar](50) NULL,
	[Browser] [varchar](50) NULL,
	[EndDateTime] [datetime] NULL,
 CONSTRAINT [PK_SRPUserLoginHistory] PRIMARY KEY CLUSTERED 
(
	[UIDLH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SRPUserPermissions]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SRPUserPermissions](
	[UID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
 CONSTRAINT [PK_SRPUserPermissions] PRIMARY KEY CLUSTERED 
(
	[UID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Survey]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Survey](
	[SID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[LongName] [varchar](150) NULL,
	[Description] [text] NULL,
	[Preamble] [text] NULL,
	[Status] [int] NULL,
	[TakenCount] [int] NULL,
	[PatronCount] [int] NULL,
	[CanBeScored] [bit] NULL,
	[TenID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED 
(
	[SID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SurveyAnswers]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SurveyAnswers](
	[SAID] [int] IDENTITY(1,1) NOT NULL,
	[SRID] [int] NOT NULL,
	[TenID] [int] NULL,
	[PID] [int] NULL,
	[SID] [int] NULL,
	[QID] [int] NULL,
	[SQMLID] [int] NULL,
	[DateAnswered] [datetime] NULL,
	[QType] [int] NULL,
	[FreeFormAnswer] [text] NULL,
	[ClarificationText] [text] NULL,
	[ChoiceAnswerIDs] [varchar](2000) NULL,
	[ChoiceAnswerText] [text] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SurveyAnswers] PRIMARY KEY CLUSTERED 
(
	[SAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SurveyQuestion]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SurveyQuestion](
	[QID] [int] IDENTITY(1,1) NOT NULL,
	[SID] [int] NULL,
	[QNumber] [int] NULL,
	[QType] [int] NULL,
	[QName] [varchar](150) NULL,
	[QText] [text] NULL,
	[DisplayControl] [int] NULL,
	[DisplayDirection] [int] NULL,
	[IsRequired] [bit] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SurveyQuestion] PRIMARY KEY CLUSTERED 
(
	[QID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SurveyResults]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SurveyResults](
	[SRID] [int] IDENTITY(1,1) NOT NULL,
	[TenID] [int] NULL,
	[PID] [int] NULL,
	[SID] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[IsComplete] [bit] NULL,
	[IsScorable] [bit] NULL,
	[LastAnswered] [int] NULL,
	[Score] [int] NULL,
	[ScorePct] [decimal](5, 2) NULL,
	[Source] [varchar](50) NULL,
	[SourceID] [int] NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_SurveyResults_1] PRIMARY KEY CLUSTERED 
(
	[SRID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tenant]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tenant](
	[TenID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NULL,
	[LandingName] [varchar](50) NULL,
	[AdminName] [varchar](50) NULL,
	[isActiveFlag] [bit] NULL,
	[isMasterFlag] [bit] NULL,
	[Description] [text] NULL,
	[DomainName] [varchar](50) NULL,
	[showNotifications] [bit] NULL,
	[showOffers] [bit] NULL,
	[showBadges] [bit] NULL,
	[showEvents] [bit] NULL,
	[NotificationsMenuText] [varchar](50) NULL,
	[OffersMenuText] [varchar](50) NULL,
	[BadgesMenuText] [varchar](50) NULL,
	[EventsMenuText] [varchar](50) NULL,
	[LastModDate] [datetime] NULL,
	[LastModUser] [varchar](50) NULL,
	[AddedDate] [datetime] NULL,
	[AddedUser] [varchar](50) NULL,
	[FldInt1] [int] NULL,
	[FldInt2] [int] NULL,
	[FldInt3] [int] NULL,
	[FldBit1] [bit] NULL,
	[FldBit2] [bit] NULL,
	[FldBit3] [bit] NULL,
	[FldText1] [text] NULL,
	[FldText2] [text] NULL,
	[FldText3] [text] NULL,
 CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED 
(
	[TenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TenantInitData]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TenantInitData](
	[InitID] [int] IDENTITY(1,1) NOT NULL,
	[IntitType] [varchar](50) NULL,
	[DestTID] [int] NULL,
	[SrcPK] [int] NULL,
	[DateCreated] [datetime] NULL,
	[DstPK] [int] NULL,
 CONSTRAINT [PK_TenantInitData] PRIMARY KEY CLUSTERED 
(
	[InitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[rpt_GamePlayStats1]    Script Date: 9/4/2015 13:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[rpt_GamePlayStats1]
AS
WITH stats
AS (
	SELECT DISTINCT gps.GPSID,
		gps.PID,
		p.Username,
		p.FirstName,
		p.LastName,
		p.Gender,
		p.EmailAddress,
		gps.MGID,
		g.GameName,
		g.AdminName,
		gps.MGType,
		g.MiniGameTypeName,
		gps.CompletedPlay,
		gps.Difficulty,
		gps.Started,
		gps.Completed
	FROM dbo.GamePlayStats AS gps
	LEFT JOIN dbo.Patron AS p ON gps.PID = p.PID
	LEFT JOIN dbo.Minigame AS g ON gps.MGID = g.MGID
	)
SELECT DISTINCT TOP (100) PERCENT PID,
	Username,
	FirstName,
	LastName,
	Gender,
	EmailAddress,
	MGID,
	GameName,
	AdminName,
	MGType,
	MiniGameTypeName,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Easy')
		) AS EasyLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Easy')
			AND (CompletedPlay = 1)
		) AS EasyLevelCompleted,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Medium')
		) AS MediumLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Medium')
			AND (CompletedPlay = 1)
		) AS MediumLevelCompleted,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Hard')
		) AS HardLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Hard')
			AND (CompletedPlay = 1)
		) AS HardLevelCompleted
FROM stats AS s
ORDER BY Username,
	FirstName,
	LastName,
	Gender,
	EmailAddress,
	MGID,
	GameName,
	AdminName,
	MGType,
	MiniGameTypeName
GO
ALTER TABLE [dbo].[Avatar] ADD  CONSTRAINT [DF_Avatar_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Avatar] ADD  CONSTRAINT [DF_Avatar_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Avatar] ADD  CONSTRAINT [DF_Avatar_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Avatar] ADD  CONSTRAINT [DF_Avatar_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_BadgeID]  DEFAULT ((0)) FOR [BadgeID]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_NumPoints]  DEFAULT ((0)) FOR [NumPoints]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_BranchID]  DEFAULT ((0)) FOR [BranchID]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_ProgramID]  DEFAULT ((0)) FOR [ProgramID]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_District]  DEFAULT ('') FOR [District]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_SchoolName]  DEFAULT ('') FOR [SchoolName]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_BadgeList]  DEFAULT ('') FOR [BadgeList]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Award] ADD  CONSTRAINT [DF_Award_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Badge] ADD  CONSTRAINT [DF_Badge_AssignProgramPrizeCode]  DEFAULT ((0)) FOR [AssignProgramPrizeCode]
GO
ALTER TABLE [dbo].[Badge] ADD  CONSTRAINT [DF_Badge_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Badge] ADD  CONSTRAINT [DF_Badge_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Badge] ADD  CONSTRAINT [DF_Badge_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Badge] ADD  CONSTRAINT [DF_Badge_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[BookList] ADD  CONSTRAINT [DF_BookList_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[BookList] ADD  CONSTRAINT [DF_BookList_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[BookList] ADD  CONSTRAINT [DF_BookList_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[BookList] ADD  CONSTRAINT [DF_BookList_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[BookListBooks] ADD  CONSTRAINT [DF_BookListBooks_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[BookListBooks] ADD  CONSTRAINT [DF_BookListBooks_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[BookListBooks] ADD  CONSTRAINT [DF_BookListBooks_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[BookListBooks] ADD  CONSTRAINT [DF_BookListBooks_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[CodeType] ADD  CONSTRAINT [DF_CodeType_isSystem]  DEFAULT ((0)) FOR [isSystem]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_Use1]  DEFAULT ((0)) FOR [Use1]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_Use2]  DEFAULT ((0)) FOR [Use2]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_Use3]  DEFAULT ((0)) FOR [Use3]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[CustomEventFields] ADD  CONSTRAINT [DF_CustomEventFields_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_Use1]  DEFAULT ((0)) FOR [Use1]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_Use2]  DEFAULT ((0)) FOR [Use2]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_Use3]  DEFAULT ((0)) FOR [Use3]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_Use4]  DEFAULT ((0)) FOR [Use4]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_Use5]  DEFAULT ((0)) FOR [Use5]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[CustomRegistrationFields] ADD  CONSTRAINT [DF_CustomRegistrationFields_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[GamePlayStats] ADD  CONSTRAINT [DF_GamePlayStats_Started]  DEFAULT (getdate()) FOR [Started]
GO
ALTER TABLE [dbo].[GamePlayStats] ADD  CONSTRAINT [DF_GamePlayStats_CompletedPlay]  DEFAULT ((0)) FOR [CompletedPlay]
GO
ALTER TABLE [dbo].[MGChooseAdv] ADD  CONSTRAINT [DF_MGChooseAdv_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGChooseAdv] ADD  CONSTRAINT [DF_MGChooseAdv_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGChooseAdv] ADD  CONSTRAINT [DF_MGChooseAdv_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGChooseAdv] ADD  CONSTRAINT [DF_MGChooseAdv_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_Difficulty]  DEFAULT ((1)) FOR [Difficulty]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_StepNumber]  DEFAULT ((-1)) FOR [StepNumber]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] ADD  CONSTRAINT [DF_MGChooseAdvSlides_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGCodeBreaker] ADD  CONSTRAINT [DF_MGCodeBreaker_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGCodeBreaker] ADD  CONSTRAINT [DF_MGCodeBreaker_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGCodeBreaker] ADD  CONSTRAINT [DF_MGCodeBreaker_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGCodeBreaker] ADD  CONSTRAINT [DF_MGCodeBreaker_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGHiddenPic] ADD  CONSTRAINT [DF_MGHiddenPic_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGHiddenPic] ADD  CONSTRAINT [DF_MGHiddenPic_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGHiddenPic] ADD  CONSTRAINT [DF_MGHiddenPic_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGHiddenPic] ADD  CONSTRAINT [DF_MGHiddenPic_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGHiddenPicBk] ADD  CONSTRAINT [DF_MGHiddenPicBk_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGHiddenPicBk] ADD  CONSTRAINT [DF_MGHiddenPicBk_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGHiddenPicBk] ADD  CONSTRAINT [DF_MGHiddenPicBk_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGHiddenPicBk] ADD  CONSTRAINT [DF_MGHiddenPicBk_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_EasyGameSize]  DEFAULT ((2)) FOR [EasyGameSize]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_MediumGameSize]  DEFAULT ((4)) FOR [MediumGameSize]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_HardGameSize]  DEFAULT ((6)) FOR [HardGameSize]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGMatchingGame] ADD  CONSTRAINT [DF_MGMatchingGame_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGMatchingGameTiles] ADD  CONSTRAINT [DF_MGMatchingGameTiles_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGMatchingGameTiles] ADD  CONSTRAINT [DF_MGMatchingGameTiles_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGMatchingGameTiles] ADD  CONSTRAINT [DF_MGMatchingGameTiles_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGMatchingGameTiles] ADD  CONSTRAINT [DF_MGMatchingGameTiles_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGMixAndMatch] ADD  CONSTRAINT [DF_MGMixAndMatch_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGMixAndMatch] ADD  CONSTRAINT [DF_MGMixAndMatch_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGMixAndMatch] ADD  CONSTRAINT [DF_MGMixAndMatch_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGMixAndMatch] ADD  CONSTRAINT [DF_MGMixAndMatch_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGMixAndMatchItems] ADD  CONSTRAINT [DF_MGMixAndMatchItems_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGMixAndMatchItems] ADD  CONSTRAINT [DF_MGMixAndMatchItems_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGMixAndMatchItems] ADD  CONSTRAINT [DF_MGMixAndMatchItems_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGMixAndMatchItems] ADD  CONSTRAINT [DF_MGMixAndMatchItems_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGOnlineBook] ADD  CONSTRAINT [DF_MGOnlineBook_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGOnlineBook] ADD  CONSTRAINT [DF_MGOnlineBook_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGOnlineBook] ADD  CONSTRAINT [DF_MGOnlineBook_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGOnlineBook] ADD  CONSTRAINT [DF_MGOnlineBook_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGOnlineBookPages] ADD  CONSTRAINT [DF_MGOnlineBookPages_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGOnlineBookPages] ADD  CONSTRAINT [DF_MGOnlineBookPages_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGOnlineBookPages] ADD  CONSTRAINT [DF_MGOnlineBookPages_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGOnlineBookPages] ADD  CONSTRAINT [DF_MGOnlineBookPages_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGWordMatch] ADD  CONSTRAINT [DF_MGWordMatch_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGWordMatch] ADD  CONSTRAINT [DF_MGWordMatch_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGWordMatch] ADD  CONSTRAINT [DF_MGWordMatch_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGWordMatch] ADD  CONSTRAINT [DF_MGWordMatch_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[MGWordMatchItems] ADD  CONSTRAINT [DF_MGWordMatchItems_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[MGWordMatchItems] ADD  CONSTRAINT [DF_MGWordMatchItems_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[MGWordMatchItems] ADD  CONSTRAINT [DF_MGWordMatchItems_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[MGWordMatchItems] ADD  CONSTRAINT [DF_MGWordMatchItems_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Minigame] ADD  CONSTRAINT [DF_Minigame_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Minigame] ADD  CONSTRAINT [DF_Minigame_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Minigame] ADD  CONSTRAINT [DF_Minigame_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Minigame] ADD  CONSTRAINT [DF_Minigame_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_PID_To]  DEFAULT ((0)) FOR [PID_To]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_PID_From]  DEFAULT ((0)) FOR [PID_From]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_isQuestion]  DEFAULT ((0)) FOR [isQuestion]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_Subject]  DEFAULT ('') FOR [Subject]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_isUnread]  DEFAULT ((1)) FOR [isUnread]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_RedirectURL]  DEFAULT ('') FOR [RedirectURL]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_MaxImpressions]  DEFAULT ((0)) FOR [MaxImpressions]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_TotalImpressions]  DEFAULT ((0)) FOR [TotalImpressions]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_ZipCode]  DEFAULT ('') FOR [ZipCode]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_AgeStart]  DEFAULT ((0)) FOR [AgeStart]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_AgeEnd]  DEFAULT ((0)) FOR [AgeEnd]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_ProgramId]  DEFAULT ((0)) FOR [ProgramId]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_BranchId]  DEFAULT ((0)) FOR [BranchId]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_IsMasterAccount]  DEFAULT ((0)) FOR [IsMasterAccount]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_RegistrationDate]  DEFAULT (getdate()) FOR [RegistrationDate]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_Score1]  DEFAULT ((0)) FOR [Score1]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_Score2]  DEFAULT ((0)) FOR [Score2]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_Score1Pct]  DEFAULT ((0)) FOR [Score1Pct]
GO
ALTER TABLE [dbo].[Patron] ADD  CONSTRAINT [DF_Patron_Score2Pct]  DEFAULT ((0)) FOR [Score2Pct]
GO
ALTER TABLE [dbo].[PatronBookLists] ADD  CONSTRAINT [DF_PatronBookLists_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PatronPrizes] ADD  CONSTRAINT [DF_PatronPrizes_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PatronPrizes] ADD  CONSTRAINT [DF_PatronPrizes_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[PatronPrizes] ADD  CONSTRAINT [DF_PatronPrizes_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[PatronPrizes] ADD  CONSTRAINT [DF_PatronPrizes_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[PatronRewardCodes] ADD  CONSTRAINT [DF_PatronRewardCodes_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PatronRewardCodes] ADD  CONSTRAINT [DF_PatronRewardCodes_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[PatronRewardCodes] ADD  CONSTRAINT [DF_PatronRewardCodes_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[PatronRewardCodes] ADD  CONSTRAINT [DF_PatronRewardCodes_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[PrizeDrawing] ADD  CONSTRAINT [DF_PrizeDrawing_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PrizeDrawing] ADD  CONSTRAINT [DF_PrizeDrawing_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[PrizeDrawing] ADD  CONSTRAINT [DF_PrizeDrawing_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[PrizeDrawing] ADD  CONSTRAINT [DF_PrizeDrawing_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[PrizeDrawingWinners] ADD  CONSTRAINT [DF_PrizeDrawingWinners_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PrizeDrawingWinners] ADD  CONSTRAINT [DF_PrizeDrawingWinners_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[PrizeDrawingWinners] ADD  CONSTRAINT [DF_PrizeDrawingWinners_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[PrizeDrawingWinners] ADD  CONSTRAINT [DF_PrizeDrawingWinners_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[PrizeTemplate] ADD  CONSTRAINT [DF_PrizeTemplate_NumPrizes]  DEFAULT ((1)) FOR [NumPrizes]
GO
ALTER TABLE [dbo].[PrizeTemplate] ADD  CONSTRAINT [DF_PrizeTemplate_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[PrizeTemplate] ADD  CONSTRAINT [DF_PrizeTemplate_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[PrizeTemplate] ADD  CONSTRAINT [DF_PrizeTemplate_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[PrizeTemplate] ADD  CONSTRAINT [DF_PrizeTemplate_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[ProgramCodes] ADD  CONSTRAINT [DF_ProgramCodes_isUsed]  DEFAULT ((0)) FOR [isUsed]
GO
ALTER TABLE [dbo].[ProgramCodes] ADD  CONSTRAINT [DF_ProgramCodes_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_Minigame1ID]  DEFAULT ((0)) FOR [Minigame1ID]
GO
ALTER TABLE [dbo].[ProgramGame] ADD  CONSTRAINT [DF_ProgramGame_Minigame2ID]  DEFAULT ((0)) FOR [Minigame2ID]
GO
ALTER TABLE [dbo].[ProgramGameLevel] ADD  CONSTRAINT [DF_ProgramGameLevel_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[ProgramGameLevel] ADD  CONSTRAINT [DF_ProgramGameLevel_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[ProgramGameLevel] ADD  CONSTRAINT [DF_ProgramGameLevel_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[ProgramGameLevel] ADD  CONSTRAINT [DF_ProgramGameLevel_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[ProgramGamePointConversion] ADD  CONSTRAINT [DF_ProgramGamePointConversion_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[ProgramGamePointConversion] ADD  CONSTRAINT [DF_ProgramGamePointConversion_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[ProgramGamePointConversion] ADD  CONSTRAINT [DF_ProgramGamePointConversion_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[ProgramGamePointConversion] ADD  CONSTRAINT [DF_ProgramGamePointConversion_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_ParentalConsentFlag]  DEFAULT ((0)) FOR [ParentalConsentFlag]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_CompletionPoints]  DEFAULT ((0)) FOR [CompletionPoints]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Programs] ADD  CONSTRAINT [DF_Programs_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_Literacy1Label]  DEFAULT ('AR Level') FOR [Literacy1Label]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_Literacy2Label]  DEFAULT ('Lexile Level') FOR [Literacy2Label]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[RegistrationSettings] ADD  CONSTRAINT [DF_RegistrationSettings_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_ProgId]  DEFAULT ((0)) FOR [ProgId]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_RegistrationDate1]  DEFAULT (getdate()) FOR [RegistrationDateStart]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_RegistrationDate]  DEFAULT (getdate()) FOR [RegistrationDateEnd]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[ReportTemplate] ADD  CONSTRAINT [DF_ReportTemplate_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SentEmailLog] ADD  CONSTRAINT [DF_SentEmailLog_SentDateTime]  DEFAULT (getdate()) FOR [SentDateTime]
GO
ALTER TABLE [dbo].[SRPGroupPermissions] ADD  CONSTRAINT [DF_SRPGroupPermissions_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPGroupPermissions] ADD  CONSTRAINT [DF_SRPGroupPermissions_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SRPGroups] ADD  CONSTRAINT [DF_SRPGroups_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[SRPGroups] ADD  CONSTRAINT [DF_SRPGroups_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[SRPGroups] ADD  CONSTRAINT [DF_SRPGroups_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPGroups] ADD  CONSTRAINT [DF_SRPGroups_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_ProgId]  DEFAULT ((0)) FOR [ProgId]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_RegistrationDateStart]  DEFAULT (getdate()) FOR [RegistrationDateStart]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_RegistrationDateEnd]  DEFAULT (getdate()) FOR [RegistrationDateEnd]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPReport] ADD  CONSTRAINT [DF_SRPReport_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SRPSettings] ADD  CONSTRAINT [DF_SRPSettings_StorageType]  DEFAULT ('Text') FOR [StorageType]
GO
ALTER TABLE [dbo].[SRPSettings] ADD  CONSTRAINT [DF_SRPSettings_EditType]  DEFAULT ('TextBox') FOR [EditType]
GO
ALTER TABLE [dbo].[SRPUser] ADD  CONSTRAINT [DF_SRPUser_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[SRPUser] ADD  CONSTRAINT [DF_SRPUser_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[SRPUser] ADD  CONSTRAINT [DF_SRPUser_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPUser] ADD  CONSTRAINT [DF_SRPUser_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SRPUserGroups] ADD  CONSTRAINT [DF_SRPUserGroups_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPUserGroups] ADD  CONSTRAINT [DF_SRPUserGroups_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[SRPUserPermissions] ADD  CONSTRAINT [DF_SRPUserPermissions_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[SRPUserPermissions] ADD  CONSTRAINT [DF_SRPUserPermissions_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_isActiveFlag]  DEFAULT ((0)) FOR [isActiveFlag]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_isMasterFlag]  DEFAULT ((0)) FOR [isMasterFlag]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_showNotifications]  DEFAULT ((1)) FOR [showNotifications]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_showOffers]  DEFAULT ((1)) FOR [showOffers]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_showBadges]  DEFAULT ((1)) FOR [showBadges]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_showEvents]  DEFAULT ((1)) FOR [showEvents]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_NotificationsMenuText]  DEFAULT ('Notifications') FOR [NotificationsMenuText]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_OffersMenuText]  DEFAULT ('Offers') FOR [OffersMenuText]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_BadgesMenuText]  DEFAULT ('Badges') FOR [BadgesMenuText]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_EventsMenuText]  DEFAULT ('Events') FOR [EventsMenuText]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_LastModDate]  DEFAULT (getdate()) FOR [LastModDate]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_LastModUser]  DEFAULT ('N/A') FOR [LastModUser]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_AddedDate]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[Tenant] ADD  CONSTRAINT [DF_Tenant_AddedUser]  DEFAULT ('N/A') FOR [AddedUser]
GO
ALTER TABLE [dbo].[TenantInitData] ADD  CONSTRAINT [DF_TenantInitData_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[BookListBooks]  WITH CHECK ADD  CONSTRAINT [FK_BookListBooks_BookList] FOREIGN KEY([BLID])
REFERENCES [dbo].[BookList] ([BLID])
GO
ALTER TABLE [dbo].[BookListBooks] CHECK CONSTRAINT [FK_BookListBooks_BookList]
GO
ALTER TABLE [dbo].[Code]  WITH CHECK ADD  CONSTRAINT [FK_Code_CodeType] FOREIGN KEY([CTID])
REFERENCES [dbo].[CodeType] ([CTID])
GO
ALTER TABLE [dbo].[Code] CHECK CONSTRAINT [FK_Code_CodeType]
GO
ALTER TABLE [dbo].[MGChooseAdv]  WITH CHECK ADD  CONSTRAINT [FK_MGChooseAdv_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGChooseAdv] CHECK CONSTRAINT [FK_MGChooseAdv_Minigame]
GO
ALTER TABLE [dbo].[MGChooseAdvSlides]  WITH CHECK ADD  CONSTRAINT [FK_MGChooseAdvSlides_MGChooseAdv] FOREIGN KEY([CAID])
REFERENCES [dbo].[MGChooseAdv] ([CAID])
GO
ALTER TABLE [dbo].[MGChooseAdvSlides] CHECK CONSTRAINT [FK_MGChooseAdvSlides_MGChooseAdv]
GO
ALTER TABLE [dbo].[MGCodeBreaker]  WITH CHECK ADD  CONSTRAINT [FK_MGCodeBreaker_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGCodeBreaker] CHECK CONSTRAINT [FK_MGCodeBreaker_Minigame]
GO
ALTER TABLE [dbo].[MGCodeBreakerKey]  WITH CHECK ADD  CONSTRAINT [FK_MGCodeBreakerKey_MGCodeBreaker] FOREIGN KEY([CBID])
REFERENCES [dbo].[MGCodeBreaker] ([CBID])
GO
ALTER TABLE [dbo].[MGCodeBreakerKey] CHECK CONSTRAINT [FK_MGCodeBreakerKey_MGCodeBreaker]
GO
ALTER TABLE [dbo].[MGHiddenPic]  WITH CHECK ADD  CONSTRAINT [FK_MGHiddenPic_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGHiddenPic] CHECK CONSTRAINT [FK_MGHiddenPic_Minigame]
GO
ALTER TABLE [dbo].[MGHiddenPicBk]  WITH CHECK ADD  CONSTRAINT [FK_MGHiddenPicBk_MGHiddenPic1] FOREIGN KEY([HPID])
REFERENCES [dbo].[MGHiddenPic] ([HPID])
GO
ALTER TABLE [dbo].[MGHiddenPicBk] CHECK CONSTRAINT [FK_MGHiddenPicBk_MGHiddenPic1]
GO
ALTER TABLE [dbo].[MGMatchingGame]  WITH CHECK ADD  CONSTRAINT [FK_MGMatchingGame_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGMatchingGame] CHECK CONSTRAINT [FK_MGMatchingGame_Minigame]
GO
ALTER TABLE [dbo].[MGMatchingGameTiles]  WITH CHECK ADD  CONSTRAINT [FK_MGMatchingGameTiles_MGMatchingGame] FOREIGN KEY([MAGID])
REFERENCES [dbo].[MGMatchingGame] ([MAGID])
GO
ALTER TABLE [dbo].[MGMatchingGameTiles] CHECK CONSTRAINT [FK_MGMatchingGameTiles_MGMatchingGame]
GO
ALTER TABLE [dbo].[MGMixAndMatch]  WITH CHECK ADD  CONSTRAINT [FK_MGMixAndMatch_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGMixAndMatch] CHECK CONSTRAINT [FK_MGMixAndMatch_Minigame]
GO
ALTER TABLE [dbo].[MGMixAndMatchItems]  WITH CHECK ADD  CONSTRAINT [FK_MGMixAndMatchItems_MGMixAndMatch] FOREIGN KEY([MMID])
REFERENCES [dbo].[MGMixAndMatch] ([MMID])
GO
ALTER TABLE [dbo].[MGMixAndMatchItems] CHECK CONSTRAINT [FK_MGMixAndMatchItems_MGMixAndMatch]
GO
ALTER TABLE [dbo].[MGOnlineBook]  WITH CHECK ADD  CONSTRAINT [FK_MGOnlineBook_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGOnlineBook] CHECK CONSTRAINT [FK_MGOnlineBook_Minigame]
GO
ALTER TABLE [dbo].[MGOnlineBookPages]  WITH CHECK ADD  CONSTRAINT [FK_MGOnlineBookPages_MGOnlineBook] FOREIGN KEY([OBID])
REFERENCES [dbo].[MGOnlineBook] ([OBID])
GO
ALTER TABLE [dbo].[MGOnlineBookPages] CHECK CONSTRAINT [FK_MGOnlineBookPages_MGOnlineBook]
GO
ALTER TABLE [dbo].[MGWordMatch]  WITH CHECK ADD  CONSTRAINT [FK_MGWordMatch_Minigame] FOREIGN KEY([MGID])
REFERENCES [dbo].[Minigame] ([MGID])
GO
ALTER TABLE [dbo].[MGWordMatch] CHECK CONSTRAINT [FK_MGWordMatch_Minigame]
GO
ALTER TABLE [dbo].[MGWordMatchItems]  WITH CHECK ADD  CONSTRAINT [FK_MGWordMatchItems_MGWordMatch] FOREIGN KEY([WMID])
REFERENCES [dbo].[MGWordMatch] ([WMID])
GO
ALTER TABLE [dbo].[MGWordMatchItems] CHECK CONSTRAINT [FK_MGWordMatchItems_MGWordMatch]
GO
ALTER TABLE [dbo].[ProgramCodes]  WITH CHECK ADD  CONSTRAINT [FK_ProgramCodes_Programs] FOREIGN KEY([PID])
REFERENCES [dbo].[Programs] ([PID])
GO
ALTER TABLE [dbo].[ProgramCodes] CHECK CONSTRAINT [FK_ProgramCodes_Programs]
GO
ALTER TABLE [dbo].[ProgramGameLevel]  WITH CHECK ADD  CONSTRAINT [FK_ProgramGameLevel_ProgramGame] FOREIGN KEY([PGID])
REFERENCES [dbo].[ProgramGame] ([PGID])
GO
ALTER TABLE [dbo].[ProgramGameLevel] CHECK CONSTRAINT [FK_ProgramGameLevel_ProgramGame]
GO
ALTER TABLE [dbo].[ProgramGamePointConversion]  WITH CHECK ADD  CONSTRAINT [FK_ProgramGamePointConversion_Programs] FOREIGN KEY([PGID])
REFERENCES [dbo].[Programs] ([PID])
GO
ALTER TABLE [dbo].[ProgramGamePointConversion] CHECK CONSTRAINT [FK_ProgramGamePointConversion_Programs]
GO
ALTER TABLE [dbo].[SRPGroupPermissions]  WITH CHECK ADD  CONSTRAINT [FK_GroupPermissions_SRPGroups] FOREIGN KEY([GID])
REFERENCES [dbo].[SRPGroups] ([GID])
GO
ALTER TABLE [dbo].[SRPGroupPermissions] CHECK CONSTRAINT [FK_GroupPermissions_SRPGroups]
GO
ALTER TABLE [dbo].[SRPUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_SRPUserGroups_SRPGroups] FOREIGN KEY([GID])
REFERENCES [dbo].[SRPGroups] ([GID])
GO
ALTER TABLE [dbo].[SRPUserGroups] CHECK CONSTRAINT [FK_SRPUserGroups_SRPGroups]
GO
ALTER TABLE [dbo].[SRPUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_SRPUserGroups_SRPUser] FOREIGN KEY([UID])
REFERENCES [dbo].[SRPUser] ([UID])
GO
ALTER TABLE [dbo].[SRPUserGroups] CHECK CONSTRAINT [FK_SRPUserGroups_SRPUser]
GO
ALTER TABLE [dbo].[SRPUserPermissions]  WITH CHECK ADD  CONSTRAINT [FK_UserPermissions_SRPUser] FOREIGN KEY([UID])
REFERENCES [dbo].[SRPUser] ([UID])
GO
ALTER TABLE [dbo].[SRPUserPermissions] CHECK CONSTRAINT [FK_UserPermissions_SRPUser]
GO