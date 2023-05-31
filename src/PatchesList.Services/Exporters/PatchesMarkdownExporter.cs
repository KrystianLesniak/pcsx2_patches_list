using Flurl;
using PatchesList.Common;
using PatchesList.Common.Interfaces;

namespace PatchesList.Services.Exporters
{
    public class PatchesMarkdownExporter : IPatchesExporter
    {
        private readonly IPatchesImporter _patchesImporter;
        private readonly string _filePath;
        private readonly string _header;

        public PatchesMarkdownExporter(IPatchesImporter patchesImporter, string filePath, string header)
        {
            _patchesImporter = patchesImporter;
            _filePath = filePath;
            _header = header;
        }

        public async Task ExportData()
        {
            using StreamWriter sw = File.CreateText(_filePath);

            await CreateHeader(sw);

            var orderedSet = _patchesImporter.GameDataSet
                .OrderBy(x => x.GameTitle.FirstOrDefault() == null)
                .ThenBy(x => x.GameTitle.FirstOrDefault())
                .ThenBy(x => x.PatchComment.FirstOrDefault() == null)
                .ThenBy(x => x.PatchComment.FirstOrDefault())
                .ThenBy(x => x.CRCCode);

            foreach (var data in orderedSet)
            {
                var titles = string.Join("<br />", data.GameTitle);
                var comments = string.Join("<br />", data.PatchComment);
                var downloadLink = Url.Combine("https://raw.githubusercontent.com/PCSX2/", Consts.Pcsx2SubModuleName, "/main/", data.FilePath.Replace('\\', '/'));

                await sw.WriteLineAsync($"|{data.CRCCode}|{titles}|{comments}|[Download]({downloadLink})|");
            }
        }

        private async Task CreateHeader(StreamWriter stream)
        {
            await stream.WriteLineAsync("If you are looking for specific game use shortcut CTRL+F");
            await stream.WriteLineAsync($"## {_header}");
            await stream.WriteLineAsync(Environment.NewLine);
            await stream.WriteLineAsync("|CRC|Game Title|Comment|Link|");
            await stream.WriteLineAsync("|---|----------|-------|----|");
        }
    }
}
