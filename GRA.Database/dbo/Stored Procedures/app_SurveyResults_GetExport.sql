
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
