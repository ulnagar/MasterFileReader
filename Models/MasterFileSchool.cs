namespace MasterFileReader.Models;

public sealed record MasterFileSchool(
    string SiteCode,
    string Name,
    SchoolStatus Status,
    string PrincipalName,
    string PrincipalEmail);