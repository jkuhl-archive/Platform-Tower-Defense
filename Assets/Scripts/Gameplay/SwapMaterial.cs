using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class SwapMaterial : MonoBehaviour
    {
        [Header("Materials")]
        public Material material1;
        public Material material2;
    
        // Optional sound effect to be played when swapping material
        public AudioClip swapSoundEffect;

        private bool isSwapped;
    
        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponent<MeshRenderer>().material = material1;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Swaps the active material on this GameObject
        /// </summary>
        /// <param name="toggle"> If true set the active material to material 2, if false set it to material 1 </param>
        /// <param name="audioSource"> Optional audio source if we want to play a sound effect when swapping materials </param>
        public void Swap(bool toggle, AudioSource audioSource=null)
        {
            if (toggle)
            {
                if (!isSwapped)
                {
                    gameObject.GetComponent<MeshRenderer>().material = material2;
                    isSwapped = true;

                    if (audioSource != null && swapSoundEffect != null)
                    {
                        if (PlayerDataUtils.IsSoundEnabled())
                        {
                            audioSource.PlayOneShot(swapSoundEffect);
                        }
                    }
                }
            }
            else
            {
                if (gameObject.GetComponent<MeshRenderer>().material != material1)
                {
                    gameObject.GetComponent<MeshRenderer>().material = material1;
                }

                isSwapped = false;
            }
        }
    }
}
