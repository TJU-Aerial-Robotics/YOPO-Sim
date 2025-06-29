using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YOPO.SIM {
    public class YOPOSceneManager : MonoBehaviour {
        private readonly string evaluationSceneName = "EvaluationScene";
        private readonly string dataGenerationSceneName = "DataGenerationScene";
        void Start() {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                LoadEvaluationScene();
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                LoadDataGenerationScene();
            }

        }

        private void LoadDataGenerationScene() {
            SceneManager.LoadScene(dataGenerationSceneName);
        }

        private void LoadEvaluationScene() {
            SceneManager.LoadScene(evaluationSceneName);
        }
    }

}