
using System;
using Oculus.Interaction;
using PurrNet;
using UnityEngine;

public class TransferOwnership : NetworkIdentity
{
    public bool UseGravity;
    private Grabbable grabbable;
    private Rigidbody myRigidbody;
    private bool isBeingHeld = false;
    //private NetworkIdentity network;
    
    private void Awake()
    {
        
    
        grabbable = GetComponentInChildren<Grabbable>();
    
        if (grabbable == null)
        {
            throw new InvalidOperationException("Object requires a Grabbable component");
        }
    
        grabbable.WhenPointerEventRaised += OnPointerEventRaised;

        //_transferOwnership = this.GetInterfaceComponent<ITransferOwnership>();
        //if (_transferOwnership == null)
        //{
        //    throw new InvalidOperationException("Object requires an ITransferOwnership component");
        //}

        if (!UseGravity)
        {
            return;
        }
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            throw new InvalidOperationException("Object requires a Rigidbody component when useGravity enabled");
        }
    }
    
    private void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= OnPointerEventRaised;
        }
    }
    
    private void OnPointerEventRaised(PointerEvent pointerEvent)
    {
        isBeingHeld = false;

        if (grabbable == null || pointerEvent.Type != PointerEventType.Select)
        {
            return;
        }

        if (grabbable.SelectingPointsCount == 1 && !isBeingHeld)
        {
            if (owner.HasValue && !isController)
            {
                isBeingHeld = true;
                GiveOwnership(localPlayer);
            }
        }
    }

    private void LateUpdate()
    {
        if (owner.HasValue && isController && UseGravity)
        {
            // When network objects transferring ownership during interactions from ISDK, we need to guarantee a proper
            // kinematic state. We recommend developers to use RigidbodyKinematicLocker for other custom isKinematic controls.
            myRigidbody.isKinematic = myRigidbody.IsLocked();
        }
    }

}

