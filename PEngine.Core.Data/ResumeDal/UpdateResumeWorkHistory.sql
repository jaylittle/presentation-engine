UPDATE
  ResumeWorkHistory
SET
  LegacyID = @LegacyID,
  Employer = @Employer,
  EmployerURL = @EmployerURL,
  JobTitle = @JobTitle,
  JobDescription = @JobDescription,
  Started = @Started,
  Completed = @Completed,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;