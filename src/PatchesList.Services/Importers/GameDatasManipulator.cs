namespace PatchesList.Services.Importers
{
    public static class GameDatasManipulator
    {
        public static IEnumerable<string> RemoveGameCodeFromTitles(IEnumerable<string> gameTitles, string gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
                return gameTitles;

            return gameTitles.Select(x =>
                //Remove gamecode ex. "SLUS-21423" from title
                x.Replace(gameCode, string.Empty)
                //Remove gamecode ex. "SLPM_66851" from title
                .Replace(gameCode.Replace('-', '_'), string.Empty)
                //Remove gamecode ex. "SLES_541.35" from title
                .Replace(gameCode.Replace('-', '_').Insert(gameCode.Length - 2, "."), string.Empty)
                //Remove empty parenthess from title
                .Replace("()", string.Empty)
                .Replace("[]", string.Empty));
        }
    }
}
