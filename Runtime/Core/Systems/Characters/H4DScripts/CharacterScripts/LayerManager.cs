using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.HeroEditor4D.Common.Scripts.CharacterScripts
{
    /// <summary>
    /// Used to order sprite layers (character parts).
    /// </summary>
    public class LayerManager : MonoBehaviour
    {
        /// <summary>
        /// SortingGroup can be used when you have multiple characters on scene.
        /// </summary>
        public SortingGroup SortingGroup;

        [Header("Sorting group scope")]
        public Transform SortingRoot;

        public bool AssignSortingLayer = true;

        public string CharacterSortingLayer = "Characters";

        [Header("Runtime depth sorting")]
        public int DepthSortingPrecision = 100;

        public int DepthSortingOffset;

        public Camera DepthSortingCamera;

        /// <summary>
        /// The full list of character sprites.
        /// </summary>
        public List<SpriteRenderer> Sprites;

        public LayerManager CopyTo;

        private void LateUpdate()
        {
            if (!Application.isPlaying)
                return;

            UpdateRuntimeDepthSorting();
        }

        private void UpdateRuntimeDepthSorting()
        {
            var sortingGroup = EnsureSortingGroup();
            var camera = DepthSortingCamera != null ? DepthSortingCamera : Camera.main;

            if (camera == null || sortingGroup == null)
                return;

            if (AssignSortingLayer && !string.IsNullOrWhiteSpace(CharacterSortingLayer))
            {
                sortingGroup.sortingLayerName = CharacterSortingLayer;
            }

            var precision = DepthSortingPrecision <= 0 ? 100 : DepthSortingPrecision;
            var depth = Vector3.Dot(GetSortingRootTransform().position, camera.transform.forward);
            sortingGroup.sortingOrder = Mathf.RoundToInt(-depth * precision) + DepthSortingOffset;
        }

        public void SetSortingGroupOrder(int index)
        {
            var sortingGroup = EnsureSortingGroup();

            if (sortingGroup == null)
                return;

            sortingGroup.sortingOrder = index;
        }

        /// <summary>
        /// Get character sprites and order by Sorting Order.
        /// </summary>
        public void GetSpritesBySortingOrder()
        {
            Sprites = GetSortingRootTransform()
                .GetComponentsInChildren<SpriteRenderer>(true)
                .OrderBy(i => i.sortingOrder)
                .ToList();
        }

        /// <summary>
        /// Set Sorting Order for character sprites.
        /// </summary>
        public void SetSpritesBySortingOrder()
        {
            for (var i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].sortingOrder = 5 * i;
            }

#if UNITY_EDITOR

            EditorUtility.SetDirty(this);

#endif
        }

        public void CopyOrder()
        {
            if (CopyTo == null)
                throw new ArgumentNullException(nameof(CopyTo));

            foreach (var sprite in CopyTo.Sprites)
            {
                sprite.sortingOrder = Sprites
                    .Single(i =>
                        i.name == sprite.name
                        && GetSpriteRendererPath(i) == GetSpriteRendererPath(sprite)
                    )
                    .sortingOrder;
            }

            Debug.Log("Copied!");
        }

        private SortingGroup EnsureSortingGroup()
        {
            var sortingRoot = GetSortingRootTransform();

            if (SortingGroup != null && SortingGroup.transform != sortingRoot)
            {
                SortingGroup = null;
            }

            if (SortingGroup == null)
            {
                SortingGroup = sortingRoot.GetComponent<SortingGroup>();

                if (SortingGroup == null)
                {
                    SortingGroup = sortingRoot.gameObject.AddComponent<SortingGroup>();
                }
            }

            return SortingGroup;
        }

        private Transform GetSortingRootTransform()
        {
            if (SortingRoot != null)
                return SortingRoot;

            var character = GetComponentInChildren<Character4D>(true);

            return character != null ? character.transform : transform;
        }

        private static string GetSpriteRendererPath(SpriteRenderer spriteRenderer)
        {
            var path = spriteRenderer.name;
            var t = spriteRenderer.transform;

            while (t.parent != null && t.parent.GetComponent<Character4D>() == null)
            {
                path = t.parent.name + "/" + path;
                t = t.parent;
            }

            return path;
        }
    }
}
