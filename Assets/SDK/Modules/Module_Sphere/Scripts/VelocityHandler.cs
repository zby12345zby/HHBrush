using SC.XR.Unity.Module_InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


namespace SC.XR.Unity.Module_Sphere
{
    [RequireComponent(typeof(ManipulationHandler))]
    [RequireComponent(typeof(Rigidbody))]
    public class VelocityHandler : MonoBehaviour
    {

        /// <summary>
        /// Whether to enable rigidbody's velocity.
        /// </summary>
        [SerializeField]
        private bool useVelocity = true;
        /// <summary>
        /// Whether to enable rigidbody's angular velocity.
        /// </summary>
        [SerializeField]
        private bool useAngularVelocity = true;
        /// <summary>
        /// The type of rigidbody's origin.
        /// </summary>
        [SerializeField]
        protected OriginType originType = OriginType.Pointer;

        // The number of frames to estimating velocity.
        public int velocityAverageFrames = 5;
        // The number of frames to estimating angular velocity.
        public int angularVelocityAverageFrames = 10;

        public float velocityIntensity = 1.0f;
        public float angularVelocityIntensity = 1.0f;

        /// <summary>
        /// Enable velocity handling.
        /// </summary>
        protected void OnEnable()
        {
            InitCaches();
            ManipulationHandler.PointerDown.AddListener(OnPointerDown);
            ManipulationHandler.PointerUp.AddListener(OnPointerUp);
        }

        /// <summary>
        /// Disable velocity handling.
        /// </summary>
        protected void OnDisable()
        {
            ManipulationHandler.PointerDown.RemoveListener(OnPointerDown);
            ManipulationHandler.PointerUp.RemoveListener(OnPointerUp);
        }

        private ManipulationHandler _manipulationHandler;
        public ManipulationHandler ManipulationHandler
        {
            get
            {
                if (_manipulationHandler == null)
                    _manipulationHandler = GetComponent<ManipulationHandler>();
                return _manipulationHandler;
            }
        }

        private Rigidbody _rigidbody;
        public Rigidbody RigidBody
        {
            get
            {
                if (_rigidbody == null)
                    _rigidbody = GetComponent<Rigidbody>();
                return _rigidbody;
            }
        }

        public Transform Origin
        {
            get
            {
                switch (originType)
                {
                    case OriginType.Pointer:
                        return Pointer;
                    case OriginType.Cursor:
                        return Cursor;
                    case OriginType.Transform:
                        return transform;
                }
                return null;
            }
        }

        private DetectorBase _detectorBase;
        public DetectorBase DetectorBase { get => _detectorBase;set => _detectorBase = value; }
        public Transform Pointer {get => DetectorBase.currentPointer.transform;}
        public Transform Cursor { get => DetectorBase.currentPointer.cursorBase.transform; }

        public void OnPointerDown(PointerEventData eventData)
        {
            DetectorBase = (eventData as SCPointEventData).inputDevicePartBase.detectorBase;
            HandleVelocityStarted();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HandleVelocityEnded();
        }

        /// <summary>
        /// Start handle rigidbody's velocity and angular velocity.
        /// </summary>
        private void HandleVelocityStarted()
        {
            estimateVelocity = StartCoroutine(EstimateVelocity());
        }

        /// <summary>
        /// End handle rigidbody's velocity and angular velocity.
        /// </summary>
        private void HandleVelocityEnded()
        {
            if (useVelocity) RigidBody.velocity = GetVelocityEstimate();
            if (useAngularVelocity) RigidBody.angularVelocity = GetAngularVelocityEstimate();

            StopCoroutine(estimateVelocity);
            estimateVelocity = null;
            DetectorBase = null;
        }


        private Vector3[] velocityCaches;
        private Vector3[] angularVelocityCaches;
        private int currentCacheCount;

        // The coroutine to estimate velocity and angular velocity.
        private Coroutine estimateVelocity;


