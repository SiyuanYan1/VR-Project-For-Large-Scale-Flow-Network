using UnityEngine;

namespace Arcs
{
    public class Arc : MonoBehaviour
    {
        // Arc properties shared between all Arcs (set by ArcDrawer)
        public static float MaxHeight = 0.7f;
        public static Gradient ArcGradient;
        public static int SegmentCount = 100;

        // height to normalise against (set by ArcDrawer)
        public static float MaxVolume = 0.5f;

        private ArcData arcData;
        private bool dirty = false;

        private float PrevMaxHeightQuotient = 0f;

        public bool o_enabled = false;
        public bool d_enabled = false;

        private LineRenderer lr;

        private void Awake()
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.sortingLayerID = 0;
            lr.alignment = LineAlignment.TransformZ;
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
            lr.colorGradient = ArcGradient;
            lr.startWidth = 0.0005f;
            lr.endWidth = 0.0005f;
        }

        public bool IsEnabled()
        {
            return (o_enabled && d_enabled);
        }

        public void SetArcData(ArcData data)
        {
            arcData = data;
        }

        public ArcData GetArcData()
        {
            return arcData;
        }

        public void Draw(float height_multiplier)
        {
            var origin = arcData.startpos;
            var dest = arcData.endpos;
            //Vector3 test = 0.5f * (dest + origin);
            //Debug.Log(test.magnitude);  
            Vector3 apex = dest + origin;
            //Debug.Log(height_multiplier);
            apex.Normalize();
            apex *= height_multiplier;
            lr.positionCount = SegmentCount + 1;
            for (int i = 0; i <= SegmentCount; i++)
            {
                float t = i / (float)SegmentCount;
                Vector3 next_point = GetPoint(origin, apex, dest, t);
                lr.SetPosition(i, next_point);
            }
        }

        public void UpdateDrawing()
        {
            float NewMaxHeightQuotient = MaxVolume * transform.parent.parent.localScale.x;
            if (PrevMaxHeightQuotient != NewMaxHeightQuotient)
            {
                PrevMaxHeightQuotient = NewMaxHeightQuotient;
                dirty = true;
            }
            if (dirty)
            {
                float height_multiplier = 0.51f + (arcData.volume * MaxHeight) / (PrevMaxHeightQuotient);
                Draw(height_multiplier);
                dirty = false;
            }
        }

        // calculate the position of a point in a parabola from p0 (t = 0) to p2 (t = 1), with the peak at p1
        private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
    }
}
