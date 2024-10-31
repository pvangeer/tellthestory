using System.ComponentModel;
using Forest.Data;
using Forest.Gui;

namespace Forest.Visualization
{
    public class GuiViewModelBase : NotifyPropertyChangedObject
    {
        protected readonly ForestGui Gui;

        protected GuiViewModelBase(ForestGui gui)
        {
            Gui = gui;
            if (gui != null)
                gui.PropertyChanged += GuiPropertyChanged;
        }

        protected virtual void GuiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}