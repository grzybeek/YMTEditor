
using System.ComponentModel;

namespace YMTEditor
{
    public class ComponentTexture : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string textureIndex { get; set; }
        private string _textureTexId;

        public string textureTexId
        {
            get { return _textureTexId; }
            set
            {
                _textureTexId = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("textureTexId"));
            }
        }

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