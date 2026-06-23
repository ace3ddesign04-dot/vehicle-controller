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

            for (int i = 0; i < wheels.Length; i++) {
                if (wheels[i]) {
                    wheels[i].Simulate(vehicleRigidbody, fixedDeltaTime);
                }

            }
        }
    }
}