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