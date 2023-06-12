namespace PatchesList.Common.Models
{
    public record GameData(
        string CRCCode,
        string GameCode,
        string FileName,
        IEnumerable<string> GameTitle,
        IEnumerable<string> PatchComment);
}
