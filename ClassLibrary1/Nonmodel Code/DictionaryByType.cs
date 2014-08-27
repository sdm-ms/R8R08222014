using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Nonmodel_Code
{
    public class DictionaryByType
    {
        MultiDictionary<Type, object> multidictionary = new MultiDictionary<Type, object>();

        public void Add(object o)
        {
            multidictionary.Add(o.GetType(), o);
        }

        public ICollection<T> GetCollectionOfType<T>()
        {
            var md = multidictionary[typeof(T)];
            var result = md.Select(x => (T)x).ToList(); // not sure why I need to do this
            return (ICollection<T>)result;
        }
    }
}
