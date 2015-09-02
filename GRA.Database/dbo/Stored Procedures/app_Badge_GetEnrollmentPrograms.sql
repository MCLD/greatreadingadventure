
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetEnrollmentPrograms] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.TabName
			ELSE @List + ', ' + p.TabName
			END, '')
FROM Programs p
WHERE p.TenID = @TenID
	AND p.RegistrationBadgeID = @BID
ORDER BY p.POrder

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN r.TabName
			ELSE @List + ', ' + r.TabName
			END, '')
FROM Award p
INNER JOIN Programs r ON ProgramID = PID
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
ORDER BY r.POrder
