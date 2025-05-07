using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class NetworkConnectionsUIManager : BBehaviour
{
    [SerializeField] private BFrame bFrame;
    [SerializeField] private BMenu notConnectedMenu;
    [SerializeField] private BMenu hostMenu;
    [SerializeField] private BMenu clientMenu;
    [SerializeField] private BButton connectToServerButton;
    [SerializeField] private BText serversCountText;
    [SerializeField] private BText dispatcherText;
    [SerializeField] private BButton dispatcherButton;
    
    //private float connectionTimeoutInterval = 5.0f;
    //private IEnumerator connctionTimeoutEnumerator;


    public void StartHost()
    {
        BEventManager.Inst.StartHost();
    }

    public void ConnectToFoundHost()
    {
        BEventManager.Inst.ConnectToHost(0);
    }

    public void Disconnect()
    {
        BEventManager.Inst.Disconnect();
    }

    public void Ping()
    {
        BEventManager.Inst.CalculatePing(ENetworkID.HOST_1);
    }

    #region Life Cycle
    protected override void OnEnable()
    {
        base.OnEnable();

        BEvents.NETWORK_NewBEventDispatcherSet.Event += On_NETWORK_NewBEventDispatcherSet;
        BEvents.NETWORK_NetworkStateUpdated.Event += On_NETWORK_NetworkStateUpdated;
        BEvents.NETWORK_ConnectedToHost.Event += On_NETWORK_ConnectedToHost;
        BEvents.NETWORK_ConnectionStoped.Event += On_NETWORK_ConnectionStoped;
        BEvents.NETWORK_StartedHost.Event += On_NETWORK_StartedHost;
        BEvents.NETWORK_NewNetworkIDConnected.Event += On_NETWORK_NewNetworkIDConnected;
        BEvents.NETWORK_NetworkIDDisconnected.Event += On_NETWORK_NewNetworkIDDisconnected;
        BEvents.NETWORK_DiscoveredHostsUpdated.Event += On_NETWORK_DiscoveredHostsUpdated;

        if (IS_NOT_NULL(dispatcherButton))
        {
            dispatcherButton.Released += On_DispatcherButton_ButtonReleased;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BEvents.NETWORK_NewBEventDispatcherSet.Event -= On_NETWORK_NewBEventDispatcherSet;
        BEvents.NETWORK_NetworkStateUpdated.Event -= On_NETWORK_NetworkStateUpdated;
        BEvents.NETWORK_ConnectedToHost.Event -= On_NETWORK_ConnectedToHost;
        BEvents.NETWORK_ConnectionStoped.Event -= On_NETWORK_ConnectionStoped;
        BEvents.NETWORK_StartedHost.Event -= On_NETWORK_StartedHost;
        BEvents.NETWORK_NewNetworkIDConnected.Event -= On_NETWORK_NewNetworkIDConnected;
        BEvents.NETWORK_NetworkIDDisconnected.Event -= On_NETWORK_NewNetworkIDDisconnected;
        BEvents.NETWORK_DiscoveredHostsUpdated.Event -= On_NETWORK_DiscoveredHostsUpdated;

        if (dispatcherButton)
        {
            dispatcherButton.Released -= On_DispatcherButton_ButtonReleased;
        }

    }

    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        base.OnValidate();
        
        if (bFrame == null)
        {
            bFrame = GetComponent<BFrame>();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (IS_NOT_NULL(serversCountText))
        {
            serversCountText.SetText("No Servers found");
        }

        if (IS_NOT_NULL(notConnectedMenu))
        {
            connectToServerButton.DisableButton();
        }
    }

    protected override void Start()
    {
        base.Start();

        // MF hack (7chouma)
        On_NETWORK_NewBEventDispatcherSet(new BEHandle<AbstractBEventDispatcher>(BEventManager.Inst.BEventDispatcher));
    }

    #endregion

    #region Events Callbacks
    private void On_NETWORK_NewBEventDispatcherSet(BEHandle<AbstractBEventDispatcher> handle)
    {
        if (IS_NOT_NULL(dispatcherText))
        {
            dispatcherText.SetText(handle.Arg1.GetBEventDispatcherType().ToString());
        }

        if (IS_NOT_NULL(dispatcherButton))
        {
            switch (handle.Arg1.GetBEventDispatcherType())
            {
                case BEventDispatcherType.MIRROR:
                    dispatcherButton.SetButtonText("Switch Ubii");
                    break;

                default:
                    dispatcherButton.SetButtonText("Switch Mirror");
                    break;
            }
        }
    }

    private void On_NETWORK_NetworkStateUpdated(BEHandle<ENetworkState> handle)
    {
        //if (IS_NOT_NULL(bFrame)
        //    && (IS_NOT_NULL(notConnectedMenu))
        //    && (IS_NOT_NULL(hostMenu))
        //    && (IS_NOT_NULL(clientMenu)))
        //{
        //    switch (handle.Arg1)
        //    {
        //        case ENetworkState.NOT_CONNECTED:
        //            bFrame.UpdateCurrentBMenu(notConnectedMenu);
        //            break;

        //        case ENetworkState.HOST:
        //            bFrame.UpdateCurrentBMenu(hostMenu);
        //            break;

        //        case ENetworkState.CLIENT:
        //            bFrame.UpdateCurrentBMenu(clientMenu);
        //            break;
        //    }
        //}
    }

    private void On_NETWORK_StartedHost(BEHandle handle)
    {
        LogNotification("Started new Host.");

        // Stop looking for hosts on the netwrok
        //StopCoroutine(connctionTimeoutEnumerator);
    }

    private void On_NETWORK_ConnectionStoped(BEHandle obj)
    {
        LogNotification("The connection was stopped");
    }

    private void On_NETWORK_ConnectedToHost(BEHandle<ENetworkID> handle)
    {
        LogNotification("Connected to host as : " + handle.Arg1);

        // Stop looking for hosts on the netwrok
        //StopCoroutine(connctionTimeoutEnumerator);
    }

    private void On_NETWORK_NewNetworkIDConnected(BEHandle<ENetworkID> handle)
    {
        LogNotification("New client connected : " + handle.Arg1);
    }

    private void On_NETWORK_NewNetworkIDDisconnected(BEHandle<ENetworkID> handle)
    {
        LogNotification("Client disconnected : " + handle.Arg1);
    }


    private void On_NETWORK_DiscoveredHostsUpdated(BEHandle<string[]> handle)
    {
        int hostsCount = handle.Arg1.Length;
        LogNotification("Discovered hosts list updated : " + hostsCount);

        if (hostsCount == 0)
        {
            serversCountText.SetText("No Hosts found.");
            connectToServerButton.DisableButton();
            //connectToServerButton.CanBeShown = false;
        }
        else
        {
            serversCountText.SetText("Hosts : " + hostsCount);
            //connectToServerButton.CanBeShown = true;
            connectToServerButton.EnableButton();
        }
    }


    private void On_DispatcherButton_ButtonReleased(BButton arg1, bool arg2)
    {
        switch (BEventManager.Inst.BEventDispatcher.GetBEventDispatcherType())
        {
            case BEventDispatcherType.MIRROR:
                BEventManager.Inst.SetBEventDispatcher(BEventDispatcherType.UBI_INTERACT);
                break;

            default:
                BEventManager.Inst.SetBEventDispatcher(BEventDispatcherType.MIRROR);
                break;
        }
    }
    
    #endregion

    #region other

    // TODO : Move this to the new global OnHostLost event
    //private IEnumerator ConnectionTimeoutCoroutine()
    //{
    //    yield return new WaitForSeconds(connectionTimeoutInterval);

    //    MirrorNetworkConnectionsListener.Instance.FindServers();
    //    serversCountText.SetText("No Servers found");
    //    connectToServerButton.DisableButton();
    //    //connectToServerButton.CanBeShown = false;
    //}
    #endregion
}
