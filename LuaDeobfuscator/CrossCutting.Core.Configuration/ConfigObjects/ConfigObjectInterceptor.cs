using Castle.DynamicProxy;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.Configuration.DataClasses;
using System;
using System.Linq;
using System.Reflection;

namespace CrossCutting.Core.Configuration.ConfigObjects
{
    public class ConfigObjectInterceptor : IInterceptor
    {
        private readonly IConfigurator _configurator;

        public ConfigObjectInterceptor(IConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            _configurator = configurator;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method = invocation.Method;

            bool isGetter = method.Name.StartsWith("get_");

            bool isProperty = isGetter;
            if (!isProperty)
            {
                invocation.Proceed();
            }

            string propertyName = method.Name.Split('_')[1];

            Type originalType = invocation.TargetType;
            PropertyInfo propertyInfo = originalType.GetProperties().Single(p => p.Name == propertyName);
            var attribute = propertyInfo.GetCustomAttribute<ConfigMapAttribute>();

            bool hasAttribute = attribute != null;
            if (!hasAttribute)
                return;

            if (isGetter)
            {
                foreach (string key in attribute.Keys)
                {
                    if (!_configurator.Contains(attribute.Category, key))
                        continue;

                    var value = _configurator.Get<object>(attribute.Category, key);
                    invocation.ReturnValue = Convert.ChangeType(value, propertyInfo.PropertyType);

                    return;
                }

                invocation.ReturnValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
            }
        }
    }
}
