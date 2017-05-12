CREATE TABLE [dbo].[DAS_EmployeeDetails]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [NINumber] NVARCHAR(9) NOT NULL, 
    [ApprenticeshipID] BIGINT NOT NULL, 
    [ActualStartDate] DATE NOT NULL, 
	[ILRCollectionPeriod] nchar(4) NULL,
    [CreatedOn] TIMESTAMP NOT NULL
)

GO

CREATE INDEX [IX_DAS_EmploymentCheck_NINO] ON [dbo].[DAS_EmployeeDetails] ([NINumber])
