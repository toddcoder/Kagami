using Core.Applications.CommandProcessing;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Tests;

public class Processor : CommandProcessor
{
   public Processor() : base("kagami-tests")
   {
   }

   [Switch("folder", "string", "Folder of tests")]
   public Maybe<FolderName> Folder { get; set; } = nil;

   [Switch("file", "string", "Individual test")]
   public Maybe<FileName> File { get; set; } = nil;

   [Command("run", "Runs all tests in the Kagami.Tests project.")]
   public void RunTests()
   {
      var tests = new Tests();
      if (Folder is (true, var folder))
      {
         tests.RunAllTests(folder);
      }
      else if (File is (true, var file))
      {
         tests.RunTest(file);
      }
   }

   [Command("generate", "Generate expected files")]
   public void GenerateTests()
   {
      var tests = new Tests();
      if (Folder is (true, var folder))
      {
         tests.GenerateExpectedTexts(folder);
      }
      else if (File is (true, var file))
      {
         tests.GenerateExpectedText(file);
      }
   }

   public override StringHash GetConfigurationDefaults() => [];

   public override StringHash GetConfigurationHelp() => [];
}