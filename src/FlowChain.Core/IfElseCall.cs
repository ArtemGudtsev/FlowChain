using System;

namespace FlowChain.Core
{
    public class IfElseCall : ICall
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
}