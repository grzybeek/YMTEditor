using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YMTEditor
{
    public class PropDrawable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _propIndex;
        public int propIndex
        {
            get { return _propIndex; }
            set
            {
                _propIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("propIndex"));
            }
        }
        public string propAudioId { get; set; }
        public string[] propExpressionMods { get; set; }
        public ObservableCollection<PropTexture> propTextureList { get; set; }
        private int _propTextureCount;
        public int propTextureCount
        {
            get { return _propTextureCount; }
            set
            {
                _propTextureCount = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("propTextureCount"));
            }
        }
        public string propRenderFlags { get; set; }
        public int propPropFlags { get; set; }
        public int propFlags { get; set; }
        public int propAnchorId { get; set; }
        public int propPropId { get; set; }
        public int propHash_AC887A91 { get; set; }
        public PropDrawable(int index, int txtCount, string audioId, string[] expressionMods, ObservableCollection<PropTexture> textureList, string renderFlag, int propFlag, int flag, int anchorId, int propId, int hash)
        {
            propIndex = index;
            propTextureCount = txtCount;
            propAudioId = audioId;
            propExpressionMods = expressionMods;
            propTextureList = textureList;
            propRenderFlags = renderFlag;
            propPropFlags = propFlag;
            propFlags = flag;
            propAnchorId = anchorId;
            propPropId = propId;
            propHash_AC887A91 = hash;
        }

        public PropDrawable(int anchorId, int index, int textureCount = 1)
        {
            propIndex = index;
            propAudioId = "none";
            propExpressionMods = new string[] { "0", "0", "0", "0", "0" };
            propTextureList = new ObservableCollection<PropTexture>();
            for (int i = 0; i < textureCount; i++)
            {
                string txtLetter = XMLHandler.Number2String(i, false);
                propTextureList.Add(new PropTexture(txtLetter, i));
            };
            propRenderFlags = "";
            propPropFlags = 0;
            propFlags = 0;
            propAnchorId = anchorId;
            propPropId = index;
            propHash_AC887A91 = 0;
            propTextureCount = textureCount;
        }
        public override string ToString() //used to determine if clicked texture or prop button (return is displayed as DataContext of button) (MainWindow.xaml.cs -> Button_Click_AddProp)
        {
            return propIndex + " " + propTextureCount;
        }
    }
}
