// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Reflection;

namespace Adept
{
    static internal class ReflectionExtensions
    {
        static public MemberInfo GetMemeber(this object obj, string memberName)
        {
            if ((obj == null) || (string.IsNullOrEmpty(memberName)))
            {
                return null;
            }
            else
            {
                MemberInfo member = obj.GetType().GetProperty(memberName);
                if (member == null)
                {
                    member = obj.GetType().GetField(memberName);
                }
                return member;
            }
        }

        static public object GetValue(this MemberInfo member, object obj)
        {
            if ((member == null) || (obj == null)) { return null; }

            var prop = member as PropertyInfo;
            var field = member as FieldInfo;

            if (prop != null)
            {
                return prop.GetValue(obj, null);
            }
            else if (field != null)
            {
                return field.GetValue(obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown MemberInfo type in GetValue");
            }
        }

        static public Type GetValueType(this MemberInfo member)
        {
            if (member == null) { return typeof(object); }

            var prop = member as PropertyInfo;
            var field = member as FieldInfo;

            if (prop != null)
            {
                return prop.PropertyType;
            }
            else if (field != null)
            {
                return field.FieldType;
            }
            else
            {
                throw new InvalidOperationException("Unknown MemberInfo type in GetValueType");
            }
        }

        static public void SetValue(this MemberInfo member, object obj, object value)
        {
            if ((member == null) || (obj == null)) { return; }

            var prop = member as PropertyInfo;
            var field = member as FieldInfo;

            if (prop != null)
            {
                prop.SetValue(obj, value, null);
            }
            else if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new InvalidOperationException("Unknown MemberInfo type in SetValue");
            }
        }
    }
}
