UPDATE
  Post
SET
  LegacyID = @LegacyID,
  Title = @Title,
  Data = @Data,
  IconFileName = @IconFileName,
  VisibleFlag = @VisibleFlag,
  UniqueName = @UniqueName,
  CreatedUTC = @CreatedUTC,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;