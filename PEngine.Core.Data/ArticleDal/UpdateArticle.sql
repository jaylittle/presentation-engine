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
  UniqueName = @UniqueName,
  HideButtonsFlag = @HideButtonsFlag,
  HideDropDownFlag = @HideDropDownFlag,
  AdminPass = @AdminPass,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;