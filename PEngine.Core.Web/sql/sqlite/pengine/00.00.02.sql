/* This script will insert the default content to be displayed to a new user */

INSERT INTO Post (Guid, LegacyID, IconFileName, VisibleFlag, Name, UniqueName, Data, CreatedUTC, ModifiedUTC)
VALUES (randomblob(16), null, '', 1, 'Welcome to the Presentation Engine 5.0', 'welcome-to-the-presentation-engine-50', 'Welcome to the Presentation Engine 5.0.  This version has been a long time coming as it has been four years since the last official release of the Presentation Engine. This wait has been the result of many years in the field spent experimenting, studying and implementing efficient user interfaces for my clientele.  Now you get to benefit from that hard work.  But more importantly you, as a perspective future employer, get to take a peek at some of work and see for yourself whether or not I am the real deal.  Make no mistake there is more where this came from.  There is only so much that I am willing to give away for free as I make my living off writing code :)

To get started, simply click on the "Login" button in the upper right hand corner of the screen.  The username is "PEngineAdmin" and the password is blank.  Please change these as soon as possible by clicking the "Settings" button after logging in.  Not changing them constitutes a major security hole and will leave your site open to unauthorized changes from malicious visitors!

Regards,

Jay Little
Creator of PEngine', datetime('2001-01-01 00:00:00'), datetime('2001-01-01 00:00:00'));
