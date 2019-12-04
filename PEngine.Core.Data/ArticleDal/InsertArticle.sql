INSERT INTO Article (
  Guid,
  LegacyID,
  Name,
  Description,
  Category,
  ContentURL,
  DefaultSection,
  VisibleFlag,
  NoIndexFlag,
  UniqueName,
  HideButtonsFlag,
  HideDropDownFlag,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Name,
  @Description,
  @Category,
  @ContentURL,
  @DefaultSection,
  @VisibleFlag,
  @NoIndexFlag,
  @UniqueName,
  @HideButtonsFlag,
  @HideDropDownFlag,
  @CreatedUTC,
  @ModifiedUTC
);