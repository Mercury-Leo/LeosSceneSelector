using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static LeosSceneSelector.Editor.SceneSelectionConfig;

namespace LeosSceneSelector.Editor
{
    internal sealed class SceneSelectionOverlaySettingsProvider : SettingsProvider
    {
        private SceneSelectionOverlaySettingsProvider(string path, SettingsScope scopes,
            IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        private readonly GUIContent _addSceneButton =
            new("Add Scene", "Add a scene to appear in the Build Scenes dropdown");

        private readonly GUIContent _removeSceneButton = new("Remove", "Removes the scene from the Build Scene");

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            GUILayout.Space(20f);

            SceneSelectionOverlaySettings.instance.AdditiveOptionEnabled = EditorGUILayout.Toggle(AdditiveOptionLabel,
                SceneSelectionOverlaySettings.instance.AdditiveOptionEnabled, GUILayout.Width(200f));

            SceneSelectionOverlaySettings.instance.NotificationsEnabled = EditorGUILayout.Toggle(NotificationsLabel,
                SceneSelectionOverlaySettings.instance.NotificationsEnabled, GUILayout.Width(200f));

            GUILayout.Space(20f);

            EditorGUILayout.LabelField("Scenes added to Scene Selection Tool", EditorStyles.boldLabel);

            var addedScenes = SceneSelectionOverlaySettings.instance.AddedScenes;
            var scenes = SceneExtensions.GetAllSceneAssetsWithoutBuildScenes();

            var scenesToPopulate = scenes.Except(addedScenes);

            var sceneAssets = scenesToPopulate as SceneAsset[] ?? scenesToPopulate.ToArray();
            GUI.enabled = sceneAssets.Any();
            if (GUILayout.Button(_addSceneButton))
            {
                var menu = new GenericMenu();
                foreach (var scene in sceneAssets)
                {
                    if (addedScenes.Contains(scene))
                    {
                        continue;
                    }

                    menu.AddItem(new GUIContent(scene.name), false, () => AddScene(scene));
                }

                menu.ShowAsContext();
            }

            GUI.enabled = true;

            foreach (var scene in addedScenes.ToList())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(scene, typeof(SceneAsset), false);
                if (GUILayout.Button(_removeSceneButton, GUILayout.Width(60f)))
                {
                    SceneSelectionOverlaySettings.instance.RemoveScene(scene);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddScene(SceneAsset scene)
        {
            SceneSelectionOverlaySettings.instance.AddScene(scene);
        }


        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SceneSelectionOverlaySettingsProvider(SelectionOverlayPath, SettingsScope.User);
        }
    }
}