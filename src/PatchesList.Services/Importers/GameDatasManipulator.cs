using System.Text.RegularExpressions;

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

        // Many titles carry leftovers like "(PAL-M)  5A1D79D4", "[2F7B4DB8]" or ";1) Game CRC = 0x9C033483".
        public static IEnumerable<string> RemoveCrcFromTitles(IEnumerable<string> gameTitles, string crcCode)
        {
            var crcPattern = new Regex($@"(;\d\)?\s*)?(game\s*crc\s*=\s*)?(0x)?\[?{Regex.Escape(crcCode)}\]?", RegexOptions.IgnoreCase);

            return gameTitles
                .Select(x => Regex.Replace(crcPattern.Replace(x, string.Empty), @"\s{2,}", " ").Trim())
                // Drop titles that were nothing but leftovers, so the game sorts with the untitled ones
                .Where(x => x.Length > 0);
        }
    }
}
