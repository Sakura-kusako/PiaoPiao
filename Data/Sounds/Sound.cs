using Data.Globals;
using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data.Sounds
{
    public class Sound
    {
        public enum EFFECT_ID : int
        {
            SHANGTIAN = 23,
            BEIDIAN = 24,
            TRAPS_BINGSHUANG = 25,
            TRAPS_HUOYAN = 34,
            TRAPS_JIGUANG = 36,
            QIQIUBAOZHA = 42,
            TRAPS_ZHIZHU = 43,
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

        private int BGMPlayID = 0;
        private int volume = 0;
        private int effVol = 0;
        private SoundEngine engine;
        private EngineSound Sound_BGM;
        private List<EngineSound> Sound_Effs;

        public static int IntToDb(int vol)
        {
            int ret = (int)((Math.Log10(vol + 1) - 2) * 5000);
            if (ret > 0) ret = 0;
            return ret;
        }
        public Sound(Control ptr)
        {
            OnCreate();
            CheckError();
            Init(ptr);
        }
        private void CheckError()
        {
            for (int i = 0; i < bgm_str.Length; i++)
            {
                bgm_str[i] = bgm_str[i].Replace(".mp3", ".wav");
            }
        }
        public void SetVolume(int t)
        {
            volume = t;
            ChangeVolume();
        }
        public void ChangeVolume()
        {
            if(Sound_BGM!=null)
            {
                Sound_BGM.SetVol(IntToDb(volume));
            }
        }
        public int GetVolume()
        {
            return volume;
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
                 //10-19
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
                 //20-29
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
                 //30-39
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
                 //40-43
                 "碰撞2.wav",
                 "启动.wav",
                 "气球破裂.wav",
                 "蜘蛛陷阱.wav",
            };
        }
        public void Init(Control ptr)
        {
            DelRes();
            //引擎初始化
            engine = new SoundEngine(ptr.Handle);

            //加载音效
            Sound_Effs = new List<EngineSound>();
            for (int i = 0; i < effect_str.Length; i++)
            {
                var s = WavFileReader.Read(engine, effect_path + effect_str[i]);
                s.SetVol(effVol);
                Sound_Effs.Add(s);
            }
        }
        public void DelRes()
        {
            if(Sound_BGM!=null)
            {
                Sound_BGM.Dispose();
                Sound_BGM = null;
            }
            if(Sound_Effs!=null)
            {
                foreach (var s in Sound_Effs)
                {
                    s.Dispose();
                }
                Sound_Effs = null;
            }
            if(engine!=null)
            {
                engine = null;
            }
        }
        public void PlayBgm(int ID, bool IsLoop = true)
        {
            if (ID == BGMPlayID) return;
            if(IsLoop)
            {
                BGMPlayID = ID;
            }
            else
            {
                BGMPlayID = -1;
            }

            if (Sound_BGM != null)
            {
                Sound_BGM.Stop();
                Sound_BGM.Dispose();
            }
            Sound_BGM = WavFileReader.Read(engine, bgm_path + bgm_str[ID]);
            Sound_BGM.SetBgmLoop(IsLoop);
            Sound_BGM.SetVol(IntToDb(volume));
            Sound_BGM.Play();
        }
        public void PlayBgm(BGM_ID ID, bool IsLoop = true)
        {
            PlayBgm((int)(ID), IsLoop);
        }
        public void StopBgm()
        {
            BGMPlayID = -1;
        }
        public void PlayEffect(int ID)
        {
            Sound_Effs[ID].Play();
        }
        public void PlayEffect(EFFECT_ID ID)
        {
            PlayEffect((int)(ID));
        }
        public void SetEffVol(int vol)
        {
            effVol = vol;
            ChangeEffVol();
        }
        private void ChangeEffVol()
        {
            if (Sound_Effs == null) return;
            foreach (var s in Sound_Effs)
            {
                s.SetVol(IntToDb(effVol));
            }
        }
    }
}
