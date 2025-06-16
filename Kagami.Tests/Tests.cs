using System.Reflection;
using Core.Computers;

namespace Kagami.Tests;

public class Tests
{
   protected Lazy<FolderName> testFolder = new(() => ((FileName)Assembly.GetExecutingAssembly().Location).Folder);

   protected void runTest()
   {
      
   }
}