using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor;

namespace LeosSceneSelector.Editor
{
    public static class SceneExtensions
    {
        private const string SceneSearchFilter = "t:Scene";

        [Pure]
        public static IEnumerable<string> GetAllScenesPath()
        {
            return AssetDatabase.FindAssets(SceneSearchFilter).Select(AssetDatabase.GUIDToAssetPath);
        }

        [Pure]
        public static IEnumerable<string> GetAllBuildScenesPath()
        {
            return EditorBuildSettings.scenes.Select(scene => scene.path);
        }

        [Pure]
        public static IEnumerable<string> GetAllScenesPathWithoutBuildScenes()
        {
            return GetAllScenesPath().Except(GetAllBuildScenesPath());
        }


        [Pure]
        public static IEnumerable<SceneAsset> GetAllSceneAssets()
        {
            return GetAllScenesPath().Select(AssetDatabase.LoadAssetAtPath<SceneAsset>);
        }

        [Pure]
        public static IEnumerable<SceneAsset> GetAllSceneAssetsWithoutBuildScenes()
        {
            return GetAllScenesPathWithoutBuildScenes().Select(AssetDatabase.LoadAssetAtPath<SceneAsset>);
        }
    }
}