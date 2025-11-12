using Core.Computers;
using Kagami.Library;
using Kagami.Library.Runtime;

namespace Kagami.Playground;

public record ExecutionParameters(IContext Context, CompilerConfiguration Configuration, string Source, bool Execute, string PackageFolder,
   FileName TraceFile);