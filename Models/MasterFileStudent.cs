namespace MasterFileReader.Models;
public sealed record MasterFileStudent(
    string SRN,
    string FirstName,
    string LastName,
    StudentGrade Grade,
    string Parent1Email,
    string Parent2Email);
