using System;

namespace FlowChain.Core
{
    public class Call : ICall
    {
        public Call(Action step) => Step = step;

        protected Action Step { get; set; }

        void ICall.Execute() => Step();
    }
}