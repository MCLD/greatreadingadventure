
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_GetAll]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_GetAll] @PID INT
AS
SELECT pp.*,
	ISNULL(b.AdminName, '') AS Badge
FROM [PatronPrizes] pp
LEFT JOIN Badge b ON pp.BadgeID = b.BID
WHERE pp.PID = @PID
