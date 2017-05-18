CREATE TABLE [dbo].[DAS_SubmissionEvents]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[ApprenticeshipId] Int NOT NULL,
	[NiNumber] nvarchar(9) NULL,
	[EmployerReferenceNumber] Int,
	[IlrFileName] nvarchar(50) NOT NULL,
	[SubmittedDateTime] Date NOT NULL,
	[Uln] nvarchar(9) NULL,
	[AcademicYear] int not null,
	[ActualEndDate] Date NULL,
	[ActualStartDate] Date, 
    [CreatedOn] DATE NOT NULL DEFAULT GetDate()
)
