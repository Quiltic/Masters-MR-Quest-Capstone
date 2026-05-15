using LearnXR.Core.Utilities;
using Oculus.Interaction;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ResizeMe : MonoBehaviour
{

    public float minimumSize = 0.1f; // the minimum size we want the object to be. Defaults to the creation size.
    private float startingDistance; // What the starting distance between controllers prior to resizing
    private Vector3 startingSizeOfResize; // What the size of the gameobject is prior to resizing
    public float resizeSpeed = 2.0f;
    private Grabbable grabbable;
    private int isBeingHeld = 0; // number of hands holding the thing
    //public bool UseGravity;
    //private Rigidbody myRigidbody;

    void Awake()
    {
        minimumSize = transform.localScale.x;

        grabbable = GetComponentInChildren<Grabbable>();

        if (grabbable == null)
        {
            throw new InvalidOperationException("Object requires a Grabbable component");
        }

        grabbable.WhenPointerEventRaised += OnPointerEventRaised;
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
        if (isBeingHeld == 2 && pointerEvent.Type == PointerEventType.Move) {
            //print("Resizing");
            resize();
            return;
        }
        //print("Something is Happening");
        if (grabbable == null || pointerEvent.Type == PointerEventType.Unselect)
        {
            isBeingHeld--;
            return;
        }
        

        if (pointerEvent.Type == PointerEventType.Select)
        {
            isBeingHeld++;
            if (grabbable.SelectingPointsCount == 2)
            {
                startingDistance = getControlerDistances();
                startingSizeOfResize = transform.localScale;
            }
            return;
        }
        //print($"Pointer Type: {pointerEvent.Type}");
        //print($"PointerEvent: {pointerEvent}");


        //if (isBeingHeld == 2) { 
        //    print("Hello! Im Being Double Grabbed!");
        //}

        //if (grabbable.SelectingPointsCount == 2 && isBeingHeld != 0)
        //{
        //    print($"Grab amount: {grabbable.SelectingPointsCount}");
        //    return;
        //}

    }

    void resize()
    {
        float sizeAmount = getControlerDistances();
        sizeAmount -= startingDistance;
        sizeAmount *= resizeSpeed;

        transform.localScale = startingSizeOfResize+ new Vector3(sizeAmount,sizeAmount,sizeAmount);
        
        if (transform.localScale.x < minimumSize) { 
            transform.localScale = new Vector3(minimumSize, minimumSize, minimumSize);
        }

        //print($"ResizeAmount: {sizeAmount} / {transform.localScale}");
    }

    float getControlerDistances()
    {
        Vector3 hand1 = grabbable.GrabPoints[0].position;
        Vector3 hand2 = grabbable.GrabPoints[1].position;

        return (Vector3.Distance(hand1, hand2));
    }


    public void print(string message)
    {
        Debug.Log(message);
        SpatialLogger.Instance.LogInfo(message);
    }
}
