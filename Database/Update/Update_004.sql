IF COL_LENGTH('UserDetails','ProfilePictureUrlSmall') IS NULL
 BEGIN
 PRINT 'Creating Column [ProfilePictureUrlSmall]'
 ALTER TABLE UserDetails ADD ProfilePictureUrlSmall VARCHAR(100);
 PRINT 'Column created'
 END

 IF COL_LENGTH('UserDetails','ProfilePictureUrlMedium') IS NULL
 BEGIN
 PRINT 'Creating Column [ProfilePictureUrlMedium]'
 ALTER TABLE UserDetails ADD ProfilePictureUrlMedium VARCHAR(100);
 PRINT 'Column created'
 END

 IF COL_LENGTH('CompanyDetails','ProfilePictureUrlSmall') IS NULL
 BEGIN
 PRINT 'Creating Column [ProfilePictureUrlSmall]'
 ALTER TABLE CompanyDetails ADD ProfilePictureUrlSmall VARCHAR(100);
 PRINT 'Column created'
 END

 IF COL_LENGTH('CompanyDetails','ProfilePictureUrlMedium') IS NULL
 BEGIN
 PRINT 'Creating Column [ProfilePictureUrlMedium]'
 ALTER TABLE CompanyDetails ADD ProfilePictureUrlMedium VARCHAR(100);
 PRINT 'Column created'
 END