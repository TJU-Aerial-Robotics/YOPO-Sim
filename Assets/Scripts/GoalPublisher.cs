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

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using UnityEngine;

namespace YOPO.SIM {
    public class GoalPublisher : MonoBehaviour {
        [SerializeField] private string _goalPoseTopic = "/goal_pose";
        [SerializeField] private float _publishPeriod = 1;
        private float _publishTimer;
        private ROSConnection _rosConnector;
        private PoseStampedMsg _poseMsg;

        void Awake() {
            _poseMsg = new();
            _poseMsg.header.frame_id = "map";
        }

        void Start() {
            _rosConnector = ROSConnection.GetOrCreateInstance();
            _rosConnector.RegisterPublisher<PoseStampedMsg>(_goalPoseTopic);
        }

        void Update() {
            _publishTimer += Time.deltaTime;
            if (_publishTimer >= _publishPeriod) {
                _publishTimer -= _publishPeriod;
                var position = transform.position.To<FLU>();
                _poseMsg.pose.position.x = position.x;
                _poseMsg.pose.position.y = position.y;
                _rosConnector.Publish(_goalPoseTopic, _poseMsg);
            }
        }
    }
}