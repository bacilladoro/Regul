using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Regul.ViewModels;
using System;

namespace Regul
{
    internal class ViewLocator : IDataTemplate
    {
        public IControl Build(object param)
        {
            string name = param.GetType().FullName.Replace("ViewModel", "View");
            Type type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
