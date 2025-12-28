using UnityEngine;

namespace BaltoroGames{
    public abstract class BGInputBase{
        public abstract bool GetButtonDown(BGGamepadButton gamepadButton, int playerId = 0);
        public abstract bool GetButtonUp(BGGamepadButton gamepadButton, int playerId = 0);
        public abstract bool GetButton(BGGamepadButton gamepadButton, int playerId = 0);

        public abstract float GetAxis(BGGamepadAxis gamepadAxis, int playerId = 0);

        public abstract Vector2 GetJoystickLeft(int playerId = 0);
        public abstract Vector2 GetJoystickRight(int playerId = 0);

        public virtual void Update(){}

        public virtual void SetVibration(BGGamepad.Vibration vibration, int playerId = 0) { }

		protected Vector2 GetVectorWithDeadzone(Vector2 vector) {
			float magnitude = vector.magnitude;

			if (magnitude <= BGInput.AxisDeadzone || BGInput.AxisDeadzone >= 1.0f)
				return Vector2.zero;

			return vector.normalized * ((magnitude - BGInput.AxisDeadzone) / (1f - BGInput.AxisDeadzone));
		}
	}
}
