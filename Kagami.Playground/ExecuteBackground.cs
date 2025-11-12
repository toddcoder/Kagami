using Core.Applications.Messaging;
using Core.Monads;
using Core.WinForms.Components;
using Core.WinForms.Controls;
using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using System.Text;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class ExecuteBackground(ExecutionParameters parameters) : Background
{
   public bool Cancelled { get; set; }

   public string Type { get; set; } = "";

   public string Value { get; set; } = "";

   public Maybe<int> ExceptionIndex { get; set; } = nil;

   public (string message, UiActionType type) Status { get; set; } = ("", UiActionType.Failure);

   public Maybe<Exception> Exception { get; set; } = nil;

   public LateLazy<Compiler> Compiler { get; set; } = new();

   public LateLazy<Machine> Machine { get; set; } = new();

   public readonly MessageEvent<string> TraceOutput = new();
   public readonly MessageEvent<(int, Operation)> OperationStarting = new();
   public readonly MessageEvent<(int, Operation, Optional<IObject>)> OperationFinished = new();
   public readonly MessageEvent<Frame> FramePushed = new();
   public readonly MessageEvent<Frame> FramePopped = new();
   public readonly MessageEvent<Exception> UnhandledException = new();
   public readonly MessageEvent<string> DebugTrace = new();
   public readonly MessageEvent<(int, Operation)> BreakpointHit = new();

   public override void Initialize()
   {
      Cancelled = false;
      Type = "";
      Value = "";
      ExceptionIndex = nil;
      Status = ("", UiActionType.Failure);
      Exception = nil;
      parameters.Context.Reset();
   }

   public override void DoWork()
   {
      try
      {
         var compiler = new Compiler(parameters.Source, parameters.Configuration, parameters.Context);
         Compiler.ActivateWith(() => compiler);
         var _machine = compiler.Generate();
         if (_machine is (true, var machine))
         {
            machine.TraceOutput.Handler = s => TraceOutput.Invoke(s);
            machine.OperationStarting.Handler = tuple => OperationStarting.Invoke(tuple);
            machine.OperationFinished.Handler = tuple => OperationFinished.Invoke(tuple);
            machine.FramePushed.Handler = frame => FramePushed.Invoke(frame);
            machine.FramePopped.Handler = frame => FramePopped.Invoke(frame);
            machine.UnhandledException.Handler = exception => UnhandledException.Invoke(exception);
            machine.DebugTrace.Handler = s => DebugTrace.Invoke(s);
            machine.BreakpointHit.Handler = tuple => BreakpointHit.Invoke(tuple);
            machine.PackageFolder = parameters.PackageFolder;

            Value = "not executed";
            Type = "";
            Cancelled = false;

            if (parameters.Execute)
            {
               var _result = machine.Execute();
               if (_result is (true, var result))
               {
                  Cancelled = parameters.Context.Cancelled();
                  parameters.Context.Reset();
                  Value = result.Image;
                  Type = result.ClassName;
               }
               else
               {
                  ExceptionIndex = compiler.ExceptionIndex;

                  Value = "exception";
                  Type = "";
                  Status = (message: _result.Exception.Message, type: UiActionType.Failure);
               }
            }
         }
      }
      catch (Exception exception)
      {
         Exception = exception;
      }
   }
}