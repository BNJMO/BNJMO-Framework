
namespace BNJMO
{
    public abstract class AbstractGameMode : BBehaviour
    {
        public EGameMode GameModeType { get; protected set; } = EGameMode.NONE;
        public bool IsRunning { get; set; } = false;

        protected override void Awake()
        {
            base.Awake();

            // Define in child class corresponding game mode!
            GameModeType = EGameMode.NONE;
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            BEvents.Inst.GAME_GameStarted += On_GAME_GameStarted;
            BEvents.Inst.GAME_GameEnded += On_GAME_GameEnded;
            BEvents.Inst.GAME_GamePaused += On_GAME_GamePaused;
            BEvents.Inst.GAME_GameUnPaused += On_GAME_GameUnPaused;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (BEvents.IsInstanceSet)
            {
                BEvents.Inst.GAME_GameStarted -= On_GAME_GameStarted;
                BEvents.Inst.GAME_GameEnded -= On_GAME_GameEnded;
                BEvents.Inst.GAME_GamePaused -= On_GAME_GamePaused;
                BEvents.Inst.GAME_GameUnPaused -= On_GAME_GameUnPaused;
            }
        }

        protected virtual void On_GAME_GameStarted(AbstractGameMode gameMode)
        {
            if (ARE_ENUMS_EQUAL(GameModeType, gameMode.GameModeType))
            {
                IsRunning = true;
            }
        }

        protected virtual void On_GAME_GameEnded(AbstractGameMode gameMode, bool wasAborted)
        {
            if (ARE_ENUMS_EQUAL(GameModeType, gameMode.GameModeType))
            {
                IsRunning = false;
            }
        }

        protected virtual void On_GAME_GamePaused(AbstractGameMode gameMode)
        {
            if (ARE_ENUMS_EQUAL(GameModeType, gameMode.GameModeType))
            {
                IsRunning = false;
            }
        }


        protected virtual void On_GAME_GameUnPaused(AbstractGameMode gameMode)
        {
            if (ARE_ENUMS_EQUAL(GameModeType, gameMode.GameModeType))
            {
                IsRunning = true;
            }
        }
    }
}
