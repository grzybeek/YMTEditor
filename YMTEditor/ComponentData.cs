using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMTEditor
{
    public class ComponentData
    {
        public string compType;
        public int compId;
        public int compIndex;
        public ObservableCollection<ComponentDrawable> compListItemsControl { get; set; }

        public string compHeader { get; set; }

        public ComponentData(string type, int id, int index, ObservableCollection<ComponentDrawable> drawables)
        {
            compType = type;
            compId = id;
            compIndex = index;
            compListItemsControl = drawables;

        }

        public override string ToString()
        {
            return compType + " " + compId;
        }
    }
}
