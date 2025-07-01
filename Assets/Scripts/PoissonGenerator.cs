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

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace YOPO.SIM {
    public struct SamplingPoints {
        public float3 point;
        public float3 direction;
        public float2 localPoint;
    }

    // Modified from: https://github.com/SebLague/Poisson-Disc-Sampling.git
    // Licensed under the MIT License
    class PoissonGenerator : MonoBehaviour {
        [SerializeField] private uint _seed = 0;
        [SerializeField] private LayerMask _rejectionLayer;
        [SerializeField, Range(1, 100.0f)] private float _minRadius = 10;
        [SerializeField, Range(1, 50)] private int _samplesThreshold = 30;
        [SerializeField, Range(0.1f, 10)] private float _displayRadius = 1;
        [SerializeField, Range(0, 10)] private float _inflateRadius = 3;
        [SerializeField, Range(0, 5)] private float _elevation = 0.5f;
        [SerializeField] private bool _drawDebug = false;
        [SerializeField] private bool _test = false;
        private List<SamplingPoints> _samplingPoints;
        private Bounds _sampleRegionBound;
        private float _cellSize;
        private int[,] _grid;
        private float _maxHitDistance;
        private float3 _realCandidateOffset;
        private Unity.Mathematics.Random _random;
        public void Init() {
            _random = new Unity.Mathematics.Random(_seed + 1);
            _sampleRegionBound.center = transform.position;
            _sampleRegionBound.size = transform.lossyScale;
            _samplingPoints = new List<SamplingPoints>();
            _cellSize = _minRadius / math.SQRT2;
            _grid = new int[Mathf.CeilToInt(_sampleRegionBound.size.x / _cellSize), Mathf.CeilToInt(_sampleRegionBound.size.z / _cellSize)];
            _maxHitDistance = transform.lossyScale.y;
            _realCandidateOffset = transform.position - new Vector3(transform.lossyScale.x, -transform.lossyScale.y, transform.lossyScale.z) / 2;
        }
        public void GeneratorPoints() {
            _samplingPoints.Clear();
            // FIXME: If there is an obstacle in the center, the sampling points will be empty
            var spawnPoints = new List<float2> { new(_sampleRegionBound.extents.x, _sampleRegionBound.extents.z) };

            while (spawnPoints.Count > 0) {
                int spawnIndex = _random.NextInt(spawnPoints.Count);
                float2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < _samplesThreshold; i++) {
                    float2 candidate = spawnCenter + _random.NextFloat2Direction() * _random.NextFloat(_minRadius, 2 * _minRadius);
                    float3 realCandidate = new float3(candidate.x, 0, candidate.y) + _realCandidateOffset;
                    if (IsValid(candidate, realCandidate, _cellSize, _minRadius, _grid)) {
                        float2 dir = _random.NextFloat2Direction();
                        Physics.Raycast(realCandidate, Vector3.down, out RaycastHit hit, _maxHitDistance);
                        var candidatePoint = new SamplingPoints {
                            point = realCandidate + new float3(0, _elevation - hit.distance, 0),
                            direction = new float3(dir.x, 0, dir.y),
                            localPoint = candidate
                        };
                        _samplingPoints.Add(candidatePoint);
                        spawnPoints.Add(candidate);
                        _grid[(int)(candidate.x / _cellSize), (int)(candidate.y / _cellSize)] = _samplingPoints.Count;
                        candidateAccepted = true;
                        break;
                    }
                }
                if (!candidateAccepted) {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }
            Debug.Log("Points: " + _samplingPoints.Count);
        }
        private bool IsValid(float2 candidate, float3 realCandidate, float cellSize, float radius, int[,] grid) {
            if (_sampleRegionBound.Contains(realCandidate)) {
                int cellX = (int)(candidate.x / cellSize);
                int cellY = (int)(candidate.y / cellSize);
                int searchStartX = Mathf.Max(0, cellX - 2);
                int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Mathf.Max(0, cellY - 2);
                int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

                bool hitResult = Physics.SphereCast(realCandidate, _inflateRadius, Vector3.down, out RaycastHit _, _maxHitDistance, _rejectionLayer);
                if (hitResult) return false;

                for (int x = searchStartX; x <= searchEndX; x++)
                    for (int y = searchStartY; y <= searchEndY; y++) {
                        int pointIndex = grid[x, y] - 1;
                        if (pointIndex != -1) {
                            float dist = math.length(candidate - _samplingPoints[pointIndex].localPoint);
                            if (dist < radius) return false;
                        }
                    }
                return true;
            }
            return false;
        }
        private void OnDrawGizmos() {
            if (_drawDebug) {
                Gizmos.color = Color.red;
                var wireCubeSize = transform.lossyScale;
                Gizmos.DrawWireCube(transform.position, wireCubeSize);
                if (_samplingPoints == null) return;
                foreach (var point in _samplingPoints) {
                    Gizmos.DrawSphere(point.point, _displayRadius);
                    Gizmos.DrawLine(point.point, point.point + point.direction * _displayRadius * 2);
                }
            }
        }
        private void OnValidate() {
            if (_test && gameObject.activeInHierarchy) {
                Init();
                GeneratorPoints();
            }
        }
        public List<SamplingPoints> GetSamplingPoints() {
            return _samplingPoints;
        }
        public List<SamplingPoints> SamplingAndGetPoints(uint seed) {
            _seed = seed;
            Init();
            GeneratorPoints();
            return GetSamplingPoints();
        }
    }
}