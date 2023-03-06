namespace MasterFileReader.Models;
public sealed record DataCollectionsSchool(
    string SchoolCode, //Column 0
    string Name, //Column 1
    string Status, //Column 9
    string PrincipalName, //Column 73
    string PrincipalFirstName, 
    string PrincipalLastName, 
    string PrincipalEmail //Column 74
    );