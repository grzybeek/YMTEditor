using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YMTEditor
{
    public class ComponentDrawable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _drawableIndex;
        public int drawableIndex
        {
            get { return _drawableIndex; }
            set
            {
                _drawableIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("drawableIndex"));
            }
        }
        private int _drawableTextureCount;
        public int drawableTextureCount
        {
            get { return _drawableTextureCount; }
            set
            {
                _drawableTextureCount = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("drawableTextureCount"));
            }
        }

        private int _drawablePropMask;
        public int drawablePropMask
        {
            get { return _drawablePropMask; }
            set
            {
                _drawablePropMask = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("drawablePropMask"));
            }
        }

        public int drawableAlternatives { get; set; }
        public bool drawableHasCloth { get; set; }

        public ObservableCollection<ComponentTexture> drawableTextures { get; set; }
        public ObservableCollection<ComponentInfo> drawableInfo { get; set; }

        private int _dTexturesTexId;
        public int dTexturesTexId
        {
            get { return _dTexturesTexId; }
            set
            {
                _dTexturesTexId = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("dTexturesTexId"));
            }
        }

        public ComponentDrawable(int index, int txtCount, int propMask, int numAlternatives, bool hasCloth, ObservableCollection<ComponentTexture> textureList, ObservableCollection<ComponentInfo> infoList)
        {
            drawableIndex = index;
            drawableTextureCount = txtCount;
            drawablePropMask = propMask;
            drawableAlternatives = numAlternatives;
            drawableHasCloth = hasCloth;
            drawableTextures = textureList;
            drawableInfo = infoList;
        }

        public override string ToString() //used to determine if clicked texture or drawable button (return is displayed as DataContext of button) (MainWindow.xaml.cs -> Button_Click_AddComponent)
        {
            return drawableIndex + " " + drawableTextureCount;
        }

        //default values
        public ComponentDrawable(int componentId, int index, int textureCount = 1)
        {
            drawableIndex = index;
            drawableTextureCount = textureCount;
            drawablePropMask = 1;
            drawableAlternatives = 0;
            drawableHasCloth = false;
            drawableTextures = new ObservableCollection<ComponentTexture>();
            for (int i = 0; i < textureCount; i++)
            {
                string txtLetter = XMLHandler.Number2String(i, false);
                drawableTextures.Add(new ComponentTexture(txtLetter, 0));
            };
            drawableInfo = new ObservableCollection<ComponentInfo>()
            {
                new ComponentInfo(componentId, index) // default compInfo values
            };
        }
    }
}
