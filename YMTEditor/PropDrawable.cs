using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string propRenderFlags { get; set; }
        public int propPropFlags { get; set; }
        public int propFlags { get; set; }
        public int propAnchorId { get; set; }
        public int propPropId { get; set; }
        public int propHash_AC887A91 { get; set; }

        public PropDrawable(int index, string audioId, string[] expressionMods, ObservableCollection<PropTexture> textureList, string renderFlag, int propFlag, int flag, int anchorId, int propId, int hash)
        {
            propIndex = index;
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

        public int propTextureCount
        {
            get { return propTextureList.Count(); }
            set { }
        }

        public override string ToString() //used to determine if clicked texture or prop button (return is displayed as DataContext of button) (MainWindow.xaml.cs -> Button_Click_AddProp)
        {
            return propIndex + " " + propTextureCount;
        }
    }
}
