namespace MasterFileReader.Services;

using MasterFileReader.Models;
using OfficeOpenXml;

internal sealed class MasterFileService
{
	public async Task<List<MasterFileSchool>> GetSchoolList()
	{
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

        return FileSchools;
    }


    public async Task<List<MasterFileStudent>> GetStudentList()
    {
        List<MasterFileStudent> FileStudents = new();

        string path = "Master_file_2023.xlsx";
        FileInfo fileInfo = new(path);

        ExcelPackage package = new(fileInfo);
        ExcelWorksheet worksheet = package.Workbook.Worksheets.First(sheet => sheet.Name == "Students_2023");

        int rows = worksheet.Dimension.Rows;
        int columns = worksheet.Dimension.Columns;

        int[] dataColumns = new[] { 1, 3, 4, 6, 42, 43 };

        for (int i = 2; i <= rows; i++)
        {
            var codeCell = worksheet.Cells[i, 1].Value?.ToString().Trim();
            if (codeCell is null || string.IsNullOrWhiteSpace(codeCell))
                continue;

            var code = codeCell;

            var fNameCell = worksheet.Cells[i, 3].Value as string;
            if (fNameCell is null || string.IsNullOrWhiteSpace(fNameCell))
                continue;

            var fName = fNameCell.Trim();

            var sNameCell = worksheet.Cells[i, 4].Value as string;
            if (sNameCell is null || string.IsNullOrWhiteSpace(sNameCell))
                continue;

            var sName = sNameCell.Trim();

            var gradeCell = worksheet.Cells[i, 6].Value?.ToString().Trim();
            if (gradeCell is null || string.IsNullOrWhiteSpace(gradeCell))
                continue;

            StudentGrade grade = gradeCell.Trim() switch
            {
                "5" => StudentGrade.Y05,
                "6" => StudentGrade.Y06,
                "6*" => StudentGrade.Y06,
                "7" => StudentGrade.Y07,
                "8" => StudentGrade.Y08,
                "9" => StudentGrade.Y09,
                "10" => StudentGrade.Y10,
                "11" => StudentGrade.Y11,
                "12" => StudentGrade.Y12,
                _ => StudentGrade.Unknown
            };

            var parent1Cell = worksheet.Cells[i, 42].Value as string;
            string parent1 = string.Empty;
            if (parent1Cell is not null && !string.IsNullOrWhiteSpace(parent1Cell))
                parent1 = parent1Cell.Trim();

            var parent2Cell = worksheet.Cells[i, 43].Value as string;
            string parent2 = string.Empty;
            if (parent2Cell is not null && !string.IsNullOrWhiteSpace(parent2Cell))
                parent2 = parent2Cell.Trim();

            var entry = new MasterFileStudent(
                code,
                fName,
                sName,
                grade,
                parent1,
                parent2);

            FileStudents.Add(entry);
        }

        return FileStudents;
    }
}
