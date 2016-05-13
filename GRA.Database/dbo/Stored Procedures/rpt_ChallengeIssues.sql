CREATE PROCEDURE [dbo].[rpt_ChallengeIssues] @TenID INT = 0
AS
SET NOCOUNT ON;

SELECT bl.[BLID],
	bl.[AdminName],
	bl.[AddedUser],
	bl.[LastModUser],
	bl.[NumBooksToComplete] [TasksToComplete],
	COUNT(DISTINCT blb.[BLBID]) AS [AvailableTasks],
	'invalid badge (none or deleted)' [Issue]
FROM [BookList] bl
LEFT OUTER JOIN [BookListBooks] blb ON bl.[BLID] = blb.[BLID]
WHERE (
		bl.[AwardBadgeID] NOT IN (
			SELECT [BID]
			FROM [Badge]
			)
		OR bl.[AwardBadgeID] = 0
		OR bl.[AwardBadgeID] IS NULL
		)
	AND bl.[TenID] = @TenID
GROUP BY bl.[BLID],
	bl.[AdminName],
	bl.[AddedUser],
	bl.[LastModUser],
	bl.[NumBooksToComplete]

UNION

SELECT bl.[BLID],
	bl.[AdminName],
	bl.[AddedUser],
	bl.[LastModUser],
	bl.[NumBooksToComplete] [TasksToComplete],
	COUNT(DISTINCT blb.[BLBID]) AS [AvailableTasks],
	'not enough tasks' [Issue]
FROM [BookList] bl
LEFT OUTER JOIN [BookListBooks] blb ON bl.[BLID] = blb.[BLID]
WHERE bl.[TenID] = @TenID
GROUP BY bl.[BLID],
	bl.[AdminName],
	bl.[AddedUser],
	bl.[LastModUser],
	bl.[NumBooksToComplete]
HAVING COUNT(DISTINCT blb.[BLBID]) < bl.[NumBooksToComplete]
GO