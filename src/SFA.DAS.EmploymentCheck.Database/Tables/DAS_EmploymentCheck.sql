CREATE TABLE [dbo].[DAS_EmploymentCheck]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY, 
    [NINumber] NVARCHAR(9) NOT NULL, 
    [PAYEScheme] NVARCHAR(11) NOT NULL, 
    [FromDate] DATE NOT NULL, 
    [ToDate] DATE NULL,
	[CreatedOn] Date NOT NULL DEFAULT GetDate()

)
GO

CREATE UNIQUE INDEX [IX_DAS_EmploymentCheck_Column] ON [dbo].[DAS_EmploymentCheck] ([NiNumber])
