using UnityEngine;

namespace UltimateVehicleController {
    public class Wheel : MonoBehaviour {

        private WheelContact _contact;

        private RaycastHit _mainHit;
        private RaycastHit _forwardHit;
        private RaycastHit _rearHit;
        private RaycastHit _upHit;
        private RaycastHit _leftHit;
        private RaycastHit _rightHit;

        private float _previousCompression;
        private float _suspensionVelocity;
        private float _springForce;
        private float _damperForce;
        private float _suspensionForce;

        private float _previousForwardCompression;
        private float _previousRearCompression;
        private float _forwardHorizontalForce;
        private float _rearHorizontalForce;

        private Vector3 _visualInitialLocalPosition;
        private bool _hasVisual;

        [Header("Config")]
        [SerializeField] private WheelConfig config = new();

        [Header("Visual")]
        [SerializeField] private Transform visual;

        private void Awake() {
            _hasVisual = visual != null;

            if (_hasVisual) {
                _visualInitialLocalPosition = visual.localPosition;
            }
        }

        public WheelConfig Config => config;
        public Transform Visual => visual;

        public ref readonly WheelContact Contact => ref _contact;

        public bool IsGrounded => _contact.HasGroundContact;

        public float SuspensionVelocity => _suspensionVelocity;
        public float SpringForce => _springForce;
        public float DamperForce => _damperForce;
        public float SuspensionForce => _suspensionForce;

        public float ForwardHorizontalForce => _forwardHorizontalForce;
        public float RearHorizontalForce => _rearHorizontalForce;

        public Vector3 MountPosition => transform.position;
        public Vector3 Up => transform.up;
        public Vector3 Down => -transform.up;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;

        public void Simulate(Rigidbody vehicleRigidbody, float fixedDeltaTime) {
            SimulateContact();
            ApplyVerticalSuspension(vehicleRigidbody, fixedDeltaTime);
            ApplyHorizontalSuspension(vehicleRigidbody, fixedDeltaTime);
            UpdateVisual();
        }

        private void SimulateContact() {
            _contact.Clear();

            switch (config.ContactMode) {
                case WheelContactMode.Raycast:
                    DetectRaycastContact();
                    break;

                case WheelContactMode.SphereCast:
                    DetectSphereCastContact();
                    break;

                case WheelContactMode.MultiRaycast:
                    DetectMultiRaycastContact();
                    break;

                case WheelContactMode.Hybrid:
                    DetectHybridContact();
                    break;
            }
        }

        private void ApplyVerticalSuspension(Rigidbody vehicleRigidbody, float fixedDeltaTime) {
            if (vehicleRigidbody == null || fixedDeltaTime <= 0f) {
                ResetVerticalSuspension();
                return;
            }

            if (!_contact.HasGroundContact) {
                ResetVerticalSuspension();
                return;
            }

            _suspensionVelocity = (_contact.Compression - _previousCompression) / fixedDeltaTime;

            _springForce = _contact.Compression * config.SpringStrength;
            _damperForce = _suspensionVelocity * config.DamperStrength;

            _suspensionForce = Mathf.Clamp(
                _springForce + _damperForce,
                0f,
                config.MaxSuspensionForce);

            vehicleRigidbody.AddForceAtPosition(
                Up * _suspensionForce,
                _contact.Point,
                ForceMode.Force);

            _previousCompression = _contact.Compression;
        }

