INSERT INTO Post (
  Guid,
  LegacyID,
  Title,
  Data,
  IconFileName,
  VisibleFlag,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Title,
  @Data,
  @IconFileName,
  @VisibleFlag,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);