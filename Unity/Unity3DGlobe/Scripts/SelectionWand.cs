using Arcs;
using Facilities;
using UnityEngine;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Throwable))]
public class SelectionWand : MonoBehaviour
{
    [Tooltip("Whether this Wand will mark nodes as Origin or Destination nodes")]
    public WandTypes WandType;

    [Tooltip("The material the wand will apply to selected Facility nodes")]
    public Material HighlightMaterial;

    [Tooltip("Where the location data will be sent to display")]
    public LocInfoDisplay locationInfo;

    [Tooltip("Where node data will be sent for Arc purposes")]
    public ShowArcInfo arcInfo;

    // the currently selected node
    private Facility currentlySelected = null;

    // whether or not the wand is currently being grabbed
    private bool grabbed = false;

    // when the wand is grabbed, remove the state selection laser pointer
    protected virtual void OnAttachedToHand(Hand hand)
    {
        grabbed = true;
        var lp = hand.GetComponent<SteamVR_LaserPointer>();
        if (lp)
        {
            lp.holder.SetActive(false);
            lp.enabled = false;
        }
    }

    // reinstate the state selection laser pointer upon wand release
    protected virtual void OnDetachedFromHand(Hand hand)
    {
        grabbed = false;
        var lp = hand.GetComponent<SteamVR_LaserPointer>();
        if (lp)
        {
            lp.enabled = true;
            lp.holder.SetActive(true);
        }
    }

    // collision checker
    private void OnTriggerEnter(Collider other)
    {
        if (grabbed)
        {
            // check if the collided object is a Facility (ie has Facility component)
            Facility facInfo = other.GetComponent<Facility>();
            //Debug.Log(facInfo.GetFac().SLIC);
            if (facInfo)
            {
                // unhighlight the previously selected point
                if (currentlySelected)
                {
                    currentlySelected.GetComponent<Renderer>().material = Facility.PointMaterial;
                }

                // highlight the new selected point
                other.GetComponent<Renderer>().material = HighlightMaterial;
                currentlySelected = facInfo;

                // tell the data displayers that there has been an update
                if (locationInfo)
                {
                    locationInfo.UpdateLoc(facInfo.GetFac());
                }

                if (arcInfo)
                {
                    if (WandType == WandTypes.Origin)
                    {
                        arcInfo.SetOrigin(facInfo.GetFac());
                    }

                    else if (WandType == WandTypes.Destination)
                    {
                        arcInfo.SetDest(facInfo.GetFac());
                    }
                }

                Debug.Log("Selection is successful");
            }
            else
            {
                Debug.Log("Selection is unsuccessful");
            }
        }
        else
        {
            Debug.Log("not selecting a node");
        }
    }
}

public enum WandTypes
{
    Origin,
    Destination
}
