
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_Export] @TenID INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT bc.[Code] AS [Branch],
		dc.[Code] AS [LibraryDistrict],
		lcw.[BranchLink] AS [Link],
		lcw.[BranchAddress] AS [Address],
		lcw.[BranchTelephone] AS [Telephone]
	FROM librarycrosswalk lcw
	INNER JOIN code bc ON lcw.[BranchId] = bc.[CID]
	INNER JOIN code dc ON lcw.[DistrictId] = dc.[CID]
	WHERE lcw.[TenID] = @TenID
	ORDER BY dc.[Code],
		bc.[Code]
END