INSERT INTO ResumeEducation (
  Guid,
  LegacyID,
  Institute,
  InstituteURL,
  Program,
  Started,
  Completed,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Institute,
  @InstituteURL,
  @Program,
  @Started,
  @Completed,
  @CreatedUTC,
  @ModifiedUTC
);