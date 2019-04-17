using System;
using System.Runtime.CompilerServices;

namespace ADTConvert2.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChunkOrderAttribute : System.Attribute
    {
        public ChunkOrderAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }

        public int Order { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChunkOptionalAttribute : System.Attribute
    {
        public ChunkOptionalAttribute(bool optional = true)
        {
            Optional = optional;
        }

        public bool Optional { get; }
    }
}
