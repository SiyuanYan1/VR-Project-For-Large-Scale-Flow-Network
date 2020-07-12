using UnityEngine;

namespace Facilities
{
    public class FacData
    {
        public Vector3 pos;
        public string SLIC;
        public string CC;
        public string SORT;
        public string CAP;
        public string SPAN;
        public string START;
        public string STATE;
        public FacData(Vector3 pos,string SLIC,string CC,string SORT,string CAP,string SPAN,string START,string STATE)
        {
            this.pos = pos;
            this.SLIC = SLIC;
            this.CC = CC;
            this.SORT = SORT;
            this.CAP = CAP;
            this.SPAN = SPAN;
            this.START = START;
            this.STATE = STATE;
        }

        public FacData(Vector3 pos, string SLIC,string STATE)
        {
            this.pos = pos;
            this.SLIC = SLIC;
            this.STATE = STATE;
        }

    }
}
