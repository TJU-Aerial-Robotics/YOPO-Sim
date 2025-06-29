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