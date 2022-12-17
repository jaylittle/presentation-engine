UPDATE
  Article
SET
  LegacyID = @LegacyID,
  Name = @Name,
  Description = @Description,
  Category = @Category,
  ContentURL = @ContentURL,
  ContentLinkAttributes = @ContentLinkAttributes,
  DefaultSection = @DefaultSection,
  VisibleFlag = @VisibleFlag,
  NoIndexFlag = @NoIndexFlag,
  UniqueName = @UniqueName,
  HideButtonsFlag = @HideButtonsFlag,
  HideDropDownFlag = @HideDropDownFlag,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;