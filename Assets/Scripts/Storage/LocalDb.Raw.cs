using System;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Storage.Model;

namespace Storage
{
    public partial class LocalDb
    {
        private UserInfo userInfo;

        public UserInfo USER_INFO
        {
            get => userInfo;
            set
            {
                userInfo = value;
                userInfo.RandomizeCryptoKey();
                SetFromTData(DbKey.USER_INFO, userInfo);
            }
        }

        private ObscuredInt cheatInt;

        public ObscuredInt CHEAT_INT
        {
            get => cheatInt;
            set
            {
                cheatInt = value;
                cheatInt.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.CHEAT_INT, cheatInt);
            }
        }

        private ObscuredBool isCheater;

        public ObscuredBool IS_CHEATER
        {
            get => isCheater;
            set
            {
                isCheater = value;
                isCheater.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.IS_CHEATER, isCheater);
            }
        }
    }
}