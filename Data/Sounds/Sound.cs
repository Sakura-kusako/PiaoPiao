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
            QIQIUBAOZHA = 42,
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
        const int effLen = 5;

        private string bgm_path;
        private string effect_path;
        private string[] bgm_str;
        private string[] effect_str;

        private Control form;
        private BufferDescription buffDes = null; 
        private Microsoft.DirectX.DirectSound.Device ds = null; //设备对象
        private int BGMPlayID = 0;
        private int volume = 0;
        private int effVol = 0;
        private SecondaryBuffer secBufferBGM;
        public EffectSound[] secBufferEffs;

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
            form = ptr;
            Init();
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
            if(secBufferBGM!=null)
            {
                secBufferBGM.Volume = IntToDb(volume);
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
            DelRes();
            ds = new Microsoft.DirectX.DirectSound.Device();           
            ds.SetCooperativeLevel(form,CooperativeLevel.Normal);
            secBufferEffs = new EffectSound[effLen];
            for (int i = 0; i < effLen; i++)
            {
                secBufferEffs[i] = new EffectSound();
            }
            //*/
        }
        public void DelRes()
        {
            if(secBufferBGM!=null)
            {
                secBufferBGM.Stop();
                secBufferBGM.Dispose();
                secBufferBGM = null;
            }
            if(ds != null)
            {
                ds.Dispose();
                ds = null;
            }
            if(secBufferEffs!=null)
            {
                for (int i = 0; i < secBufferEffs.Length; i++)
                {
                    if (secBufferEffs[i] != null)
                    {
                        secBufferEffs[i].DelRes();
                    }
                }
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

            buffDes = new BufferDescription();
            buffDes.GlobalFocus = true;
            buffDes.ControlVolume = true;

            if(secBufferBGM!=null)
            {
                secBufferBGM.Stop();
                secBufferBGM.Dispose();
                secBufferBGM = null;
            }
            secBufferBGM = new SecondaryBuffer(bgm_path + bgm_str[ID], buffDes,ds);
            ChangeVolume();
            secBufferBGM.Play(0, IsLoop ? BufferPlayFlags.Looping : BufferPlayFlags.Default);
        }
        public void PlayBgm(BGM_ID ID, bool IsLoop = true)
        {
            PlayBgm((int)(ID), IsLoop);
        }
        public void StopBgm()
        {
            if (secBufferBGM != null)
                secBufferBGM.Stop();
            BGMPlayID = -1;
        }
        public void PlayEffect(int ID)
        {
            if (secBufferEffs != null)
            {
                for (int i = 0; i < secBufferEffs.Length; i++)
                {
                    if (secBufferEffs[i] != null)
                    {
                        if(secBufferEffs[i].IsPlaying() == false)
                        {
                            secBufferEffs[i].Play(effect_path + effect_str[ID], ds, effVol);
                        }
                    }
                }
            }
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
            if (secBufferEffs != null)
            {
                for (int i = 0; i < secBufferEffs.Length; i++)
                {
                    if(secBufferEffs[i]!=null)
                    {
                        secBufferEffs[i].SetVol(effVol);
                    }
                }
            }
        }

        public class EffectSound
        {
            private SecondaryBuffer secBuffer = null;
            private BufferDescription buffDes = null;
            private int len = 0;

            public bool IsPlaying()
            {
                if(secBuffer!=null)
                {
                    if(secBuffer.PlayPosition >= len - 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            public void Play(string path,Device ds,int vol)
            {
                buffDes = new BufferDescription();
                buffDes.GlobalFocus = true;
                buffDes.ControlVolume = true;

                if (secBuffer != null)
                {
                    secBuffer.Stop();
                    secBuffer.Dispose();
                    secBuffer = null;
                }
                secBuffer = new SecondaryBuffer(path, buffDes, ds);
                len = buffDes.BufferBytes;
                secBuffer.Volume = IntToDb(vol);
                secBuffer.Play(0, BufferPlayFlags.Default);
            }
            public void DelRes()
            {
                try
                {
                    if(secBuffer!=null)
                    {
                        secBuffer.Dispose();
                        secBuffer = null;
                    }
                }
                catch(Exception)
                {

                }
            }
            public void SetVol(int vol)
            {
                if (secBuffer != null)
                    secBuffer.Volume = IntToDb(vol);
            }
        }
    }
}
