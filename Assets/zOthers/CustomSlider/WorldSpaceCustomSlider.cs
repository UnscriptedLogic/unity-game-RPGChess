using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Assets.Scripts.CustomSlider
{
    public class WorldSpaceCustomSlider : CustomSlider
    {
        [Header("3D Settings")]
        [Tooltip("Scales the UI to attempt to keep it at the same size regardless of distance from the camera")]
        [SerializeField] private bool keepSize;
        [SerializeField] private float trackSpeed;
        [SerializeField] private float sizeScale = 1f;
        [SerializeField] private Ease trackEase = Ease.InOutSine;

        [Tooltip("Leave empty if the current transform is the root. This transform is in charge of the tracking and orientation of the object")]
        [SerializeField] private Transform root;
        [SerializeField] private Transform lookTarget;
        [SerializeField] private Transform followTarget;

        public Transform LookTarget { get => lookTarget; }
        public Transform FollowTarget { get => followTarget; }

        private bool doFace;
        private bool doFollow;

        #region Initializers
        public void Initialize(bool keepSize, Transform lookTarget, Transform followTarget, float intializeValue, float maxValue)
        {
            Initialize(currentValue, maxValue, SetInInspector, UsePredictive, OnlyWhenNotFull);

            this.keepSize = keepSize;
            this.lookTarget = lookTarget;
            this.followTarget = followTarget;
            root = root != null ? root : transform.parent;
        }

        //Just in case there is a need to set the root manually by script. More often than not, the root is predefined in the inspector.
        public void Initialize(bool keepSize, Transform lookTarget, Transform followTarget, Transform root)
        {
            this.keepSize = keepSize;
            this.lookTarget = lookTarget;
            this.followTarget = followTarget;
            this.root = root;

            Initialize(currentValue, maxValue, SetInInspector, UsePredictive, OnlyWhenNotFull);
        }
        #endregion

        private void Update()
        {
            FaceCamera();
            Follow();
            Scale();
        }

        private void FaceCamera()
        {
            if (!doFace) return;
            root.LookAt(lookTarget);
        }

        private void Follow()
        {
            if (!doFollow) return;
            root.DOMove(followTarget.position, trackSpeed).SetEase(trackEase);
        }

        private void Scale()
        {
            if (!lookTarget) return;
            root.localScale = (Vector3.one * sizeScale) * Vector3.Distance(root.position, lookTarget.position);
        }

        #region Setters
        public void SetCamera(Transform newCamera)
        {
            lookTarget = newCamera;
            doFace = newCamera != null;
        }

        public void SetFollowTarget(Transform newTarget)
        {
            followTarget = newTarget;
            doFollow = newTarget != null;
        } 
        #endregion
    }
}