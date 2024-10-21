#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.Reflection;

#pragma warning disable 1591

namespace MigraDoc.DocumentObjectModel.Internals
{
    /// <summary>
    /// Base class of all value descriptor classes.
    /// </summary>
    public abstract class ValueDescriptor
    {
        internal ValueDescriptor(string valueName, Type valueType, Type memberType, MemberInfo memberInfo, VDFlags flags)
        {
            // Take new naming convention into account.
            if (valueName.StartsWith("_"))
                valueName = valueName.Substring(1);

            ValueName = valueName;
            ValueType = valueType;
            MemberType = memberType;
            MemberInfo = memberInfo;
            _flags = flags;
        }

        public object CreateValue()
        {
#if !NETFX_CORE
            ConstructorInfo constructorInfo = ValueType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
#else
            var constructorInfos = ValueType.GetTypeInfo().DeclaredConstructors;
            ConstructorInfo constructorInfo = null;
            foreach (var info in constructorInfos)
            {
                if (info.GetParameters().Length == 0)
                    constructorInfo = info;
            }
#endif
            Debug.Assert(constructorInfo != null);
            return constructorInfo.Invoke(null);
        }

        public abstract object GetValue(DocumentObject dom, GV flags);
        public abstract void SetValue(DocumentObject dom, object val);
        public abstract void SetNull(DocumentObject dom);
        public abstract bool IsNull(DocumentObject dom);

        internal static ValueDescriptor CreateValueDescriptor(MemberInfo memberInfo, DVAttribute attr)
        {
            VDFlags flags = VDFlags.None;
            if (attr.RefOnly)
                flags |= VDFlags.RefOnly;

            string name = memberInfo.Name;

            FieldInfo fieldInfo = memberInfo as FieldInfo;
            Type type = fieldInfo != null ? fieldInfo.FieldType : ((PropertyInfo)memberInfo).PropertyType;

            if (type == typeof(NBool))
                return new NullableDescriptor(name, typeof(Boolean), type, memberInfo, flags);

            if (type == typeof(NInt))
                return new NullableDescriptor(name, typeof(Int32), type, memberInfo, flags);

            if (type == typeof(NDouble))
                return new NullableDescriptor(name, typeof(Double), type, memberInfo, flags);

            if (type == typeof(NString))
                return new NullableDescriptor(name, typeof(String), type, memberInfo, flags);

            if (type == typeof(String))
                return new ValueTypeDescriptor(name, typeof(String), type, memberInfo, flags);

            if (type == typeof(NEnum))
            {
                Type valueType = attr.Type;
#if !NETFX_CORE
                Debug.Assert(valueType.IsSubclassOf(typeof(Enum)), "NEnum must have 'Type' attribute with the underlying type");
#else
                Debug.Assert(valueType.GetTypeInfo().IsSubclassOf(typeof(Enum)), "NEnum must have 'Type' attribute with the underlying type");
#endif
                return new NullableDescriptor(name, valueType, type, memberInfo, flags);
            }

#if !NETFX_CORE
            if (type.IsSubclassOf(typeof(ValueType)))
#else
            if (type.GetTypeInfo().IsSubclassOf(typeof(ValueType)))
#endif
                return new ValueTypeDescriptor(name, type, type, memberInfo, flags);

#if !NETFX_CORE
            if (typeof(DocumentObjectCollection).IsAssignableFrom(type))
#else
            if (typeof(DocumentObjectCollection).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
#endif
                return new DocumentObjectCollectionDescriptor(name, type, type, memberInfo, flags);

#if !NETFX_CORE
            if (typeof(DocumentObject).IsAssignableFrom(type))
#else
            if (typeof(DocumentObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
#endif
                return new DocumentObjectDescriptor(name, type, type, memberInfo, flags);

            Debug.Assert(false, type.FullName);
            return null;
        }

        public bool IsRefOnly
        {
            get { return (_flags & VDFlags.RefOnly) == VDFlags.RefOnly; }
        }

        public FieldInfo FieldInfo
        {
            get { return MemberInfo as FieldInfo; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return MemberInfo as PropertyInfo; }
        }

        /// <summary>
        /// Name of the value.
        /// </summary>
        public readonly string ValueName;

        /// <summary>
        /// Type of the described value, e.g. typeof(Int32) for an NInt.
        /// </summary>
        public readonly Type ValueType;

        /// <summary>
        /// Type of the described field or property, e.g. typeof(NInt) for an NInt.
        /// </summary>
        public readonly Type MemberType;

        /// <summary>
        /// FieldInfo of the described field.
        /// </summary>
        public readonly MemberInfo MemberInfo;

        /// <summary>
        /// Flags of the described field, e.g. RefOnly.
        /// </summary>
        readonly VDFlags _flags;
    }

    /// <summary>
    /// Value descriptor of all nullable types.
    /// </summary>
    internal class NullableDescriptor : ValueDescriptor
    {
        internal NullableDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
            : base(valueName, valueType, fieldType, memberInfo, flags)
        { }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(flags), "flags");

#if !NETFX_CORE
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetValue(dom);
#endif
            INullableValue ival = (INullableValue)val;
            if (ival.IsNull && flags == GV.GetNull)
                return null;
            return ival.GetValue();
        }

        public override void SetValue(DocumentObject dom, object value)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetValue(value);
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
#if !NETFX_CORE
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
                val = PropertyInfo.GetValue(dom);
#endif
                ival = (INullableValue)val;
                ival.SetValue(value);
#if !NETFX_CORE
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
#else
                PropertyInfo.SetValue(dom, ival);
#endif
            }
        }

