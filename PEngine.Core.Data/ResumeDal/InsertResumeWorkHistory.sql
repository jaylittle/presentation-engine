INSERT INTO ResumeWorkHistory (
  Guid,
  LegacyID,
  Employer,
  EmployerURL,
  JobTitle,
  JobDescription,
  Started,
  Completed,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Employer,
  @EmployerURL,
  @JobTitle,
  @JobDescription,
  @Started,
  @Completed,
  @CreatedUTC,
  @ModifiedUTC
);