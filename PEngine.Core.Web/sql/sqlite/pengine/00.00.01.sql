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

CREATE TABLE ResumeWorkHistory
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Employer TEXT NOT NULL,
  EmployerURL TEXT NOT NULL,
  JobTitle TEXT NOT NULL,
  JobDescription TEXT NOT NULL,
  Started REAL,
  Completed REAL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ResumeWorkHistory_LegacyID_index ON ResumeWorkHistory (LegacyID);

CREATE TABLE ResumeSkill
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Type TEXT NOT NULL,
  Name TEXT NOT NULL,
  Hint TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ResumeSkill_LegacyID_index ON ResumeSkill (LegacyID);

CREATE TABLE ResumePersonal
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  FullName TEXT NOT NULL,
  Address1 TEXT NOT NULL,
  Address2 TEXT NOT NULL,
  City TEXT NOT NULL,
  State TEXT NOT NULL,
  Zip TEXT NOT NULL,
  Phone TEXT NOT NULL,
  Fax TEXT NOT NULL,
  Email TEXT NOT NULL,
  WebsiteURL TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ResumePersonal_LegacyID_index ON ResumePersonal (LegacyID);

CREATE TABLE ResumeObjective
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Data TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ResumeObjective_LegacyID_index ON ResumeObjective (LegacyID);

CREATE TABLE ResumeEducation
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Institute TEXT NOT NULL,
  InstituteURL TEXT NOT NULL,
  Program TEXT NOT NULL,
  Started REAL,
  Completed REAL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ResumeEducation_LegacyID_index ON ResumeEducation (LegacyID);

CREATE TABLE Post
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Name TEXT NOT NULL,
  Data TEXT NOT NULL,
  IconFileName TEXT,
  VisibleFlag REAL,
  UniqueName TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX Post_LegacyID_index ON Post (LegacyID);
CREATE INDEX Post_UniqueNameCreatedUTC_index ON Post (UniqueName, CreatedUTC);

CREATE TABLE Article
(
  Guid BLOB PRIMARY KEY NOT NULL,
  LegacyID INT,
  Name TEXT NOT NULL,
  Description TEXT NOT NULL,
  Category TEXT NOT NULL,
  ContentURL TEXT,
  DefaultSection TEXT NOT NULL,
  VisibleFlag REAL NOT NULL,
  UniqueName TEXT NOT NULL,
  HideButtonsFlag REAL NOT NULL,
  HideDropDownFlag REAL NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX Article_Category_index ON Article (Category);
CREATE INDEX Article_LegacyID_index ON Article (LegacyID);
CREATE UNIQUE INDEX Article_UniqueNameCreatedUTC_index ON Article (UniqueName, CreatedUTC);

CREATE TABLE ArticleSection
(
  Guid BLOB PRIMARY KEY NOT NULL,
  ArticleGuid BLOB NOT NULL,
  Name TEXT NOT NULL,
  Data TEXT NOT NULL,
  SortOrder REAL NOT NULL,
  UniqueName TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL,
  FOREIGN KEY (ArticleGuid) REFERENCES Article (Guid)
);
CREATE INDEX ArticleSection_ArticleGuidSortOrder_index ON ArticleSection (ArticleGuid, SortOrder);
CREATE INDEX ArticleSection_SortOrder_index ON ArticleSection (SortOrder);
