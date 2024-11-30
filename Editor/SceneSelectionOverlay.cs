using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LeosSceneSelector.Editor.SceneSelectionConfig;

namespace LeosSceneSelector.Editor
{
    [Overlay(typeof(SceneView), "Scene Selection")]
    [Icon(IconPath)]
    internal sealed class SceneSelectionOverlay : ToolbarOverlay
    {
        private SceneSelectionOverlay() : base(SceneDropdownToggle.ID) { }

        [EditorToolbarElement(ID, typeof(SceneView))]
        private class SceneDropdownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public const string ID = "SceneSelectionOverlay/SceneDropdownToggle";
            public EditorWindow containerWindow { get; set; }
            private readonly GUIContent _notificationContent = new();

            private SceneDropdownToggle()
            {
                text = ScenesLabel;
                tooltip = ToolTip;
                icon = EditorGUIUtility.IconContent(IconPath).image as Texture2D;

                dropdownClicked += InitializeSceneMenu;
            }

            private void InitializeSceneMenu()
            {
                var menu = new GenericMenu();

                AddSceneLoadTypeToggle(menu);
                menu.AddSeparator(string.Empty);

                if (SceneSelectionOverlaySettings.instance.IsBuildScenes)
                {
                    CreateBuildScenes(menu);
                }
                else
                {
                    CreateAllScenes(menu);
                }

                menu.ShowAsContext();
            }

            private void AddSceneLoadTypeToggle(GenericMenu menu)
            {
                var buttonName = SceneSelectionOverlaySettings.instance.IsBuildScenes
                    ? AllScenesLabel
                    : BuildScenesLabel;

                menu.AddItem(new GUIContent(buttonName), true,
                    () => SceneSelectionOverlaySettings.instance.IsBuildScenes =
                        !SceneSelectionOverlaySettings.instance.IsBuildScenes);
            }

            private void CreateBuildScenes(GenericMenu menu)
            {
                var activeScene = SceneManager.GetActiveScene();
                var buildScenes = SceneExtensions.GetAllBuildScenesPath();
                var addedScenes = SceneSelectionOverlaySettings.instance.AddedScenePaths;
                var scenes = buildScenes.Concat(addedScenes);

                CreateScenes(menu, scenes, activeScene);
            }

            private void CreateAllScenes(GenericMenu menu)
            {
                var activeScene = SceneManager.GetActiveScene();

                CreateScenes(menu, SceneExtensions.GetAllScenesPath(), activeScene);
            }

            private void CreateScenes(GenericMenu menu, IEnumerable<string> scenePaths, Scene activeScene)
            {
                foreach (var scenePath in scenePaths)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                    if (string.Equals(activeScene.name, sceneName, StringComparison.Ordinal) ||
                        SceneManager.GetSceneByName(sceneName).isLoaded)
                    {
                        menu.AddDisabledItem(new GUIContent(sceneName));
                        continue;
                    }

                    AddSceneMenuItems(menu, scenePath, sceneName, activeScene);
                }
            }

            /// <summary>
            /// Populates the dropdown based on user's preferences
            /// </summary>
            /// <param name="menu"></param>
            /// <param name="path"></param>
            /// <param name="sceneName"></param>
            /// <param name="activeScene"></param>
            private void AddSceneMenuItems(GenericMenu menu, string path, string sceneName, Scene activeScene)
            {
                if (SceneSelectionOverlaySettings.instance.AdditiveOptionEnabled)
                {
                    menu.AddItem(new GUIContent(sceneName + SingleLabel), false,
                        () => OpenScene(activeScene, path, OpenSceneMode.Single));
                    menu.AddItem(new GUIContent(sceneName + AdditiveLabel), false,
                        () => OpenScene(activeScene, path, OpenSceneMode.Additive));
                }
                else
                {
                    menu.AddItem(new GUIContent(sceneName), false,
                        () => OpenScene(activeScene, path, OpenSceneMode.Single));
                }
            }

            private void OpenScene(Scene activeScene, string path, OpenSceneMode mode)
            {
                if (activeScene.isDirty)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        ShowNotification(path);
                        EditorSceneManager.OpenScene(path, mode);
                    }
                }
                else
                {
                    ShowNotification(path);
                    EditorSceneManager.OpenScene(path, mode);
                }
            }

            private void ShowNotification(string scenePath)
            {
                if (!SceneSelectionOverlaySettings.instance.NotificationsEnabled)
                {
                    return;
                }

                _notificationContent.text = $"Loaded {Path.GetFileNameWithoutExtension(scenePath)}";
                SceneView.lastActiveSceneView.ShowNotification(_notificationContent, 2);
            }
        }
    }
}