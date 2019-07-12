using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.VM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyProperyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected string GetPropertyName<T>(Expression<Func<T>> property)
        {
            MemberExpression expression = property.Body as MemberExpression;
            return expression.Member.Name;
        }

        protected void NotifyProperyChangedByName<T>(Expression<Func<T>> property)
        {
            MemberExpression expression = property.Body as MemberExpression;
            NotifyProperyChanged(expression.Member.Name);
        }

        public PropertyChangedEventArgs GetPropertyChangedEventArgs<T>(Expression<Func<T>> property)
        {
            MemberExpression expression = property.Body as MemberExpression;
            return new PropertyChangedEventArgs(expression.Member.Name);
        }
    }
}
