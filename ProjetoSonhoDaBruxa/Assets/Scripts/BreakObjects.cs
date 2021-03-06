﻿using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts
{
    public class BreakObjects : MonoBehaviour {
    
        public float Radius;
        [Tooltip("Tempo para segurar espaço")]
        public float HoldTime; 

        public Light Light;

		public ParticleEmitter p;
		public Particle[] particles;
		public float affectDistance = 10.0f;
		private float _sqrDist;
		private Transform _tr;

        private float _startTime = 0f;
        private bool _ready;
        private float _pressTime;
		private PlayerController _playerControl;
        private bool _pressing;
        public static BreakObjects Instance = null;
        public Color ColorForInteraction;
        private Color _originalColor;

        public void Awake()
        {
            if (Instance == null) //Check if instance already exists
                Instance = this; //if not, set instance to this
            else if (Instance != this) //If instance already exists and it's not this:
                Destroy(gameObject);
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
        }

        public void Start()
        {
            _originalColor = Light.color;
            _playerControl = PlayerController.Instance;
           /* _tr = _playerControl.gameObject.transform;
			p = (ParticleEmitter)(GameObject.Find("puxaluz (1)").GetComponent(typeof(ParticleEmitter)));
			particles = p.particles;
			_sqrDist = affectDistance*affectDistance;*/
        }

        public void Update()
        {
            if (Pause.Instance.IsPauseActive())
                return;

            Collider2D col = FindNearestObject();

            if (Input.GetKey(KeyCode.Space) && _playerControl.IsMoving && _ready == false && col != null && !_pressing)
            {
				_pressing = true;
				StillPressing();
            }
			if (Input.GetKeyUp(KeyCode.Space) || !_playerControl.IsMoving || col == null)
            {
                _pressing = false;
                StoppedPressing();
            }
            if (Time.time >= _pressTime && _ready)
            {
                _pressing = false;
                _playerControl.GetLight = true;
                StartCoroutine(EndGetLight());
                PressedEnoughTime(col);
            }

        }

        IEnumerator EndGetLight()
        {
            yield return new WaitForSeconds(0.3f);
            _playerControl.GetLight = false;
        }

        private void StillPressing()
        {
			_startTime = Time.time;
            _pressTime = _startTime + HoldTime;
            _playerControl.IsPulling = true;
            _ready = true;
            ShowLight();
            //StartParticle();
        }

        private void StoppedPressing()
        {
            _ready = false;
            EndLight();
            //StopParticle();
            _playerControl.IsPulling = false;
        }

        private void PressedEnoughTime(Collider2D col)
        {
            _ready = false;
            BreakObject(col);
            _playerControl.IsPulling = false;
			_playerControl.ApplyForce ();
        }

        private void StopParticle()
        {
            throw new System.NotImplementedException();
        }

        private void StartParticle()
        {
            throw new System.NotImplementedException();
        }

        private void ShowLight()
        {
            Light.color = ColorForInteraction;
        }

        private void EndLight()
        {
            Light.color = _originalColor;
        }

        private void FindObjectAndBreak()
        {
            Collider2D col = FindNearestObject();
            if (col != null)
                BreakObject(col);
        }

        private void BreakObject(Collider2D col)
        {
            var breakable = col.GetComponent<BreakableObject>();
            Mirror.Instance.AddBrokenObject(breakable);
            breakable.BreakObject();
			// libera a bola de luz até o player
        }

        Collider2D FindNearestObject()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Radius);

            Collider2D nearestCollider = null;
            float minSqrDistance = Mathf.Infinity;

            foreach (Collider2D col in colliders)
            {
                float sqrDistanceToCenter = (transform.position - col.transform.position).sqrMagnitude;

                if (sqrDistanceToCenter < minSqrDistance && col.CompareTag("Interactive"))
                {
                    minSqrDistance = sqrDistanceToCenter;
                    nearestCollider = col;
					_playerControl.BreakableObjectPos = col.transform.position;
                }
            }
            return nearestCollider;
        }

        public bool GetIfPressingButtonsToBreakObjects()
        {
            return _pressing;
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
