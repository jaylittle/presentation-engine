UPDATE
  Post
SET
  LegacyID = @LegacyID,
  Name = @Name,
  Data = @Data,
  IconFileName = @IconFileName,
  VisibleFlag = @VisibleFlag,
  NoIndexFlag = @NoIndexFlag,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;