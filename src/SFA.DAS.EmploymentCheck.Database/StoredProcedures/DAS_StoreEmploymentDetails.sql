CREATE PROCEDURE [dbo].[DAS_StoreEmploymentCheck]
	@paye nvarchar(11),
	@nino nvarchar(9),
	@fromDate date,
	@toDate date
AS
	declare @createdOn date = GetDate()

	insert into [dbo] .[DAS_EmploymentCheck] (NINumber, PAYEScheme, FromDate, toDate, CreatedOn)  values (@nino, @paye, @fromDate, @toDate, @createdOn) 
	
		
RETURN SELECT SCOPE_IDENTITY()
