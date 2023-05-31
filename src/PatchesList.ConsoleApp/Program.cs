using PatchesList.Services.Exporters;

var importAndExportTasks = new List<Task>
{
    Task.Run(async () =>
    {
        var wsPatcherImporter = new PatchesImporter("cheats_ws");
        await wsPatcherImporter.ImportData();

        var exporter = new PatchesMarkdownExporter(wsPatcherImporter, "markdown_ws.md", "Official PCSX2 Widescreen Patches");
        await exporter.ExportData();

    }),

    Task.Run(async () =>
    {
        var niPatcherImporter = new PatchesImporter("cheats_ni");
        await niPatcherImporter.ImportData();

        var exporter = new PatchesMarkdownExporter(niPatcherImporter, "markdown_ni.md", "Official PCSX2 No Interlace Patches");
        await exporter.ExportData();
    })
};

await Task.WhenAll(importAndExportTasks);

Console.WriteLine("Markdowns updated");
Console.ReadLine();