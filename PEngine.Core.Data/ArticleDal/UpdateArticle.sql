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
  LockDownVisibleFlag = @LockDownVisibleFlag,
  NoIndexFlag = @NoIndexFlag,
  UniqueName = @UniqueName,
  HideButtonsFlag = @HideButtonsFlag,
  HideDropDownFlag = @HideDropDownFlag,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;