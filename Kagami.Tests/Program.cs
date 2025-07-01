using Core.Computers;

var testPath = Environment.ProcessPath ?? FolderName.Temp;
testPath.Parents(3).Map(p => p["Code"]);

var test = new Kagami.Tests.Tests(testPath);
test.RunAllTests();