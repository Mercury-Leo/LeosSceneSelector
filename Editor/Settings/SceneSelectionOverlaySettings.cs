using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LeosSceneSelector.Editor
{
    using static SceneSelectionConfig;

    [FilePath(SelectionProjectSettingsPath, FilePathAttribute.Location.ProjectFolder)]
    internal sealed class SceneSelectionOverlaySettings : ScriptableSingleton<SceneSelectionOverlaySettings>
    {
        [SerializeField] private bool additiveOptionEnabled;
        [SerializeField] private bool notificationsEnabled;
        [SerializeField] private bool isBuildScenes;

        [SerializeField] private List<SceneAsset> addedScenes = new();
        private readonly HashSet<string> _addedScenePaths = new();

        public IReadOnlyList<SceneAsset> AddedScenes => addedScenes;
        public IEnumerable<string> AddedScenePaths => _addedScenePaths;

        public bool AdditiveOptionEnabled
        {
            get => additiveOptionEnabled;
            set
            {
                if (additiveOptionEnabled == value)
                {
                    return;
                }

                additiveOptionEnabled = value;
                SaveAndSetDirty();
            }
        }

        public bool NotificationsEnabled
        {
            get => notificationsEnabled;
            set
            {
                if (notificationsEnabled == value)
                {
                    return;
                }

                notificationsEnabled = value;
                SaveAndSetDirty();
            }
        }

        public bool IsBuildScenes
        {
            get => isBuildScenes;
            set
            {
                isBuildScenes = value;
                Save(true);
            }
        }

        public void AddScene(SceneAsset scene)
        {
            if (addedScenes.Contains(scene))
            {
                return;
            }

            _addedScenePaths.Add(GetScenePath(scene));
            addedScenes.Add(scene);
            Save(true);
            EditorUtility.SetDirty(this);
        }

        public void RemoveScene(SceneAsset scene)
        {
            if (addedScenes.Remove(scene))
            {
                _addedScenePaths.Remove(GetScenePath(scene));
                Save(true);
                EditorUtility.SetDirty(this);
            }
        }

        private string GetScenePath(SceneAsset scene)
        {
            var path = AssetDatabase.GetAssetPath(scene);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"Could not find path for {scene}", scene);
                return string.Empty;
            }

            return path;
        }

        private void SaveAndSetDirty()
        {
            Save(true);
            EditorUtility.SetDirty(this);
        }
    }
}