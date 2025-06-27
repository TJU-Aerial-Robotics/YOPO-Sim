using System.Collections;
using System.Collections.Generic;
using Pinwheel.Vista;
using UnityEngine;
using Diablo.Utils;
using VoxelGenerator;
using System;

namespace Diablo {
    public class PCGManager : MonoBehaviour {
        // TODO: Use delegate to make the code more modular
        [SerializeField] private LocalProceduralBiome _localProceduralBiome;
        [SerializeField] private VistaManager _vistaManager;
        [SerializeField] private int _terrainSeedStart;
        [SerializeField, ReadOnly] private int _terrainSeed;
        [SerializeField] private PoissonGenerator _poissonGenerator;
        [SerializeField] private int _poissonSeedStart;
        [SerializeField, ReadOnly] private int _poissonSeed;
        [SerializeField] private VoxelGeneratorOverlap _voxelGeneratorTree;
        [SerializeField] private VoxelGeneratorRaycast _voxelGeneratorTerrain;
        [SerializeField] private DepthImageManager _depthImageManager;
        [SerializeField] private DiabloControl _diabloControl;
        [SerializeField] private TomlDataManager _tomlDataManager;
        [SerializeField, Range(0, 10)] private int _terrainRoundNum = 1;
        [SerializeField, Range(0, 10)] private int _poissonRoundNum = 1;
        [SerializeField] private DataManager _dataManager;
        [SerializeField] private bool _isDebug = false;
        private int _imageIndexLast = 0;
        public int sceneIndex { get => _terrainSeed; }
        void Start() {
            _localProceduralBiome.seed = _terrainSeed;
            StartCoroutine(_vistaManager.ForceGenerate());
        }
        void Update() {
            if (_isDebug && Input.GetKeyDown(KeyCode.Space)) {
                StartCoroutine(RunForAll(_terrainRoundNum, _poissonRoundNum));
            }
        }
        void OnValidate() {
            if (_localProceduralBiome && _localProceduralBiome.seed != _terrainSeed) {
                _localProceduralBiome.seed = _terrainSeed;
                _vistaManager.ForceGenerate();
            }

            if (_poissonGenerator && _poissonGenerator.seed != _poissonSeed) {
                _poissonGenerator.seed = _poissonSeed;
                _poissonGenerator.Init();
                _poissonGenerator.GeneratorPoints();
            }
        }
        IEnumerator SavePointCloud() {
            _localProceduralBiome.seed = _terrainSeed;
            yield return _vistaManager.ForceGenerate();

            _voxelGeneratorTree.SetFileProperties(true, folderName: _dataManager.GetFullSceneFolderPath(sceneIndex), fileName: _dataManager.GetTreeFileName());
            _voxelGeneratorTerrain.SetFileProperties(true, folderName: _dataManager.GetFullSceneFolderPath(sceneIndex), fileName: _dataManager.GetTerrainFileName());
            yield return _voxelGeneratorTree.GeneratePointCloud();
            _voxelGeneratorTree.ClearPointCloud();
            yield return _voxelGeneratorTerrain.GeneratePointCloud();
            _voxelGeneratorTerrain.ClearPointCloud();
        }
        IEnumerator SampleAndSaveDepth() {
            var samplingPoints = _poissonGenerator.SamplingAndGetPoints(_poissonSeed);

            var rb = _diabloControl.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            for (int i = 0; i < samplingPoints.Count; i++) {
                var samplingPoint = samplingPoints[i];
                var imageFileName = _dataManager.GetTextrueFileName(_imageIndexLast + i);
                var imageFullPath = _dataManager.GetFullTextureFilePath(sceneIndex, _imageIndexLast + i);

                var rot = Quaternion.LookRotation(samplingPoint.direction);
                Debug.Log($"Sampling Point {i}: {samplingPoint.point}, {rot.eulerAngles}");
                rb.position = samplingPoint.point;
                rb.rotation = rot;
                _diabloControl.targetEularAngleY = rot.eulerAngles.y;


                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();

                // startPos ans startYaw in unity follows left-hand rule, while in toml follows right-hand rule
                var startPos = new List<float> { samplingPoint.point.z, -samplingPoint.point.x };
                var startYaw = 360.0f - rot.eulerAngles.y;
                Debug.Log($"Start yaw in FLU: {startYaw}");
                _tomlDataManager.AddData(imageFileName, startPos, startYaw);

                _depthImageManager.SaveImage(imageFullPath);
            }
            rb.isKinematic = false;
            _imageIndexLast += samplingPoints.Count;
        }
        IEnumerator RunForAll(int terrainRoundNum, int poissonRoundNum) {
            int targetFrameRateBackup = GameSettings.Instance.TargetFrameRate;
            bool enableVSyncBackup = GameSettings.Instance.EnableVSync;
            GameSettings.Instance.ChangeSettings(0, enableVSync: false);
            _dataManager.ClearData();
            int total_poissonSeed = 0;
            for (int terrainSeed = _terrainSeedStart; terrainSeed < terrainRoundNum + _terrainSeedStart; terrainSeed++) {
                _terrainSeed = terrainSeed;
                yield return SavePointCloud();
                _tomlDataManager.Init();
                _imageIndexLast = 0;
                for (int poissonSeed = _poissonSeedStart; poissonSeed < poissonRoundNum + _poissonSeedStart; poissonSeed++) {
                    _poissonSeed = total_poissonSeed;
                    yield return SampleAndSaveDepth();
                    total_poissonSeed++;
                }
                string tomlFullPath = _dataManager.GetFullTomlFilePath(sceneIndex);
                _tomlDataManager.SaveData(tomlFullPath);
            }
            GameSettings.Instance.ChangeSettings(targetFrameRateBackup, enableVSync: enableVSyncBackup);
        }
    }
}
