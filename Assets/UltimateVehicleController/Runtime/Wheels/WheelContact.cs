using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateVehicleController {
    public struct WheelContact {
        public bool HasGroundContact;

        public Vector3 Point;
        public Vector3 Normal;

        public float Distance;
        public float Compression;
        public float Compression01;

        public Collider Collider;
        public Rigidbody Rigidbody;

        public bool HasForwardContact;
        public Vector3 ForwardPoint;
        public Vector3 ForwardNormal;
        public float ForwardDistance;

        public bool HasRearContact;
        public Vector3 RearPoint;
        public Vector3 RearNormal;
        public float RearDistance;

        public bool HasUpContact;
        public Vector3 UpPoint;
        public Vector3 UpNormal;
        public float UpDistance;

        public bool HasSideContact;
        public Vector3 SidePoint;
        public Vector3 SideNormal;
        public float SideDistance;

        public void Clear() {
            HasGroundContact = false;

            Point = Vector3.zero;
            Normal = Vector3.up;

            Distance = 0f;
            Compression = 0f;
            Compression01 = 0f;

            Collider = null;
            Rigidbody = null;

            HasForwardContact = false;
            ForwardPoint = Vector3.zero;
            ForwardNormal = Vector3.zero;
            ForwardDistance = 0f;

            HasRearContact = false;
            RearPoint = Vector3.zero;
            RearNormal = Vector3.zero;
            RearDistance = 0f;

            HasUpContact = false;
            UpPoint = Vector3.zero;
            UpNormal = Vector3.zero;
            UpDistance = 0f;

            HasSideContact = false;
            SidePoint = Vector3.zero;
            SideNormal = Vector3.zero;
            SideDistance = 0f;
        }
    }
}