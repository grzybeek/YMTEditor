using System;
using System.Collections.Generic;
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
        public int drawableTextureCount { get; set; }
        public int drawablePropMask { get; set; }
        public int drawableAlternatives { get; set; }
        public bool drawableHasCloth { get; set; }

        public ObservableCollection<ComponentTexture> drawableTextures { get; set; }

        public ComponentDrawable(int index, int txtCount, int propMask, int numAlternatives, bool hasCloth, ObservableCollection<ComponentTexture> textureList)
        {
            drawableIndex = index;
            drawableTextureCount = txtCount;
            drawablePropMask = propMask;
            drawableAlternatives = numAlternatives;
            drawableHasCloth = hasCloth;
            drawableTextures = textureList;
        }

        public override string ToString()
        {
            return drawableIndex + " " + drawableTextureCount;
        }

    }
}
