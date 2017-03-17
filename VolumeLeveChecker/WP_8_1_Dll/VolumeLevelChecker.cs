using Windows.Media;

namespace DTI.Lantern.WPPlugins
{
    public class VolumeLevelChecker
    {
        public static float GetRelativeVolumeLevel()
        {
            return (float)SystemMediaTransportControls.GetForCurrentView().SoundLevel / 2;
        }
    }
}
