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

CREATE TABLE Forum
(
  Guid BLOB PRIMARY KEY NOT NULL,
  Name TEXT NOT NULL,
  Description TEXT NOT NULL,
  VisibleFlag REAL NOT NULL,
  UniqueName TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX Forum_UniqueName_index ON Forum (UniqueName);

CREATE TABLE ForumUser
(
  Guid BLOB PRIMARY KEY NOT NULL,
  UserID TEXT NOT NULL,
  Password TEXT NOT NULL,
  AdminFlag REAL NOT NULL,
  BanFlag REAL NOT NULL,
  Email TEXT NOT NULL,
  Website TEXT NOT NULL,
  Comment TEXT NOT NULL,
  LastIPAddress TEXT,
  LastLogon REAL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX ForumUser_UserID_index ON ForumUser (UserID);
CREATE INDEX ForumUser_LastIPAddress_index ON ForumUser (LastIPAddress);

CREATE TABLE ForumThread
(
  Guid BLOB PRIMARY KEY NOT NULL,
  ForumGuid BLOB NOT NULL,
  ForumUserGuid BLOB NOT NULL,
  VisibleFlag REAL NOT NULL,
  LockFlag REAL NOT NULL,
  Name TEXT NOT NULL,
  UniqueName TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL,
  FOREIGN KEY (ForumGuid) REFERENCES Forum (Guid),
  FOREIGN KEY (ForumUserGuid) REFERENCES ForumUser (Guid)
);
CREATE INDEX ForumThread_ForumGuid_index ON ForumThread (ForumGuid);
CREATE INDEX ForumThread_ForumUserGuid_index ON ForumThread (ForumUserGuid);
CREATE INDEX ForumThread_UniqueName_index ON ForumThread (UniqueName);

CREATE TABLE ForumThreadPost
(
  Guid BLOB PRIMARY KEY NOT NULL,
  ForumThreadGuid BLOB NOT NULL,
  ForumUserGuid BLOB NOT NULL,
  VisibleFlag REAL NOT NULL,
  LockFlag REAL NOT NULL,
  Data TEXT NOT NULL,
  IPAddress TEXT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL,
  FOREIGN KEY (ForumThreadGuid) REFERENCES ForumThread (Guid),
  FOREIGN KEY (ForumUserGuid) REFERENCES ForumUser (Guid)
);
CREATE INDEX ForumThreadPost_ForumThreadGuid_index ON ForumThreadPost (ForumThreadGuid);
CREATE INDEX ForumThreadPost_ForumUserGuid_index ON ForumThreadPost (ForumUserGuid);