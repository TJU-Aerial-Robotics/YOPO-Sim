/* 
 *  Copyright 2025 Hongyu Cao
 *  
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *  
 *      http://www.apache.org/licenses/LICENSE-2.0
 *  
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {
    public enum Scene {
        LoadingScene,
        EvaluationScene,
        DataGenerationScene
    }

    private class LoadingMonoBehaviour : MonoBehaviour { }
    private static Scene targetScene;
    private static AsyncOperation loadingOperation;
    public static Func<bool> OnBeforeSceneSwitch;
    public static void LoadScene(Scene scene) {
        if (OnBeforeSceneSwitch != null && !OnBeforeSceneSwitch.Invoke()) {
            Debug.LogWarning("Scene switch cancelled by OnBeforeSceneSwitch callback.");
            return;
        }
        targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback() {
        GameObject loadingGameObject = new GameObject("LoadingMonoBehaviour");
        loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(targetScene));
    }

    public static float GetLoadProgress() {
        if (loadingOperation != null) return loadingOperation.progress;
        else return 0.0f;
    }

    private static IEnumerator LoadSceneAsync(Scene scene) {
        loadingOperation = SceneManager.LoadSceneAsync(scene.ToString());
        yield return loadingOperation;

        loadingOperation.allowSceneActivation = false;
        while (!loadingOperation.isDone) {
            if (loadingOperation.progress >= 0.9f) loadingOperation.allowSceneActivation = true;
            yield return null;
        }
    }
}