        public override void SetNull(DocumentObject dom)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetNull();
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
#if !NETFX_CORE
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
                val = PropertyInfo.GetValue(dom);
#endif
                ival = (INullableValue)val;
                ival.SetNull();
#if !NETFX_CORE
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
#else
                PropertyInfo.SetValue(dom, ival);
#endif
            }
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
#if !NETFX_CORE
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetValue(dom);
#endif
            return ((INullableValue)val).IsNull;
        }
    }

    /// <summary>
    /// Value descriptor of value types.
    /// </summary>
    internal class ValueTypeDescriptor : ValueDescriptor
    {
        internal ValueTypeDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
            : base(valueName, valueType, fieldType, memberInfo, flags)
        { }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(flags), "flags");

#if !NETFX_CORE
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetValue(dom);
#endif
            INullableValue ival = val as INullableValue;
            if (ival != null && ival.IsNull && flags == GV.GetNull)
                return null;
            return val;
        }

        public override void SetValue(DocumentObject dom, object value)
        {
            if (FieldInfo != null)
                FieldInfo.SetValue(dom, value);
            else
#if !NETFX_CORE
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { value });
#else
                PropertyInfo.SetValue(dom, value);
#endif
        }

        public override void SetNull(DocumentObject dom)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetNull();
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
#if !NETFX_CORE
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
                val = PropertyInfo.GetValue(dom);
#endif
                ival = (INullableValue)val;
                ival.SetNull();
#if !NETFX_CORE
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
#else
                PropertyInfo.SetValue(dom, ival);
#endif
            }
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
#if !NETFX_CORE
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetGetMethod(true).Invoke(dom, null);
#else
            object val = FieldInfo != null ? FieldInfo.GetValue(dom) : PropertyInfo.GetValue(dom);
#endif
            INullableValue ival = val as INullableValue;
            if (ival != null)
                return ival.IsNull;
            return false;
        }
    }

    /// <summary>
    /// Value descriptor of DocumentObject.
    /// </summary>
    internal class DocumentObjectDescriptor : ValueDescriptor
    {
        internal DocumentObjectDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
            : base(valueName, valueType, fieldType, memberInfo, flags)
        { }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(flags), "flags");

            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            if (fieldInfo != null)
            {
                // Member is a field
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val == null && flags == GV.ReadWrite)
                {
                    val = (DocumentObject)CreateValue();
                    val._parent = dom;
                    FieldInfo.SetValue(dom, val);
                    return val;
                }
            }
            else
            {
                // Member is a property
#if !NETFX_CORE
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, null) as DocumentObject;
#else
                val = PropertyInfo.GetValue(dom) as DocumentObject;
#endif
            }
            if (val != null && (val.IsNull() && flags == GV.GetNull))
                return null;

            return val;
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            FieldInfo fieldInfo = FieldInfo;
            // Member is a field
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(dom, val);
                return;
            }
            throw new InvalidOperationException("This value cannot be set.");
        }

        public override void SetNull(DocumentObject dom)
        {
            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            // Member is a field.
            if (fieldInfo != null)
            {
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val != null)
                    val.SetNull();
            }

            // Member is a property.
            if (PropertyInfo != null)
            {
                PropertyInfo propInfo = PropertyInfo;
#if !NETFX_CORE
                val = propInfo.GetGetMethod(true).Invoke(dom, null) as DocumentObject;
#else
                val = propInfo.GetValue(dom) as DocumentObject;
#endif
                if (val != null)
                    val.SetNull();
            }
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            // Member is a field
            if (fieldInfo != null)
            {
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val == null)
                    return true;
                return val.IsNull();
            }
            // Member is a property
            PropertyInfo propInfo = PropertyInfo;
#if !NETFX_CORE
            val = propInfo.GetGetMethod(true).Invoke(dom, null) as DocumentObject;
#else
            val = propInfo.GetValue(dom) as DocumentObject;
#endif
            if (val != null)
                val.IsNull();
            return true;
        }
    }

    /// <summary>
    /// Value descriptor of DocumentObjectCollection.
    /// </summary>
    internal class DocumentObjectCollectionDescriptor : ValueDescriptor
    {
        internal DocumentObjectCollectionDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
            : base(valueName, valueType, fieldType, memberInfo, flags)
        { }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(flags), "flags");

            Debug.Assert(MemberInfo is FieldInfo, "Properties of DocumentObjectCollection not allowed.");
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val == null && flags == GV.ReadWrite)
            {
                val = (DocumentObjectCollection)CreateValue();
                val._parent = dom;
                FieldInfo.SetValue(dom, val);
                return val;
            }
            if (val != null && val.IsNull() && flags == GV.GetNull)
                return null;
            return val;
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            FieldInfo.SetValue(dom, val);
        }

        public override void SetNull(DocumentObject dom)
        {
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val != null)
                val.SetNull();
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val == null)
                return true;
            return val.IsNull();
        }
    }
}
