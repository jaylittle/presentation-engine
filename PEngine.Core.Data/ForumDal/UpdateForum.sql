UPDATE
  Forum
SET
  Name = @Name,
  Description = @Description,
  VisibleFlag = @VisibleFlag,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;