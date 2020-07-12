using Facilities;
using UnityEngine;

namespace Arcs
{
    public class ShowFullPath : MonoBehaviour
    {
        public DataVisualizer Visualizer;
        private FacData prevOrigin = null;
        private FacData prevDest = null;

        public void DrawPath(FacData originFac, FacData destFac)
        {
            bool update = false;
            if (prevOrigin != originFac || prevDest != destFac)
            {
                if (prevOrigin != originFac)
                {
                    prevOrigin = originFac;
                }
                if (prevDest != destFac)
                {
                    prevDest = destFac;
                }
                if (originFac != null && prevDest != null)
                {
                    update = true;
                }
            }
            //it only one of the wand changes the target will create new arcs
            if (update)
            {
                //var existed = GameObject.FindGameObjectsWithTag("Arc");
                ////Debug.Log(existed.Length);
                //foreach (GameObject arc in existed)
                //{
                //    arc.SetActive(false);
                //}
                //Debug.Log("origin selects " + originFac.SLIC);
                //Debug.Log("dest selects " + destFac.SLIC);
                Visualizer.DrawPath(originFac.SLIC, destFac.SLIC);
            }
        }
    }
}
