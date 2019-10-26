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
        }

        public Mod(Mod mod)
        {
            ModName = mod.ModName;
            modType = mod.modType;
            ModPath = mod.ModPath;
            Flag = mod.Flag;
            UsingModPath = mod.UsingModPath;
        }

        public string ModName { get; set; }

        public ModType modType { get; set; }

        public string ModPath { get; set; }

        public string Flag { get; set; }

        public bool UsingModPath { get; set; }
    }
}