        private void ApplyHorizontalSuspension(Rigidbody vehicleRigidbody, float fixedDeltaTime) {
            if (vehicleRigidbody == null || fixedDeltaTime <= 0f || !config.UseHorizontalSuspension) {
                ResetHorizontalSuspension();
                return;
            }

            _forwardHorizontalForce = 0f;
            _rearHorizontalForce = 0f;

            if (_contact.HasForwardContact) {
                float compression = Mathf.Clamp(
                    config.HorizontalSuspensionLength - _contact.ForwardDistance,
                    0f,
                    config.HorizontalSuspensionLength);

                float velocity = (compression - _previousForwardCompression) / fixedDeltaTime;

                float springForce = compression * config.HorizontalSpringStrength;
                float damperForce = velocity * config.HorizontalDamperStrength;

                _forwardHorizontalForce = Mathf.Clamp(
                    springForce + damperForce,
                    0f,
                    config.MaxHorizontalSuspensionForce);

                vehicleRigidbody.AddForceAtPosition(
                    -Forward * _forwardHorizontalForce,
                    _contact.ForwardPoint,
                    ForceMode.Force);

                _previousForwardCompression = compression;
            }
            else {
                _previousForwardCompression = 0f;
            }

            if (_contact.HasRearContact) {
                float compression = Mathf.Clamp(
                    config.HorizontalSuspensionLength - _contact.RearDistance,
                    0f,
                    config.HorizontalSuspensionLength);

                float velocity = (compression - _previousRearCompression) / fixedDeltaTime;

                float springForce = compression * config.HorizontalSpringStrength;
                float damperForce = velocity * config.HorizontalDamperStrength;

                _rearHorizontalForce = Mathf.Clamp(
                    springForce + damperForce,
                    0f,
                    config.MaxHorizontalSuspensionForce);

                vehicleRigidbody.AddForceAtPosition(
                    Forward * _rearHorizontalForce,
                    _contact.RearPoint,
                    ForceMode.Force);

                _previousRearCompression = compression;
            }
            else {
                _previousRearCompression = 0f;
            }
        }
        private void UpdateVisual() {
            if (!_hasVisual) {
                return;
            }

            float verticalOffset = _contact.HasGroundContact
                ? _contact.Compression
                : 0f;

            float horizontalOffset = 0f;

            if (config.UseHorizontalSuspension) {
                if (_contact.HasForwardContact) {
                    float compression01 = config.HorizontalSuspensionLength > 0f
                        ? Mathf.Clamp01((config.HorizontalSuspensionLength - _contact.ForwardDistance) / config.HorizontalSuspensionLength)
                        : 0f;

                    horizontalOffset = -compression01 * config.MaxRearVisualOffset;
                }
                else if (_contact.HasRearContact) {
                    float compression01 = config.HorizontalSuspensionLength > 0f
                        ? Mathf.Clamp01((config.HorizontalSuspensionLength - _contact.RearDistance) / config.HorizontalSuspensionLength)
                        : 0f;

                    horizontalOffset = compression01 * config.MaxForwardVisualOffset;
                }
            }

            visual.localPosition = _visualInitialLocalPosition
                                   + Vector3.up * verticalOffset
                                   + Vector3.forward * horizontalOffset;
        }

        private void ResetVerticalSuspension() {
            _previousCompression = 0f;
            _suspensionVelocity = 0f;
            _springForce = 0f;
            _damperForce = 0f;
            _suspensionForce = 0f;
        }

        private void ResetHorizontalSuspension() {
            _previousForwardCompression = 0f;
            _previousRearCompression = 0f;
            _forwardHorizontalForce = 0f;
            _rearHorizontalForce = 0f;
        }

        private void DetectRaycastContact() {
            Vector3 origin = GetCastOrigin();

            if (Physics.Raycast(
                    origin,
                    Down,
                    out _mainHit,
                    config.CastLength,
                    config.GroundMask,
                    config.TriggerInteraction)) {
                FillGroundContact(_mainHit);
            }
        }

        private void DetectSphereCastContact() {
            Vector3 origin = GetCastOrigin();

            if (Physics.SphereCast(
                    origin,
                    config.Radius,
                    Down,
                    out _mainHit,
                    config.CastLength,
                    config.GroundMask,
                    config.TriggerInteraction)) {
                FillGroundContact(_mainHit);
            }
        }

        private void DetectMultiRaycastContact() {
            DetectRaycastContact();
            DetectExtraContacts();
        }

        private void DetectHybridContact() {
            DetectSphereCastContact();
            DetectExtraContacts();
        }

        private void DetectExtraContacts() {
            DetectDirectionalContact(
                MountPosition + Forward * config.ForwardRayOffset,
                Forward,
                config.HorizontalSuspensionLength,
                ref _forwardHit,
                ref _contact.HasForwardContact,
                ref _contact.ForwardPoint,
                ref _contact.ForwardNormal,
                ref _contact.ForwardDistance);

            DetectDirectionalContact(
                MountPosition - Forward * config.RearRayOffset,
                -Forward,
                config.HorizontalSuspensionLength,
                ref _rearHit,
                ref _contact.HasRearContact,
                ref _contact.RearPoint,
                ref _contact.RearNormal,
                ref _contact.RearDistance);

            DetectDirectionalContact(
                MountPosition,
                Up,
                config.UpRayLength,
                ref _upHit,
                ref _contact.HasUpContact,
                ref _contact.UpPoint,
                ref _contact.UpNormal,
                ref _contact.UpDistance);

            DetectSideContact();
        }

