using UnityEngine;
using UnitySensors.Sensor.Camera;

namespace Diablo.Utils {
    public class DepthImageManager : MonoBehaviour {
        [SerializeField] private DepthCameraSensor _depthCameraSensor;
        public void SaveImage(string fullPath) {
            _depthCameraSensor.UpdateSensorManually();
            var texture = _depthCameraSensor.texture0;
            System.IO.File.WriteAllBytes(fullPath, texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP));
            Debug.Log($"Saved to {fullPath}");
        }

    }
}
