-----------------------------------------------------------
----------------- Main categories -------------------------
-----------------------------------------------------------

IF COL_LENGTH('Category','MainKey') IS NULL
 BEGIN
 PRINT 'Creating Column [MainKey]'
 ALTER TABLE Category ADD MainKey VARCHAR(50);
 PRINT 'Column created'
END

IF COL_LENGTH('Category','ValueEn') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueEn]'
 ALTER TABLE Category ADD ValueEn VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('Category','ValueSk') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueSk]'
 ALTER TABLE Category ADD ValueSk VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('Category','ValueHu') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueHu]'
 ALTER TABLE Category ADD ValueHu VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('Category','ValueCz') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueCz]'
 ALTER TABLE Category ADD ValueCz VARCHAR(100);
 PRINT 'Column created'
END

-----------------------------------------------------------
----------------- Sub categories --------------------------
-----------------------------------------------------------

IF COL_LENGTH('SubCategory','SubKey') IS NULL
 BEGIN
 PRINT 'Creating Column [SubKey]'
 ALTER TABLE SubCategory ADD SubKey VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('SubCategory','ValueEn') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueEn]'
 ALTER TABLE SubCategory ADD ValueEn VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('SubCategory','ValueSk') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueSk]'
 ALTER TABLE SubCategory ADD ValueSk VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('SubCategory','ValueHu') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueHu]'
 ALTER TABLE SubCategory ADD ValueHu VARCHAR(100);
 PRINT 'Column created'
END

IF COL_LENGTH('SubCategory','ValueCz') IS NULL
 BEGIN
 PRINT 'Creating Column [ValueCz]'
 ALTER TABLE SubCategory ADD ValueCz VARCHAR(100);
 PRINT 'Column created'
END