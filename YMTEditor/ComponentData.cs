using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YMTEditor
{
    public class ComponentData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string compType;
        public int compId;
        public int compIndex;
        private ObservableCollection<ComponentDrawable> _compList;
        public ObservableCollection<ComponentDrawable> compList
        {
            get { return _compList; }
            set
            {
                _compList = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("compList"));
            }
        }

        public string compHeader { get; set; }

        public ComponentData(string type, int id, int index, ObservableCollection<ComponentDrawable> drawables)
        {
            compType = type;
            compId = id;
            compIndex = index;
            compList = drawables;
            compHeader = compType.ToUpper();
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
