using System;
using System.Collections.Generic;
using FlowChain.Log;

namespace FlowChain.Core
{
    public class Chain
    {
        protected Chain() => _steps = new List<ICall>();
        protected List<ICall> _steps { get; set; }

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

            if (titleIsHere) AddAction(() => Logger.SubChainCalling(subChain.ChainTitle));
            _steps.AddRange(subChain._steps);
            if (titleIsHere) AddAction(() => Logger.SubChainCompleted(subChain.ChainTitle));

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
            Logger.HoldOnPlease();
            Console.ReadKey();
        });

        public Chain ShowLine(string line) => AddAction(() => Console.WriteLine(line));
    }
}