        private void DetectSideContact() {
            bool hasLeft = Physics.Raycast(
                MountPosition,
                -Right,
                out _leftHit,
                config.SideRayOffset,
                config.GroundMask,
                config.TriggerInteraction);

            bool hasRight = Physics.Raycast(
                MountPosition,
                Right,
                out _rightHit,
                config.SideRayOffset,
                config.GroundMask,
                config.TriggerInteraction);

            if (!hasLeft && !hasRight) {
                return;
            }

            RaycastHit hit = hasLeft && hasRight
                ? _leftHit.distance <= _rightHit.distance ? _leftHit : _rightHit
                : hasLeft ? _leftHit : _rightHit;

            _contact.HasSideContact = true;
            _contact.SidePoint = hit.point;
            _contact.SideNormal = hit.normal;
            _contact.SideDistance = hit.distance;
        }

        private void DetectDirectionalContact(
            Vector3 origin,
            Vector3 direction,
            float distance,
            ref RaycastHit hit,
            ref bool hasContact,
            ref Vector3 point,
            ref Vector3 normal,
            ref float hitDistance) {
            if (distance <= 0f) {
                return;
            }

            if (!Physics.Raycast(
                    origin,
                    direction,
                    out hit,
                    distance,
                    config.GroundMask,
                    config.TriggerInteraction)) {
                return;
            }

            hasContact = true;
            point = hit.point;
            normal = hit.normal;
            hitDistance = hit.distance;
        }

        private void FillGroundContact(RaycastHit hit) {
            float distanceFromMount = Mathf.Max(0f, hit.distance - config.CastStartOffset);

            float compression = Mathf.Clamp(
                config.SuspensionTravelWithRadius - distanceFromMount,
                0f,
                config.SuspensionLength);

            float compression01 = config.SuspensionLength > 0f
                ? compression / config.SuspensionLength
                : 0f;

            _contact.HasGroundContact = true;
            _contact.Point = hit.point;
            _contact.Normal = hit.normal;
            _contact.Distance = distanceFromMount;
            _contact.Compression = compression;
            _contact.Compression01 = compression01;
            _contact.Collider = hit.collider;
            _contact.Rigidbody = hit.rigidbody;
        }

        private Vector3 GetCastOrigin() {
            return MountPosition + Up * config.CastStartOffset;
        }

        private void OnDrawGizmosSelected() {
            if (config == null || !config.DrawDebug) {
                return;
            }

            Vector3 origin = transform.position + transform.up * config.CastStartOffset;
            Vector3 down = -transform.up;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Gizmos.DrawLine(origin, origin + down * config.CastLength);

            Gizmos.DrawLine(transform.position, transform.position + forward * config.HorizontalSuspensionLength);
            Gizmos.DrawLine(transform.position, transform.position - forward * config.HorizontalSuspensionLength);

            Gizmos.DrawLine(transform.position, transform.position + transform.up * config.UpRayLength);

            Gizmos.DrawLine(transform.position, transform.position + right * config.SideRayOffset);
            Gizmos.DrawLine(transform.position, transform.position - right * config.SideRayOffset);

            if (_contact.HasGroundContact) {
                Gizmos.DrawSphere(_contact.Point, 0.04f);
                Gizmos.DrawLine(_contact.Point, _contact.Point + _contact.Normal * 0.35f);
            }

            if (_contact.HasForwardContact) {
                Gizmos.DrawSphere(_contact.ForwardPoint, 0.035f);
            }

            if (_contact.HasRearContact) {
                Gizmos.DrawSphere(_contact.RearPoint, 0.035f);
            }

            if (_contact.HasUpContact) {
                Gizmos.DrawSphere(_contact.UpPoint, 0.035f);
            }

            if (_contact.HasSideContact) {
                Gizmos.DrawSphere(_contact.SidePoint, 0.035f);
            }
        }
    }
}