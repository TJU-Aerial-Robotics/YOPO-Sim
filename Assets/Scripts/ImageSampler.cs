using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Diablo;
using UnityEngine;
using UnitySensors.Sensor.Camera;

namespace YOPO.SIM {
    public class ImageSampler : MonoBehaviour {
        private enum SourceTexture {
            Texture0,
            Texture1
        }
        private enum Encoding {
            EXR,
            PNG,
            JPG
        }
        [Serializable]
        class CameraSensorSettings {
            public CameraSensor cameraSensor;
            public SourceTexture sourceTexture;
            public Encoding encoding;
            public string fileNamePrefix;

        }
        [SerializeField] private List<CameraSensorSettings> _cameraSensorList;
        [SerializeField] private PoissonGenerator _poissonGenerator;
        [SerializeField] DiabloControl _diabloControl;
        private int _lastRoundImageIndex = 0;
        private Rigidbody _diabloRigidbody;
        void Awake() {
            _diabloRigidbody = _diabloControl.GetComponent<Rigidbody>();
        }

        void Start() {
            DataGenerationEvents.SampleAndSaveImagesCoroutine = SampleAndSaveImages;
            DataGenerationEvents.BeforePoissonGeneration += OnBeforePoissonGeneration;
        }

        void OnDestroy() {
            DataGenerationEvents.SampleAndSaveImagesCoroutine = null;
            DataGenerationEvents.BeforePoissonGeneration -= OnBeforePoissonGeneration;
        }

        private void OnBeforePoissonGeneration(object sender, EventArgs e) {
            _lastRoundImageIndex = 0;
        }

        private string GetImageFileName(int imageIndex, CameraSensorSettings cameraSensorSettings) {
            string fileNamePrefix = cameraSensorSettings.fileNamePrefix;
            if (string.IsNullOrEmpty(fileNamePrefix)) {
                fileNamePrefix = "image_";
            }
            return $"{fileNamePrefix}{imageIndex}.{cameraSensorSettings.encoding.ToString().ToLower()}";
        }
        private void SaveImage(string fileFullPath, CameraSensorSettings cameraSensorSettings) {
            var cameraSensor = cameraSensorSettings.cameraSensor;
            cameraSensor.UpdateSensorManually();
            var texture = cameraSensorSettings.sourceTexture == SourceTexture.Texture0
                ? cameraSensor.texture0
                : cameraSensor.texture1;

            byte[] imageData;
            switch (cameraSensorSettings.encoding) {
                case Encoding.EXR:
                    imageData = texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
                    break;
                case Encoding.PNG:
                    imageData = texture.EncodeToPNG();
                    break;
                case Encoding.JPG:
                default:
                    imageData = texture.EncodeToJPG();
                    break;
            }
            File.WriteAllBytes(fileFullPath, imageData);

            Debug.Log($"Saved to {fileFullPath}");
        }
        public IEnumerator SampleAndSaveImages(int sceneIndex, int poissonSeed, FilePathManager filePathManager) {
            var samplingPoints = _poissonGenerator.SamplingAndGetPoints(poissonSeed);

            _diabloRigidbody.isKinematic = true;
            for (int i = 0; i < samplingPoints.Count; i++) {
                var samplingPoint = samplingPoints[i];
                int imageIndex = _lastRoundImageIndex + i;

                var rot = Quaternion.LookRotation(samplingPoint.direction);
                _diabloRigidbody.position = samplingPoint.point;
                _diabloRigidbody.rotation = rot;
                _diabloControl.targetEularAngleY = rot.eulerAngles.y;

                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
                Debug.Log($"Sampling Point {i}: {samplingPoint.point}, {rot.eulerAngles}");

                List<string> imageFileNameList = new List<string>();
                foreach (var cameraSensorSettings in _cameraSensorList) {
                    if (cameraSensorSettings.cameraSensor == null) {
                        Debug.LogWarning("Camera sensor is not set, skipping image sampling.");
                        continue;
                    }
                    string imageFileName = GetImageFileName(imageIndex, cameraSensorSettings);
                    string imageFilePath = filePathManager.GetImageFilePath(sceneIndex, imageFileName);
                    SaveImage(imageFilePath, cameraSensorSettings);
                    imageFileNameList.Add(imageFileName);
                }
                DataGenerationEvents.RaiseImageSampled(this,
                    new DataGenerationEvents.ImageSampledEventArgs {
                        ImageFileNameList = imageFileNameList,
                        WorldPosition = samplingPoint.point,
                        WorldYaw = rot.eulerAngles.y
                    });
            }
            _diabloRigidbody.isKinematic = false;
            _lastRoundImageIndex += samplingPoints.Count;
        }

    }
}