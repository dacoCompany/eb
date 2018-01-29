IF COL_LENGTH('CompanyDetails','CompanySettingId') IS NOT NULL
 BEGIN
 PRINT 'Altering Column [CompanySettingId]'
 ALTER TABLE CompanyDetails ALTER COLUMN CompanySettingId INT NULL;
 PRINT 'Column altered'
 END