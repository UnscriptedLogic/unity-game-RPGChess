using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public class InitSettings
    {
        public UnitBehaviour unitBehaviour;

        public InitSettings(UnitBehaviour unitBehaviour)
        {
            this.unitBehaviour = unitBehaviour;
        }
    }

    [SerializeField] private Slider slider;
    
    
    private InitSettings initSettings;

    public Slider HealthBarSlider => slider;

    public void Initialize(InitSettings initSettings)
    {
        this.initSettings = initSettings;

        slider.maxValue = initSettings.unitBehaviour.Stats.Health;
        slider.value = initSettings.unitBehaviour.Stats.Health;

        initSettings.unitBehaviour.Stats.HealthHandler.OnModified += HealthHandler_OnModified;
    }

    private void HealthHandler_OnModified(object sender, UnscriptedLogic.IntHandlerEventArgs e)
    {
        slider.value = e.currentValue;
    }

    private void Update()
    {
        if (initSettings == null) return;

        if (initSettings.unitBehaviour != null)
        {
            transform.forward = (transform.position - Camera.main.transform.position).normalized;
        }
    }

    private void OnDestroy()
    {
        initSettings.unitBehaviour.Stats.HealthHandler.OnModified -= HealthHandler_OnModified;
    }
}
