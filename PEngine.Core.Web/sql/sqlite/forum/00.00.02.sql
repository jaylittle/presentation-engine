/* This script will insert the default content to enable forum related functionality */

INSERT INTO ForumUser(Guid, UserID, Password, AdminFlag, BanFlag, Email, Website, Comment, LastIPAddress, LastLogon, CreatedUTC, ModifiedUTC)
VALUES (randomblob(16), 'Admin', 'D90FF84A3F4EBF0D3803DA6C4D901C455A6AA74CE16B28B3AD2A69D475721390', 1, 0, 'john.doe@imaginary.com', 'http://imaginary.com', '', '', null, datetime('2017-05-15 00:00:00'), datetime('2017-05-15 00:00:00'))
