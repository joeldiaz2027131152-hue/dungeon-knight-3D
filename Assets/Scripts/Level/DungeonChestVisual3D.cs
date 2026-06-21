using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonChestVisual3D : MonoBehaviour
    {
        [SerializeField] private Transform lidPivot;
        [SerializeField] private GameObject treasureRoot;
        [SerializeField] private Light treasureLight;
        [SerializeField] private float openDuration = 0.72f;

        private bool opening;
        private bool open;
        private float openTimer;
        private Quaternion closedRotation;
        private Quaternion openRotation;

        public void Configure(Transform lid, GameObject treasure, Light lightSource)
        {
            lidPivot = lid;
            treasureRoot = treasure;
            treasureLight = lightSource;
            closedRotation = lidPivot ? lidPivot.localRotation : Quaternion.identity;
            openRotation = closedRotation * Quaternion.Euler(-108f, 0f, 0f);

            if (treasureRoot) treasureRoot.SetActive(false);
            if (treasureLight) treasureLight.enabled = false;
        }

        public void Open()
        {
            if (open || opening) return;

            opening = true;
            openTimer = 0f;
            if (treasureRoot) treasureRoot.SetActive(true);
            if (treasureLight) treasureLight.enabled = true;
        }

        private void Update()
        {
            if (!opening || !lidPivot) return;

            openTimer += Time.deltaTime;
            float t = Mathf.Clamp01(openTimer / openDuration);
            float stepped = Mathf.Floor(t * 8f) / 8f;
            float smooth = Mathf.SmoothStep(0f, 1f, Mathf.Max(t, stepped));
            lidPivot.localRotation = Quaternion.Slerp(closedRotation, openRotation, smooth);

            if (treasureRoot)
            {
                float pop = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
                treasureRoot.transform.localScale = Vector3.one * (0.82f + pop * 0.18f);
            }

            if (treasureLight)
            {
                treasureLight.intensity = 0.6f + smooth * 1.4f + Mathf.Sin(Time.time * 14f) * 0.12f;
            }

            if (t < 1f) return;

            lidPivot.localRotation = openRotation;
            opening = false;
            open = true;
        }
    }
}
