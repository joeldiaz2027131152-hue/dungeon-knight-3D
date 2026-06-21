using UnityEngine;

namespace DungeonKnight.Level
{
    public sealed class WorldOneOneBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildOnPlay()
        {
            DungeonKnight3DBootstrap.BuildOnPlayScene();
        }
    }
}
