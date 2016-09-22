using DevelopmentVirtualSensorShield.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevelopmentVirtualSensorShield.Common
{
    public class DelegateCommand
       : ICommand
    {
        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Fields

        private Func<object, bool> canExecuteFunction;
        private Action<object> executeAction;
        private bool canExecutePreviousValue;

        #endregion

        #region .ctor

        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecuteFunction = canExecute;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            try
            {
                bool functionResult = canExecuteFunction(parameter);

                if (this.canExecutePreviousValue != functionResult)
                {
                    this.canExecutePreviousValue = functionResult;

                    if (this.CanExecuteChanged != null)
                        this.CanExecuteChanged(this, new EventArgs());
                }

                return this.canExecutePreviousValue;
            }
            catch (NullReferenceException)
            {
                return true;
            }
            catch (Exception exc)
            {
                StringBuilder errorBuilder = null;

                errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Error executing MVVM command.");
                errorBuilder.AppendLine(exc.ToString());
                Tracer.Default.Log(errorBuilder.ToString(), LogLevels.Error);

                return true;
            }
        }

        public void Execute(object parameter)
        {
            this.executeAction.Invoke(parameter);
        }


        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            try
            {
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, e);
            }
            catch (Exception exc)
            {
                StringBuilder errorBuilder = null;

                errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Error executing MVVM command.");
                errorBuilder.AppendLine(exc.ToString());
                Tracer.Default.Log(errorBuilder.ToString(), LogLevels.Error);
            }
        }

        #endregion
    }
}
