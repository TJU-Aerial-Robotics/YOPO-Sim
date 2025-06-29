using System;
using System.Collections;
using UnityEngine;
using UnitySensors.Attribute;
namespace YOPO.SIM {
    public class DataGenerationManager : MonoBehaviour {
        [Serializable]
        class DataGenerationSettings {
            [Range(0, 10)] public int terrainRoundNum;
            [Range(0, 10)] public int poissonRoundNum;
            public string dataFolderName;
        }
        [SerializeField] private FilePathManager _filePathManager;
        [SerializeField] private int _terrainSeedStart;
        [SerializeField] private int _poissonSeedStart;
        [SerializeField, ReadOnly] private int _terrainSeed;
        [SerializeField, ReadOnly] private int _poissonSeed;
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

        void Update() {
            if (_isDebug && Input.GetKeyDown(KeyCode.Space)) {
                StartCoroutine(GenerateData());
            }
        }

        IEnumerator GenerateData() {
            int targetFrameRateBackup = GameSettings.Instance.TargetFrameRate;
            bool enableVSyncBackup = GameSettings.Instance.EnableVSync;
            GameSettings.Instance.ChangeSettings(0, enableVSync: false);

            // Traverse _terrainRoundNum terrain seeds
            _terrainSeed = _terrainSeedStart;
            _poissonSeed = _poissonSeedStart;
            yield return RunTrainingOrTesting(true);
            yield return RunTrainingOrTesting(false);

            GameSettings.Instance.ChangeSettings(targetFrameRateBackup, enableVSync: enableVSyncBackup);
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
                int sceneIndex = _terrainSeed;
                yield return DataGenerationEvents.TriggerSavePointCloud(_terrainSeed, sceneIndex, _filePathManager);
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