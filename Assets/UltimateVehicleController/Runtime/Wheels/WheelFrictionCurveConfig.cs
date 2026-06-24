using UnityEngine;

namespace UltimateVehicleController {
    [System.Serializable]
    public class WheelFrictionCurveConfig {
        [Min(0.001f)]
        [SerializeField] private float extremumSlip = 0.25f;

        [Min(0f)]
        [SerializeField] private float extremumValue = 1.2f;

        [Min(0.001f)]
        [SerializeField] private float asymptoteSlip = 0.8f;

        [Min(0f)]
        [SerializeField] private float asymptoteValue = 0.75f;

        [Min(0f)]
        [SerializeField] private float stiffness = 1f;

        public float ExtremumSlip => extremumSlip;
        public float ExtremumValue => extremumValue;
        public float AsymptoteSlip => asymptoteSlip;
        public float AsymptoteValue => asymptoteValue;
        public float Stiffness => stiffness;

        public float Evaluate(float slip) {
            slip = Mathf.Abs(slip);

            if (slip <= 0f) {
                return 0f;
            }

            if (slip <= extremumSlip) {
                float t = slip / extremumSlip;
                return extremumValue * t * stiffness;
            }

            if (slip <= asymptoteSlip) {
                float t = Mathf.InverseLerp(extremumSlip, asymptoteSlip, slip);
                return Mathf.Lerp(extremumValue, asymptoteValue, t) * stiffness;
            }

            return asymptoteValue * stiffness;
        }
    }
    [System.Serializable]
    public class WheelFrictionConfig {
        [Header("Forward Friction")]
        [SerializeField] private WheelFrictionCurveConfig forwardFriction = new();

        [Header("Sideways Friction")]
        [SerializeField] private WheelFrictionCurveConfig sidewaysFriction = new();

        [Header("Load")]
        [Min(0f)]
        [SerializeField] private float loadSensitivity = 1f;

        [Min(0f)]
        [SerializeField] private float minimumNormalLoad = 100f;

        [Min(0f)]
        [SerializeField] private float maximumNormalLoad = 50000f;

        [Header("Resistance")]
        [Min(0f)]
        [SerializeField] private float rollingResistance = 0.015f;

        public WheelFrictionCurveConfig ForwardFriction => forwardFriction;
        public WheelFrictionCurveConfig SidewaysFriction => sidewaysFriction;

        public float LoadSensitivity => loadSensitivity;
        public float MinimumNormalLoad => minimumNormalLoad;
        public float MaximumNormalLoad => maximumNormalLoad;

        public float RollingResistance => rollingResistance;

        public float GetClampedNormalLoad(float normalLoad) {
            return Mathf.Clamp(normalLoad, minimumNormalLoad, maximumNormalLoad);
        }
    }
}