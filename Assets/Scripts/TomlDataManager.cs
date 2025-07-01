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
using System.Collections.Generic;
using Tomlet;
using UnityEngine;
using UnitySensors.Sensor.Camera;

namespace YOPO.SIM {
    class TomlDataManager : MonoBehaviour {
        [SerializeField] private CameraSensor _depthCameraSensor;
        private TomlData _dataToml;

        void Start() {
            DataGenerationEvents.BeforePoissonGeneration += OnBeforePoissonGeneration;
            DataGenerationEvents.AfterPoissonGeneration += OnAfterPoissonGeneration;
            DataGenerationEvents.OnImageSampled += OnImageSampled;
        }

        private void OnImageSampled(object sender, DataGenerationEvents.ImageSampledEventArgs e) {
            // WorldPosition ans WorldYaw in unity follows left-hand rule, while in toml follows right-hand rule
            var startPos = new List<float> { e.WorldPosition.z, -e.WorldPosition.x };
            var startYaw = 360.0f - e.WorldYaw;
            AddData(e.ImageFileNameList, startPos, startYaw);
        }

        void OnDestroy() {
            DataGenerationEvents.BeforePoissonGeneration -= OnBeforePoissonGeneration;
            DataGenerationEvents.AfterPoissonGeneration -= OnAfterPoissonGeneration;
            DataGenerationEvents.OnImageSampled -= OnImageSampled;
        }

        public void Init() {
            if (_dataToml == null) {
                _dataToml = new TomlData();
                if (_dataToml.dataArray == null) _dataToml.dataArray = new List<Data>();
            }
            _dataToml.depthCameraFarClipPlane = _depthCameraSensor.texture0FarClipPlane;
            var camera = _depthCameraSensor.m_camera;
            float _fov = camera.fieldOfView;
            var _resolution = new Vector2Int(camera.pixelWidth, camera.pixelHeight);
            _dataToml.depthCameraHorizontalFOV = Camera.VerticalToHorizontalFieldOfView(_fov, _resolution.x / (float)_resolution.y);
        }

        public void ClearData() {
            _dataToml.dataArray.Clear();
        }

        public void AddData(List<string> imageFileNameList, List<float> startPos, float startYaw) {
            Debug.Assert(startPos.Count == 2, "Start position should have 2 elements (z, -x) in toml format.");
            Data data = new Data();
            data.imageFileNameList = imageFileNameList;
            data.posStart = startPos;
            data.yawStart = startYaw;
            _dataToml.dataArray.Add(data);
        }

        public void SaveData(string fullPath) {
            string tomlDoc = TomletMain.TomlStringFrom(_dataToml);
            System.IO.File.WriteAllText(fullPath, tomlDoc);
            Debug.Log($"Saved toml to {fullPath}");
        }

        private void OnBeforePoissonGeneration(object sender, EventArgs e) {
            Init();
        }

        private void OnAfterPoissonGeneration(object sender, DataGenerationEvents.AfterPoissonGenerationEventArgs e) {
            string tomlFullPath = e.FilePathManager.GetTomlFilePath(e.SceneIndex);
            SaveData(tomlFullPath);
            ClearData();
        }
    }
}
