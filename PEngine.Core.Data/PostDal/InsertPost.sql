INSERT INTO Post (
  Guid,
  LegacyID,
  Name,
  Data,
  IconFileName,
  VisibleFlag,
  LockDownVisibleFlag,
  NoIndexFlag,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Name,
  @Data,
  @IconFileName,
  @VisibleFlag,
  @LockDownVisibleFlag,
  @NoIndexFlag,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);