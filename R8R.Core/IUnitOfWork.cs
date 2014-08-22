using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R8R.Core
{
    public abstract class IUnityOfWork
    {
        IR8RContext R8RContext { get; set; }
        public abstract void Commit();
        public IUnityOfWork UnityOfWork
        {
            get
            {
                return this;
            }
        }
    }
}
