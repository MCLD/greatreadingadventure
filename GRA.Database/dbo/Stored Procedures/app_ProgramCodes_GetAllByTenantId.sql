
CREATE PROCEDURE [dbo].[app_ProgramCodes_GetAllByTenantId] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(pc.[PCID]) [CodeCount]
	FROM [ProgramCodes] pc
	INNER JOIN [Programs] p ON pc.[PID] = p.[PID]
		AND p.[TenID] = @TenID
END