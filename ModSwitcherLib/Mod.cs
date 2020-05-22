namespace ModSwitcherLib
{
    public class Mod
    {
        public Mod()
        {
            ModName = null;
            modType = ModType.File;
            ModPath = null;
            ExtraFlags = null;
            OverrideGamePath = false;
            GamePath = null;
        }

        public string ModName { get; set; }

        public ModType modType { get; set; }

        public string ModPath { get; set; }

        public string ExtraFlags { get; set; }

        public bool OverrideGamePath { get; set; }

        public string GamePath { get; set; }
    }
}
