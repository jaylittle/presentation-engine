INSERT INTO Forum (
  Guid,
  Name,
  Description,
  VisibleFlag,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @Name,
  @Description,
  @VisibleFlag,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);