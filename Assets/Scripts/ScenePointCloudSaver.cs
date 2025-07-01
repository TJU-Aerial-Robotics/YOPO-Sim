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

#if VISTA
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
#endif