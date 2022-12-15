using SplitFile;
using System.Diagnostics;

var stopWatch = new Stopwatch();
stopWatch.Start();
await Arguments.Process(args);
stopWatch.Stop();
Console.WriteLine($"Ellapsed time: {stopWatch.ElapsedMilliseconds} ms");
