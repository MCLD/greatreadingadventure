
CREATE PROCEDURE [dbo].[app_TenantInitData_GetPKbyOriginalPK] (
	@IntitType VARCHAR(50),
	@DestTID INT,
	@SrcPK INT
	)
AS
BEGIN
	SELECT DstPK
	FROM TenantInitData
	WHERE IntitType = @IntitType
		AND DestTID = @DestTID
		AND SrcPK = @SrcPK
END
