using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowChain
{
    internal abstract class BaseCall
    {
        public abstract void Execute();
    }

    internal class Call : BaseCall
    {
        public Call(Action step) => Step = step;
        protected Action Step { get; set; }
        public override void Execute() => Step();
    }

    internal class IfElseCall : BaseCall
    {
        public IfElseCall(Func<bool> predicate, Action ifTrue, Action ifFalse)
        {
            Predicate = predicate;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        protected Func<bool> Predicate { get; set; }
        protected Action IfTrue { get; set; }
        protected Action IfFalse { get; set; }

        public override void Execute()
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
        public static readonly Action HoldOnPlease = () => Msg("Press any key to continue");
        public static readonly Action<string> SubChainCalling = (subChainTitle) 
            => Msg($"### Calling \"{subChainTitle}\" subchain...");
        public static readonly Action<string> SubChainCompleted = (subChainTitle) 
            => Msg($"### Subchain \"{subChainTitle}\" completed.");
        public static readonly Action<string> This = (msg) => Msg(msg);

        private static void Msg(string msg) => Console.WriteLine(msg);
    }

    internal class Chain
    {
        protected Chain() => _steps = new List<BaseCall>();

        protected List<BaseCall> _steps { get; set; }

        public string ChainTitle { get; set; }

        public static Chain Begin() => Begin(string.Empty);
        public static Chain Begin(string title) => new Chain { ChainTitle = title };

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

        public Chain ShowLine(string line) => AddAction(() => Console.WriteLine(line));

        public Chain Pause() => AddAction(() =>
        {
            Say.HoldOnPlease();
            Console.ReadKey();
        });
    }

    internal class Program
    {
        private static void Main(string[] args) =>
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
