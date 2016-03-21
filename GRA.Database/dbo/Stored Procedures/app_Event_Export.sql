
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