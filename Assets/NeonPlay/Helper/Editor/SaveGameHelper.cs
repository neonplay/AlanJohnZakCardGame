using UnityEditor;
using UnityEngine;

namespace NeonPlayHelper.Editor {

    public static class SaveGameHelper {

        private const string KeyPrefix = "SaveGameHelper";
        private const string SaveGameKey = KeyPrefix + "SaveGame";

        [MenuItem("Neon Play Helper/Delete Save Game")]
        private static void DeleteSaveGame() {

            PlayerPrefs.DeleteKey(SaveGameKey);
            PlayerPrefs.Save();
        }
    }
}
