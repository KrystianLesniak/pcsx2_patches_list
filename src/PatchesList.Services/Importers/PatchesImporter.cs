using PatchesList.Common;
using PatchesList.Common.Interfaces;
using PatchesList.Common.Models;
using PatchesList.Services.Importers;
using System.Diagnostics;

namespace PatchesList.Services.Exporters
{
    public class PatchesImporter : IPatchesImporter
    {
        public async Task<IEnumerable<GameData>> ImportData()
        {
            List<GameData> gameDatasSet = new();
            object dataSetLocker = new();

            await Parallel.ForEachAsync(Directory.GetFiles(GetLocalFilesPath(), "*.pnach"), async (file, ct) =>
            {
                //Prepare a File
                var lines = await File.ReadAllLinesAsync(file, ct);
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);

                //Download data from File
                var gameTitles = GameDatasDownloader.GetGameDataFromLines(lines, "gametitle=");

                var patchComments = GameDatasDownloader.GetGameDataFromLines(lines, "comment=")
                    .Concat(GameDatasDownloader.GetGameDataFromLines(lines, "description="));

                var crcCode = GameDatasDownloader.GetCrcCode(fileNameWithoutExt);

                var gameCode = GameDatasDownloader.GetGameCode(fileNameWithoutExt);


                //Manipulate data
                gameTitles = GameDatasManipulator.RemoveGameCodeFromTitles(gameTitles, gameCode);

                //Create a record
                var gameData = new GameData(
                    crcCode,
                    gameCode,
                    Path.GetFileName(file),
                    gameTitles,
                    patchComments
                    );

                lock (dataSetLocker)
                {
                    gameDatasSet.Add(gameData);
                }

            });

            return gameDatasSet;
        }

        private static string GetLocalFilesPath()
        {
            if (Debugger.IsAttached)
            {
                var directory = Directory.GetCurrentDirectory();

                while (Directory.Exists(Path.Combine(directory, Consts.Pcsx2SubModuleName)) == false)
                {
                    directory = Directory.GetParent(directory)!.FullName;
                }

                return Path.Combine(directory, Consts.Pcsx2SubModuleName, Consts.Pcsx2PatchesFolder);
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(), Consts.Pcsx2SubModuleName, Consts.Pcsx2PatchesFolder);
            }
        }
    }
}
