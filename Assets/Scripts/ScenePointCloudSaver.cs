using System.Collections;
using Pinwheel.Vista;
using UnityEngine;
using VoxelGenerator;
namespace YOPO.SIM {

    class ScenePointCloudSaver : MonoBehaviour {
        [SerializeField] private VistaManager _vistaManager;
        [SerializeField] private LocalProceduralBiome _biome;
        [SerializeField] private VoxelGeneratorOverlap _voxelGeneratorTree;
        [SerializeField] private VoxelGeneratorRaycast _voxelGeneratorTerrain;

        void Start() {
            DataGenerationEvents.SavePointCloudCoroutine = SavePointCloud;
        }
        void OnDestroy() {
            DataGenerationEvents.SavePointCloudCoroutine = null;
        }

        IEnumerator SavePointCloud(int terrainSeed, int sceneIndex, FilePathManager filePathManager) {
            _biome.seed = terrainSeed;
            yield return _vistaManager.ForceGenerate();

            string folderName = filePathManager.GetSceneFolderPath(sceneIndex);
            string treeFileName = filePathManager.GetTreeFileName();
            string terrainFileName = filePathManager.GetTerrainFileName();

            _voxelGeneratorTree.SetFileProperties(true, folderName: folderName, fileName: treeFileName);
            yield return _voxelGeneratorTree.GeneratePointCloud();
            _voxelGeneratorTree.ClearPointCloud();

            _voxelGeneratorTerrain.SetFileProperties(true, folderName: folderName, fileName: terrainFileName);
            yield return _voxelGeneratorTerrain.GeneratePointCloud();
            _voxelGeneratorTerrain.ClearPointCloud();
        }
    }
}