INSERT INTO ArticleSection (
  Guid,
  ArticleGuid,
  Name,
  Data,
  SortOrder,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @ArticleGuid,
  @Name,
  @Data,
  @SortOrder,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);