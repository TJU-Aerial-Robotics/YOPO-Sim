using System.Collections.Generic;
using System.IO;
using Tomlet;
using UnityEngine;
using UnitySensors.Sensor.Camera;

namespace Diablo.Utils {
    class TomlDataManager : MonoBehaviour {
        [SerializeField] private DepthCameraSensor _depthCameraSensor;
        private TomlData _dataToml;
        public void Init() {
            _dataToml = new TomlData();
            _dataToml.dataArray = new List<Data>();
            _dataToml.depthCameraFarClipPlane = _depthCameraSensor.texture0FarClipPlane;
            var camera = _depthCameraSensor.m_camera;
            float _fov = camera.fieldOfView;
            var _resolution = new Vector2Int(camera.pixelWidth, camera.pixelHeight);
            _dataToml.depthCameraHorizontalFOV = Camera.VerticalToHorizontalFieldOfView(_fov, _resolution.x / (float)_resolution.y);
        }
        public void AddData(string imageFileName, List<float> startPos, float startYaw) {
            Debug.Assert(startPos.Count == 2);
            Data data = new Data();
            data.imageFileName = imageFileName;
            data.posStart = startPos;
            data.yawStart = startYaw;
            _dataToml.dataArray.Add(data);
        }
        public void SaveData(string fullPath) {
            string tomlDoc = TomletMain.TomlStringFrom(_dataToml);
            File.WriteAllText(fullPath, tomlDoc);
            Debug.Log($"Saved toml to {fullPath}");
        }
        class TomlData {
            public float depthCameraFarClipPlane;
            public float depthCameraHorizontalFOV;
            public List<Data> dataArray;
        }
        class Data {
            public string imageFileName;
            public List<float> posStart;
            public float yawStart;
        }
    }
}
