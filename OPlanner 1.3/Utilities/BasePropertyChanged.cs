using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Reflection;
using System.Linq.Expressions;

namespace PlannerNameSpace
{
    public abstract class BasePropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BasePropertyChanged()
        {
        }

        public void NotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            NotifyPropertyChangedByName(Utils.GetExpressionName(expression));
        }

        public void NotifyPropertyChangedByName([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
