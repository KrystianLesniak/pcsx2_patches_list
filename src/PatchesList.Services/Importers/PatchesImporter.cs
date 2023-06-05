using PatchesList.Common;
using PatchesList.Common.Interfaces;
using PatchesList.Common.Models;
using System.Diagnostics;

namespace PatchesList.Services.Exporters
{
    public class PatchesImporter : IPatchesImporter
    {
        public List<GameData> GameDataSet { get; private set; } = new();
        private static readonly object dataSetLocker = new();

        private readonly string _patchesDirectory;
        public PatchesImporter(string patchesDirectory)
        {
            _patchesDirectory = patchesDirectory;
        }

        public async Task ImportData()
        {
            await Parallel.ForEachAsync(Directory.GetFiles(GetLocalFilesPath(), "*.pnach"), async (file, ct) =>
            {
                var lines = await File.ReadAllLinesAsync(file, ct);
                var fileName = Path.GetFileNameWithoutExtension(file);

                var gameTile = GetGameDataFromLines(lines, "gametitle=");

                string crcCode;
                string gameCode;
                if (fileName.Contains('_'))
                {
                    crcCode = fileName.Split('_')[1];
                    gameCode = fileName.Split('_')[0];

                    gameTile = gameTile.Select(x => x.Replace(gameCode, string.Empty).Replace("()", string.Empty));
                }
                else
                {
                    crcCode = fileName;
                    gameCode = String.Empty;
                }


                var gameData = new GameData(
                    crcCode,
                    gameCode,
                    Path.Combine(_patchesDirectory,
                    Path.GetFileName(file)),
                    gameTile,
                    GetGameDataFromLines(lines, "comment="));

                lock (dataSetLocker)
                {
                    GameDataSet.Add(gameData);
                }
            });
        }

        private string GetLocalFilesPath()
        {
            if (Debugger.IsAttached)
            {
                var directory = Directory.GetCurrentDirectory();

                while (Directory.Exists(Path.Combine(directory, Consts.Pcsx2SubModuleName)) == false)
                {
                    directory = Directory.GetParent(directory)!.FullName;
                }

                return Path.Combine(directory, Consts.Pcsx2SubModuleName, _patchesDirectory);
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(), Consts.Pcsx2SubModuleName, _patchesDirectory);
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
