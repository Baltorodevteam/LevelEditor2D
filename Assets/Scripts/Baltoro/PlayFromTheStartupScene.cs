#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromTheStartupScene {
	static PlayFromTheStartupScene() {
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state) {
		if (state == PlayModeStateChange.EnteredEditMode) {
			if (EditorPrefs.HasKey("lastLoadedScenePath")) {
				EditorSceneManager.OpenScene(EditorPrefs.GetString("lastLoadedScenePath"));
				EditorPrefs.DeleteKey("lastLoadedScenePath");
			}
		}
	}

	[MenuItem("Edit/Play From Startup Scene %&#r")]
	public static void PlayFromStartupScene() {
		if (EditorApplication.isPlaying == true) {
			EditorApplication.isPlaying = false;
			return;
		}

		string firstActiveScenePath = null;
		foreach (var scene in EditorBuildSettings.scenes) {
			if (scene.enabled) {
				firstActiveScenePath = scene.path;
				break;
			}
		}

		if(firstActiveScenePath == null)
			return;

		EditorPrefs.SetString("lastLoadedScenePath", EditorSceneManager.GetActiveScene().path);
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(firstActiveScenePath);
		EditorApplication.isPlaying = true;
	}
}

#endif