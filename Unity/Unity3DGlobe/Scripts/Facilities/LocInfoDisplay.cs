using UnityEngine;
using UnityEngine.UI;

namespace Facilities
{
    public class LocInfoDisplay : MonoBehaviour
    {
        // Start is called before the first frame update
        private Text SLIC;
        private Text CC;
        private Text SORT;
        private Text CAP;
        private Text SPAN;
        private Text START;
        private Text STATE;
        void Start()
        {
            string[] attributes = { "SLIC", "CC", "SORT", "CAP", "SPAN", "START" };
            SLIC = transform.Find("SLIC").GetComponent<Text>();
            //SLIC.transform.position = new Vector3(0, 0, 0);

            CC = transform.Find("CC").GetComponent<Text>();

            SORT = transform.Find("SORT").GetComponent<Text>();

            CAP = transform.Find("CAP").GetComponent<Text>();

            SPAN = transform.Find("SPAN").GetComponent<Text>();

            START = transform.Find("START").GetComponent<Text>();

            STATE = transform.Find("STATE").GetComponent<Text>();

        }

        public void UpdateLoc(FacData fac)
        {
            SLIC.text = fac.SLIC;
            CC.text = fac.CC;
            SORT.text = fac.SORT;
            CAP.text = fac.CAP;
            SPAN.text = fac.SPAN;
            START.text = fac.START;
            STATE.text = fac.STATE;
        }
    }
}
