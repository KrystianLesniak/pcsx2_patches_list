using PatchesList.Common.Models;

namespace PatchesList.Common.Interfaces
{
    public interface IPatchesImporter
    {
        Task<IEnumerable<GameData>> ImportData();
    }
}
