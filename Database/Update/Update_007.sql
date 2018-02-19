IF COL_LENGTH('Address','PostalCode') IS NULL
 BEGIN
 PRINT 'Creating Column [PostalCode]'
 ALTER TABLE Address ADD PostalCode VARCHAR(10);
 PRINT 'Column created'

 PRINT 'Create indexes'
 CREATE INDEX IXAddressPostalCode ON Address (PostalCode);
 PRINT 'Create indexes finished successfully'
 END