using Core.Applications.CommandProcessing;
using Core.Collections;

namespace Kagami.Tests;

public class Processor : CommandProcessor
{
   protected Tests tests;

   public Processor() : base("kagami-tests")
   {
      var testFolder = configuration.Maybe.FolderName("testFolder").Required("testFolder required");
      tests = new Tests(testFolder);
   }

   [Command("run-all-tests", "Runs all tests in the Kagami.Tests project.")]
   public void RunAllTests() => tests.RunAllTests();

   [Command("generate-expected", "Generate expected files")]
   public void GenerateExpected()
   {
   }

   public override StringHash GetConfigurationDefaults() => [];

   public override StringHash GetConfigurationHelp() => [];
}