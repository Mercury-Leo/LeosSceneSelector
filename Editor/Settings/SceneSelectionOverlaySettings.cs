using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor.SceneSelection
{
    using static SceneSelectionConfig;

    [FilePath(SelectionProjectSettingsPath, FilePathAttribute.Location.ProjectFolder)]
    internal sealed class SceneSelectionOverlaySettings : ScriptableSingleton<SceneSelectionOverlaySettings>
    {
        [SerializeField] private bool _additiveOptionEnabled;
        [SerializeField] private bool _notificationsEnabled;

        [SerializeField] private List<SceneAsset> _addedScenes = new();
        private readonly HashSet<string> _addedScenePaths = new();

        public IReadOnlyList<SceneAsset> AddedScenes => _addedScenes;
        public IEnumerable<string> AddedScenePaths => _addedScenePaths;

        public bool AdditiveOptionEnabled
        {
            get => _additiveOptionEnabled;
            set
            {
                _additiveOptionEnabled = value;
                Save(true);
            }
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                _notificationsEnabled = value;
                Save(true);
            }
        }

        public void AddScene(SceneAsset scene)
        {
            if (_addedScenes.Contains(scene))
            {
                return;
            }

            _addedScenePaths.Add(GetScenePath(scene));
            _addedScenes.Add(scene);
            Save(true);
            EditorUtility.SetDirty(this);
        }

        public void RemoveScene(SceneAsset scene)
        {
            if (_addedScenes.Remove(scene))
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
    }
}