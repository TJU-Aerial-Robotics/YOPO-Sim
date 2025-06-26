using CSPID;
using UnityEngine;

namespace Diablo {
    public class DiabloControl : MonoBehaviour {
        [SerializeField, Header("Keyboard Params")] private bool useKeyboard = true;
        [SerializeField] private float velocityMax = 2;
        [SerializeField] private float angularVelocityMax = Mathf.PI;
        [SerializeField, Header("Rigidbody Params")] private float mass = 20;
        [SerializeField] private Vector3 centreOfMass = new(0, -0.1f, 0);
        [SerializeField, Header("Wheel Collider Params")] private WheelCollider wheelCollider;
        [SerializeField] private float motorTorqueMax = 10;
        [SerializeField] private float brakeTorque = 10;
        [SerializeField] private float jumpTargetPosition = 0.1f;
        [SerializeField] private float jumpSpringStrength = 8000.0f;
        [SerializeField, Header("Upright Torque Params")] private float uprightSpringStrength = 400;
        [SerializeField] private float uprightSpringDamper = 20;
        [SerializeField, Header("Wheel PID Params")] private float Kp = 40;
        [SerializeField] private float Ti = 0.5f;
        [SerializeField] private float Td = 0;
        [SerializeField, Header("Debug")] private bool showCenterOfMass = false;
        [SerializeField] private bool showActualWheel = false;
        [SerializeField] private Transform actualWheelVisual;
        [Range(-180, 180)] public float targetEularAngleY;
        new Rigidbody rigidbody;
        private JointSpring suspensionSpringOriginal;
        private PIDController pidController;
        private float velocity, angularVelocity;
        private bool isBraking, isJumping;

        void Awake() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.mass = mass;
            rigidbody.centerOfMass = centreOfMass;

            suspensionSpringOriginal = wheelCollider.suspensionSpring;
            targetEularAngleY = transform.rotation.eulerAngles.y;

            pidController = new(-5, 5, -motorTorqueMax, motorTorqueMax) {
                MaximumStep = double.MaxValue,
                ProportionalGain = Kp,
                IntegralGain = Kp / Ti,
                DerivativeGain = Kp * Td
            };
        }
        void Update() {
            if (useKeyboard)
                KeyboardCarControl();

            if (showActualWheel) {
                wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
                actualWheelVisual.gameObject.SetActive(true);
                actualWheelVisual.position = pos;
                actualWheelVisual.localScale = new Vector3(wheelCollider.radius * 2, wheelCollider.radius * 2, wheelCollider.radius * 2);
            }
            else {
                actualWheelVisual.gameObject.SetActive(false);
            }
        }

        private void KeyboardCarControl() {
            velocity = Input.GetAxis("Vertical") * velocityMax;
            angularVelocity = Input.GetAxis("Horizontal") * angularVelocityMax;
            isBraking = Input.GetKey(KeyCode.LeftControl);
            isJumping = Input.GetKey(KeyCode.Space);

        }

        private void CarControl(float deltaTime) {
            if (!Mathf.Approximately(velocity, 0)) {
                float realVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
                float errorVelocity = velocity - realVelocity;

                float wheelTorque = (float)pidController.Next(errorVelocity, deltaTime);

                wheelCollider.motorTorque = wheelTorque;
                wheelCollider.brakeTorque = 0;
            }
            else {
                wheelCollider.motorTorque = 0;
                pidController.Reset();
            }

            targetEularAngleY += angularVelocity * Mathf.Rad2Deg * deltaTime;

            if (targetEularAngleY > 180) targetEularAngleY -= 360;
            else if (targetEularAngleY < -180) targetEularAngleY += 360;

            if (isBraking) {
                wheelCollider.motorTorque = 0;
                wheelCollider.brakeTorque = brakeTorque;
                pidController.Reset();
            }
            else {
                wheelCollider.brakeTorque = 0;
            }
            if (isJumping) {
                wheelCollider.suspensionSpring = new() {
                    spring = jumpSpringStrength,
                    damper = 0,
                    targetPosition = jumpTargetPosition,
                };
            }
            else {
                wheelCollider.suspensionSpring = suspensionSpringOriginal;
            }
        }
        private void FixedUpdate() {
            UpdateUprightForce();
            CarControl(Time.fixedDeltaTime);
        }

        private void UpdateUprightForce() {
            var currentRotation = transform.rotation;
            var targetRotation = Quaternion.Euler(0, targetEularAngleY, 0);
            var deltaRotation = targetRotation * Quaternion.Inverse(currentRotation);

            deltaRotation.ToAngleAxis(out float rotationDegrees, out Vector3 rotationAxis);

            if (rotationDegrees > 180) rotationDegrees -= 360;
            else if (rotationDegrees < -180) rotationDegrees += 360;

            rotationAxis.Normalize();

            float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

            var torque = rotationAxis * rotationRadians * uprightSpringStrength - rigidbody.angularVelocity * uprightSpringDamper;

            rigidbody.AddTorque(torque);
        }
        public void SetCarState(float velocity, float angularVelocity, bool isBraking = false, bool isJumping = false) {
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
            this.isBraking = isBraking;
            this.isJumping = isJumping;
        }
        public void SetKeyboardControl(bool useKeyboard) {
            this.useKeyboard = useKeyboard;
        }

        void OnDrawGizmos() {
            if (showCenterOfMass)
                if (rigidbody != null)
                    Gizmos.DrawSphere(transform.TransformPoint(rigidbody.centerOfMass), 0.1f);
        }
    }
}