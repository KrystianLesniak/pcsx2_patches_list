using PatchesList.Services.Exporters;

var wsPatcherImporter = new PatchesImporter();
var gameDataCollection = await wsPatcherImporter.ImportData();

var exporter = new PatchesMarkdownExporter(gameDataCollection);
await exporter.ExportData();

Console.WriteLine("Markdowns updated");
Console.ReadLine();