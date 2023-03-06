namespace MasterFileReader.Services;

using MasterFileReader.Models;
using System.Text.RegularExpressions;

internal sealed class DataCollectionsSiteService
{
    private readonly HttpClient _client;

    public DataCollectionsSiteService(HttpClient client)
    {
        _client = client;
    }


    public async Task<List<DataCollectionsSchool>> GetSchoolList()
    {
        var response = await _client.GetAsync("https://datacollections.det.nsw.edu.au/listofschools/csv/listofschool_all.csv");
        response.EnsureSuccessStatusCode();

        var list = new List<DataCollectionsSchool>();

        #region Raw CSV Read
        var content = await response.Content.ReadAsStringAsync();
        var entries = content.Split('\u000A').ToList();

        var CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        foreach (var entry in entries)
        {
            var splitString = CSVParser.Split(entry);
            if (splitString[0].Length > 4 || splitString.Length == 1)
                continue;

            try
            {
                var principalName = splitString[76].Trim();
                string principalFirstName = string.Empty;
                string principalLastName = string.Empty;

                if (principalName.IndexOf(',') > 0)
                {
                    principalName = principalName.Trim('"').Trim();
                    var principals = principalName.Split(',');
                    principalName = $"{principals[1].Trim()} {principals[0].Trim()}";
                    principalFirstName = principals[1].Trim();
                    principalLastName = principals[0].Trim();
                }

                var school = new DataCollectionsSchool(
                    splitString[0].Trim(),
                    splitString[1].Trim('"').Trim(),
                    splitString[9].Trim(),
                    principalName,
                    principalFirstName,
                    principalLastName,
                    splitString[77].Trim());

                list.Add(school);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        return list;
    }
}
