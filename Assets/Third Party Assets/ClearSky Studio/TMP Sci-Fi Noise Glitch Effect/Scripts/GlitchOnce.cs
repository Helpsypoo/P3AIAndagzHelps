using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClearSkyStudio
{
    public class GlitchOnce : MonoBehaviour
    {
        [Header("Play Option")] [SerializeField]
        private bool playOnEnable = true;

        [Description("Set -1 for infinite replay")] [SerializeField]
        private int rePlay = -1;

        [SerializeField] private float rePlayCoolTime = 1;


        [Space(10)] [Range(0, 10)] public float playTime = 0.5f;
        [Range(0, 1000)] public float startSpeed = 66.1f;
        public float Speed = 0;
        public float Distance = 1, Amplitude = 1;

        private Material TextMat;
        private TextMeshProUGUI selfText;

        private IEnumerator curGlitch;

        void Awake()
        {
            selfText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            replayCount = rePlay;

            if (playOnEnable)
                Glitch();
        }

        private float curRePlayCoolTime;
        private int replayCount = 0;

        private void Update()
        {
            if (curGlitch != null && (replayCount > 0 || rePlay == -1))
            {
                if (curRePlayCoolTime > 0)
                {
                    curRePlayCoolTime -= Time.deltaTime;
                    if (curRePlayCoolTime < 0)
                    {
                        if (replayCount > 0)
                            replayCount--;
                        Glitch();
                    }
                }
            }
        }


        private void OnDisable()
        {
            if (curGlitch != null)
            {
                replayCount = rePlay;
                StopCoroutine(curGlitch);

                TextMat.SetFloat("_BienDo", 0);
                TextMat.SetFloat("_Distance", 0);
                TextMat.SetFloat("_Speed", 0);
            }
        }

        public void Glitch()
        {
            TextMat = selfText.fontSharedMaterial;

            curGlitch = GlichCoroutine();
            StartCoroutine(curGlitch);
        }

        private float curTime;

        private IEnumerator GlichCoroutine()
        {
            curTime = playTime;
            Speed = startSpeed;

            while (curTime > 0)
            {
                curTime = curTime - Time.deltaTime;

                float curSpeed = Speed - Mathf.Pow(playTime - curTime, 5);
                float curAmplitude = Mathf.Lerp(Amplitude, 0, curTime);
                float curDistance = Mathf.Lerp(Distance, 0, curTime);

                TextMat.SetFloat("_Speed", curSpeed);

                if (curTime > playTime * 0.45f)
                {
                    TextMat.SetFloat("_BienDo", curAmplitude);
                    TextMat.SetFloat("_Distance", curDistance);
                }
                else
                {
                    TextMat.SetFloat("_BienDo", 0);
                    TextMat.SetFloat("_Distance", 0);
                }

                yield return null;
            }

            // 리플레이 설정
            curRePlayCoolTime = rePlayCoolTime;

            TextMat.SetFloat("_Speed", 0);
        }
    }
}