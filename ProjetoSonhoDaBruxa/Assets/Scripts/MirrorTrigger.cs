﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorTrigger : MonoBehaviour
    {
        [SerializeField]
        private int _orbsCounter = 0;

        public GameObject[] Orbs;
        private int _difference;
        public float WaitTimeBetweenOrbs;

        public GameObject CloseToMirror;
        public GameObject FarFromMirror;
        
        public void OnTriggerEnter2D(Collider2D other){

            if (other.tag == "Player") {
                // Trigger close to mirror
                TriggerCloseToMirror();
                //Trigger orbs
                var orbsCollected = Mirror.Instance.GetObjectCounter();
               _difference = orbsCollected - _orbsCounter;
                StartCoroutine("StartOrbs");
                // Reset Objects To collect
                Mirror.Instance.ResetObjectsToCollect();
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            TriggerFarFromMirror();
        }

        private void TriggerCloseToMirror()
        {
            CloseToMirror.SetActive(true);
            FarFromMirror.SetActive(false);
        }

        private void TriggerFarFromMirror()
        {
            CloseToMirror.SetActive(false);
            FarFromMirror.SetActive(true);
        }

        private IEnumerator StartOrbs()
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = _orbsCounter; i < _difference; i++)
            {
                Orbs[i].SetActive(true);
                yield return new WaitForSeconds(WaitTimeBetweenOrbs);
            }
            Mirror.Instance.CheckIfWin();

        }
    }
}
