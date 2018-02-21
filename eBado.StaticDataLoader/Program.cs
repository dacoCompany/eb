using CmdLine;
using DataAccess;
using Infrastructure.Common.DB;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace eBado.StaticDataLoader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                CmdLineArguments argument = CommandLine.Parse<CmdLineArguments>();

                if (args.Length == 0)
                {
                    throw new CommandLineException(new CommandArgumentHelp(argument.GetType())
                    {
                        Message = "No command specified\n",
                    });
                }

                if (argument.Categories || argument.All)
                {
                    watch.Restart();
                    ImportMainCategories();
                    Console.WriteLine($"Categories data imported. Elapsed time: {watch.ElapsedMilliseconds} ms.");
                }

                if (argument.SubCategories || argument.All)
                {
                    watch.Restart();
                    ImportSubCategories();
                    Console.WriteLine($"Sub-categories data imported. Elapsed time: {watch.ElapsedMilliseconds} ms.");
                }
            }
            catch (CommandLineException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            watch.Stop();

            Environment.Exit(0);
        }

        private static void ImportMainCategories()
        {
            string mainCatsPath = @"C:\eBado\CompanyCategories\MainCategories.csv";

            if (File.Exists(mainCatsPath))
            {
                Console.WriteLine($"Reading categories data from file '{mainCatsPath}'.");
                var mainCats = DataTable.New.ReadCsv(mainCatsPath);
                Console.WriteLine($"{mainCats.Rows.Count()} records found.");
                Console.WriteLine("\r\nStarting import...");

                int updatedCount = 0;
                int addedCount = 0;

                using (var uow = new UnitOfWork())
                {
                    foreach (var row in mainCats.Rows.Select((value, i) => new { i, value }))
                    {
                        Console.Write($"\rImporting data... {row.i + 1}/{mainCats.Rows.Count()}");

                        string mainKey = row.value["Key"];
                        var categoryDbo = uow.CategoryRepository.FindFirstOrDefault(c => c.MainKey == mainKey);

                        if (categoryDbo != null)
                        {
                            categoryDbo.ValueEn = row.value["EnMain"];
                            categoryDbo.ValueSk = row.value["SkMain"];
                            categoryDbo.ValueHu = row.value["HuMain"];
                            categoryDbo.ValueCz = row.value["CzMain"];
                            categoryDbo.IsActive = bool.TryParse(row.value["IsActive"], out bool isExistingActive) && isExistingActive;
                            categoryDbo.DateModified = DateTime.Now;

                            ++updatedCount;
                        }
                        else
                        {
                            var mainDbo = new CategoryDbo
                            {
                                MainKey = mainKey,
                                ValueEn = row.value["EnMain"],
                                ValueSk = row.value["SkMain"],
                                ValueHu = row.value["HuMain"],
                                ValueCz = row.value["CzMain"],
                                IsActive = bool.TryParse(row.value["IsActive"], out bool isActive) && isActive
                            };

                            uow.CategoryRepository.Add(mainDbo);

                            ++addedCount;
                        }
                    }

                    uow.Commit();

                    Console.Write("\rImporting data... Done!");
                }

                Console.WriteLine($"\r\nImport of Categories completed. Summary:\r\nAdded items: \t{addedCount}\r\nUpdated items: \t{updatedCount}");
            }
            else
            {
                Console.WriteLine($"File '{mainCatsPath}' was not found. Make sure that the path is correct and the file exist, verify permissions.");
            }
        }

        private static void ImportSubCategories()
        {
            string subCatsPath = @"C:\eBado\CompanyCategories\SubCategories.csv";

            if (File.Exists(subCatsPath))
            {
                Console.WriteLine($"Reading sub-categories data from file '{subCatsPath}'.");
                var subCats = DataTable.New.ReadCsv(subCatsPath);
                Console.WriteLine($"{subCats.Rows.Count()} records found.");
                Console.WriteLine("\r\nStarting import...");

                int updatedCount = 0;
                int addedCount = 0;

                using (var uow = new UnitOfWork())
                {
                    foreach (var row in subCats.Rows.Select((value, i) => new { i, value }))
                    {
                        Console.Write($"\rImporting data... {row.i + 1}/{subCats.Rows.Count()}");

                        string subKey = row.value["Key"];
                        string mainKey = row.value["MainKey"];
                        var subCategoryDbo = uow.SubCategoryRepository.FindFirstOrDefault(s => s.SubKey == subKey);
                        int mainCatId = uow.CategoryRepository.FindFirstOrDefault(c => c.MainKey == mainKey).Id;

                        if (subCategoryDbo != null)
                        {
                            subCategoryDbo.ValueEn = row.value["EnSub"];
                            subCategoryDbo.ValueSk = row.value["SkSub"];
                            subCategoryDbo.ValueHu = row.value["HuSub"];
                            subCategoryDbo.ValueCz = row.value["CzSub"];
                            subCategoryDbo.IsActive = bool.TryParse(row.value["IsActive"], out bool isExistingActive) && isExistingActive;
                            subCategoryDbo.DateModified = DateTime.Now;
                            subCategoryDbo.CategoryId = mainCatId;

                            ++updatedCount;
                        }
                        else
                        {
                            var subDbo = new SubCategoryDbo
                            {
                                SubKey = subKey,
                                ValueEn = row.value["EnSub"],
                                ValueSk = row.value["SkSub"],
                                ValueHu = row.value["HuSub"],
                                ValueCz = row.value["CzSub"],
                                IsActive = bool.TryParse(row.value["IsActive"], out bool isActive) && isActive,
                                CategoryId = mainCatId
                            };

                            uow.SubCategoryRepository.Add(subDbo);

                            ++addedCount;
                        }
                    }

                    uow.Commit();

                    Console.Write("\rImporting data... Done!");
                }

                Console.WriteLine($"Import of Categories completed. Summary:\r\nAdded items: \t{addedCount}\r\nUpdated items: \t{updatedCount}");
            }
            else
            {
                Console.WriteLine($"File '{subCatsPath}' was not found. Make sure that the path is correct and the file exist, verify permissions.");
            }
        }
    }
}
