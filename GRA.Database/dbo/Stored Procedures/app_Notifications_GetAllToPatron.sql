
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Notifications_GetAllToPatron] @PID INT,
	@TenID INT = NULL
AS
SELECT n.*,
	isnull(p1.Username, 'System') AS ToUsername,
	isnull(p1.FirstName, 'System') AS ToFistName,
	isnull(p1.LastName, 'System') AS ToLastName,
	isnull(p2.Username, 'System') AS FromUsername,
	isnull(p2.FirstName, 'System') AS FromFistName,
	isnull(p2.LastName, 'System') AS FromLastName
FROM [Notifications] n
LEFT JOIN Patron p1 ON n.PID_To = p1.pid
LEFT JOIN Patron p2 ON n.PID_From = p2.pid
WHERE PID_To = @PID
	AND n.TenID = @TenID
ORDER BY AddedDate DESC
