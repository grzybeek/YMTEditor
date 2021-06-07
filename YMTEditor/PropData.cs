using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMTEditor
{
    public class PropData
    {
        public string propType;
        public int propId;
        public ObservableCollection<PropDrawable> propList { get; set; }

        public string propHeader { get; set; }

        public PropData(string type, int id, ObservableCollection<PropDrawable> props)
        {
            propType = type;
            propId = id;
            propList = props;

        }

    }
}
