using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class SwapMaterial : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material material1;
        [SerializeField] private Material material2;

        [Header("Swap Sound Effect (Optional)")]
        [SerializeField] private AudioClip swapSoundEffect;

        private bool isSwapped;

        // Start is called before the first frame update
        private void Start()
        {
            gameObject.GetComponent<MeshRenderer>().material = material1;
        }

        /// <summary>
        ///     Swaps the active material on this GameObject
        /// </summary>
        /// <param name="toggle"> If true set the active material to material 2, if false set it to material 1 </param>
        /// <param name="audioSource"> Optional audio source if we want to play a sound effect when swapping materials </param>
        public void Swap(bool toggle, AudioSource audioSource = null)
        {
            if (toggle)
            {
                if (!isSwapped)
                {
                    gameObject.GetComponent<MeshRenderer>().material = material2;
                    isSwapped = true;

                    if (audioSource != null && swapSoundEffect != null)
                        if (GameSettingsUtils.IsSoundEnabled())
                            audioSource.PlayOneShot(swapSoundEffect);
                }
            }
            else
            {
                if (gameObject.GetComponent<MeshRenderer>().material != material1)
                    gameObject.GetComponent<MeshRenderer>().material = material1;

                isSwapped = false;
            }
        }
    }
}