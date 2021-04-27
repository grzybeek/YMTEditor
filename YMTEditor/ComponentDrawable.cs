using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YMTEditor
{
    public class ComponentDrawable
    {
        public int drawableIndex { get; set; }
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
