using System.Collections.ObjectModel;

namespace YMTEditor
{
    public class ComponentData
    {
        public string compType;
        public int compId;
        public int compIndex;
        public ObservableCollection<ComponentDrawable> compList { get; set; }

        public string compHeader { get; set; }

        public ComponentData(string type, int id, int index, ObservableCollection<ComponentDrawable> drawables)
        {
            compType = type;
            compId = id;
            compIndex = index;
            compList = drawables;

        }

        public static int GetComponentIndexByID(int number)
        {
            foreach (var comp in MainWindow.Components)
            {
                if(comp.compId == number)
                {
                    return comp.compIndex;
                }
            }
            return -1;
        }
    }
}
