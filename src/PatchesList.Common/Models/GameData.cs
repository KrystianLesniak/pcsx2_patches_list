namespace PatchesList.Common.Models
{
    public record GameData(
        string CRCCode,
        string GameCode,
        string FilePath,
        IEnumerable<string> GameTitle,
        IEnumerable<string> PatchComment);
}
