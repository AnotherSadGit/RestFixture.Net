using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using fit;
using RestClient.Data;

// Modified or written by Object Mentor, Inc. for inclusion with FitNesse.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.
namespace RestFixture.Net.Support
{
    // Copyright (c) 2002 Cunningham & Cunningham, Inc.
    // Released under the terms of the GNU General Public License version 2 or later.

    public class TypeAdapter
    {
        public object target;
        public Fixture fixture;
        public FieldInfo field;
        public MethodInfo method;
        public Type type;
        public bool isRegex;
        private static readonly IDictionary<Type, TypeAdapter> PARSE_DELEGATES = new Dictionary<Type, TypeAdapter>();
        // Factory //////////////////////////////////

        public static TypeAdapter on(Fixture target, Type type)
        {
            TypeAdapter a = adapterFor(type);
            a.init(target, type);
            return a;
        }

        public static TypeAdapter on(Fixture fixture, FieldInfo field)
        {
            TypeAdapter a = on(fixture, field.FieldType);
            a.target = fixture;
            a.field = field;
            return a;
        }

        public static TypeAdapter on(Fixture fixture, MethodInfo method)
        {
            return on(fixture, method, false);
        }

        public static TypeAdapter on(Fixture fixture, MethodInfo method, bool isRegex)
        {
            TypeAdapter a = on(fixture, method.ReturnType);
            a.target = fixture;
            a.method = method;
            a.isRegex = isRegex;
            return a;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static TypeAdapter adapterFor(Class type) throws UnsupportedOperationException
        public static TypeAdapter adapterFor(Type type)
        {
            if (type.IsPrimitive)
            {
                if (type.Equals(typeof(sbyte)))
                {
                    return new ByteAdapter();
                }
                if (type.Equals(typeof(short)))
                {
                    return new ShortAdapter();
                }
                if (type.Equals(typeof(int)))
                {
                    return new IntAdapter();
                }
                if (type.Equals(typeof(long)))
                {
                    return new LongAdapter();
                }
                if (type.Equals(typeof(float)))
                {
                    return new FloatAdapter();
                }
                if (type.Equals(typeof(double)))
                {
                    return new DoubleAdapter();
                }
                if (type.Equals(typeof(char)))
                {
                    return new CharAdapter();
                }
                if (type.Equals(typeof(bool)))
                {
                    return new BooleanAdapter();
                }
                throw new System.NotSupportedException("can't yet adapt " + type);
            }
            else
            {
                object @delegate = PARSE_DELEGATES[type];
                if (@delegate is DelegateClassAdapter)
                {
                    return (TypeAdapter)((DelegateClassAdapter)@delegate).clone();
                }
                if (@delegate is DelegateObjectAdapter)
                {
                    return (TypeAdapter)((DelegateObjectAdapter)@delegate).clone();
                }
                if (type.Equals(typeof(Byte)))
                {
                    return new ClassByteAdapter();
                }
                if (type.Equals(typeof(short)))
                {
                    return new ClassShortAdapter();
                }
                if (type.Equals(typeof(int)))
                {
                    return new ClassIntegerAdapter();
                }
                if (type.Equals(typeof(long)))
                {
                    return new ClassLongAdapter();
                }
                if (type.Equals(typeof(float)))
                {
                    return new ClassFloatAdapter();
                }
                if (type.Equals(typeof(Double)))
                {
                    return new ClassDoubleAdapter();
                }
                if (type.Equals(typeof(char)))
                {
                    return new ClassCharacterAdapter();
                }
                if (type.Equals(typeof(Boolean)))
                {
                    return new ClassBooleanAdapter();
                }
                if (type.IsArray)
                {
                    return new ArrayAdapter();
                }
                return new TypeAdapter();
            }
        }

        // Accessors ////////////////////////////////

        public virtual void init(Fixture fixture, Type type)
        {
            this.fixture = fixture;
            this.type = type;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object get() throws IllegalAccessException, InvocationTargetException
        //public virtual object get()
        //{
        //    if (field != null)
        //    {
        //        return field.get(target);
        //    }
        //    if (method != null)
        //    {
        //        return invoke();
        //    }
        //    return null;
        //}

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void set(Object value) throws Exception
        //public virtual void set(object value)
        //{
        //    field.set(target, value);
        //}

        public virtual object Target
        {
            get
            {
                if (field != null)
                {
                    return field.GetValue(target);
                }
                if (method != null)
                {
                    return invoke();
                }
            }
            set { field.SetValue(target, value); }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object invoke() throws IllegalAccessException, InvocationTargetException
        public virtual object invoke()
        {
            object[] @params = new object[] { };
            return method.Invoke(target, @params);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object parse(String s) throws Exception
        public virtual object parse(string s)
        {
            object obj;
            obj = isRegex ? s : fixture.parse(s, type);
            return obj;
        }

        public virtual bool Equals(object a, object b)
        {
            bool isEqual = false;

            if (isRegex)
            {
                if (b != null)
                {
                    isEqual = Pattern.matches(a.ToString(), b.ToString());
                }
            }
            else
            {
                if (a == null)
                {
                    isEqual = (b == null);
                }
                else
                {
                    isEqual = a.Equals(b);
                }
            }
            return isEqual;
        }

        public virtual string ToString(object o)
        {
            if (o == null)
            {
                return "null";
            }
            else if (o is string && ((string)o).Equals(""))
            {
                return "blank";
            }
            else
            {
                return o.ToString();
            }
        }

        /*
         * Registers a delegate, a class that will handle parsing of other types of values.
         */
        public static void registerParseDelegate(Type type, Type parseDelegate)
        {
            try
            {
                PARSE_DELEGATES[type] = new DelegateClassAdapter(parseDelegate);
            }
            catch (Exception)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw new Exception("Parse delegate class " + parseDelegate.FullName + " does not have a suitable static parse() method.");
            }
        }

        /*
         * Registers a delegate object that will handle parsing of other types of values.
         */
        public static void registerParseDelegate(Type type, object parseDelegate)
        {
            try
            {
                PARSE_DELEGATES[type] = new DelegateObjectAdapter(parseDelegate);
            }
            catch (Exception)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw new Exception("Parse delegate object of class " + parseDelegate.GetType().FullName + " does not have a suitable parse() method.");
            }
        }

        public static void clearDelegatesForNextTest()
        {
            PARSE_DELEGATES.Clear();
        }

        // Subclasses ///////////////////////////////

        internal class ByteAdapter : ClassByteAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setByte(target, ((sbyte?)i).Value);
            }
        }

