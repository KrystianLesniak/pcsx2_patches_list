using PatchesList.Common.Models;

namespace PatchesList.Common.Interfaces
{
    public interface IPatchesImporter
    {
        List<GameData> GameDataSet { get; }

        Task ImportData();
    }
}
