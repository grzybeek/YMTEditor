using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMTEditor
{
    public class ComponentDrawable
    {
        public int drawableIndex { get; set; }
        public int drawableTextureCount { get; set; }

        public ComponentDrawable(int index, int txtCount)
        {
            drawableIndex = index;
            drawableTextureCount = txtCount;
        }

        public override string ToString()
        {
            return drawableIndex + " " + drawableTextureCount;
        }
    }
}
