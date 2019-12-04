UPDATE
  Article
SET
  LegacyID = @LegacyID,
  Name = @Name,
  Description = @Description,
  Category = @Category,
  ContentURL = @ContentURL,
  DefaultSection = @DefaultSection,
  VisibleFlag = @VisibleFlag,
  NoIndexFlag = @NoIndexFlag,
  UniqueName = @UniqueName,
  HideButtonsFlag = @HideButtonsFlag,
  HideDropDownFlag = @HideDropDownFlag,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;