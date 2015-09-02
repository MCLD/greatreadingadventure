
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
