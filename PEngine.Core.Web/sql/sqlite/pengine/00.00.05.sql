ALTER TABLE Post
  ADD COLUMN NoIndexFlag REAL NOT NULL DEFAULT 0;

ALTER TABLE Article
  ADD COLUMN NoIndexFlag REAL NOT NULL DEFAULT 0;