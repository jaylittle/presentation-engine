PRAGMA defer_foreign_keys=on;

UPDATE
  Version
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ResumeWorkHistory
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ResumeSkill
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ResumePersonal
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ResumeObjective
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ResumeEducation
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  Post
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  Article
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ArticleSection
SET
  Guid = hex(substr(Guid, 4, 1)) ||
  hex(substr(Guid, 3, 1)) ||
  hex(substr(Guid, 2, 1)) ||
  hex(substr(Guid, 1, 1)) || '-' ||
  hex(substr(Guid, 6, 1)) ||
  hex(substr(Guid, 5, 1)) || '-' ||
  hex(substr(Guid, 8, 1)) ||
  hex(substr(Guid, 7, 1)) || '-' ||
  hex(substr(Guid, 9, 2)) || '-' ||
  hex(substr(Guid, 11, 6))
WHERE
  typeof(Guid) == 'blob';

UPDATE
  ArticleSection
SET
  ArticleGuid = hex(substr(ArticleGuid, 4, 1)) ||
  hex(substr(ArticleGuid, 3, 1)) ||
  hex(substr(ArticleGuid, 2, 1)) ||
  hex(substr(ArticleGuid, 1, 1)) || '-' ||
  hex(substr(ArticleGuid, 6, 1)) ||
  hex(substr(ArticleGuid, 5, 1)) || '-' ||
  hex(substr(ArticleGuid, 8, 1)) ||
  hex(substr(ArticleGuid, 7, 1)) || '-' ||
  hex(substr(ArticleGuid, 9, 2)) || '-' ||
  hex(substr(ArticleGuid, 11, 6))
WHERE
  typeof(ArticleGuid) == 'blob';

PRAGMA defer_foreign_keys=off;