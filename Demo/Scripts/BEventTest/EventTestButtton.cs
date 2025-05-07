using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class EventTestButtton : BBehaviour
{

    #region Public Methods
    public void OnIncrementToAll()
    {
        LogConsole("OnIncrementToAll : <color=red>[" + BUtils.GetTimeAsString() + "] </color>");
        BEvents.TEST_CounterIncrement.Invoke(new BEHandle<int>(counter), BEventReplicationType.TO_ALL, true);
    }
    
    public void OnIncrementLocal()
    {
        BEvents.TEST_CounterIncrement.Invoke(new BEHandle<int>(counter), BEventReplicationType.LOCAL, true);
    }
    
    public void OnIncrementToHost()
    {
        BEvents.TEST_CounterIncrement.Invoke(new BEHandle<int>(counter), BEventReplicationType.TO_TARGET, true, ENetworkID.HOST_1);
    }

    public void OnIncrementToTarget()
    {
        BEvents.TEST_CounterIncrement.Invoke(new BEHandle<int>(counter), BEventReplicationType.TO_TARGET, true, ENetworkID.CLIENT_2);
    }

    /* Benchmark */
    public void OnFloatTest()
    {
        string serialized = BUtils.SerializeObject(new BEHandle<int>(counter));
        LogConsole(serialized);
        //BUtils.DeserializeObject<BAnchorInformation>(serialized);
        
        BEvents.TEST_FloatTest.Invoke(new BEHandle<float>(BUtils.GetTimeAsInt()), BEventReplicationType.TO_ALL, true);
    }

    public void OnVector3Test()
    {
        BEvents.TEST_Vector3Test.Invoke(new BEHandle<Vector3>(new Vector3(5.0450f, -3.24533f, 704.7499f)), BEventReplicationType.TO_ALL, true);
    }

    public void OnNativeIntTest()
    {
        AbstractBEventDispatcher bEventDispatcher = BEventManager.Inst.BEventDispatcher;
        if (IS_NOT_NULL(bEventDispatcher))
        {
            switch (bEventDispatcher.GetBEventDispatcherType())
            {

   
            }
        }
    }
    #endregion

    #region Inspector Variables
    [SerializeField] 
    private BText counterBText;

    [SerializeField] 
    private BText networkIDText;
    #endregion

    #region Private Variables
    private int counter = 0;

    #endregion

    #region Life Cycle
    protected override void OnEnable()
    {
        base.OnEnable();

        BEvents.NETWORK_NetworkStateUpdated += On_NETWORK_NetworkStateUpdated;
        BEvents.TEST_CounterIncrement += On_TEST_CounterIncrement;
        BEvents.TEST_FloatTest += On_TEST_FloatTest;
        BEvents.TEST_Vector3Test += On_TEST_Vector3Test;

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BEvents.NETWORK_NetworkStateUpdated -= On_NETWORK_NetworkStateUpdated;
        BEvents.TEST_CounterIncrement -= On_TEST_CounterIncrement;
        BEvents.TEST_FloatTest -= On_TEST_FloatTest;
        BEvents.TEST_Vector3Test -= On_TEST_Vector3Test;
    }
    #endregion

    #region Events Callbacks
    private void On_NETWORK_NetworkStateUpdated(BEHandle<ENetworkState> handle)
    {
        if (networkIDText)
        {
            networkIDText.SetText("NetworkID : " + BEventManager.Inst.LocalNetworkID);
        }
    }

    private void On_TEST_CounterIncrement(BEHandle<int> handle)
    {
        counter = handle.Arg1 + 1;

        if (IS_NOT_NULL(counterBText))
        {
            counterBText.SetText("Counter : " + counter);
        }
    }


    private void On_TEST_FloatTest(BEHandle<float> handle)
    {
        LogConsole("On_TEST_FloatTest : " + BUtils.GetTimeAsString());
    }

    private void On_TEST_Vector3Test(BEHandle<Vector3> handle)
    {
        LogConsole("On_TEST_Vector3Test : " + BUtils.GetTimeAsString());
    }

    #endregion
}
