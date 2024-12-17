using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Storage.Model
{
    
    [Serializable]
    public class UserInfo
    {
        public ObscuredInt coin;
        public ObscuredInt score;
        public ObscuredInt level;

        public void RandomizeCryptoKey()
        {
            coin.RandomizeCryptoKey();
            score.RandomizeCryptoKey();
            level.RandomizeCryptoKey();
        }

        public override string ToString()
        {
            return $@"coin: {coin}
score: {score}
level: {level}";
        }
    }

}
