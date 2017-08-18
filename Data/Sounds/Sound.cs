using Data.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Sounds
{
    public class Sound
    {
        public enum EFFECT_ID : int
        {

        }
        public enum BGM_ID : int
        {
            JINGSU = 0,
            DRAW,
            FLAG_MIC_WIN,
            FLAG_TOY_WIN,
            HALL,
            LOSE,
            MAP_DEADMODE_A,
            MAP_DEADMODE_B,
            MAP_LOOTMODE,
            MAP_MACCITY,
            ROOM,
            WIN,
        }

        private string bgm_path;
        private string effect_path;
        private string[] bgm_str;
        private string[] effect_str;

        public Sound()
        {
            OnCreate();
            CheckError();
        }
        private void CheckError()
        {
            for (int i = 0; i < bgm_str.Length; i++)
            {
                bgm_str[i] = bgm_str[i].Replace(".mp3", ".wav");
            }
        }
        public void OnCreate()
        {
            bgm_path = GlobalB.GetRootPath() + @"\balloon\music\";
            effect_path = GlobalB.GetRootPath() + @"\balloon\effect\";

            bgm_str = new string[]
            {
                "cityoftoys.mp3",
                "Draw.mp3",
                "flag_mic_win.mp3",
                "flag_toy_win.mp3",
                "hall.mp3",
                "Lose.mp3",
                "map_deadmode_1.mp3",
                "map_deadmode_2.mp3",
                "map_lootmode.mp3",
                "map_maccity.mp3",
                "room.mp3",
                "Win.mp3"
            };

            effect_str = new string[]
            {
                 "bighit.wav",
                 "But_Click.wav",
                 "Dart_Begin.wav",
                 "Dart_Circle.wav",
                 "Dart_Hit.wav",
                 "Dart_Lose.wav",
                 "dm_dida.wav",
                 "dm_hint.wav",
                 "Entropy_Full.wav",
                 "flag_mic_get.wav",
                 "flag_mic_levelup.wav",
                 "flag_toy_get.wav",
                 "flag_toy_levelup.wav",
                 "hit.wav",
                 "lm_ts1.wav",
                 "lm_ts2.wav",
                 "lm_ts3.wav",
                 "lm_ts4.wav",
                 "lm_ufo.wav",
                 "lm_ufo_throw.wav",
                 "VS.wav",
                 "按钮1.wav",
                 "按钮2.wav",
                 "爆炸.wav",
                 "被电.wav",
                 "冰霜陷阱.wav",
                 "吹气.wav",
                 "大炮陷阱_爆炸.wav",
                 "大炮陷阱_发射.wav",
                 "道具1.wav",
                 "道具2.wav",
                 "地图.wav",
                 "滑动1.wav",
                 "滑动2.wav",
                 "火焰陷阱.wav",
                 "击杀.wav",
                 "激光陷阱.wav",
                 "角色滑动.wav",
                 "禁止.wav",
                 "巨锤陷阱.wav",
                 "碰撞2.wav",
                 "启动.wav",
                 "气球破裂.wav",
                 "蜘蛛陷阱.wav",
            };
        }
        public void Init()
        {

        }
        public void DelRes()
        {

        }
        public void PlayBgm(int ID, bool IsLoop = true)
        {
        }
        public void PlayBgm(BGM_ID ID, bool IsLoop = true)
        {
            PlayBgm((int)(ID), IsLoop);
        }
        public void StopBgm()
        {
        }
        public void PlayEffect(int ID)
        {
        }
        public void PlayEffect(EFFECT_ID ID)
        {
            PlayEffect((int)(ID));
        }
    }
}
