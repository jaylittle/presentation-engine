UPDATE
  ArticleSection
SET
  ArticleGuid = @ArticleGuid,
  Name = @Name,
  Data = @Data,
  SortOrder = @SortOrder,
  UniqueName = @UniqueName,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;