CREATE TABLE Version
(
  Guid BLOB PRIMARY KEY NOT NULL,
  Major INT NOT NULL,
  Minor INT NOT NULL,
  Build INT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX Version_MajorMinorBuild_index ON Version (Major, Minor, Build);

CREATE TABLE Quote
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Data TEXT NOT NULL
);
CREATE INDEX Quote_LegacyID_index ON Quote (LegacyID);