using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace Assets.Scripts.CustomSlider
{
    public class CustomSlider : MonoBehaviour
    {
        [SerializeField] protected bool SetInInspector = false;
        [SerializeField] protected bool UsePredictive = false;
        [SerializeField] protected bool OnlyWhenNotFull = true;
        [Space(15)]
        [SerializeField] protected float currentValue;
        [SerializeField] protected float maxValue = 100f;
        [SerializeField] protected float predictDelay = 1f;
        [SerializeField] protected float hideDelay = 3f;

        [Header("Components")]
        [SerializeField] protected Slider slider;
        [SerializeField] protected Slider differenceSlider;
        [SerializeField] protected Image background;

        private Image predictFill;
        private Image sliderFill;

        [Header("Animation")]
        [SerializeField] private Gradient sliderColour;
        [SerializeField] private Gradient predictSliderColour;
        [SerializeField] private float lerpDuration = 0.5f;

        [Header("Test Value")]
        [SerializeField] protected float amount = 25;

        private void Start()
        {
            if (SetInInspector)
            {
                Initialize(currentValue, maxValue, SetInInspector, UsePredictive);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SetValue(currentValue += amount);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SetValue(currentValue -= amount);
            }
        }

        public void Initialize(float current, float max, bool setInInspector = false, bool usePredict = false, bool onlyWhenNotFull = false)
        {
            slider.interactable = false;
            differenceSlider.interactable = false;

            slider.maxValue = max;
            slider.value = current;
            differenceSlider.maxValue = max;
            differenceSlider.value = current;

            SetInInspector = setInInspector;
            UsePredictive = usePredict;
            OnlyWhenNotFull = onlyWhenNotFull;

            predictFill = differenceSlider.fillRect.GetComponent<Image>();
            sliderFill = slider.fillRect.GetComponent<Image>();

            if (OnlyWhenNotFull)
            {
                SetVisibility(!(current >= max));
            }

            sliderFill.DOColor(sliderColour.Evaluate(currentValue / maxValue), lerpDuration);
            predictFill.DOColor(predictSliderColour.Evaluate(currentValue / maxValue), lerpDuration);
        }

        public void SetValue(float amount)
        {
            currentValue = amount;

            if (UsePredictive)
            {
                differenceSlider.value = slider.value;
                predictFill.enabled = true;
                DOTween.To(() => slider.value, x => slider.value = x, currentValue, lerpDuration);
                DOTween.To(() => differenceSlider.value, x => differenceSlider.value = x, currentValue, lerpDuration).SetDelay(predictDelay);

            } else
            {
                DOTween.To(() => slider.value, x => slider.value = x, currentValue, lerpDuration);
            }

            sliderFill.DOColor(sliderColour.Evaluate(currentValue/maxValue), lerpDuration);
            predictFill.DOColor(predictSliderColour.Evaluate(currentValue/maxValue), lerpDuration);

            if (OnlyWhenNotFull)
            {
                if (currentValue >= maxValue)
                {
                    StartCoroutine(HideBar());
                } else
                {
                    StopAllCoroutines();
                    SetVisibility(true);
                }
            }
        }

        protected IEnumerator HideBar()
        {
            yield return new WaitForSeconds(hideDelay);
            SetVisibility(false);
        }

        protected void SetVisibility(bool value)
        {
            background.enabled = value;
            sliderFill.enabled = value;
            predictFill.enabled = value;
        }
    }
}
