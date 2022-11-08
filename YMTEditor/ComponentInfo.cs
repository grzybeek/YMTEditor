namespace YMTEditor
{
    public class ComponentInfo
    {
        public string infoHash_2FD08CEF { get; set; } //unknown usage
        public string infoHash_FC507D28 { get; set; } //unknown usage
        public string[] infoHash_07AE529D { get; set; } //probably expressionMods(?) - used for heels for example
        public int infoFlags { get; set; } //unknown usage
        public string infoInclusions { get; set; } //unknown usage
        public string infoExclusions { get; set; } //unknown usage
        public string infoHash_6032815C { get; set; } //unknown usage - always "PV_COMP_HEAD" (?)
        public int infoHash_7E103C8B { get; set; } //unknown usage

        public int infoHash_D12F579D { get; set; } //component id (jbib = 11, feet = 6, etc)
        public int infoHash_FA1F27BF { get; set; } //drawable index (000, 001, 002, etc)

        public ComponentInfo(string hash_2FD08CEF, string hash_FC507D28, string[] hash_07AE529D, int flags, string inclusions, string exclusions, string hash_6032815C, int hash_7E103C8B, int hash_D12F579D, int hash_FA1F27BF)
        {
            infoHash_2FD08CEF = hash_2FD08CEF;
            infoHash_FC507D28 = hash_FC507D28;
            infoHash_07AE529D = hash_07AE529D;
            infoFlags = flags;
            infoInclusions = inclusions;
            infoExclusions = exclusions;
            infoHash_6032815C = hash_6032815C;
            infoHash_7E103C8B = hash_7E103C8B;

            infoHash_D12F579D = hash_D12F579D;
            infoHash_FA1F27BF = hash_FA1F27BF;
        }

        public ComponentInfo(int componentId, int drawableIndex)
        {
            infoHash_2FD08CEF = "none";
            infoHash_FC507D28 = "none";
            infoHash_07AE529D = new string[] { "0", "0", "0", "0", "0" };
            infoFlags = 0;
            infoInclusions = "0";
            infoExclusions = "0";
            infoHash_6032815C = "PV_COMP_HEAD";
            infoHash_7E103C8B = 0;

            infoHash_D12F579D = componentId;
            infoHash_FA1F27BF = drawableIndex;
        }
    }
}