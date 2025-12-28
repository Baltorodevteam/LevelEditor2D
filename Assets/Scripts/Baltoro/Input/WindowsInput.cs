//#define ENABLE_WINMD_SUPPORT
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

#if ENABLE_WINMD_SUPPORT
using Windows.Gaming.Input;
using System.Threading.Tasks;
#endif

namespace BaltoroGames
{
    public class WindowsInput : BGInputBase{

		private Dictionary<BGGamepadButton, KeyCode> keysMap = new Dictionary<BGGamepadButton, KeyCode>{
            {BGGamepadButton.LeftArrow, KeyCode.LeftArrow},
            {BGGamepadButton.RightArrow, KeyCode.RightArrow},
            {BGGamepadButton.UpArrow, KeyCode.UpArrow},
            {BGGamepadButton.DownArrow, KeyCode.DownArrow},
            {BGGamepadButton.ButtonSouth, KeyCode.X},
            {BGGamepadButton.ButtonEast, KeyCode.D},
            {BGGamepadButton.ButtonWest, KeyCode.A},
            {BGGamepadButton.ButtonNorth, KeyCode.W},
            {BGGamepadButton.LeftBumper, KeyCode.Q},
            {BGGamepadButton.RightBumper, KeyCode.E},
            {BGGamepadButton.StartButton, KeyCode.KeypadPlus},
            {BGGamepadButton.SelectButton, KeyCode.KeypadMinus},
            {BGGamepadButton.LeftStickButton, KeyCode.S},
            {BGGamepadButton.RightStickButton, KeyCode.F},
            {BGGamepadButton.Submit, KeyCode.Return},
            {BGGamepadButton.Cancel, KeyCode.Escape},
			{BGGamepadButton.LeftTrigger, KeyCode.Z},
			{BGGamepadButton.RightTrigger, KeyCode.C},
		};

        private Dictionary<BGGamepadAxis, KeyCode> keysAxisMap = new Dictionary<BGGamepadAxis, KeyCode>{
            {BGGamepadAxis.LeftTrigger, KeyCode.Z},
            {BGGamepadAxis.RightTrigger, KeyCode.C},
        };

#if ENABLE_WINMD_SUPPORT
        private class XboxGamepad{
            public Gamepad gamepad;
            public Coroutine vibrationCoroutine;
            public XboxGamepad(Gamepad gamepad) {
                this.gamepad = gamepad;
                this.vibrationCoroutine = null;
            }
        }
        private enum EventType{
            Added,
            Removed,
		}
        private struct GamepadEvent{
            public EventType type;
            public int playerId;
            public Gamepad gamepad;
		}

        private readonly object gamepadLock = new object();
        private List<XboxGamepad> xboxGamepads = new List<XboxGamepad>();
        private List<GamepadEvent> gamepadsEvents = new List<GamepadEvent>();

        private void GetXboxGamepad(){
            lock (gamepadLock){
                foreach (var gamepad in Gamepad.Gamepads){
                    if (xboxGamepads.Find(g => g.gamepad == gamepad) == null){
                        ResetXboxGamepad(gamepad);
                        xboxGamepads.Add(new XboxGamepad(gamepad));

                        gamepadsEvents.Add(new GamepadEvent { type = EventType.Added, playerId = -1, gamepad = gamepad });
                    }
                }
            } 
        }

        private void XboxGamepadAdded(object sender, Gamepad gamepad) {
            lock (gamepadLock) {
                if (xboxGamepads.Find(g => g.gamepad == gamepad) == null) {
                    ResetXboxGamepad(gamepad);
                    xboxGamepads.Add(new XboxGamepad(gamepad));

                    gamepadsEvents.Add(new GamepadEvent { type = EventType.Added, playerId = -1, gamepad = gamepad });
                }
            }
        }

