using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowChain
{
    internal interface ICall
    {
        void Execute();
    }

    internal class Call : ICall
    {
        public Call(Action step) => Step = step;
        
        protected Action Step { get; set; }

        void ICall.Execute() => Step();
    }

    internal class IfElseCall : ICall
    {
        protected Func<bool> Predicate { get; set; }
        protected Action IfTrue { get; set; }
        protected Action IfFalse { get; set; }

        public IfElseCall(Func<bool> predicate, Action ifTrue, Action ifFalse)
        {
            Predicate = predicate;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        void ICall.Execute()
        {
            if (Predicate())
            {
                IfTrue();
            }
            else
            {
                IfFalse();
            }
        }
    }

    internal static class Say
    {
        public static void SetMsgMethod(Action<string> msg) => Msg = msg;
        public static readonly Action HoldOnPlease = () => Msg("Press any key to continue");
        public static readonly Action<string> SubChainCalling = (subChainTitle) 
            => Msg($"### Calling \"{subChainTitle}\" subchain...");
        public static readonly Action<string> SubChainCompleted = (subChainTitle) 
            => Msg($"### Subchain \"{subChainTitle}\" completed.");
        public static readonly Action<string> This = (msg) => Msg(msg);

        private static Action<string> Msg { get; set; }
    }

    internal class Chain
    {
        protected Chain() => _steps = new List<ICall>();
        protected List<ICall> _steps { get; set; }

        public string ChainTitle { get; set; }
        public static Chain Begin() => Begin(string.Empty);
        public static Chain Begin(string title) => new Chain { ChainTitle = title };
        public Chain ShowLine(string line) => AddAction(() => Console.WriteLine(line));

        public Chain AddAction(Action action)
        {
            _steps.Add(new Call(action));
            return this;
        }

        public Chain AddIfElse(Func<bool> predicate, Action ifTrue, Action ifFalse)
        {
            _steps.Add(new IfElseCall(predicate, ifTrue, ifFalse));
            return this;
        }

        public Chain SubChain(Chain subChain)
        {
            var titleIsHere = !string.IsNullOrEmpty(subChain.ChainTitle);

            if (titleIsHere) AddAction(() => Say.SubChainCalling(subChain.ChainTitle));
            _steps.AddRange(subChain._steps);
            if (titleIsHere) AddAction(() => Say.SubChainCompleted(subChain.ChainTitle));

            return this;
        }

        public void Complete()
        {
            _steps.ForEach(call => call.Execute());
            _steps = null;
            Environment.ExitCode = 0;
        }

        public Chain Pause() => AddAction(() =>
        {
            Say.HoldOnPlease();
            Console.ReadKey();
        });
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Say.SetMsgMethod(msg => Console.WriteLine(msg));
            Chain
                .Begin()
                .ShowLine("I really miss you")
                .ShowLine("turbo pascal")
                .ShowLine(":,(")
                .SubChain(
                    Chain
                        .Begin("Post scriptum")
                        .ShowLine("But please keep being history!")
                )
                .AddIfElse(
                    () => true,
                    () => Say.This("That's all"),
                    () => Say.This("To be continued"))
                .Pause()
                .Complete();
        }
    }
}
