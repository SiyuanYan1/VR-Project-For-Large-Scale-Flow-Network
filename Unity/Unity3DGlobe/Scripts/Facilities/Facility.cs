using UnityEngine;

namespace Facilities
{
    public class Facility : MonoBehaviour
    {
        public static Material PointMaterial;

        private FacData fac;

        public void SetFac(FacData fac)
        {
            this.fac = fac;
        }

        public FacData GetFac()
        {
            return fac;
        }

    }
}
