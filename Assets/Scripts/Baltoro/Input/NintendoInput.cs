//#define UNITY_SWITCH
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace BaltoroGames{
    public class NintendoInput : BGInputBase{

#if UNITY_SWITCH
        static Dictionary<int, Coroutine> vibrationCoroutine = new Dictionary<int, Coroutine>();

        private static Dictionary<BGGamepadButton, nn.hid.NpadButton> buttonsMap = new Dictionary<BGGamepadButton, nn.hid.NpadButton>{
            {BGGamepadButton.LeftArrow, nn.hid.NpadButton.Left},
            {BGGamepadButton.RightArrow, nn.hid.NpadButton.Right},
            {BGGamepadButton.UpArrow, nn.hid.NpadButton.Up},
            {BGGamepadButton.DownArrow, nn.hid.NpadButton.Down},
            {BGGamepadButton.ButtonSouth, nn.hid.NpadButton.B},
            {BGGamepadButton.ButtonEast, nn.hid.NpadButton.A},
            {BGGamepadButton.ButtonWest, nn.hid.NpadButton.Y},
            {BGGamepadButton.ButtonNorth, nn.hid.NpadButton.X},
            {BGGamepadButton.LeftBumper, nn.hid.NpadButton.L},
            {BGGamepadButton.RightBumper, nn.hid.NpadButton.R},
            {BGGamepadButton.StartButton, nn.hid.NpadButton.Plus},
            {BGGamepadButton.SelectButton, nn.hid.NpadButton.Minus},
            {BGGamepadButton.LeftStickButton, nn.hid.NpadButton.StickL},
            {BGGamepadButton.RightStickButton, nn.hid.NpadButton.StickR},
            {BGGamepadButton.Submit, nn.hid.NpadButton.A},
            {BGGamepadButton.Cancel, nn.hid.NpadButton.B},
			{BGGamepadButton.LeftTrigger, nn.hid.NpadButton.ZL},
			{BGGamepadButton.RightTrigger, nn.hid.NpadButton.ZR},
		};

        private static Dictionary<BGGamepadAxis, nn.hid.NpadButton> axisMap = new Dictionary<BGGamepadAxis, nn.hid.NpadButton>{
            {BGGamepadAxis.LeftTrigger, nn.hid.NpadButton.ZL},
            {BGGamepadAxis.RightTrigger, nn.hid.NpadButton.ZR},
        };

        public override bool GetButtonDown(BGGamepadButton gamepadButton, int playerId = 0){
            return NintendoSwitch.GetButtonDown(buttonsMap[gamepadButton], playerId);
        }

        public override bool GetButtonUp(BGGamepadButton gamepadButton, int playerId = 0){
            return NintendoSwitch.GetButtonUp(buttonsMap[gamepadButton], playerId);
        }

        public override bool GetButton(BGGamepadButton gamepadButton, int playerId = 0){
            return NintendoSwitch.GetButton(buttonsMap[gamepadButton], playerId);
        }

        public override float GetAxis(BGGamepadAxis gamepadAxis, int playerId = 0){
            return NintendoSwitch.GetButton(axisMap[gamepadAxis], playerId) ? 1f : 0f;
        }

        public override Vector2 GetJoystickLeft(int playerId = 0){
            return GetVectorWithDeadzone(NintendoSwitch.GetAnalogMain(playerId));
        }

        public override Vector2 GetJoystickRight(int playerId = 0){
            return GetVectorWithDeadzone(NintendoSwitch.GetAnalogSecondary(playerId));
        }

		public override void SetVibration(BGGamepad.Vibration vibration, int playerId = 0) {
            if (playerId <= 0)
                return;

			if (vibrationCoroutine.ContainsKey(playerId) && vibrationCoroutine[playerId] != null) {
                BGInput.BGInputUpdate.Instance.StopCoroutine(vibrationCoroutine[playerId]);
                vibrationCoroutine[playerId] = null;
            }

            NintendoSwitch.SetVibration(playerId, vibration.power_1, vibration.power_2);

            if (vibration.time > 0.0f) {
                if (!vibrationCoroutine.ContainsKey(playerId))
                    vibrationCoroutine.Add(playerId, null);

                vibrationCoroutine[playerId] = BGInput.BGInputUpdate.Instance.StartCoroutine(VibrationUpdate(playerId, vibration.time));
            }
        }

		private IEnumerator VibrationUpdate(int playerId, float time) {
            yield return new WaitForSeconds(time);
            NintendoSwitch.SetVibration(playerId, 0f, 0f);
        }

#else

        public override bool GetButtonDown(BGGamepadButton gamepadButton, int playerId = 0){
            return false;
        }

        public override bool GetButtonUp(BGGamepadButton gamepadButton, int playerId = 0){
            return false;
        }

        public override bool GetButton(BGGamepadButton gamepadButton, int playerId = 0){
            return false;
        }

        public override float GetAxis(BGGamepadAxis gamepadAxis, int playerId = 0){
            return 0;
        }

        public override Vector2 GetJoystickLeft(int playerId = 0){
            return Vector2.zero;
        }

        public override Vector2 GetJoystickRight(int playerId = 0){
            return Vector2.zero;
        }
#endif
    }
}
