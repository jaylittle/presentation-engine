UPDATE
  Post
SET
  LegacyID = @LegacyID,
  Name = @Name,
  Data = @Data,
  IconFileName = @IconFileName,
  VisibleFlag = @VisibleFlag,
  LockDownVisibleFlag = @LockDownVisibleFlag,
  NoIndexFlag = @NoIndexFlag,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;