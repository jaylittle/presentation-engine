UPDATE
  ArticleSection
SET
  ArticleGuid = @ArticleGuid,
  Name = @Name,
  Data = @Data,
  SortOrder = @SortOrder,
  UniqueName = @UniqueName,
  CreatedUTC = @CreatedUTC,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;