using Flurl;
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

            var orderedSet = gameDatas
                .OrderBy(x => x.GameTitle.FirstOrDefault() == null)
                .ThenBy(x => x.GameTitle.FirstOrDefault())
                .ThenBy(x => x.GameCode == null)
                .ThenBy(x => x.GameCode)
                .ThenBy(x => x.CRCCode == null)
                .ThenBy(x => x.CRCCode);

            foreach (var data in orderedSet)
            {
                var titles = await CreateLineBreakList(data.GameTitle);
                var comments = await CreateLineBreakList(data.PatchComment);
                var downloadLink = await PrepareDownloadLink(data.FileName);

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

        private static Task<string> CreateLineBreakList(IEnumerable<string> list)
            => Task.FromResult(string.Join("<br />", list));

        private static Task<string> PrepareDownloadLink(string fileName)
            => Task.FromResult(Url.Combine("https://raw.githubusercontent.com/PCSX2/", Consts.Pcsx2SubModuleName, "/main/", Consts.Pcsx2PatchesFolder, fileName));
    }
}
