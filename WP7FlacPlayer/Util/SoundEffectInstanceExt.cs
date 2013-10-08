using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Audio;

namespace WP7FlacPlayer.Util
{
    public static class SoundEffectInstanceExt
    {
        public static void Play(this SoundEffectInstance instance,float volume,float pitch,float pan)
        {
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
        }
    }
}
