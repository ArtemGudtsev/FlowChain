using System;
using FlowChain.Core;
using FlowChain.Log;

namespace FlowChain
{
    // Example of FlowChain usage in console tool
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.SetMsgMethod(msg => Console.WriteLine(msg));
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
                    () => Logger.This("That's all"),
                    () => Logger.This("To be continued"))
                .Pause()
                .Complete();
        }
    }
}
