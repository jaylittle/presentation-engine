UPDATE
  ResumeSkill
SET
  LegacyID = @LegacyID,
  Type = @Type,
  Name = @Name,
  Hint = @Hint,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;