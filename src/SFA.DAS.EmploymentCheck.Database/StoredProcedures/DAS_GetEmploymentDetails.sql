CREATE PROCEDURE [dbo].[DAS_GetEmploymentDetails]
	@NINumber nvarchar(9)
AS
	SELECT	*
	FROM	DAS_EmploymentCheck c
	WHERE	c.NINumber = @NINumber 
 
