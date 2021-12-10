using System;
using UnityEngine;

namespace YsoCorp {

    [DefaultExecutionOrder(-1)]
    public class DataManager : ADataManager {

        private static string PSEUDO = "PSEUDO";
        private static string LEVEL = "LEVEL";

        private static int DEFAULT_LEVEL = 1;

        /***** CUSTOM  *****/

        // LEVEL
        public int GetLevel() {
            return this.GetInt(LEVEL, DEFAULT_LEVEL);
        }
        public int NextLevel() {
            int level = this.GetLevel() + 1;
            this.SetInt(LEVEL, this.GetLevel() + 1);
            return level;
        }
        public int PrevLevel() {
            int level = Mathf.Max(this.GetLevel() - 1, DEFAULT_LEVEL);
            this.SetInt(LEVEL, level);
            return level;
        }

        // PLAYER NAME
        public string GetPseudo() {
            return this.GetString(PSEUDO, "Player");
        }
        public void SetPseudo(string pseudo) {
            this.SetString(PSEUDO, pseudo);
        }

        // UNLOCKABLE
        private string GetId<T>() where T : UnlockableItem {
            return (string)typeof(T).GetField("ID").GetValue(null);
        }

        public int[] GetUnlocked<T>() where T : UnlockableItem {
            return this.GetArray<int>(GetId<T>() + "/unlocked");
        }
        public void SetUnlocked<T>(int num) where T : UnlockableItem {
            int[] unlocked = this.GetArray<int>(GetId<T>() + "/unlocked");
            Array.Resize(ref unlocked, unlocked.Length + 1);
            unlocked[unlocked.Length - 1] = num;
            this.SetArray(GetId<T>() + "/unlocked", unlocked);
        }
        public bool IsUnlocked<T>(int num) where T : UnlockableItem {
            int[] unlocked = this.GetArray<int>(GetId<T>() + "/unlocked");
            return Array.IndexOf(unlocked, num) != -1;
        }

        public int GetSelected<T>() where T : UnlockableItem {
            return this.GetInt(GetId<T>() + "/selected");
        }
        public void SetSelected<T>(int num) where T : UnlockableItem {
            this.SetInt(GetId<T>() + "/selected", num);
        }
        public bool IsSelected<T>(int num) where T : UnlockableItem {
            return GetSelected<T>() == num;
        }
    }
}