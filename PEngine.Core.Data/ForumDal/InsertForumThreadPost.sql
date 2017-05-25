INSERT INTO ForumThreadPost (
  Guid,
  ForumThreadGuid,
  ForumUserGuid,
  VisibleFlag,
  LockFlag,
  Data,
  IPAddress,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @ForumThreadGuid,
  @ForumUserGuid,
  @VisibleFlag,
  @LockFlag,
  @Data,
  @IPAddress,
  @CreatedUTC,
  @ModifiedUTC
);