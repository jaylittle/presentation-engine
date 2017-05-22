UPDATE
  ResumeEducation
SET
  LegacyID = @LegacyID,
  Institute = @Institute,
  InstituteURL = @InstituteURL,
  Program = @Program,
  Started = @Started,
  Completed = @Completed,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;