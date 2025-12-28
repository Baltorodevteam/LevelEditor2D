using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaltoroGames
{
    public class PS4Gamepad : BGGamepadMap
    {
		public PS4Gamepad() {
			id = new List<string> {
				"WIRELESS CONTROLLER"
			};

			buttons = new Dictionary<BGGamepadButton, Action> {
				{BGGamepadButton.LeftArrow, new Action{ id = "7", axis = true, invert = true} },
				{BGGamepadButton.RightArrow, new Action{ id = "7", axis = true, invert = false} },
				{BGGamepadButton.UpArrow, new Action{ id = "8", axis = true, invert = false} },
				{BGGamepadButton.DownArrow, new Action{ id = "8", axis = true, invert = true} },
				{BGGamepadButton.ButtonSouth, new Action{ id = "1", axis = false, invert = false} },
				{BGGamepadButton.ButtonEast, new Action{ id = "2", axis = false, invert = false} },
				{BGGamepadButton.ButtonWest, new Action{ id = "0", axis = false, invert = false} },
				{BGGamepadButton.ButtonNorth, new Action{ id = "3", axis = false, invert = false} },
				{BGGamepadButton.LeftBumper, new Action{ id = "4", axis = false, invert = false} },
				{BGGamepadButton.RightBumper, new Action{ id = "5", axis = false, invert = false} },
				{BGGamepadButton.StartButton, new Action{ id = "9", axis = false, invert = false} },
				{BGGamepadButton.SelectButton, new Action{ id = "8", axis = false, invert = false} },
				{BGGamepadButton.LeftStickButton, new Action{ id = "10", axis = false, invert = false} },
				{BGGamepadButton.RightStickButton, new Action{ id = "11", axis = false, invert = false} },
				{BGGamepadButton.Submit, new Action{ id = "1", axis = false, invert = false} },
				{BGGamepadButton.Cancel, new Action{ id = "2", axis = false, invert = false} },
				{BGGamepadButton.LeftTrigger, new Action{ id = "4", axis = true, invert = false} },
				{BGGamepadButton.RightTrigger, new Action{ id = "5", axis = true, invert = false} },
			};

			axis = new Dictionary<BGGamepadAxis, Action> {
				{BGGamepadAxis.LeftTrigger, new Action{ id = "4", axis = true, invert = false} },
				{BGGamepadAxis.RightTrigger, new Action{ id = "5", axis = true, invert = false} },
				{BGGamepadAxis.Horizontal_1, new Action{ id = "1", axis = true, invert = false} },
				{BGGamepadAxis.Vertical_1, new Action{ id = "2", axis = true, invert = true} },
				{BGGamepadAxis.Horizontal_2, new Action{ id = "3", axis = true, invert = false} },
				{BGGamepadAxis.Vertical_2, new Action{ id = "6", axis = true, invert = true} },
			};

			Init();
		}
	}
}