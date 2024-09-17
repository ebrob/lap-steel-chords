//var targetDir = Path.GetFullPath("../../../../../lap-steel-chord-displayer/scripts");
var targetDir = Path.GetFullPath("../../lap-steel-chord-displayer/scripts");
Console.WriteLine(targetDir);
var generator = new LapSteelChordGenerator();
await generator.Generate(targetDir, 18, "OpenD");