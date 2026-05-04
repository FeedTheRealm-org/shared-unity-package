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
        private List<(string name, Material material)> materials = new();

        public List<Material> GetAllMaterials() => materials.Select(m => m.material).ToList();

        public List<Material> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAllMaterials();

            return materials
                .Where(m => m.name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                .Select(m => m.material)
                .ToList();
        }

        public void Add(Material mat)
        {
            if (mat == null || materials.Any(m => m.material == mat))
                return;
            materials.Add((mat.name, mat));
        }

        public void Remove(Material mat)
        {
            materials.RemoveAll(m => m.material == mat);
        }

        public bool Contains(Material mat) => materials.Any(m => m.material == mat);
    }
}
