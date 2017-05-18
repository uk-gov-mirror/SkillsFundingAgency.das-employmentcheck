CREATE PROCEDURE [dbo].[DAS_StoreSubmissionEvents]
	@id int,
	@ApprenticeshipId Int,
	@NiNumber nvarchar(9),
	@EmployerReferenceNumber Nvarchar(50),
	@IlrFileName nvarchar(50),
	@SubmittedDateTime Date,
	@Uln nvarchar(9),
	@AcademicYear int,
	@ActualEndDate Date,
	@ActualStartDate Date
AS

	insert into [dbo] .[DAS_SubmissionEvents] (	
	id,
	ApprenticeshipId,
	NiNumber,
	EmployerReferenceNumber,
	IlrFileName,
	SubmittedDateTime,
	Uln,
	AcademicYear,
	ActualEndDate,
	ActualStartDate)  values (
	@id,
	@ApprenticeshipId,
	@NiNumber ,
	@EmployerReferenceNumber ,
	@IlrFileName ,
	@SubmittedDateTime ,
	@Uln ,
	@AcademicYear ,
	@ActualEndDate ,
	@ActualStartDate 
	) 
	
		
RETURN 
