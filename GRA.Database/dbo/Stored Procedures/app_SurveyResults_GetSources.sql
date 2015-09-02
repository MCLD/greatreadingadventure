
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
