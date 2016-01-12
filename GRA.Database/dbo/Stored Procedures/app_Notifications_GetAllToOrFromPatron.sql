CREATE PROCEDURE [dbo].[app_Notifications_GetAllToOrFromPatron] @PID INT
AS
SELECT n.*,
	isnull(p1.Username, 'System') AS ToUsername,
	isnull(p1.FirstName, '') AS ToFirstName,
	isnull(p1.LastName, '') AS ToLastName,
	isnull(p2.Username, 'System') AS FromUsername,
	isnull(p2.FirstName, '') AS FromFirstName,
	isnull(p2.LastName, '') AS FromLastName
FROM [Notifications] n
LEFT JOIN Patron p1 ON n.PID_To = p1.pid
LEFT JOIN Patron p2 ON n.PID_From = p2.pid
WHERE (PID_To = @PID OR PID_From = @PID)
ORDER BY AddedDate DESC