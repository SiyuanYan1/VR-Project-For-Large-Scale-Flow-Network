using Arcs;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class GrabScaler : MonoBehaviour
{
    [Tooltip("Whether the object moves relative to both hands, as well as scales")]
    public bool CanMove = true;
    
    [Tooltip("If Can Move is enabled, whether the object will maintain original rotation when moved")]
    public bool LockOrientation = true;

    // this object will be cloned so that two hands can grab it (one for original and one for clone)
    [Tooltip("This object should be a child and contain a Collider component and an Interactable component")]
    public Interactable PrimaryCollider;

    // the number of hands currently attached
    private int HandsAttached = 0;

    // values remembered when grabbing occurs
    private Vector3 PrevForward;
    private float PrevDistance = 1f;

    // the Hands that are attached
    private List<Hand> attachedHands;

    // references to the two GameObjects created during GrabScaler operations
    private GameObject rotationPointer;
    private GameObject scalePointer;

    private void Awake()
    {
        attachedHands = new List<Hand>();

        // create a clone of the collider + interactable
        GameObject clone = Instantiate(PrimaryCollider.gameObject);
        clone.name = "SecondaryCollider";

        clone.transform.SetParent(PrimaryCollider.transform.parent, false);

        // if the object is being rendered, disable it on the clone so it doesn't double render
        var renderer = clone.GetComponent<Renderer>();
        if (renderer) renderer.enabled = false;

        PrimaryCollider.gameObject.AddComponent<GrabScalerAux>().SetParentScaler(this);
        clone.gameObject.AddComponent<GrabScalerAux>().SetParentScaler(this);
    }

    // attach a hand - a reference object may be created
    public void AttachHand(Hand hand)
    {
        // turn off laser if applicable
        var lp = hand.GetComponent<SteamVR_LaserPointer>();
        if (lp)
        {
            lp.holder.SetActive(false);
            lp.enabled = false;
        }

        HandsAttached++;
        attachedHands.Add(hand);
        // one hand - on-the-spot rotation functionality
        if (HandsAttached == 1)
        {
            // create a new reference objet 
            rotationPointer = new GameObject("Rotation Reference (Temp)");
            var new_forward = attachedHands[0].transform.position - transform.position;

            // this forces rotation to only be around the y-axis (which is more natural to use)
            new_forward.y = 0;

            rotationPointer.transform.forward = new_forward;
            rotationPointer.transform.parent = transform;
            rotationPointer.transform.localPosition = new Vector3(0, 0, 0);
            rotationPointer.transform.parent = transform.parent;
            transform.parent = rotationPointer.transform;
        }
        
        // two hands - scaling and perhaps spatial movement functionality
        if (HandsAttached == 2)
        {
            if (rotationPointer)
            {
                transform.parent = rotationPointer.transform.parent;
                Destroy(rotationPointer);
            }

            // previous object forward direction (used for setting rotation if CanRotateOnMove is disabled)
            PrevForward = transform.forward;

            // the current distance between the two hands - used as a base point for scaling
            PrevDistance = Vector3.Distance(hand.transform.position, hand.otherHand.transform.position);
            // safety check (in case PrevDistance is somehow 0f, i.e. the hands are perfectly overlapping)
            // to prevent div by 0 error later
            if (PrevDistance == 0f)
            {
                PrevDistance = 0.01f;
            }

            // create a new GameObject to serve as the new parent for the scaling object
            scalePointer = new GameObject("Grabber Reference (Temp)");

            // the object is placed between the two hands and faces the average direction of them
            scalePointer.transform.localPosition = 0.5f * (attachedHands[0].transform.position + attachedHands[1].transform.position);
            scalePointer.transform.forward = attachedHands[0].transform.forward + attachedHands[1].transform.forward;

            scalePointer.transform.parent = transform.parent;
            transform.parent = scalePointer.transform;
            Debug.Log(transform.localPosition);
        }
    }

    // detach a hand, which may require cleanup
    public void DetachHand(Hand hand)
    {
        // remove the rotation reference object if it exists
        if (HandsAttached == 1 && rotationPointer)
        {
            transform.parent = rotationPointer.transform.parent;
            Destroy(rotationPointer);
        }

        if (HandsAttached == 2)
        {
            // having fulfilled its purpose, the temp pointer object is destroyed and the object's original parent is restored
            transform.parent = scalePointer.transform.parent;
            Destroy(scalePointer);

            // redraw Arcs (they may have to be rescaled)
            var arcDrawer = GetComponent<ArcDrawer>();
            if (arcDrawer)
            {
                arcDrawer.UpdateArcs();
            }
        }
        HandsAttached--;
        attachedHands.Remove(hand);

        // re-enable laser if applicable
        var lp = hand.GetComponent<SteamVR_LaserPointer>();
        if (lp)
        {
            lp.enabled = true;
            lp.holder.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // single hand - rotation functionality
        if (HandsAttached == 1 && rotationPointer)
        {
            var new_forward = attachedHands[0].transform.position - transform.position;
            new_forward.y = 0;
            rotationPointer.transform.forward = new_forward;   
        }

        // two hands - scaling and movement functionality
        else if (HandsAttached == 2)
        {
            // find the new scale multiplier, which is based on the ratio of distance between the two hands now compared to when scaling started
            float NewMagnitudeRatio = Vector3.Distance(attachedHands[0].transform.position, attachedHands[1].transform.position) / PrevDistance;

            // adjusting the reference object will also change its children (in this case, the object we want scaled)
            // since the child is scaled relative to the pointer, it will scale away from the hands (i.e. its position will be adjusted)
            // this prevents the object from blowing up into the viewer's face
            scalePointer.transform.localScale = new Vector3(NewMagnitudeRatio, NewMagnitudeRatio, NewMagnitudeRatio);

            // additional two-handed movement and rotation functionality
            if (CanMove)
            {
                scalePointer.transform.localPosition = 0.5f * (attachedHands[0].transform.position + attachedHands[1].transform.position);

                // set rotation
                scalePointer.transform.forward = attachedHands[0].transform.forward + attachedHands[1].transform.forward;

                // reset the specific rotation of object if that is desired (this is so the globe's position is still rotated around the hands)
                if (LockOrientation)
                {
                    transform.forward = PrevForward;
                }
            }
        }
    }
}

// this is a component added to the children of the scaling object to allow two hands to grab the same object
public class GrabScalerAux : MonoBehaviour
{
    private GrabScaler scalerParent;
    private Interactable interactable;
    private bool attached = false;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void SetParentScaler(GrabScaler grabScaler)
    {
        scalerParent = grabScaler;
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        GrabTypes endingGrabType = hand.GetGrabEnding();

        // attach action taken
        if (!attached && startingGrabType != GrabTypes.None)
        {
            hand.HoverLock(interactable);
            scalerParent.AttachHand(hand);
            attached = true;
        }

        // detach action taken
        else if (attached && endingGrabType != GrabTypes.None)
        {
            scalerParent.DetachHand(hand);
            hand.HoverUnlock(interactable);
            attached = false;
        }


    }
}
