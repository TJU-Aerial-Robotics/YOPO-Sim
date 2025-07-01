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
using UnitySensors.Attribute;

namespace YOPO.SIM {
    public class GameSettings : MonoBehaviour {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private bool _runInBackground = true;
        [SerializeField] private bool _enableVSync = false;
        [SerializeField, ReadOnly] private int _vSyncCount = 0;
        static public GameSettings Instance { get; private set; }
        public int TargetFrameRate => _targetFrameRate;
        public bool RunInBackground => _runInBackground;
        public bool EnableVSync => _enableVSync;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Debug.LogError("Multiple instances of GameSettings detected. Destroying duplicate instance.");
                Destroy(gameObject);
            }
            ApplySettings();
        }

        private void OnValidate() {
            ApplySettings();
        }

        public void ChangeSettings(int targetFrameRate, bool runInBackground = true, bool enableVSync = true) {
            _targetFrameRate = targetFrameRate;
            _runInBackground = runInBackground;
            _enableVSync = enableVSync;
            ApplySettings();
        }

        private void ApplySettings() {
            // Apply the settings to the game
            // Debug.Log($"Applying Game Settings: Target Frame Rate: {_targetFrameRate}, Run In Background: {_runInBackground}, Enable VSync: {_enableVSync}");
            Application.targetFrameRate = _targetFrameRate;
            Application.runInBackground = _runInBackground;
            if (_enableVSync) {
                float refreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
                _vSyncCount = Mathf.Max(1, Mathf.RoundToInt(refreshRate / _targetFrameRate));
            }
            else {
                _vSyncCount = 0;
            }
            QualitySettings.vSyncCount = _vSyncCount;
        }
    }
}
