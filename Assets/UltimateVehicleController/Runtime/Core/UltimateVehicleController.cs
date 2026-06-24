using UnityEngine;

namespace UltimateVehicleController {
    [RequireComponent(typeof(Rigidbody))]
    public class UltimateVehicleController : MonoBehaviour {

        [Header("References")]
        [SerializeField] private Rigidbody vehicleRigidbody;
        [SerializeField] private Wheel[] wheels;

        [Header("Rigidbody Setup")]
        [SerializeField] private bool applyCenterOfMass = true;
        [SerializeField] private Vector3 centerOfMassOffset = new(0f, -0.35f, 0f);

        [Header("Test Input")]
        [SerializeField] private bool useTestInput = true;
        [SerializeField] private float testDriveTorque = 600f;
        [SerializeField] private float testBrakeTorque = 1200f;

        private void Reset() {
            vehicleRigidbody = GetComponent<Rigidbody>();
            wheels = GetComponentsInChildren<Wheel>();
        }

        private void Awake() {
            if (vehicleRigidbody == null) {
                vehicleRigidbody = GetComponent<Rigidbody>();
            }

            if (wheels == null || wheels.Length == 0) {
                wheels = GetComponentsInChildren<Wheel>();
            }

            if (applyCenterOfMass) {
                vehicleRigidbody.centerOfMass = centerOfMassOffset;
            }
        }

        private void FixedUpdate() {
            float fixedDeltaTime = Time.fixedDeltaTime;

            float throttle = 0f;
            float brake = 0f;

            if (useTestInput) {
                throttle = Input.GetKey(KeyCode.W) ? 1f : 0f;
                brake = Input.GetKey(KeyCode.S) ? 1f : 0f;
            }

            for (int i = 0; i < wheels.Length; i++) {
                Wheel wheel = wheels[i];

                if (wheel == null) {
                    continue;
                }

                wheel.SetDriveTorque(throttle * testDriveTorque);
                wheel.SetBrakeTorque(brake * testBrakeTorque);

                wheel.Simulate(vehicleRigidbody, fixedDeltaTime);
            }
        }
    }
}