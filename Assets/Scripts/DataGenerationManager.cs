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
using UnitySensors.Attribute;
namespace YOPO.SIM {
    public class DataGenerationManager : MonoBehaviour {
        [Serializable]
        public class DataGenerationSettings {
            [Range(0, 10)] public int terrainRoundNum;
            [Range(0, 10)] public int poissonRoundNum;
            public string dataFolderName;
        }
        [SerializeField] private FilePathManager _filePathManager;
        [SerializeField] private int _terrainSeedStart;
        [SerializeField] private uint _poissonSeedStart;
        [SerializeField, ReadOnly] private int _terrainSeed;
        [SerializeField, ReadOnly] private uint _poissonSeed;
        [SerializeField]
        private DataGenerationSettings _trainingSettings = new() {
            terrainRoundNum = 1,
            poissonRoundNum = 1,
            dataFolderName = "TrainingData"
        };
        [SerializeField]
        private DataGenerationSettings _testingSettings = new() {
            terrainRoundNum = 1,
            poissonRoundNum = 1,
            dataFolderName = "TestingData"
        };
        [SerializeField] private bool _isDebug = false;
        private bool _isBusy = false;
        private bool _isGenerating = false;

        public DataGenerationSettings TrainingSettings { get => _trainingSettings; set => _trainingSettings = value; }
        public DataGenerationSettings TestingSettings { get => _testingSettings; set => _testingSettings = value; }

        void Start() {
            Loader.OnBeforeSceneSwitch += CouldSwitchScene;
        }

        void OnDestroy() {
            Loader.OnBeforeSceneSwitch -= CouldSwitchScene;
        }

        bool CouldSwitchScene() {
            if (_isBusy) {
                Debug.LogWarning("Data generation is in progress. Please wait until it finishes.");
                return false;
            }
            return true;
        }

        void Update() {
            if (_isDebug && Input.GetKeyDown(KeyCode.Space)) {
                StartCoroutine(GenerateData());
            }
        }

        public void StartDataGeneration() {
            if (_isBusy || _isGenerating) {
                Debug.LogWarning("Data generation is already in progress.");
                return;
            }
            StartCoroutine(GenerateData());
        }

        IEnumerator GenerateData() {
            _isGenerating = true;
            int targetFrameRateBackup = GameSettings.Instance.TargetFrameRate;
            bool enableVSyncBackup = GameSettings.Instance.EnableVSync;
            GameSettings.Instance.ChangeSettings(0, enableVSync: false);

            // Traverse _terrainRoundNum terrain seeds
            _terrainSeed = _terrainSeedStart;
            _poissonSeed = _poissonSeedStart;
            yield return RunTrainingOrTesting(true);
            yield return RunTrainingOrTesting(false);

            GameSettings.Instance.ChangeSettings(targetFrameRateBackup, enableVSync: enableVSyncBackup);
            _isGenerating = false;
        }

        private IEnumerator RunTrainingOrTesting(bool isTraining = true) {
            var settings = isTraining ? _trainingSettings : _testingSettings;

            string dataFolderName = settings.dataFolderName;
            int terrainRoundNum = settings.terrainRoundNum;
            int poissonRoundNum = settings.poissonRoundNum;

            _filePathManager.SetDataFolderName(dataFolderName);
            _filePathManager.DeleteDataFolder();
            yield return RunTerrainPoissonIterations(terrainRoundNum, poissonRoundNum);
        }

        private IEnumerator RunTerrainPoissonIterations(int terrainRoundNum, int poissonRoundNum) {
            for (int i = 0; i < terrainRoundNum; i++) {
                int sceneIndex = (int)_terrainSeed;
                _isBusy = true;
                yield return DataGenerationEvents.TriggerSavePointCloud(_terrainSeed, sceneIndex, _filePathManager);
                _isBusy = false;
                DataGenerationEvents.RaiseBeforePoissonGeneration(this, EventArgs.Empty);

                // Traverse _poissonRoundNum poisson seeds
                for (int j = 0; j < poissonRoundNum; j++) {
                    yield return DataGenerationEvents.TriggerSampleAndSaveImages(sceneIndex, _poissonSeed, _filePathManager);
                    _poissonSeed++;
                }
                DataGenerationEvents.RaiseAfterPoissonGeneration(this,
                    new DataGenerationEvents.AfterPoissonGenerationEventArgs {
                        SceneIndex = sceneIndex,
                        FilePathManager = _filePathManager
                    });

                _terrainSeed++;
            }
        }
    }
}