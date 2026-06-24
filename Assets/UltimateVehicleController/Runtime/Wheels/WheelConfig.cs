using System;
using UnityEngine;

namespace UltimateVehicleController {
    [Serializable]
    public class WheelConfig {
        [Header("Wheel Role")]
        [Min(0)]
        [SerializeField] private int axleIndex = 0;
        [SerializeField] private bool canSteer = true;
        [SerializeField] private bool canDrive = true;
        [SerializeField] private bool canBrake = true;
        [SerializeField] private bool canHandbrake = false;

        [Range(0f, 1f)]
        [SerializeField] private float steeringWeight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float driveWeight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float brakeWeight = 1f;

        [Header("Contact")]
        [SerializeField] private WheelContactMode contactMode = WheelContactMode.Raycast;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        [Min(0f)]
        [SerializeField] private float castStartOffset = 0.05f;

        [Header("Shape")]
        [Min(0.01f)]
        [SerializeField] private float radius = 0.45f;
        [Min(0.01f)]
        [SerializeField] private float width = 0.35f;

        [Header("Rotation")]
        [Min(0.01f)]
        [SerializeField] private float wheelMass = 20f;
        [Min(0.01f)]
        [SerializeField] private float wheelInertia = 1.2f;
        [Min(0f)]
        [SerializeField] private float angularDrag = 0.05f;
        [Min(0f)]
        [SerializeField] private float maxAngularVelocity = 250f;

        [Header("Vertical Suspension")]
        [Min(0.01f)]
        [SerializeField] private float suspensionLength = 0.6f;
        [Min(0f)]
        [SerializeField] private float springStrength = 35000f;
        [Min(0f)]
        [SerializeField] private float damperStrength = 4500f;
        [Min(0f)]
        [SerializeField] private float maxSuspensionForce = 90000f;

        [Header("Horizontal Suspension")]
        [SerializeField] private bool useHorizontalSuspension = true;
        [Min(0f)]
        [SerializeField] private float horizontalSuspensionLength = 0.18f;
        [Min(0f)]
        [SerializeField] private float horizontalSpringStrength = 18000f;
        [Min(0f)]
        [SerializeField] private float horizontalDamperStrength = 2500f;
        [Min(0f)]
        [SerializeField] private float maxForwardVisualOffset = 0.08f;
        [Min(0f)]
        [SerializeField] private float maxRearVisualOffset = 0.12f;
        [Min(0f)]
        [SerializeField] private float maxHorizontalSuspensionForce = 35000f;

        [Header("Multi Ray / Hybrid")]
        [Min(0f)]
        [SerializeField] private float forwardRayOffset = 0.25f;
        [Min(0f)]
        [SerializeField] private float rearRayOffset = 0.25f;
        [Min(0f)]
        [SerializeField] private float sideRayOffset = 0.2f;
        [Min(0f)]
        [SerializeField] private float upRayLength = 0.5f;

        [Header("Friction")]
        [SerializeField] private WheelFrictionConfig wheelFriction = new();

        [Header("Debug")]
        [SerializeField] private bool drawDebug = true;

        public int AxleIndex => axleIndex;

        public bool CanSteer => canSteer;
        public bool CanDrive => canDrive;
        public bool CanBrake => canBrake;
        public bool CanHandbrake => canHandbrake;

        public float SteeringWeight => steeringWeight;
        public float DriveWeight => driveWeight;
        public float BrakeWeight => brakeWeight;

        public WheelContactMode ContactMode => contactMode;
        public LayerMask GroundMask => groundMask;
        public QueryTriggerInteraction TriggerInteraction => triggerInteraction;
        public float CastStartOffset => castStartOffset;

        public float Radius => radius;
        public float Width => width;

        public float WheelMass => wheelMass;
        public float WheelInertia => wheelInertia;
        public float AngularDrag => angularDrag;
        public float MaxAngularVelocity => maxAngularVelocity;

        public float SuspensionLength => suspensionLength;
        public float SpringStrength => springStrength;
        public float DamperStrength => damperStrength;
        public float MaxSuspensionForce => maxSuspensionForce;

        public bool UseHorizontalSuspension => useHorizontalSuspension;
        public float HorizontalSuspensionLength => horizontalSuspensionLength;
        public float HorizontalSpringStrength => horizontalSpringStrength;
        public float HorizontalDamperStrength => horizontalDamperStrength;
        public float MaxForwardVisualOffset => maxForwardVisualOffset;
        public float MaxRearVisualOffset => maxRearVisualOffset;
        public float MaxHorizontalSuspensionForce => maxHorizontalSuspensionForce;

        public float ForwardRayOffset => forwardRayOffset;
        public float RearRayOffset => rearRayOffset;
        public float SideRayOffset => sideRayOffset;
        public float UpRayLength => upRayLength;

        public WheelFrictionConfig WheelFriction => wheelFriction;

        public bool DrawDebug => drawDebug;

        public float CastLength => suspensionLength + radius + castStartOffset;
        public float SuspensionTravelWithRadius => suspensionLength + radius;
    }
}