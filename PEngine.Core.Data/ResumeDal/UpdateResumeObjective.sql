UPDATE
  ResumeObjective
SET
  LegacyID = @LegacyID,
  Data = @Data,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;