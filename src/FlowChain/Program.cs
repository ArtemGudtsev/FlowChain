using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowChain
{
    static class Say
    {
        public static readonly Action HoldOnPlease = () => Msg("Press any key to continue");
        public static readonly Action<string> SubChainCalling = (subChainTitle) 
            => Msg($"### Calling \"{subChainTitle}\" subchain...");
        public static readonly Action<string> SubChainCompleted = (subChainTitle) 
            => Msg($"### Subchain \"{subChainTitle}\" completed.");
        public static readonly Action<string> This = (msg) => Msg(msg);

        private static void Msg(string msg) => Console.WriteLine(msg);
    }

    class Chain
    {
        protected Chain() => _steps = new List<Action>();

        protected List<Action> _steps { get; set; }

        public string ChainTitle { get; set; }

        public static Chain Begin() => Begin(string.Empty);
        public static Chain Begin(string title) => new Chain() { ChainTitle = title };

        public Chain AddAction(Action action)
        {
            _steps.Add(action);
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
            _steps.ForEach(step => step());
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
    
    class Program
    {
        static void Main(string[] args) =>
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
                    .Pause()
                .Complete();
    }
}
