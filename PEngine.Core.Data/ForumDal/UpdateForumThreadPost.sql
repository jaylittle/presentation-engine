UPDATE
  ForumThreadPost
SET
  VisibleFlag = @VisibleFlag,
  LockFlag = @LockFlag,
  Data = @Data,
  IPAddress = @IPAddress,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;