using ModSwitcherLib.Types;

namespace ModSwitcherLib
{
    public class Mod
    {
        public Mod()
        {
            ModName = null;
            modType = ModType.File;
            ModPath = null;
            Flag = null;
            UsingModPath = true;
            OverrideGamePath = false;
            GamePath = null;
            SetVersion = false;
            Version = null;
        }

        public string ModName { get; set; }

        public ModType modType { get; set; }

        public string ModPath { get; set; }

        public string Flag { get; set; }

        public bool UsingModPath { get; set; }

        public bool OverrideGamePath { get; set; }

        public string GamePath { get; set; }

        public bool SetVersion { get; set; }

        public string Version { get; set; }
    }
}
