using ConsoleTables;
using MasterFileReader.Models;
using MasterFileReader.Services;
using OfficeOpenXml;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

List<MasterFileSchool> FileSchools = new();

string path = "Master_file_2023.xlsx";
FileInfo fileInfo = new(path);

ExcelPackage package = new(fileInfo);
ExcelWorksheet worksheet = package.Workbook.Worksheets.First(sheet => sheet.Name == "Partner_schools");

int rows = worksheet.Dimension.Rows;
int columns = worksheet.Dimension.Columns;

int[] dataColumns = new[] { 1, 2, 9, 17, 19 };

for (int i = 2; i <= rows; i++)
{
    var codeCell = worksheet.Cells[i, 9].Value;
    if (codeCell is null)
        continue;

    var code = codeCell.ToString().Trim();

    var nameCell = worksheet.Cells[i, 1].Value;
    if (nameCell is null)
        continue;

    var name = ((string)nameCell).Trim();

    var statusCell = worksheet.Cells[i, 2].Value;
    if (statusCell is null)
        continue;

    SchoolStatus status = statusCell.ToString() switch
    {
        "*INACTIVE*" => SchoolStatus.Inactive,
        "Student(s)" => SchoolStatus.Students,
        "Teacher(s)" => SchoolStatus.Teachers,
        "Student(s) and Teacher(s)" => SchoolStatus.Both,
        _ => SchoolStatus.Unknown
    };

    var principalCell = worksheet.Cells[i, 18].Value;
    var principal = (principalCell is not null) ? ((string)principalCell).Trim() : string.Empty;

    var principalEmailCell = worksheet.Cells[i, 20].Value;
    var principalEmail = (principalEmailCell is not null) ? ((string)principalEmailCell).Trim() : string.Empty;

    var entry = new MasterFileSchool(
        code,
        name,
        status,
        principal,
        principalEmail);

    FileSchools.Add(entry);
}

Console.WriteLine($"Found {FileSchools.Count} schools");

var service = new DataCollectionsSiteService(new HttpClient());

var SiteSchools = await service.GetSchoolList();

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

Console.ReadLine();