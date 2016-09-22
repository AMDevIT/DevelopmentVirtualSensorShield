using DevelopmentVirtualSensorShield.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace DevelopmentVirtualSensorShield.ViewModels
{
    public abstract class ViewModelBase
        : BindableBase
    {
        protected void Dispatch(DispatchedHandler callback)
        {
            Task dispatchedTask = null;

            if (CoreApplication.MainView != null)
                dispatchedTask = CoreApplication.MainView.
                                 Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, callback).AsTask();
        }
    }
}
