using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using Storage.Model;
using UnityEngine;

namespace Storage
{
    public partial class LocalDb
    {
        public LocalDb()
        {
            ObscuredPrefs.DeviceLockSettings.Level = DeviceLockLevel.Soft;
            ObscuredPrefs.DeviceLockSettings.Sensitivity = DeviceLockTamperingSensitivity.Low;

            ObscuredPrefs.NotGenuineDataDetected += () => { IS_CHEATER = true; };
            ObscuredPrefs.DataFromAnotherDeviceDetected += () => { IS_CHEATER = true; };

            if (!ObscuredPrefs.HasKey(DbKey.CHEAT_INT))
            {
                CHEAT_INT = 10;
            }

            if (!ObscuredPrefs.HasKey(DbKey.IS_CHEATER))
            {
                IS_CHEATER = false;
            }

            if (!ObscuredPrefs.HasKey(DbKey.USER_INFO))
            {
                USER_INFO = new UserInfo() {coin = 0, level = 1, score = 0};
            }

            Load();
        }

        private void Load()
        {
            cheatInt = ObscuredPrefs.Get(DbKey.CHEAT_INT, 0);
            isCheater = ObscuredPrefs.Get(DbKey.IS_CHEATER, false);
            userInfo = GetFromJson<UserInfo>(DbKey.USER_INFO);
        }
    }

    public class DbKey
    {
        public static readonly string CHEAT_INT = "CHEAT_INT";
        public static readonly string IS_CHEATER = "IS_CHEATER";
        public static readonly string USER_INFO = "USER_INFO";
    }
}