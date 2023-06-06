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
                    var split = fileName.Split('_');
                        
                    gameCode = split.First();
                    crcCode = split.Last();
                    //If file name is ex. "SCKA_20006_3A2EF433" instead of "SCKA-20006_3A2EF433"
                    if (split.Length > 2)
                        gameCode = $"{split.First()}-{split[1]}";

                    gameTile = gameTile.Select(x =>
                        //Remove gamecode ex. "SLUS-21423" from title
                        x.Replace(gameCode, string.Empty)
                        //Remove gamecode ex. "SLPM_66851" from title
                        .Replace(gameCode.Replace('-', '_'), string.Empty)
                        //Remove gamecode ex. "SLES_541.35" from title
                        .Replace(gameCode.Replace('-', '_').Insert(gameCode.Length - 2,"."), string.Empty)
                        //Remove empty parenthess from title
                        .Replace("()", string.Empty)
                        .Replace("[]", string.Empty));
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
