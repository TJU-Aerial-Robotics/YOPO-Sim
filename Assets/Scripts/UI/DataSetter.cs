using UnityEngine;
using UnityEngine.UI;

namespace YOPO.SIM.UI {
    public class DataSetter : MonoBehaviour {
        [SerializeField] private Slider _trainingTerrainRoundNumSlider;
        [SerializeField] private Slider _trainingPoissonRoundNumSlider;
        [SerializeField] private Slider _testingTerrainRoundNumSlider;
        [SerializeField] private Slider _testingPoissonRoundNumSlider;
        [SerializeField] private DataGenerationManager _dataGenerationManager;
        void Start() {
            // Set initial values from DataGenerationManager
            _trainingTerrainRoundNumSlider.value = _dataGenerationManager.TrainingSettings.terrainRoundNum;
            _trainingPoissonRoundNumSlider.value = _dataGenerationManager.TrainingSettings.poissonRoundNum;
            _testingTerrainRoundNumSlider.value = _dataGenerationManager.TestingSettings.terrainRoundNum;
            _testingPoissonRoundNumSlider.value = _dataGenerationManager.TestingSettings.poissonRoundNum;

            _trainingTerrainRoundNumSlider.onValueChanged.AddListener((arg0) => {
                _dataGenerationManager.TrainingSettings.terrainRoundNum = (int)arg0;
            });
            _trainingPoissonRoundNumSlider.onValueChanged.AddListener((arg0) => {
                _dataGenerationManager.TrainingSettings.poissonRoundNum = (int)arg0;
            });
            _testingTerrainRoundNumSlider.onValueChanged.AddListener((arg0) => {
                _dataGenerationManager.TestingSettings.terrainRoundNum = (int)arg0;
            });
            _testingPoissonRoundNumSlider.onValueChanged.AddListener((arg0) => {
                _dataGenerationManager.TestingSettings.poissonRoundNum = (int)arg0;
            });
        }
    }
}