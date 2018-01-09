IF COL_LENGTH('UserDetails','EncryptedId') IS NULL
 BEGIN
 PRINT 'Creating Column [EncryptedId]'
 ALTER TABLE UserDetails ADD EncryptedId VARCHAR(50);
 PRINT 'Column created'
 END