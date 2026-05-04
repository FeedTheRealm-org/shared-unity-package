using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FeedTheRealm.Core.Repository
{
    [CreateAssetMenu(
        fileName = "MaterialsRepository",
        menuName = "Scriptable Objects/Materials Repository"
    )]
    public class MaterialsRepository : ScriptableObject
    {
        [SerializeField]
        private Material defaultMaterial;

        [SerializeField]
        private List<Material> materials = new();

        public List<Material> GetAllMaterials()
        {
            var result = new List<Material>();

            if (defaultMaterial != null)
                result.Add(defaultMaterial);

            result.AddRange(materials.Where(m => m != null && m != defaultMaterial));

            return result;
        }

        public List<Material> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAllMaterials();

            return GetAllMaterials()
                .Where(m => m.name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                .ToList();
        }

        public void Add(Material mat)
        {
            if (mat == null || materials.Contains(mat))
                return;
            materials.Add(mat);
        }

        public void Remove(Material mat)
        {
            materials.Remove(mat);
        }

        public bool Contains(Material mat) => mat == defaultMaterial || materials.Contains(mat);
    }
}
