UPDATE
  ForumThread
SET
  VisibleFlag = @VisibleFlag,
  LockFlag = @LockFlag,
  Name = @Name,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;