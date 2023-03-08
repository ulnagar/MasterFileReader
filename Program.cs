using ConsoleTables;
using MasterFileReader.Services;
using OfficeOpenXml;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var masterFileService = new MasterFileService();

var FileSchools = await masterFileService.GetSchoolList();

Console.WriteLine($"Found {FileSchools.Count} schools");

var dataCollectionsService = new DataCollectionsSiteService(new HttpClient());

var SiteSchools = await dataCollectionsService.GetSchoolList();

Console.WriteLine($"Found {SiteSchools.Count} schools");

foreach (var fileSchool in FileSchools)
{
    var siteSchool = SiteSchools.FirstOrDefault(school => school.SchoolCode == fileSchool.SiteCode);

    if (siteSchool is null)
    {
        // Log and or notify about this grievous error.
        Console.WriteLine($"Checking School {fileSchool.Name}");
        Console.WriteLine($"ERROR: No matching school found in DataCollections!");
        Console.WriteLine();

        continue;
    }

    if (fileSchool.PrincipalEmail.ToLower() != siteSchool.PrincipalEmail.ToLower())
    {
        Console.WriteLine($"Checking School {fileSchool.Name}");

        var table = new ConsoleTable("Field", "MasterFile", "DataCollections");
        table.AddRow("Principal", fileSchool.PrincipalName, siteSchool.PrincipalName);
        table.AddRow("Email", fileSchool.PrincipalEmail, siteSchool.PrincipalEmail);

        table.Write();

        Console.WriteLine();
    }
}

var FileStudents = await masterFileService.GetStudentList();

Console.WriteLine($"Found {FileStudents.Count} students");

Console.ReadLine();