IF COL_LENGTH('UserSettings','Language') IS NULL
 BEGIN
 PRINT 'Creating Column [Language]'
 ALTER TABLE UserSettings ADD Language VARCHAR(10);
 PRINT 'Column created'
 END

 IF COL_LENGTH('CompanySettings','Language') IS NULL
 BEGIN
 PRINT 'Creating Column [Language]'
 ALTER TABLE CompanySettings ADD Language VARCHAR(10);
 PRINT 'Column created'
 END