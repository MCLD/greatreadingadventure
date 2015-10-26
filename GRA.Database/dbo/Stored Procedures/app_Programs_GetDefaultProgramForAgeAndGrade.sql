
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
		--select @ID
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
		--select @ID
END
ELSE
BEGIN
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC

	--SELECT @ID
END

IF (@ID IS NULL)
	SELECT TOP 1 @ID = PID
	FROM [Programs]
	WHERE IsActive = 1
		AND IsHidden = 0
	ORDER BY POrder ASC

SELECT @ID