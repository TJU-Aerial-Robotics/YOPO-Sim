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

using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nav;

namespace Diablo {
    public class DiabloControlROS : MonoBehaviour {
        [SerializeField] private DiabloControl diabloControl;
        [SerializeField] private bool subscribeTwistTopic = true;
        [SerializeField] private string twistTopic = "/cmd_vel";
        [SerializeField] private string odomTopic = "/odom";
        [SerializeField] private string odomLocalVelocityTopic = "/odom_local_vel";
        [SerializeField] private float odomUpdateFrequency = 10;
        [SerializeField] private bool useRigidbodyPosition = true;
        private ROSConnection _rosConnector;
        private float _frequency_inv;
        OdometryMsg _odomMsg;
        OdometryMsg _odomLocalVelocityMsg;
        private float _odomTimer;
        private Rigidbody _rigidbody;
        void Awake() {
            _odomMsg = new();
            _odomMsg.header.frame_id = "map";
            _odomMsg.child_frame_id = "base_link";

            _odomLocalVelocityMsg = new();
            _odomLocalVelocityMsg.child_frame_id = "base_link";

            _rigidbody = diabloControl.GetComponent<Rigidbody>();
            _frequency_inv = 1.0f / odomUpdateFrequency;
        }
        void Start() {
            _rosConnector = ROSConnection.GetOrCreateInstance();
            _rosConnector.RegisterPublisher<OdometryMsg>(odomTopic);
            _rosConnector.RegisterPublisher<OdometryMsg>(odomLocalVelocityTopic);
            if (subscribeTwistTopic) {
                _rosConnector.Subscribe<TwistMsg>(twistTopic, TwistCallback);
            }
        }


        private void TwistCallback(TwistMsg msg) {
            // Unity uses left-handed coordinate system, ROS uses right-handed coordinate system
            bool isBraking = Mathf.Approximately((float)msg.linear.x, 0);
            // Debug.Log("Twist msg: " + msg.linear.x + ", " + msg.angular.z + ", is breaking: " + isBraking);
            diabloControl.SetCarState((float)msg.linear.x, (float)-msg.angular.z, isBraking);
            diabloControl.SetKeyboardControl(false);
        }

        private void Update() {
            _odomTimer += Time.deltaTime;
            if (_odomTimer < _frequency_inv) return;

            _odomMsg.header.stamp = new();
#if ROS2
            int sec = (int)(Time.time);
#else
            uint sec = (uint)Time.time;
#endif
            _odomMsg.header.stamp.sec = sec;
            _odomMsg.header.stamp.nanosec = (uint)((Time.time - _odomMsg.header.stamp.sec) * 1e9);

            if (useRigidbodyPosition) {
                _odomMsg.pose.pose.position = _rigidbody.position.To<FLU>();
                _odomMsg.pose.pose.orientation = _rigidbody.rotation.To<FLU>();
            }
            else {
                _odomMsg.pose.pose.position = transform.position.To<FLU>();
                _odomMsg.pose.pose.orientation = transform.rotation.To<FLU>();
            }
            _odomMsg.pose.pose.position.z += 0.25f;

            //TODO: linear velocity should be in base_link frame instead of map frame
            // var localVelocity = _rigidbody.transform.InverseTransformDirection(_rigidbody.velocity);
            _odomMsg.twist.twist.linear = _rigidbody.velocity.To<FLU>();
            _odomMsg.twist.twist.angular = _rigidbody.angularVelocity.To<FLU>();

            _odomLocalVelocityMsg.header = _odomMsg.header;
            _odomLocalVelocityMsg.pose = _odomMsg.pose;
            _odomLocalVelocityMsg.twist.twist.linear = transform.InverseTransformDirection(_rigidbody.velocity).To<FLU>();
            _odomLocalVelocityMsg.twist.twist.angular = transform.InverseTransformDirection(_rigidbody.angularVelocity).To<FLU>();

            _rosConnector.Publish(odomTopic, _odomMsg);
            _rosConnector.Publish(odomLocalVelocityTopic, _odomLocalVelocityMsg);
            // Debug.Log($"Publish odom: {_odomMsg.pose.pose.position}, {_odomMsg.pose.pose.orientation}, " +
            //           $"{_odomMsg.twist.twist.linear}, {_odomMsg.twist.twist.angular}");
            // Debug.Log($"Publish odom_local_vel: {_odomLocalVelocityMsg.pose.pose.position}, {_odomLocalVelocityMsg.pose.pose.orientation}, " +
            //           $"{_odomLocalVelocityMsg.twist.twist.linear}, {_odomLocalVelocityMsg.twist.twist.angular}");
            _odomTimer -= _frequency_inv;
        }
    }

}