using UnityEngine;
using System.Collections;
using BaltoroGames;

public static class BGInput{
    private static BGInputBase baltoroInput;
	private static float axisDeadzone = 0.2f;
	private static float axisClickThreshold = 0.1f;

    public delegate void BGGamepadEvent(int playerId);
    public static BGGamepadEvent OnGamepadAdded;
    public static BGGamepadEvent OnGamepadRemoved;


	public static float AxisDeadzone {
		get { return axisDeadzone; }
		set { axisDeadzone = Mathf.Clamp01(value); }
	}

	public static float AxisClickThreshold {
		get { return axisClickThreshold; }
		set { axisClickThreshold = Mathf.Clamp01(value); }
	}

    public class BGInputUpdate : MonoBehaviour{
        private static BGInputUpdate instance = null;
        public static BGInputUpdate Instance { get { return instance; } }

        private WaitForEndOfFrame waitEnd = new WaitForEndOfFrame();
        private WaitForSeconds waitTime = new WaitForSeconds(2f);

        void Awake(){
            instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InputUpdate());
            StartCoroutine(DevicesUpdate());
        }

		IEnumerator InputUpdate(){
            while(true){
                yield return waitEnd;
				baltoroInput.Update();
            }
        }

        IEnumerator DevicesUpdate(){
            while(true){
                BGGamepad.Update();
                yield return waitTime;
            }
        }
    }

    static BGInput(){
        GameObject inputUpdate = new GameObject("BGInputUpdate");
        inputUpdate.AddComponent<BGInputUpdate>();


#if UNITY_EDITOR
		baltoroInput = new WindowsInput();
#elif UNITY_SWITCH
		baltoroInput = new NintendoInput();
#else
		baltoroInput = new WindowsInput();
#endif

    }
    
    public static bool GetButtonDown(BGGamepadButton gamepadButton, int playerId = 0){
        return baltoroInput.GetButtonDown(gamepadButton, playerId);
    }

    public static bool GetButtonUp(BGGamepadButton gamepadButton, int playerId = 0){
        return baltoroInput.GetButtonUp(gamepadButton, playerId);
    }

    public static bool GetButton(BGGamepadButton gamepadButton, int playerId = 0){
        return baltoroInput.GetButton(gamepadButton, playerId);
    }

    public static float GetAxis(BGGamepadAxis gamepadAxis, int playerId = 0){
        return baltoroInput.GetAxis(gamepadAxis, playerId);
    }

    public static Vector2 GetJoystickLeft(int playerId = 0){
        return baltoroInput.GetJoystickLeft(playerId);
    }

    public static Vector2 GetJoystickRight(int playerId = 0){
        return baltoroInput.GetJoystickRight(playerId);
    }

    public static void SetVibration(BGGamepad.Vibration vibration, int playerId = 0) {
        baltoroInput.SetVibration(vibration, playerId);
	}
}
