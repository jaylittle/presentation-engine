/* This script will insert the default content to enable forum related functionality */

INSERT INTO ForumUser(Guid, UserID, Password, AdminFlag, BanFlag, Email, Website, Comment, LastIPAddress, LastLogon, CreatedUTC, ModifiedUTC)
VALUES (randomblob(16), 'Admin', '', 1, 0, 'john.doe@imaginary.com', 'http://imaginary.com', '', '', null, datetime('2017-05-15 00:00:00'), datetime('2017-05-15 00:00:00'))
