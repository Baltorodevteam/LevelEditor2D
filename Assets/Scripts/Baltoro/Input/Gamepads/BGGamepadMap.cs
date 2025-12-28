using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaltoroGames
{
	public class BGGamepadMap
	{
		const int GAMEPAD_SIZE = 16;
		public List<string> id;
		public Dictionary<BGGamepadButton, Action> buttons;
		public Dictionary<BGGamepadAxis, Action> axis;

		private Dictionary<BGGamepadButton, bool>[] axisButtonsState = new Dictionary<BGGamepadButton, bool>[GAMEPAD_SIZE];
		private Dictionary<BGGamepadButton, string>[] buttonsKey = new Dictionary<BGGamepadButton, string>[GAMEPAD_SIZE];
		private Dictionary<BGGamepadAxis, string>[] axisKey = new Dictionary<BGGamepadAxis, string>[GAMEPAD_SIZE];

		public struct Action
		{
			public string id;
			public bool axis;
			public bool invert;
		}

		protected void Init() {
			for (int i = 0; i < id.Count; i++) {
				id[i] = id[i].ToUpper();
			}

			for (int i = 0; i < GAMEPAD_SIZE; i++) {
				axisButtonsState[i] = new Dictionary<BGGamepadButton, bool>();
				foreach (var button in buttons) {
					if (button.Value.axis)
						axisButtonsState[i].Add(button.Key, false);
				}
			}

			for (int i = 0; i < GAMEPAD_SIZE; i++) {
				buttonsKey[i] = new Dictionary<BGGamepadButton, string>();
				var joystick = i > 0 ? ("joystick " + i) : "joystick";

				foreach (var button in buttons) {
					if (button.Value.axis)
						buttonsKey[i].Add(button.Key, joystick + " axis " + button.Value.id);
					else
						buttonsKey[i].Add(button.Key, joystick + " button " + button.Value.id);
				}
			}

			for (int i = 0; i < GAMEPAD_SIZE; i++) {
				axisKey[i] = new Dictionary<BGGamepadAxis, string>();
				var joystick = i > 0 ? ("joystick " + i) : "joystick";

				foreach (var item in axis) {
					if (item.Value.axis)
						axisKey[i].Add(item.Key, joystick + " axis " + item.Value.id);
					else
						axisKey[i].Add(item.Key, joystick + " button " + item.Value.id);
				}
			}
		}

		private bool GetAxisButtonUp(BGGamepadButton gamepadButton, int id) {
			return axisButtonsState[id][gamepadButton] && !GetAxisButton(gamepadButton, id);
		}

		private bool GetAxisButtonDown(BGGamepadButton gamepadButton, int id) {
			return !axisButtonsState[id][gamepadButton] && GetAxisButton(gamepadButton, id);
		}

		private bool GetAxisButton(BGGamepadButton button, int id) {
			return (Input.GetAxis(buttonsKey[id][button]) * (buttons[button].invert ? -1f : 1f)) > BGInput.AxisClickThreshold;
		}

		private float GetButtonAxis(BGGamepadAxis axis, int id) {
			return (Input.GetKey(axisKey[id][axis]) ? 1f : 0f) * (this.axis[axis].invert ? -1f : 1f);
		}

		public bool GetButtonDown(BGGamepadButton button, int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return false;

			if (buttons[button].axis)
				return GetAxisButtonDown(button, id);

			return Input.GetKeyDown(buttonsKey[id][button]) == !buttons[button].invert;
		}

		public bool GetButtonUp(BGGamepadButton button, int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return false;

			if (buttons[button].axis)
				return GetAxisButtonUp(button, id);

			return Input.GetKeyUp(buttonsKey[id][button]) == !buttons[button].invert;
		}

		public bool GetButton(BGGamepadButton button, int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return false;

			if (buttons[button].axis)
				return GetAxisButton(button, id);

			return Input.GetKey(buttonsKey[id][button]) == !buttons[button].invert;
		}

		public float GetAxis(BGGamepadAxis axis, int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return 0f;

			if (!this.axis[axis].axis)
				return GetButtonAxis(axis, id);

			return Input.GetAxis(axisKey[id][axis]) * (this.axis[axis].invert ? -1f : 1f);
		}

		public Vector2 GetJoystick_1(int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return Vector2.zero;

			return new Vector2(GetAxis(BGGamepadAxis.Horizontal_1, id), GetAxis(BGGamepadAxis.Vertical_1, id));
		}

		public Vector2 GetJoystick_2(int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return Vector2.zero;

			return new Vector2(GetAxis(BGGamepadAxis.Horizontal_2, id), GetAxis(BGGamepadAxis.Vertical_2, id));
		}

		public bool ContainsName(string name) {
			for(int i=0; i<id.Count; i++) {
				if (name.Contains(id[i]))
					return true;
			}
			return false;
		}

		public void Update(int id) {
			if (id < 0 || id >= GAMEPAD_SIZE)
				return;

			//foreach(var button in axisButtonsState[id].ToArray()) {
			//	axisButtonsState[id][button.Key] = GetAxisButton(button.Key, id);
			//}

			for(int i=0; i < axisButtonsState[id].Count; i++) {
				var button = axisButtonsState[id].ElementAt(i);
				axisButtonsState[id][button.Key] = GetAxisButton(button.Key, id);
			}
		}
	}
}
