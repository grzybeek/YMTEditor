using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMTEditor
{
    public class ComponentDrawable
    {
        public int drawableIndex { get; set; }
        public int drawableTextureCount { get; set; }
        public ObservableCollection<ComponentTexture> drawableTextures { get; set; }

        public ComponentDrawable(int index, int txtCount, ObservableCollection<ComponentTexture> textureList)
        {
            drawableIndex = index;
            drawableTextureCount = txtCount;
            drawableTextures = textureList;
        }

        public override string ToString()
        {
            return drawableIndex + " " + drawableTextureCount;
        }
    }
}
