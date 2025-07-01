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
using System.Collections.Generic;
using Unity.Mathematics;

namespace YOPO.SIM {
    public static class DataGenerationEvents {
        // Poisson generation events
        public static event EventHandler BeforePoissonGeneration;
        public static void RaiseBeforePoissonGeneration(object sender, EventArgs args) => BeforePoissonGeneration?.Invoke(sender, args);
        public class AfterPoissonGenerationEventArgs : EventArgs {
            public int SceneIndex;
            public FilePathManager FilePathManager;
        }
        public static event EventHandler<AfterPoissonGenerationEventArgs> AfterPoissonGeneration;
        public static void RaiseAfterPoissonGeneration(object sender, AfterPoissonGenerationEventArgs args) => AfterPoissonGeneration?.Invoke(sender, args);

        // Point cloud saving events
        public static Func<int, int, FilePathManager, IEnumerator> SavePointCloudCoroutine;
        public static IEnumerator TriggerSavePointCloud(int terrainSeed, int sceneIndex, FilePathManager filePathManager) {
            yield return SavePointCloudCoroutine?.Invoke(terrainSeed, sceneIndex, filePathManager);
        }

        // Image sampling and saving events
        public static Func<int, uint, FilePathManager, IEnumerator> SampleAndSaveImagesCoroutine;
        public static IEnumerator TriggerSampleAndSaveImages(int sceneIndex, uint poissonSeed, FilePathManager filePathManager) {
            yield return SampleAndSaveImagesCoroutine?.Invoke(sceneIndex, poissonSeed, filePathManager);
        }

        public class ImageSampledEventArgs : EventArgs {
            public List<string> ImageFileNameList;
            public float3 WorldPosition; // Unity coordinates (x, y, z)
            public float WorldYaw;       // Unity yaw in degrees
        }
        public static event EventHandler<ImageSampledEventArgs> OnImageSampled;
        public static void RaiseImageSampled(object sender, ImageSampledEventArgs args) => OnImageSampled?.Invoke(sender, args);
    }
}