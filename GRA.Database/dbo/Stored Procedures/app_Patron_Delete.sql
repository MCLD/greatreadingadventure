
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
