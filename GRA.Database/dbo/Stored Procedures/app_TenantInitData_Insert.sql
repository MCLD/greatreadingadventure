
CREATE PROCEDURE [dbo].[app_TenantInitData_Insert] (
	@IntitType VARCHAR(50),
	@DestTID INT,
	@SrcPK INT,
	@DateCreated DATETIME,
	@DstPK INT,
	@InitID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO TenantInitData (
		IntitType,
		DestTID,
		SrcPK,
		DateCreated,
		DstPK
		)
	VALUES (
		@IntitType,
		@DestTID,
		@SrcPK,
		@DateCreated,
		@DstPK
		)

	SELECT @InitID = SCOPE_IDENTITY()
END
