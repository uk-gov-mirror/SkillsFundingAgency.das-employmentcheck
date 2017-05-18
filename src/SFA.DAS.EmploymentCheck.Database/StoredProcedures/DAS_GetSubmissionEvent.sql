CREATE PROCEDURE [dbo].[DAS_GetSubmissionEvent]
	@NINumber nvarchar(9)
AS
	SELECT	NiNumber
	FROM	DAS_SubmissionEvents e
	WHERE	e.NINumber = @NINumber 
 
