using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Haptics
{
    public class HapticManager : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = true;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
                IsEnabled = save.Data.settings.hapticsEnabled;
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            if (ServiceLocator.TryGet<Persistence.SaveManager>(out var save))
            {
                save.Data.settings.hapticsEnabled = enabled;
                save.Save();
            }
        }

        public void PlayLight()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Light);
        }

        public void PlayMedium()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Medium);
        }

        public void PlayHeavy()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Heavy);
        }

        public void PlaySuccess()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Success);
        }

        public void PlayWarning()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Warning);
        }

        public void PlayError()
        {
            if (!IsEnabled) return;
            TriggerHaptic(HapticType.Error);
        }

        private void TriggerHaptic(HapticType type)
        {
#if UNITY_IOS && !UNITY_EDITOR
            TriggerHapticIOS(type);
#elif UNITY_ANDROID && !UNITY_EDITOR
            TriggerHapticAndroid(type);
#else
            Debug.Log($"[HapticManager] Haptic: {type}");
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _triggerImpactHaptic(int style);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _triggerNotificationHaptic(int type);

        private void TriggerHapticIOS(HapticType type)
        {
            switch (type)
            {
                case HapticType.Light:
                    _triggerImpactHaptic(0);
                    break;
                case HapticType.Medium:
                    _triggerImpactHaptic(1);
                    break;
                case HapticType.Heavy:
                    _triggerImpactHaptic(2);
                    break;
                case HapticType.Success:
                    _triggerNotificationHaptic(0);
                    break;
                case HapticType.Warning:
                    _triggerNotificationHaptic(1);
                    break;
                case HapticType.Error:
                    _triggerNotificationHaptic(2);
                    break;
            }
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        private void TriggerHapticAndroid(HapticType type)
        {
            int duration = type switch
            {
                HapticType.Light => 10,
                HapticType.Medium => 20,
                HapticType.Heavy => 40,
                HapticType.Success => 15,
                HapticType.Warning => 25,
                HapticType.Error => 50,
                _ => 10
            };

            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrator.Call("vibrate", (long)duration);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[HapticManager] Android haptic failed: {e.Message}");
            }
        }
#endif

        private void OnDestroy()
        {
            ServiceLocator.Unregister<HapticManager>();
        }

        private enum HapticType
        {
            Light,
            Medium,
            Heavy,
            Success,
            Warning,
            Error
        }
    }
}
