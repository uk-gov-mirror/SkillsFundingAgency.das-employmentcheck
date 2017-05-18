CREATE PROCEDURE [dbo].[DAS_GetSubmissionEventById]
	@Id bigint
AS
	SELECT	*
	FROM	DAS_SubmissionEvents e
	WHERE	e.Id = @Id  

 
