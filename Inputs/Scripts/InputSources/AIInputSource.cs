using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AIInputSource : AbstractInputSource
    {
        [Header("AI Initialization")]
        [SerializeField] public float MinMovementThreshold = 0.2f;
        [SerializeField] public float MinSmoothMovementThreshold = 0.06f;
        [SerializeField] public float RotationSmoothSpeed = 3.0f;
        [SerializeField] public float MovemeSmoothSpeed = 3.0f;
        [SerializeField] public float ArenaSafeRadius = 0.3f;
        [SerializeField] public float ArenaDangerRadius = 0.65f;

        [Header("AI Distance Thresholds")]
        [SerializeField] public float ClosestEnemyDangerArenaDistanceStartThreshold = 0.8f;
        [SerializeField] public float ClosestEnemyDangerArenaDistanceMaxThreshold = 0.4f;
        [SerializeField] public float ClosestSpellDangerArenaDistanceStartThreshold = 0.6f;
        [SerializeField] public float ClosestSpellDangerArenaDistanceMaxThreshold = 0.17f;

        [Header("AI Influence factors")]
        public float ArenaPositionInfluenceFactor = 1.0f;
        public float ClosestPlayerInfluenceFactor = 1.0f;
        public float ClosestSpellInfluenceFactor = 1.0f;
        public float PushInfluenceFactor = 1.0f;

        [Header("AI Influence Ear Curves")]
        public AnimationCurve ArenaPositionInfluenceCurve;
        public AnimationCurve ClosestPlayerInfluenceCurve;
        public AnimationCurve ClosestSpellInfluenceCurve;
        public AnimationCurve PushInfluenceCurve;

        private List<AbstractAIPlayerController> activeAIControllers = new List<AbstractAIPlayerController>();

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();
            BEvents.PAWNS_PawnSpawned += BEents_OnPawnsPawnSpawned;
        }

        private void BEents_OnPawnsPawnSpawned(BEventHandle<PawnBase> bEventHandle)
        {
            // Add an AIPlayerController on the spawned player if he's an AI
            // EPlayerID playerID = bHandle.Arg1;
            // EControllerID controllerID = PlayerManager.Inst.GetAssignedControllerID(playerID);
            //
            // if ((controllerID.ContainedIn(BConsts.AI_CONTROLLERS))
            //     && (IS_KEY_CONTAINED(PlayerManager.Inst.ActivePlayers, playerID))
            //     && (IS_NOT_NULL(PlayerManager.Inst.ActivePlayers[playerID])))
            // {
            //     AbstractPawn pawn = PlayerManager.Inst.ActivePlayers[playerID];
            //     AbstractAIPlayerController aIPlayerController = pawn.gameObject.AddComponent<AbstractAIPlayerController>();
            //     aIPlayerController.AxisUpdated += On_AIPlayerController_JoystickMoved;
            //     aIPlayerController.ButtonPressed += On_AIPlayerController_ButtonPressed;
            //     aIPlayerController.ButtonReleased += On_AIPlayerController_ButtonReleased;
            //     aIPlayerController.WillGetDestroyed += On_AIPlayerController_WillGetDestroyed;
            //     aIPlayerController.InitializeAIController(this);
            //     activeAIControllers.Add(aIPlayerController);
            // }
        }

        private void On_AIPlayerController_JoystickMoved(EControllerID controllerID, EInputAxis axisInput, float x, float y)
        {
            InvokeAxisUpdated(controllerID, axisInput, x, y);
        }


        private void On_AIPlayerController_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonPressed(controllerID, inputButton);
        }

        private void On_AIPlayerController_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonReleased(controllerID, inputButton);
        }

        private void On_AIPlayerController_WillGetDestroyed(AbstractAIPlayerController aIPlayerController)
        {
            if (aIPlayerController)
            {
                aIPlayerController.AxisUpdated -= On_AIPlayerController_JoystickMoved;
                aIPlayerController.WillGetDestroyed -= On_AIPlayerController_WillGetDestroyed;
                if (IS_VALUE_CONTAINED(activeAIControllers, aIPlayerController))
                {
                    activeAIControllers.Remove(aIPlayerController);
                }
            }
        }
    }
}