        //Initialize the velocity caches and angular velocity caches.
        public void InitCaches()
        {
            velocityCaches = new Vector3[velocityAverageFrames];
            angularVelocityCaches = new Vector3[angularVelocityAverageFrames];
        }

        /// <summary>
        /// Estimate the average velocity based on the velocities in the cache.
        /// </summary>
        public Vector3 GetVelocityEstimate()
        {
            Vector3 averageVelocity = Vector3.zero;
            int velocityCacheCount = Mathf.Min(currentCacheCount, velocityCaches.Length);
            if (velocityCacheCount != 0)
            {
                for (int i = 0; i < velocityCacheCount; i++)
                {
                    averageVelocity += velocityCaches[i];
                }
                averageVelocity *= (1.0f / velocityCacheCount);
            }
            return averageVelocity * velocityIntensity;
        }

        /// <summary>
        /// Estimate the average angular velocity based on the angular velocities in the cache.
        /// </summary>
        public Vector3 GetAngularVelocityEstimate()
        {
            Vector3 angularVelocity = Vector3.zero;
            int angularVelocitySampleCount = Mathf.Min(currentCacheCount, angularVelocityCaches.Length);
            if (angularVelocitySampleCount != 0)
            {
                for (int i = 0; i < angularVelocitySampleCount; i++)
                {
                    angularVelocity += angularVelocityCaches[i];
                }
                angularVelocity *= (1.0f / angularVelocitySampleCount);
            }

            return angularVelocity * angularVelocityIntensity;
        }

        /// <summary>
        /// Estimate the velocity and angular velocity of the object in this frame,
        /// based on the position and rotation of the previous frame and this frame, and store it in cache.
        /// </summary>
        public IEnumerator EstimateVelocity()
        {
            currentCacheCount = 0;

            Vector3 previousPosition = Origin.position;
            Quaternion previousRotation = Origin.rotation;

            while (true)
            {

                yield return new WaitForEndOfFrame();

                int v = currentCacheCount % velocityCaches.Length;
                int w = currentCacheCount % angularVelocityCaches.Length;

                currentCacheCount++;

                float velocityFactor = 1.0f / Time.deltaTime;

                velocityCaches[v] = CalculateVelocity(Origin.position, previousPosition, velocityFactor);
                angularVelocityCaches[w] = CalculateAngularVelocity(Origin.rotation, previousRotation, velocityFactor);

                previousPosition = Origin.position;
                previousRotation = Origin.rotation;
            }
        }

        /// <summary>
        /// Calculate the rigidbody's  velocity.
        /// </summary>
        /// <param name="presentRotation">Rigidbody's position in present frame.</param>
        /// <param name="previousPosition">Rigidbody's position in previous frame.</param>
        /// <param name="velocityFactor"></param>
        public Vector3 CalculateVelocity(Vector3 presentRotation, Vector3 previousPosition, float velocityFactor)
        {
            Vector3 velocity = (presentRotation - previousPosition) * velocityFactor;

            return velocity;
        }

        /// <summary>
        /// Using Quaternion to Calculate the rigidbody's angular velocity.
        /// </summary>
        /// <param name="presentRotation">Rigidbody's rotation(Quaternion) in present frame.</param>
        /// <param name="previousRotation">Rigidbody's rotation(Quaternion) in previous frame.</param>
        /// <param name="velocityFactor"></param>
        /// <returns></returns>
        public Vector3 CalculateAngularVelocity(Quaternion presentRotation, Quaternion previousRotation, float velocityFactor)
        {
            Quaternion deltaRotation = presentRotation * Quaternion.Inverse(previousRotation);

            float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));
            if (theta > Mathf.PI)
            {
                theta -= 2.0f * Mathf.PI;
            }

            Vector3 angularAxis = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z).normalized;

            Vector3 angularVelocity = Vector3.zero;

            if (angularAxis.sqrMagnitude > 0.0f)
            {
                angularVelocity = theta * angularAxis * velocityFactor;
            }

            return angularVelocity;
        }

    }

}

