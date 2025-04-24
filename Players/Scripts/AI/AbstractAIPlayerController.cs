using UnityEngine;
using System;

namespace BNJMO
{
    public abstract class AbstractAIPlayerController : BBehaviour
    {

        public event Action<EControllerID, EInputAxis, float, float> AxisUpdated;
        public event Action<EControllerID, EInputButton> ButtonPressed;
        public event Action<EControllerID, EInputButton> ButtonReleased;

        public event Action<AbstractAIPlayerController> WillGetDestroyed;

        public EControllerID controllerID { get; private set; } = EControllerID.NONE;
        public EPlayerID playerID { get; private set; } = EPlayerID.NONE;

        private AbstractPawn myPawn;
        //private Arena arena;
        private AIInputSource aIInputSource;

        private Vector2 currentRotation;
        private Vector2 currentMovement;

        //private Vector2 currentArenaPosition;
        //private EAIArenaZone currentArenaZone;
        //private float currentArenaDistanceToCenter;
        //private AbstractPlayer currentClosestPlayer;
        //private float currentArenaDistanceToClosestPlayer;
        //private AbstractSpell currentClosestSpell;
        //private float currentArenaDistanceToClosestSpell;
        //private float currentDotToSpellAndSpellDirection = 0.0f;


        //private Vector2 arenaCenterAttraction;
        //private Vector2 closestPlayerDeviation;
        //private Vector2 closestSpellDeviation;
        //private Vector2 pushDeviation;

        //private float currentArenaPositionInfluence;
        //private float currentClosestPlayerInfluence;
        //private float currentClosestSpellInfluence;
        //private float currentPushInfluence;

        //private IEnumerator StartSpammingAllSpellButtonsEnumerator;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            myPawn = GetComponentWithCheck<AbstractPawn>();
        }

        //protected override void InitializeObjecsInScene()
        //{
        //    base.InitializeObjecsInScene();

        //    arena = FindObjectOfType<Arena>();
        //    IS_NOT_NULL(arena);
        //}

        protected override void Start()
        {
            base.Start();

            playerID = myPawn.PlayerID;
            controllerID = PlayerManager.Instance.GetAssignedControllerID(playerID);
            IS_NOT_NONE(playerID);
            IS_NOT_NONE(controllerID);

            //StartNewCoroutine(ref StartSpammingAllSpellButtonsEnumerator, StartSpammingAllSpellButtonsCoroutine());
        }

        //protected override void Update()
        //{
        //    base.Update();

        //    if ((IS_NOT_NULL(aIInputSource))
        //         && (IS_NOT_NULL(arena)))
        //    {
        //        UpdateArenaCenterInfluence();
        //        UpdateClosestPlayerInfluence();
        //        UpdateClosestSpellInfluence();
        //        UpdatePushSpellInfluence();

        //        // Debug
        //        DrawDebugArrow(myPlayer.Position, Get3DVector(arenaCenterAttraction) * 2.0f, Color.green);
        //        DrawDebugArrow(myPlayer.Position, Get3DVector(closestPlayerDeviation) * 2.0f, Color.red);
        //        DrawDebugArrow(myPlayer.Position, Get3DVector(closestSpellDeviation) * 2.0f, Color.yellow);
        //        DrawDebugArrow(myPlayer.Position, Get3DVector(pushDeviation) * 2.0f, Color.blue);
        //        LogCanvas(6, playerID + " - arena Position Influence : " + currentArenaPositionInfluence);
        //        LogCanvas(6, playerID + " - closest Player Influence : " + currentClosestPlayerInfluence);
        //        LogCanvas(6, playerID + " - closest Spell Influence : " + currentClosestSpellInfluence);
        //        LogCanvas(6, playerID + " - current PushIn Ifluence : " + currentPushInfluence);
        //        LogCanvas(6, playerID + " - dotToSpellAndSpellDirection : " + currentDotToSpellAndSpellDirection);
        //        LogCanvas(6, playerID + " - currentMovement mag : " + currentMovement.magnitude);
        //    }
        //}

        //protected override void FixedUpdate()
        //{
        //    base.FixedUpdate();
        //    if ((IS_NOT_NULL(aIInputSource))
        //        && (IS_NOT_NULL(arena))
        //        && (IS_NOT_NULL(myPlayer)))
        //    {
        //        // Rotate
        //        if (currentClosestPlayer)
        //        {
        //            currentRotation = Vector2.Lerp(currentRotation, -closestPlayerDeviation, Time.fixedDeltaTime * aIInputSource.RotationSmoothSpeed);
        //            InvokeEventIfBound(MoveJoystick, controllerID, EJoystickType.ROTATION, currentRotation.x, -currentRotation.y);
        //        }