        private void XboxGamepadRemoved(object sender, Gamepad gamepad) {
            lock (gamepadLock) {
                var xboxGamepad = xboxGamepads.Find(g => g.gamepad == gamepad);
                if (xboxGamepad != null) {
                    var playerId = GetXboxPlayerId(gamepad);

                    ResetXboxGamepad(gamepad);
                    xboxGamepads.Remove(xboxGamepad);

                    if (playerId > 0)
                        gamepadsEvents.Add(new GamepadEvent { type = EventType.Removed, playerId = playerId });
                }
            }
        }

        private int GetXboxPlayerId(Gamepad gamepad) {
            var xboxGamepad = xboxGamepads.Find(g => g.gamepad == gamepad);

			if (xboxGamepad != null) {
                var xboxId = xboxGamepads.IndexOf(xboxGamepad);
                var slots = BGGamepad.GetSlots();
                var slot = slots.Find(s => !s.empty && s.xboxId == xboxId);

                if (slot == null)
                    return -1;

                return slots.IndexOf(slot) + 1;
            }

            return -1;
		}

        private void ResetXboxGamepad(Gamepad gamepad) {
            if (gamepad == null)
                return;

            gamepad.Vibration = new GamepadVibration();
        }
#endif

        public WindowsInput() {
#if ENABLE_WINMD_SUPPORT
            GetXboxGamepad();
            Gamepad.GamepadAdded += XboxGamepadAdded;
            Gamepad.GamepadRemoved += XboxGamepadRemoved;
#endif
        }

        public override bool GetButtonDown(BGGamepadButton gamepadButton, int playerId = 0){
#if UNITY_EDITOR
            if (Input.GetKeyDown(keysMap[gamepadButton]))
                return true;
#endif
            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                return id > 0 && gamepadMap != null && gamepadMap.GetButtonDown(gamepadButton, id);
            }
			else {
                var slots = BGGamepad.GetSlots();
                for(int i=0; i<slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null && slot.gamepadMap.GetButtonDown(gamepadButton, slot.id))
                        return true;
				}
			}
            return false;
        }

        public override bool GetButtonUp(BGGamepadButton gamepadButton, int playerId = 0){
#if UNITY_EDITOR
            if (Input.GetKeyUp(keysMap[gamepadButton]))
                return true;
#endif
            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                return id > 0 && gamepadMap != null && gamepadMap.GetButtonUp(gamepadButton, id);
            }
            else {
                var slots = BGGamepad.GetSlots();
                for (int i = 0; i < slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null && slot.gamepadMap.GetButtonUp(gamepadButton, slot.id))
                        return true;
                }
            }
            return false;
        }

        public override bool GetButton(BGGamepadButton gamepadButton, int playerId = 0){
#if UNITY_EDITOR
            if (Input.GetKey(keysMap[gamepadButton]))
                return true;
#endif
            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                return id > 0 && gamepadMap != null && gamepadMap.GetButton(gamepadButton, id);
            }
            else {
                var slots = BGGamepad.GetSlots();
                for (int i = 0; i < slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null && slot.gamepadMap.GetButton(gamepadButton, slot.id))
                        return true;
                }
            }
            return false;
        }

        public override float GetAxis(BGGamepadAxis gamepadAxis, int playerId = 0){
#if UNITY_EDITOR
			if (keysAxisMap.ContainsKey(gamepadAxis) && Input.GetKey(keysAxisMap[gamepadAxis]))
				return 1f;
#endif

            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                if(id > 0 && gamepadMap != null)
                    return gamepadMap.GetAxis(gamepadAxis, id);
            }
            else {
                float value = 0f;
                var slots = BGGamepad.GetSlots();
                for (int i = 0; i < slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null) {
                        var v = slot.gamepadMap.GetAxis(gamepadAxis, slot.id);
                        value = Mathf.Abs(value) >= Mathf.Abs(v) ? value : v;
                    }
                }
                return value;
            }
            return 0f;
        }

        public override Vector2 GetJoystickLeft(int playerId = 0){
            Vector2 vector = Vector2.zero;

            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                if (id > 0 && gamepadMap != null)
                    vector = gamepadMap.GetJoystick_1(id);
            }
            else {
                var slots = BGGamepad.GetSlots();
                for (int i = 0; i < slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null) {
                        var v = slot.gamepadMap.GetJoystick_1(slot.id);
                        vector.x = Mathf.Abs(vector.x) >= Mathf.Abs(v.x) ? vector.x : v.x;
                        vector.y = Mathf.Abs(vector.y) >= Mathf.Abs(v.y) ? vector.y : v.y;
                    }
                }
            }
            return GetVectorWithDeadzone(vector);
        }

        public override Vector2 GetJoystickRight(int playerId = 0){
            Vector2 vector = Vector2.zero;

            if (playerId > 0) {
                var id = BGGamepad.GetGamepadId(playerId);
                var gamepadMap = BGGamepad.GetGamepadMap(playerId);
                if (id > 0 && gamepadMap != null)
                    vector = gamepadMap.GetJoystick_2(id);
            }
            else {
                var slots = BGGamepad.GetSlots();
                for (int i = 0; i < slots.Count; i++) {
                    var slot = slots[i];
                    if (!slot.empty && slot.gamepadMap != null) {
                        var v = slot.gamepadMap.GetJoystick_2(slot.id);
                        vector.x = Mathf.Abs(vector.x) >= Mathf.Abs(v.x) ? vector.x : v.x;
                        vector.y = Mathf.Abs(vector.y) >= Mathf.Abs(v.y) ? vector.y : v.y;
                    }
                }
            }
            return GetVectorWithDeadzone(vector);
        }
