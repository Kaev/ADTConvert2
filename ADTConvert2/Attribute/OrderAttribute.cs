using System;
using System.Runtime.CompilerServices;

namespace ADTConvert2.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : System.Attribute
    {
        private readonly int order_;
        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
}
