using System;

namespace FlowChain.Log
{
    public static class Logger
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
}