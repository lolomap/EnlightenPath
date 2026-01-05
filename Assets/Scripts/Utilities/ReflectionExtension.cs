using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace Utilities
{
    public static class ReflectionExtension
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        {
            IEnumerable<Type> parmTypes = methodInfo.GetParameters().Select(parm => parm.ParameterType);
            Type[] parmAndReturnTypes = parmTypes.Append(methodInfo.ReturnType).ToArray();
            Type delegateType = Expression.GetDelegateType(parmAndReturnTypes);

            return methodInfo.IsStatic ? methodInfo.CreateDelegate(delegateType) : methodInfo.CreateDelegate(delegateType, target);
        }
    }
}
