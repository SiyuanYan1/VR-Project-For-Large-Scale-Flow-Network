using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(Renderer))]
    public class StateColor : MonoBehaviour
    {
        [Tooltip("The color the State will be when not selected")]
        public Color DeselectedColor = Color.white;

        [Tooltip("The color the State will be when selected")]
        public Color SelectedColor = Color.green;

        // whether the State is currently selected or not
        private bool stateEnabled = false;

        private Renderer meshRenderer;
        private MaterialPropertyBlock props;
        private Color currentColor;

        // this color will be subtracted from the current color to 'highlight' it
        private Color Highlighter = new Color(0.2f, 0.2f, 0.2f, 0f);

        public void Start()
        {
            meshRenderer = GetComponent<Renderer>();
            props = new MaterialPropertyBlock();
            currentColor = stateEnabled ? SelectedColor : DeselectedColor;
            RefreshColor();
        }

        private void RefreshColor()
        {
            props.SetColor("_Color", currentColor);
            meshRenderer.SetPropertyBlock(props);
        }

        public void StateToggle()
        {
            stateEnabled = !stateEnabled;
            currentColor = stateEnabled ? SelectedColor : DeselectedColor;
            RefreshColor();
        }

        public void HoverStart()
        {
            currentColor = (stateEnabled ? SelectedColor : DeselectedColor) - Highlighter;
            RefreshColor();
        }

        public void HoverStop()
        {
            currentColor = stateEnabled ? SelectedColor : DeselectedColor;
            RefreshColor();
        }
    }
}
