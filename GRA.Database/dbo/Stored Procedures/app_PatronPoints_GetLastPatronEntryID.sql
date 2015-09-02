
CREATE PROCEDURE [dbo].[app_PatronPoints_GetLastPatronEntryID] @PID INT
AS
SELECT isnull(MAX(PPID), 0)
FROM [PatronPoints]
WHERE PID = @PID