        //        // Evaluate movement direction
        //        Vector2 newMovement =
        //            (arenaCenterAttraction * currentArenaPositionInfluence)
        //            + (closestPlayerDeviation * currentClosestPlayerInfluence)
        //            + (closestSpellDeviation * currentClosestSpellInfluence)
        //            + (pushDeviation * currentPushInfluence);

        //        // Clamp movement
        //        if (newMovement.magnitude < aIInputSource.MinMovementThreshold)
        //        {
        //            newMovement = Vector2.zero;
        //        }
        //        else
        //        {
        //            newMovement.Normalize();
        //        }

        //        // Smooth movement transitions
        //        currentMovement = Vector2.Lerp(currentMovement, newMovement, Time.fixedDeltaTime * aIInputSource.MovemeSmoothSpeed);
        //        if (currentMovement.magnitude < aIInputSource.MinSmoothMovementThreshold)
        //        {
        //            currentMovement = Vector2.zero;
        //        }

        //        InvokeEventIfBound(MoveJoystick, controllerID, EJoystickType.MOVEMENT, currentMovement.x, currentMovement.y);


        //        switch (currentArenaZone)
        //        {
        //            case EAIArenaZone.SAFE:

        //                break;

        //            case EAIArenaZone.NORMAL:

        //                break;

        //            case EAIArenaZone.DANGER:

        //                break;
        //        }
        //    }
        //}

        //protected override void OnDestroy()
        //{
        //    base.OnDestroy();

        //    InvokeEventIfBound(WillGetDestroyed, this);
        //    StopCoroutine(StartSpammingAllSpellButtonsEnumerator);

        //}

        public void InitializeAIController(AIInputSource aIInputSource)
        {
            this.aIInputSource = aIInputSource;
        }


        //private void UpdateArenaCenterInfluence()
        //{
        //    if (MotherOfManagers.Instance.IsAIArenaCenterAttractionEnabled)
        //    {
        //        // Arena 
        //        currentArenaPosition = arena.GetInArenaPosition(myPlayer.Position);
        //        currentArenaDistanceToCenter = currentArenaPosition.magnitude;
        //        UpdateArenaZone();

        //        // Influence arena position
        //        currentArenaPositionInfluence = aIInputSource.ArenaPositionInfluenceCurve.Evaluate(currentArenaDistanceToCenter);
        //        currentArenaPositionInfluence *= aIInputSource.ArenaPositionInfluenceFactor;

        //        // Attraction and deviation vectors
        //        arenaCenterAttraction = -(currentArenaPosition.normalized);
        //    }
        //}

        //private void UpdateClosestPlayerInfluence()
        //{
        //    if (MotherOfManagers.Instance.IsAIClosestPlayerDeviationEnabled)
        //    {
        //        // Closest player
        //        currentClosestPlayer = GetClosestPlayer(myPlayer);
        //        if (currentClosestPlayer)
        //        {
        //            currentArenaDistanceToClosestPlayer = Vector2.Distance(arena.GetInArenaPosition(myPlayer.Position), arena.GetInArenaPosition(currentClosestPlayer.Position));

        //            // Attraction and deviation vectors
        //            closestPlayerDeviation = (Get2DVector(myPlayer.Position - currentClosestPlayer.Position)).normalized;
        //            // Influnce closest player
        //            currentClosestPlayerInfluence = 1.0f - ((currentArenaDistanceToClosestPlayer - aIInputSource.ClosestEnemyDangerArenaDistanceMaxThreshold) / (aIInputSource.ClosestEnemyDangerArenaDistanceStartThreshold - aIInputSource.ClosestEnemyDangerArenaDistanceMaxThreshold));
        //            currentClosestPlayerInfluence = Mathf.Clamp(currentClosestPlayerInfluence, 0.0f, 1.0f);
        //            currentClosestPlayerInfluence = aIInputSource.ClosestPlayerInfluenceCurve.Evaluate(currentClosestPlayerInfluence);
        //            currentClosestPlayerInfluence *= aIInputSource.ClosestPlayerInfluenceFactor;
        //        }
        //        else
        //        {
        //            currentClosestPlayerInfluence = 0.0f;
        //        }
        //    }
        //}

        //private void UpdateClosestSpellInfluence()
        //{
        //    if (MotherOfManagers.Instance.IsAIClosestSpellDeviationEnabled)
        //    {
        //        // Closest spell
        //        currentClosestSpell = GetClosestSpell();
        //        if (currentClosestSpell)
        //        {
        //            currentArenaDistanceToClosestSpell = Vector2.Distance(arena.GetInArenaPosition(myPlayer.Position), arena.GetInArenaPosition(currentClosestSpell.Position));
        //        }
        //        else
        //        {
        //            currentArenaDistanceToClosestSpell = 0.0f;
        //        }

