using PatchesList.Common;
using PatchesList.Common.Interfaces;
using PatchesList.Common.Models;

namespace PatchesList.Services.Exporters
{
    public class PatchesMarkdownExporter : IPatchesExporter
    {
        private readonly IEnumerable<GameData> gameDatas;

        public PatchesMarkdownExporter(IEnumerable<GameData> gameDatas)
        {
            this.gameDatas = gameDatas;
        }

        public async Task ExportData()
        {
            using StreamWriter sw = File.CreateText(Consts.TargetMarkdownFilePath);

            await CreateHeader(sw);

            // Invariant culture so the order doesn't depend on the machine's locale
            var comparer = StringComparer.InvariantCulture;

            var orderedSet = gameDatas
                .OrderBy(x => x.GameTitle.FirstOrDefault() == null)
                .ThenBy(x => x.GameTitle.FirstOrDefault(), comparer)
                .ThenBy(x => x.GameCode == null)
                .ThenBy(x => x.GameCode, comparer)
                .ThenBy(x => x.CRCCode == null)
                .ThenBy(x => x.CRCCode, comparer);

            foreach (var data in orderedSet)
            {
                var titles = CreateLineBreakList(data.GameTitle);
                var comments = CreateLineBreakList(data.PatchComment);
                var downloadLink = PrepareDownloadLink(data.FileName);

                await sw.WriteLineAsync($"|{data.GameCode}|{titles}|[{data.CRCCode}]({downloadLink})|{comments}|");
            }
        }

        private static async Task CreateHeader(StreamWriter stream)
        {
            await stream.WriteLineAsync("If you are looking for specific game use shortcut CTRL+F");
            await stream.WriteLineAsync($"## PCSX2 Official Patches");
            await stream.WriteLineAsync(Environment.NewLine);
            await stream.WriteLineAsync("|Game Code|Game Title|CRC|Comments|");
            await stream.WriteLineAsync("|---------|----------|---|--------|");
        }

        private static string CreateLineBreakList(IEnumerable<string> list)
            => string.Join("<br />", list);

        private static string PrepareDownloadLink(string fileName)
            => $"https://raw.githubusercontent.com/PCSX2/{Consts.Pcsx2SubModuleName}/main/{Consts.Pcsx2PatchesFolder}/{Uri.EscapeDataString(fileName)}";
    }
}
