namespace YMTEditor
{
    public class PropTexture
    {
        public string textureIndex { get; set; }

        public string propInclusions { get; set; }
        public string propExclusions { get; set; }
        public int propTexId { get; set; }//different thing than in components, its index of texture of current prop
        public int propInclusionId { get; set; }
        public int propExclusionId { get; set; }
        public int propDistribution { get; set; }

        public PropTexture(string txtletter, string inclusions, string exclusions, int index, int inclusionId, int exclusionId, int distribution)
        {
            textureIndex = txtletter;
            propInclusions = inclusions;
            propExclusions = exclusions;
            propTexId = index;
            propInclusionId = inclusionId;
            propExclusionId = exclusionId;
            propDistribution = distribution;
        }

    }
}