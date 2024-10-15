using System;

namespace EscapeTheVoid.Core
{
    public interface IGameManager
    {
        void StartGame();
        void RestartGame();
        void PauseGame();
        void ResumeGame();
        void QuitGame();
    }
}