        //        // Attraction and deviation vectors
        //        if (currentClosestSpell)
        //        {
        //            //Vector3 toSpellDirection = (currentClosestSpell.Position - myPlayer.Position).normalized;
        //            //dotToSpellAndSpellDirection = Vector3.Dot(toSpellDirection, currentClosestSpell.Direction);
        //            //if (dotToSpellAndSpellDirection < 0.0f)
        //            //{
        //            //    dotToSpellAndSpellDirection = Mathf.Abs(dotToSpellAndSpellDirection);
        //            //}

        //            // Strategy : Negative direction 
        //            closestSpellDeviation = (Get2DVector(myPlayer.Position - currentClosestSpell.Position)).normalized;
        //        }
        //        else
        //        {
        //            closestSpellDeviation = Vector2.zero;
        //        }

        //        // Influence closest spell
        //        if (currentClosestSpell)
        //        {
        //            // Strategy : Negative direction 
        //            currentClosestSpellInfluence = 1.0f - ((currentArenaDistanceToClosestSpell - aIInputSource.ClosestSpellDangerArenaDistanceMaxThreshold) / (aIInputSource.ClosestSpellDangerArenaDistanceStartThreshold - aIInputSource.ClosestSpellDangerArenaDistanceMaxThreshold));
        //            currentClosestSpellInfluence = Mathf.Clamp(currentClosestSpellInfluence, 0.0f, 1.0f);
        //            currentClosestSpellInfluence = aIInputSource.ClosestSpellInfluenceCurve.Evaluate(currentClosestSpellInfluence);
        //            currentClosestSpellInfluence *= aIInputSource.ClosestSpellInfluenceFactor;
        //        }
        //        else
        //        {
        //            currentClosestSpellInfluence = 0.0f;
        //        }
        //    }
        //}

        //private void UpdatePushSpellInfluence()
        //{
        //    if (MotherOfManagers.Instance.IsAIPushDeviationEnabled)
        //    {
        //        pushDeviation = -Get2DVector(myPlayer.PushVelocity).normalized;

        //        // Influence push 
        //        currentPushInfluence = aIInputSource.PushInfluenceCurve.Evaluate((myPlayer.PushVelocity.magnitude / myPlayer.MaxPushVelocity));
        //    }
        //}


        //private void UpdateArenaZone()
        //{
        //    if (IS_NOT_NULL(aIInputSource))
        //    {
        //        if (currentArenaDistanceToCenter < aIInputSource.ArenaSafeRadius)
        //        {
        //            currentArenaZone = EAIArenaZone.SAFE;
        //        }
        //        else if ((currentArenaDistanceToCenter >= aIInputSource.ArenaSafeRadius) && (currentArenaDistanceToCenter < aIInputSource.ArenaDangerRadius))
        //        {
        //            currentArenaZone = EAIArenaZone.NORMAL;
        //        }
        //        else
        //        {
        //            currentArenaZone = EAIArenaZone.DANGER;
        //        }
        //    }
        //}

        //private AbstractSpell GetClosestSpell()
        //{
        //    AbstractSpell closestOtherSpell = null;
        //    float closestDistanceToOtherSpell = float.MaxValue;
        //    foreach (AbstractSpell otherSpell in SpellManager.Instance.ActiveMovingSpells)
        //    {
        //        if (otherSpell.CastingPlayerID == playerID)
        //        {
        //            continue;
        //        }

        //        float distanceToOtherSpell = Vector3.Distance(myPlayer.Position, otherSpell.Position);
        //        if (distanceToOtherSpell < closestDistanceToOtherSpell)
        //        {
        //            closestOtherSpell = otherSpell;
        //            closestDistanceToOtherSpell = distanceToOtherSpell;
        //        }
        //    }
        //    return closestOtherSpell;
        //}

        //private IEnumerator StartSpammingAllSpellButtonsCoroutine()
        //{
        //    while (true)
        //    {
        //        if (MotherOfManagers.Instance.IsAISpawnSpellsEneabled)
        //        {
        //            EInputButton spellButton = GetInputButtonFrom(UnityEngine.Random.Range(1, 4));
        //            if (IS_NOT_NONE(spellButton))
        //            {
        //                InvokeEventIfBound(ButtonPressed, controllerID, spellButton);
        //                yield return new WaitForSeconds(0.1f);
        //                InvokeEventIfBound(ButtonReleased, controllerID, spellButton);
        //            }
        //        }
        //        yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.7f));
        //    }
        //}

    }
}