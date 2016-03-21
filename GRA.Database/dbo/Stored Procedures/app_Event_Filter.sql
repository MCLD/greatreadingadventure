
CREATE PROCEDURE [dbo].[app_Event_Filter] @TenID INT = NULL,
	@SearchText NVARCHAR(max) = NULL,
	@BranchId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ltrim(rtrim(@SearchText)) = ''
	BEGIN
		SET @SearchText = NULL
	END

	IF @BranchId = 0
	BEGIN
		SET @BranchId = NULL
	END

	SELECT e.*,
		c.[Code] AS [Branch]
	FROM [Event] e
	LEFT JOIN [Code] c ON e.[BranchID] = c.[CID]
	WHERE (
			e.[TenID] = @TenID
			OR @TenID IS NULL
			)
		AND (
			(
				e.[EventTitle] LIKE @SearchText
				OR e.[HTML] LIKE @SearchText
				OR e.[AddedUser] LIKE @SearchText
				)
			OR @SearchText IS NULL
			)
		AND (
			e.[BranchId] = @BranchID
			OR @BranchID IS NULL
			)
	ORDER BY [EventTitle]
END