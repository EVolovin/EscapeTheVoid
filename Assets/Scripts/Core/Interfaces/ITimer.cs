using System;

namespace EscapeTheVoid.Core
{
    public interface ITimer
    {
        void StartNewTimer();
        void SetPaused(bool pause);
    }
}
