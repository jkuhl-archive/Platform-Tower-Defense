using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class EffectMaterial : MonoBehaviour
    {
        private readonly Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();

        /// <summary>
        ///     Applies the effect material to any mesh renderers on this GameObject
        /// </summary>
        /// <param name="effectMaterial"> Material that we want to apply for the effect </param>
        public void ApplyEffect(Material effectMaterial)
        {
            foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (meshRenderer.material != effectMaterial)
                {
                    originalMaterials.Add(meshRenderer.GetHashCode(), meshRenderer.material);
                    meshRenderer.material = effectMaterial;
                }
            }

            foreach (var meshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (meshRenderer.material != effectMaterial)
                {
                    originalMaterials.Add(meshRenderer.GetHashCode(), meshRenderer.material);
                    meshRenderer.material = effectMaterial;
                }
            }
        }
        
        /// <summary>
        ///     Removes the effect material from the GameObject and then self destructs this component
        /// </summary>
        public void DisableEffect()
        {
            foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (meshRenderer.material != originalMaterials[meshRenderer.GetHashCode()])
                {
                    meshRenderer.material = originalMaterials[meshRenderer.GetHashCode()];
                }
            }

            foreach (var meshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (meshRenderer.material != originalMaterials[meshRenderer.GetHashCode()])
                {
                    meshRenderer.material = originalMaterials[meshRenderer.GetHashCode()];
                }
            }

            Destroy(this);
        }
    }
}