using System.ComponentModel;

namespace YMTEditor
{
    public class ComponentTexture : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string textureIndex { get; set; }
        private int _textureTexId;

        public int textureTexId
        {
            get { return _textureTexId; }
            set
            {
                _textureTexId = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("textureTexId"));
            }
        }

        public ComponentTexture(string index, int texId)
        {
            textureIndex = index;
            textureTexId = texId;
        }

        public ComponentTexture()
        {
            textureIndex = "a";
            textureTexId = 0;
        }
    }
}