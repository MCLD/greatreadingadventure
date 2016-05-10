CREATE PROCEDURE [dbo].[rpt_EventIssues] @TenID INT = 0
AS
SET NOCOUNT ON;

SELECT [EventTitle],
	[EventDate],
	[AddedUser],
	[LastModUser],
	'visible with no associated branch' [Issue]
FROM [Event]
WHERE (
		[HiddenFromPublic] = 0
		AND (
			[BranchId] IS NULL
			OR [BranchId] = 0
			)
		)
	AND [TenID] = @TenID

UNION

SELECT [EventTitle],
	[EventDate],
	[AddedUser],
	[LastModUser],
	'invalid badge (none or deleted)' [Issue]
FROM [Event]
WHERE (
		[BadgeId] NOT IN (
			SELECT [BID]
			FROM [Badge]
			)
		)
	AND [TenID] = @TenID

UNION

SELECT [EventTitle],
	[EventDate],
	[AddedUser],
	[LastModUser],
	'no secret code' [Issue]
FROM [Event]
WHERE (
		[SecretCode] IS NULL
		OR [SecretCode] = ''
		)
	AND [TenID] = @TenID;
GO