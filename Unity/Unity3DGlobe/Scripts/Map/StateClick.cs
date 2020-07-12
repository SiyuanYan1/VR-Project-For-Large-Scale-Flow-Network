using Arcs;
using UnityEngine;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

namespace Map
{
    public class StateClick : MonoBehaviour
    {
        [Tooltip("Where to send state toggle messages")]
        public ArcDrawer arcDrawer;

        // Start is called before the first frame update
        void Start()
        {
            foreach (Hand hand in Player.instance.hands)
            {
                var lp = hand.GetComponent<SteamVR_LaserPointer>();
                if (lp)
                {
                    lp.PointerIn += OnPointerIn;
                    lp.PointerOut += OnPointerOut;
                    lp.PointerClick += OnPointerClick;
                }
            }
        }

        // subscribed to LaserPointer PointerIn event, used to start highlight
        private void OnPointerIn(object sender, PointerEventArgs e)
        {
            StateColor stateColor = e.target.GetComponent<StateColor>();
            if (stateColor)
            {
                stateColor.HoverStart();
            }
        }

        // subscribed to LaserPointer PointerOut event, used to end highlight
        private void OnPointerOut(object sender, PointerEventArgs e)
        {
            StateColor stateColor = e.target.GetComponent<StateColor>();
            if (stateColor)
            {
                stateColor.HoverStop();
            }
        }

        // subscribed to LaserPointer PointClick event, for 'ray' collision with an object
        private void OnPointerClick(object sender, PointerEventArgs e)
        {
            //clear path
            //var path = GameObject.FindGameObjectsWithTag("arcPath");
            //foreach (GameObject p in path)
            //{
            //    Destroy(p);
            //}
           
            // get the collided object
            GameObject obj = e.target.gameObject;
            Debug.Log(obj.name);

            // check if the collided object is a state, and flip its status if it is
            StateColor stateColor = obj.GetComponent<StateColor>();
            if (stateColor)
            {
                stateColor.StateToggle();
                if (arcDrawer)
                { 
                    arcDrawer.ClearPath();
                    arcDrawer.ToggleState(obj.name);
                }
                //UpdateArcs();
            }
        }
    }
}
