using UnityEngine;

namespace Game
{
    public class HardcodeInitializer 
    {
        [RuntimeInitializeOnLoadMethod]
        private void Initialize()
        {
            Application.targetFrameRate = 144;
        }
    }
}