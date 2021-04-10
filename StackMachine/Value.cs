using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StackMachine
{
    public enum ValueType
    {
        NIL = 0,
        INT,
        NUMBER,
        BOOL,
        SYMBOL,
        STRING, //reference types
        CLOSURE, //closure
        PAIR,
        ARRAY,
        STRUCT,

        //Internal use only
        UPVALUE
    };

    /// <summary>
    /// This is the basic Value, it supports basic Value types and a CDR pointer to the next element
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Value
    {
        [FieldOffset(0)]
        public readonly ValueType type;
        //pointer or value of the first field
        [FieldOffset(sizeof(ValueType))]
        public readonly int i32;
        [FieldOffset(sizeof(ValueType))]
        public readonly float fl;
        [FieldOffset(sizeof(ValueType))]
        public readonly bool b;

        private static readonly Value nilValue = new Value(ValueType.NIL, 0);
        private static readonly Value trueValue = new Value(true);
        private static readonly Value falseValue = new Value(false);
        public static ref readonly Value Nil => ref nilValue;
        public static ref readonly Value True => ref trueValue;
        public static ref readonly Value False => ref falseValue;


        ///why this() ?
        ///https://github.com/dotnet/roslyn/issues/7323
        public Value(ValueType type, int value) : this()
        {
            this.i32 = value;
            this.type = type;
        }

        public Value(int value) : this()
        {
            this.i32 = value;
            this.type = ValueType.INT;
        }

        public Value(float value) : this()
        {
            this.fl = value;
            this.type = ValueType.NUMBER;
        }

        public Value(bool value) : this()
        {
            this.b = value;
            this.type = ValueType.BOOL;
        }


        public readonly override String ToString()
        {
            return type switch
            {
                ValueType.INT => $"n: {i32}",
                ValueType.BOOL => $"b: {b}",
                ValueType.NUMBER => $"f: {fl}",
                _ => $"reference {type.ToString()} {i32}",
            };
        }
    };
}
