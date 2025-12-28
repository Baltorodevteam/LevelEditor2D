using System.Collections.Generic;
using UnityEngine;

public enum BGGamepadButton{
    LeftArrow,
    RightArrow,
    UpArrow,
    DownArrow,
    ButtonWest,
    ButtonEast,
    ButtonNorth,
    ButtonSouth,
    LeftBumper,
    RightBumper,
    StartButton,
    SelectButton,
    LeftStickButton,
    RightStickButton,
    Submit,
    Cancel,
	LeftTrigger,
	RightTrigger,
}

public enum BGGamepadAxis{
    LeftTrigger,
    RightTrigger,
    Horizontal_1,
    Horizontal_2,
    Vertical_1,
    Vertical_2,
}

namespace BaltoroGames{
    public static class BGGamepad{

        public class Gamepad{
            public bool empty;
            public int id;
            public int xboxId;
            public string name;
            public BGGamepadMap gamepadMap;
        }

        public class Vibration{
            public float power_1;
            public float power_2;
            public float time;
		}

        private static List<Gamepad> slots = new List<Gamepad>();
        private static List<Gamepad> currentGamepads = new List<Gamepad>();
        private static List<int> newGamepadsId = new List<int>();

        private static Gamepad tempGamepad = new Gamepad();

        private static BGGamepadMap[] gamepadsMap = new BGGamepadMap[] {
            new XboxGamepad(),
            new PS4Gamepad(),
        };

        static BGGamepad(){
            for(int i=0; i<16; i++){
                slots.Add(new Gamepad());
            }
        }

        public static void Update(){
#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE

			CreateGamepadsList();

            newGamepadsId.Clear();

            foreach(var item in slots){
                item.empty = true;
            }

            for(int i=0; i<currentGamepads.Count; i++){
                var currGamepad = currentGamepads[i];

                if(currGamepad.empty)
                    break;

                var slot = FindSlot(currGamepad);

                if(slot != null){
                    slot.empty = false;
                    slot.id = currGamepad.id;
                    slot.xboxId = currGamepad.xboxId;
                    slot.name = currGamepad.name;
                    slot.gamepadMap = currGamepad.gamepadMap;
                }
                else{
                    newGamepadsId.Add(i);
                }
            }

            foreach(var id in newGamepadsId){
                SetSlot(currentGamepads[id]);
            }

            ClearEmptySlots();

#endif
        }

        public static void UpdateButtons() {
            for(int i=0; i<slots.Count; i++) {
                var slot = slots[i];

                if (slot.empty || slot.gamepadMap == null)
                    continue;

                slot.gamepadMap.Update(slot.id);
			}
		}

        public static int GetGamepadId(int playerId){
            if(playerId == 0)
                return 0;
            if(playerId < 1 || playerId > slots.Count)
                return -1;

            return slots[playerId - 1].empty ? -1 : slots[playerId - 1].id;
        }

        public static int GetXboxGamepadId(int playerId) {
            if (playerId < 1 || playerId > slots.Count)
                return -1;

            return slots[playerId - 1].empty ? -1 : slots[playerId - 1].xboxId;
        }

        public static BGGamepadMap GetGamepadMap(int playerId) {
            if (playerId <= 0 || playerId > slots.Count)
                return null;

            return slots[playerId - 1].empty ? null : slots[playerId - 1].gamepadMap;
		}

        public static void ChangeGamepadId(int currentPlayerId, int newPlayerId){
            if(currentPlayerId == newPlayerId || currentPlayerId <= 0 || newPlayerId <= 0 || currentPlayerId > slots.Count || newPlayerId > slots.Count)
                return;

            var currentGamepad = slots[currentPlayerId - 1];
            var newGamepad = slots[newPlayerId - 1];

            CopyGamepad(newGamepad, tempGamepad);
            CopyGamepad(currentGamepad, newGamepad);
            CopyGamepad(tempGamepad, currentGamepad);
        }

        private static void CopyGamepad(Gamepad from, Gamepad to){
            to.empty = from.empty;
            to.id = from.id;
            to.xboxId = from.xboxId;
            to.name = from.name;
            to.gamepadMap = from.gamepadMap;
        }

        public static List<Gamepad> GetSlots(){
            return slots;
        }

        private static void CreateGamepadsList(){
            foreach(var item in currentGamepads){
                item.empty = true;
                item.id = -1;
                item.xboxId = -1;
            }

            string[] gamepads = Input.GetJoystickNames();
            int id = 0;

            for(int i=0; i<gamepads.Length; i++){
                if(!string.IsNullOrEmpty(gamepads[i])){
                    if(currentGamepads.Count > id){
                        var slot = currentGamepads[id];
                        slot.empty = false;
                        slot.id = i + 1;
                        slot.xboxId = id;
                        
                        if (slot.name != gamepads[i] || slot.gamepadMap == null) {
                            slot.name = gamepads[i];
                            slot.gamepadMap = FindGamepadMap(gamepads[i]);
                        }
                    }
                    else{
                        var slot = new Gamepad{
                            empty = false,
                            id = i + 1,
                            xboxId = id,
                            name = gamepads[i],
                            gamepadMap = FindGamepadMap(gamepads[i])
                        };
                        currentGamepads.Add(slot);
                    }

#if UNITY_WSA && !UNITY_EDITOR
                    var temp = currentGamepads[id];
                    string [] s = temp.name.Split(' ');
                    int.TryParse(s[s.Length - 1], out temp.id);
#endif

                    // currentGamepads[id].active = IsGamepadActive(currentGamepads[id].id);

                    id++;
                }
            }
        }

        private static BGGamepadMap FindGamepadMap(string name) {
            name = name.ToUpper();
            for(int i=0; i<gamepadsMap.Length; i++) {
                if (gamepadsMap[i].ContainsName(name))
                    return gamepadsMap[i];
			}
            return gamepadsMap[0];
		}

        private static Gamepad FindSlot(Gamepad gamepad){
            for(int i=0; i<slots.Count; i++){

#if UNITY_WSA && !UNITY_EDITOR
                if(slots[i].name != null && slots[i].name.Equals(gamepad.name))
#else
                if(slots[i].id == gamepad.id)
#endif
                {
                    return slots[i];
                }
            }

            return null;
        }

        private static void ClearEmptySlots(){
            foreach(var item in slots){
                if(item.empty){
                    item.id = -1;
                    item.xboxId = -1;
                }
            }
        }

        private static void SetSlot(Gamepad gamepad){
            if(gamepad.empty || string.IsNullOrEmpty(gamepad.name))
                return;

            for(int i=0; i<slots.Count; i++){
                var slot = slots[i];
                if(slot.empty){
                    slot.empty = false;
                    slot.id = gamepad.id;
                    slot.xboxId = gamepad.xboxId;
                    slot.name = gamepad.name;
                    slot.gamepadMap = gamepad.gamepadMap;
                    return;
                }
            }
        }
    }
}