        internal class ClassByteAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new sbyte?(sbyte.Parse(s));
            }
        }

        internal class ShortAdapter : ClassShortAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setShort(target, ((short?)i).Value);
            }
        }

        internal class ClassShortAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new short?(short.Parse(s));
            }
        }

        internal class IntAdapter : ClassIntegerAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setInt(target, ((int?)i).Value);
            }
        }

        internal class ClassIntegerAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new int?(int.Parse(s));
            }
        }

        internal class LongAdapter : ClassLongAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void set(Nullable<long> i) throws IllegalAccessException
            public virtual void set(long? i)
            {
                field.setLong(target, i.Value);
            }
        }

        internal class ClassLongAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new long?(long.Parse(s));
            }
        }

        internal class FloatAdapter : ClassFloatAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setFloat(target, ((Number)i).floatValue());
            }

            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new float?(float.Parse(s));
            }
        }

        internal class ClassFloatAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new float?(float.Parse(s));
            }
        }

        internal class DoubleAdapter : ClassDoubleAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setDouble(target, ((Number)i).doubleValue());
            }

            public override object parse(string s)
            {
                return new double?(double.Parse(s));
            }
        }

        internal class ClassDoubleAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new double?(double.Parse(s));
            }
        }

        internal class CharAdapter : ClassCharacterAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setChar(target, ((char?)i).Value);
            }
        }

        internal class ClassCharacterAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                return ("null".Equals(s)) ? null : new char?(s[0]);
            }
        }

        internal class BooleanAdapter : ClassBooleanAdapter
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void set(Object i) throws IllegalAccessException
            public override void set(object i)
            {
                field.setBoolean(target, ((bool?)i).Value);
            }
        }

        internal class ClassBooleanAdapter : TypeAdapter
        {
            public override object parse(string s)
            {
                if ("null".Equals(s))
                {
                    return null;
                }
                string ls = s.ToLower();
                if (ls.Equals("true"))
                {
                    return true;
                }
                if (ls.Equals("yes"))
                {
                    return true;
                }
                if (ls.Equals("1"))
                {
                    return true;
                }
                if (ls.Equals("y"))
                {
                    return true;
                }
                if (ls.Equals("+"))
                {
                    return true;
                }
                return false;
            }
        }

        internal class ArrayAdapter : TypeAdapter
        {
            internal Type componentType;
            internal TypeAdapter componentAdapter;

            public override void init(Fixture target, Type type)
            {
                base.init(target, type);
                componentType = type.GetElementType();
                componentAdapter = on(target, componentType);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public Object parse(String s) throws Exception
            public override object parse(string s)
            {
                StringTokenizer t = new StringTokenizer(s, ",");
                object array = Array.CreateInstance(componentType, t.countTokens());
                for (int i = 0; t.hasMoreTokens(); i++)
                {
                    ((System.Array)array).SetValue(componentAdapter.parse(t.nextToken().Trim()), i);
                }
                return array;
            }

            public override string ToString(object o)
            {
                if (o == null)
                {
                    return "";
                }
                int length = Array.getLength(o);
                StringBuilder b = new StringBuilder(5 * length);
                for (int i = 0; i < length; i++)
                {
                    b.Append(componentAdapter.ToString(Array.get(o, i)));
                    if (i < (length - 1))
                    {
                        b.Append(", ");
                    }
                }
                return b.ToString();
            }

            public override bool Equals(object a, object b)
            {
                int length = Array.getLength(a);
                if (length != Array.getLength(b))
                {
                    return false;
                }
                for (int i = 0; i < length; i++)
                {
                    if (!componentAdapter.Equals(Array.get(a, i), Array.get(b, i)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal class DelegateClassAdapter : TypeAdapter, ICloneable
        {
            internal MethodInfo parseMethod;

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public DelegateClassAdapter(Class parseDelegate) throws SecurityException, NoSuchMethodException
            public DelegateClassAdapter(Type parseDelegate)
            {
                this.parseMethod = parseDelegate.GetMethod("parse", new Type[] { typeof(string) });
                int modifiers = parseMethod.Modifiers;
                if (!Modifier.isStatic(modifiers) || !Modifier.isPublic(modifiers) || parseMethod.ReturnType == typeof(Void))
                {
                    throw new NoSuchMethodException();
                }
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public Object parse(String s) throws Exception
            public override object parse(string s)
            {
                return parseMethod.invoke(null, new object[] { s });
            }

            protected internal override object clone()
            {
                try
                {
                    return base.clone();
                }
                catch (CloneNotSupportedException)
                {
                    return null;
                }
            }
        }

        internal class DelegateObjectAdapter : TypeAdapter, ICloneable
        {
            internal object @delegate;
            internal MethodInfo parseMethod;

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public DelegateObjectAdapter(Object delegate) throws SecurityException, NoSuchMethodException
            public DelegateObjectAdapter(object @delegate)
            {
                this.@delegate = @delegate;
                this.parseMethod = @delegate.GetType().GetMethod("parse", new Type[] { typeof(string) });
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public Object parse(String s) throws Exception
            public override object parse(string s)
            {
                return parseMethod.invoke(@delegate, new object[] { s });
            }

            protected internal override object clone()
            {
                try
                {
                    return base.clone();
                }
                catch (CloneNotSupportedException)
                {
                    return null;
                }
            }
        }
    }
}