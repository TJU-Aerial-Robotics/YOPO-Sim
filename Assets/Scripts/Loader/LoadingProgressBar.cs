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

using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour {
    [SerializeField] private Image progressBar;
    private const float dummyProgressTimerMax = 3.0f;
    private float dummyProgressTimer = 0.0f;

    private void Update() {
        float progress = Loader.GetLoadProgress();

        float dummyProgress = 0.0f;
        if (progress >= 0.9f) {
            dummyProgressTimer += Time.deltaTime;
            dummyProgress = dummyProgressTimer / dummyProgressTimerMax * 0.1f;
        }

        progressBar.fillAmount = progress + dummyProgress;
    }
}
