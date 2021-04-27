
namespace YMTEditor
{
    public class ComponentTexture
    {
        public string textureIndex { get; set; }
        public string textureTexId { get; set; }

        public ComponentTexture(string index, string texId)
        {
            textureIndex = index;
            textureTexId = texId;
        }

        public ComponentTexture(string index)
        {
            textureIndex = index;
        }


    }
}