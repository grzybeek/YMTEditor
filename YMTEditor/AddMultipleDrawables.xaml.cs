using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace YMTEditor
{
    public partial class AddMultipleDrawables
    {
        public ObservableCollection<ComponentDrawable> newCompDrawables { get; set; }
        private ComponentData ModifiedComp;

        public ObservableCollection<PropDrawable> newPropDrawables { get; set; }
        private PropData ModifiedProp;

        private int tempIndex;
        private int defaultTexturesAmount;
        public AddMultipleDrawables(ComponentData comp)
        {
            ModifiedComp = comp;
            tempIndex = ModifiedComp.compList.Count;

            newCompDrawables = new ObservableCollection<ComponentDrawable>();
            defaultTexturesAmount = 1;

            this.DataContext = this;
            InitializeComponent();

            WindowHeader.Text = comp.compHeader + " Component";
            NewDrawablesAmount.Maximum = 255 - ModifiedComp.compList.Count;
        }
        public AddMultipleDrawables(PropData prop)
        {
            ModifiedProp = prop;
            tempIndex = ModifiedProp.propList.Count;

            newPropDrawables = new ObservableCollection<PropDrawable>();
            defaultTexturesAmount = 1;

            this.DataContext = this;
            InitializeComponent();

            WindowHeader.Text = prop.propHeader + " Prop";
            NewDrawablesAmount.Maximum = 255 - ModifiedProp.propList.Count;
        }

        private int oldDrawablesAmount = 0;
        private void DrawablesAmount_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null) return;
            if (e.NewValue == null) return;

            int newDrawablesAmount = (int)e.NewValue;
            if(newDrawablesAmount != oldDrawablesAmount)
            {
                int diff = Math.Abs(oldDrawablesAmount - newDrawablesAmount);

                if(oldDrawablesAmount < newDrawablesAmount)
                {
                    //add difference
                    for (int i = 0; i < diff; i++)
                    {
                        if(IsComponent())
                        {
                            newCompDrawables.Add(new ComponentDrawable(ModifiedComp.compId, tempIndex++, defaultTexturesAmount));
                        }
                        else if (IsProp())
                        {
                            newPropDrawables.Add(new PropDrawable(ModifiedProp.propAnchorId, tempIndex++, defaultTexturesAmount));
                        }
                    }
                } 
                else if(oldDrawablesAmount > newDrawablesAmount)
                {

                    //remove difference
                    for (int i = diff; i > 0; i--)
                    {
                        if (newCompDrawables != null)
                        {
                            newCompDrawables.RemoveAt(newCompDrawables.Count - 1);
                        }
                        else if (newPropDrawables != null)
                        {
                            newPropDrawables.RemoveAt(newPropDrawables.Count - 1);
                        }
                        tempIndex--;
                    }
                }

                oldDrawablesAmount = newDrawablesAmount;
            }
        }

        private void drawableTextures_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null) return;
            if (e.NewValue == null) return;

            ComponentDrawable compDrawable = null;
            PropDrawable propDrawable = null;

            int newTexturesAmount = (int)e.NewValue;
            int oldTexturesAmount = 1;

            if (IsComponent())
            {
                compDrawable = (ComponentDrawable)(sender as FrameworkElement).DataContext;
                oldTexturesAmount = compDrawable.drawableTextures.Count;
            } 
            else if (IsProp())
            {
                propDrawable = (PropDrawable)(sender as FrameworkElement).DataContext;
                oldTexturesAmount = propDrawable.propTextureList.Count;
            }

            if (newTexturesAmount != oldTexturesAmount)
            {
                int diff = Math.Abs(oldTexturesAmount - newTexturesAmount);

                if (oldTexturesAmount < newTexturesAmount)
                {
                    //add difference
                    for (int i = oldTexturesAmount; i < newTexturesAmount; i++)
                    { 
                        string txtLetter = XMLHandler.Number2String(i, false);
                        if (IsComponent())
                        {
                            compDrawable.drawableTextures.Add(new ComponentTexture(txtLetter, compDrawable.dTexturesTexId));
                        }
                        else if (IsProp())
                        {
                            propDrawable.propTextureList.Add(new PropTexture(txtLetter, i));
                        }
                    }
                }
                else if (oldTexturesAmount > newTexturesAmount)
                {
                    //remove difference
                    for (int i = diff; i > 0; i--)
                    {
                        if (IsComponent())
                        {
                            compDrawable.drawableTextures.RemoveAt(compDrawable.drawableTextures.Count - 1);
                        }
                        else if (IsProp())
                        {
                            propDrawable.propTextureList.RemoveAt(propDrawable.propTextureList.Count - 1);
                        }
                    }
                }
            }
        }

        private void DefaultTextures_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null) return;
            if (e.NewValue == null) return;

            defaultTexturesAmount = (int)e.NewValue;
        }

        private void AddDrawables_Button(object sender, RoutedEventArgs e)
        {
            if (IsComponent())
            {
                if (newCompDrawables.Count > 0)
                {
                    var result = new ObservableCollection<ComponentDrawable>(ModifiedComp.compList.Concat(newCompDrawables));
                    ModifiedComp.compList.Clear();
                    newCompDrawables.Clear();
                    ModifiedComp.compList = result;
                    newCompDrawables = null;
                }
            }
            else if (IsProp())
            {
                if(newPropDrawables.Count > 0)
                {
                    var result = new ObservableCollection<PropDrawable>(ModifiedProp.propList.Concat(newPropDrawables));
                    ModifiedProp.propList.Clear();
                    newPropDrawables.Clear();
                    ModifiedProp.propList = result;
                    newPropDrawables = null;
                }
            }

            Close();
        }

        private void TXTCombo_DropDownClosed(object sender, EventArgs e)
        {
            if (e == null) return;
            int index = (sender as ComboBox).SelectedIndex;
            ComponentDrawable drawable = (ComponentDrawable)(sender as FrameworkElement).DataContext;

            drawable.dTexturesTexId = index;

            foreach (var txt in drawable.drawableTextures)
            {
                txt.textureTexId = index;
            }

            if (index >= 1)
            {
                drawable.drawablePropMask = 17; //_r _whi/bla/chi/lat/etc propmask
            }
            else
            {
                drawable.drawablePropMask = 1; //_u _uni propmask
            }

        }
        private bool IsComponent()
        {
            if (newCompDrawables != null)
            {
                return true;
            }
            return false;
        }

        private bool IsProp()
        {
            if (newPropDrawables != null)
            {
                return true;
            }
            return false;
        }
    }

    public class SwitchCollectionsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var comp = values[0];
            var prop = values[1];

            if (comp != null)
            {
                return comp;
            }
            else if (prop != null)
            {
                return prop;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SwitchTemplate : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element)
            {
                if (item is ComponentDrawable)
                {
                    return element.FindResource("componentTemplate") as DataTemplate;
                }
                else if (item is PropDrawable)
                {
                    return element.FindResource("propTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
