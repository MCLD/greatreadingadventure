
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
