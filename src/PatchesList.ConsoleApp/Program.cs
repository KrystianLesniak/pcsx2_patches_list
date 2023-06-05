using PatchesList.Services.Exporters;

var wsPatcherImporter = new PatchesImporter("patches");
await wsPatcherImporter.ImportData();

var exporter = new PatchesMarkdownExporter(wsPatcherImporter, "markdown_patches.md");
await exporter.ExportData();

Console.WriteLine("Markdowns updated");
Console.ReadLine();