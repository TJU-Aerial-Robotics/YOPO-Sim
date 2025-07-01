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
using System.Collections;
using Pinwheel.Vista;
using Pinwheel.Vista.Graph;
using TMPro;
using UnityEngine;

namespace YOPO.SIM.UI {
    public class TreeDensitySetter : MonoBehaviour {
        [SerializeField] private TMP_Dropdown _densityDropdown;
        [SerializeField] private TerrainGraph _denseTreeGraph;
        [SerializeField] private TerrainGraph _normalTreeGraph;
        [SerializeField] private TerrainGraph _sparseTreeGraph;
        [SerializeField] private VistaManager _vistaManager;
        [SerializeField] private LocalProceduralBiome _biome;
        [SerializeField] private bool _isDebug = false;

        void Start() {
            _densityDropdown.onValueChanged.AddListener(OnDensityChanged);
        }

        void Update() {
            if (_isDebug && Input.GetKeyDown(KeyCode.Alpha1)) {
                _densityDropdown.value = 0; // Dense
            }
            else if (_isDebug && Input.GetKeyDown(KeyCode.Alpha2)) {
                _densityDropdown.value = 1; // Normal
            }
            else if (_isDebug && Input.GetKeyDown(KeyCode.Alpha3)) {
                _densityDropdown.value = 2; // Sparse
            }
        }

        private void OnDensityChanged(int arg0) {
            Debug.Log($"Selected density: {arg0}");
            _biome.terrainGraph = arg0 switch {
                0 => _denseTreeGraph,
                1 => _normalTreeGraph,
                2 => _sparseTreeGraph,
                _ => _normalTreeGraph,
            };
            StartCoroutine(GenerateTerrain());
        }


        private IEnumerator GenerateTerrain() {
            // Wait for the terrain to be generated
            var generationTask = _vistaManager.ForceGenerate();
            yield return new WaitUntil(() => generationTask.isCompleted);
            // Refresh colliders after generation
            RefreshTerrainColliders();
        }
        private void RefreshTerrainColliders() {
            foreach (ITile tile in _vistaManager.GetTiles()) {
                if (tile.gameObject.TryGetComponent(out TerrainCollider terrainCollider)) {
                    terrainCollider.enabled = false;
                    terrainCollider.enabled = true;
                }
            }
        }
    }
}