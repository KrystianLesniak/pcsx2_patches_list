namespace PatchesList.Services.Importers
{
    public static class GameDatasDownloader
    {
        public static string GetGameCode(string fileName)
        {
            if (fileName.Contains('_'))
            {
                var split = fileName.Split('_');

                //If file name is ex. "SCKA_20006_3A2EF433" instead of "SCKA-20006_3A2EF433"
                if (split.Length > 2)
                    return $"{split.First()}-{split[1]}";

                return split.First();
            }
            else
            {
                return String.Empty;
            }
        }

        public static string GetCrcCode(string fileName)
        {

            if (fileName.Contains('_'))
            {
                var split = fileName.Split('_');

                return split.Last();
            }
            else
            {
                return fileName;
            }
        }

        public static IEnumerable<string> GetGameDataFromLines(string[] lines, string dataString = "gametitle=")
        {
            return lines
                .Where(x => x.StartsWith(dataString, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Replace(dataString, "").Trim());
        }
    }
}
