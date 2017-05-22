UPDATE
  Post
SET
  LegacyID = @LegacyID,
  Name = @Name,
  Data = @Data,
  IconFileName = @IconFileName,
  VisibleFlag = @VisibleFlag,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;