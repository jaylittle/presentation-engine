INSERT INTO ForumThread (
  Guid,
  ForumGuid,
  ForumUserGuid,
  VisibleFlag,
  LockFlag,
  Name,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @ForumGuid,
  @ForumUserGuid,
  @VisibleFlag,
  @LockFlag,
  @Name,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);