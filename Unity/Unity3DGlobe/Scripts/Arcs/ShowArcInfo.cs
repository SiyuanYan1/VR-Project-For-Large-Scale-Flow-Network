using Facilities;
using UnityEngine;
using UnityEngine.UI;

namespace Arcs
{
    public class ShowArcInfo : MonoBehaviour
    {
        [Tooltip("Where to find Arc data for volume data")]
        public ArcDrawer arcDrawer;

        [Tooltip("Where to request that paths are drawn")]
        public ShowFullPath showFullPath;

        private FacData originFac;
        private FacData destFac;

        //private Text origin;
        //private Text destination;
        //private Text vol;

        public void ShowInfo(string oslic, string dslic, float volume)
        {
            Text origin = transform.Find("Origin").GetComponent<Text>();
            //Debug.Log(origin);
            origin.text = oslic;
            Text destination = transform.Find("Destination").GetComponent<Text>();
            destination.text = dslic;

            Text vol = transform.Find("Volume").GetComponent<Text>();
            string txt = string.Format("{0}", volume);
            vol.text = txt;
        }

        public void SetOrigin(FacData oFac)
        {
            originFac = oFac;
            UpdateInfo();
        }

        public void SetDest(FacData dFac)
        {
            destFac = dFac;
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            if (originFac != null & destFac != null)
            {
                if (arcDrawer)
                {
                    ShowInfo(originFac.SLIC, destFac.SLIC, arcDrawer.GetArcVolume(originFac.SLIC, destFac.SLIC));
                }

                if (showFullPath)
                {
                    showFullPath.DrawPath(originFac, destFac);
                }
            }
        }
    }
}