#if ENABLE_WINMD_SUPPORT
        public override void SetVibration(BGGamepad.Vibration vibration, int playerId = 0) {
            playerId = BGGamepad.GetXboxGamepadId(playerId);
            if (playerId < 0)
                return;

            lock (gamepadLock) {
                if (playerId >= 0 && playerId < Windows.Gaming.Input.Gamepad.Gamepads.Count) {
                    var gamepad = Windows.Gaming.Input.Gamepad.Gamepads[playerId];

                    var xboxGamepad = xboxGamepads.Find(g => g.gamepad == gamepad);

                    if (xboxGamepad != null && xboxGamepad.vibrationCoroutine != null) {
                        BGInput.BGInputUpdate.Instance.StopCoroutine(xboxGamepad.vibrationCoroutine);
                        xboxGamepad.vibrationCoroutine = null;
                    }

                    var v = new Windows.Gaming.Input.GamepadVibration();
                    v.LeftMotor = Mathf.Clamp01(vibration.power_1);
                    v.RightMotor = Mathf.Clamp01(vibration.power_2);
                    gamepad.Vibration = v;

                    if (vibration.time > 0.0f && xboxGamepad != null) {
                        xboxGamepad.vibrationCoroutine = BGInput.BGInputUpdate.Instance.StartCoroutine(VibrationUpdate(gamepad, vibration.time));
                    }
                }
            }
        }

        private IEnumerator VibrationUpdate(Windows.Gaming.Input.Gamepad gamepad, float time) {
            yield return new WaitForSeconds(time);
            ResetXboxGamepad(gamepad);
		}

#endif

        public override void Update(){
            BGGamepad.UpdateButtons();

#if ENABLE_WINMD_SUPPORT
			lock (gamepadLock) {
                if(gamepadsEvents.Count > 0) {
                    BGGamepad.Update();

                    foreach(var e in gamepadsEvents) {
                        if (e.type == EventType.Added && BGInput.OnGamepadAdded != null){
                            var playerId = e.playerId;
                            if(playerId < 0 && e.gamepad != null)
                                playerId = GetXboxPlayerId(e.gamepad);

                            if(playerId > 0)
                                BGInput.OnGamepadAdded(playerId);
                        }
                        else if(e.type == EventType.Removed && BGInput.OnGamepadRemoved != null && e.playerId > 0)
                            BGInput.OnGamepadRemoved(e.playerId);
				    }

                    gamepadsEvents.Clear();
                }
			}
#endif
        }
    }
}
