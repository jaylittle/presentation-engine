UPDATE
  ResumeSkill
SET
  LegacyID = @LegacyID,
  Type = @Type,
  Name = @Name,
  Hint = @Hint,
  [Order]= @Order,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;