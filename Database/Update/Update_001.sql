IF COL_LENGTH('CompanyDetails','EncryptedId') IS NULL
 BEGIN
 PRINT 'Creating Column [EncryptedId]'
 ALTER TABLE CompanyDetails ADD EncryptedId VARCHAR(50);
 PRINT 'Column created'
 END