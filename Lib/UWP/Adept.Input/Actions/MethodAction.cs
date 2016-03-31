// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adept.Input
{
    public class MethodAction : IInputAction
    {
        private string description;
        private MethodInfo method;
        private string name;
        private List<ParameterDefinition> parameters = new List<ParameterDefinition>();
        private Dictionary<string, int> paramPositionTable = new Dictionary<string, int>();
        private object target;
        private ParameterValueSet values = new ParameterValueSet();

        //public MethodAction()
        //{
        //    Action a = Foo;
        //    MethodCallExpression e;
        //    // (()=>Foo);
        //    var mi = SymbolExtensions.GetMethodInfo(() => Foo());


        //}

        //public void Foo() { }

        public MethodAction(MethodInfo method, object target)
        {
            // Defaults
            IsEnabled = true;

            // Validate
            if (method == null) throw new ArgumentNullException("method");
            if (target == null) throw new ArgumentNullException("target");

            // Store
            this.method = method;
            this.target = target;

            // Determine Name
            // TODO: Use attributes?
            name = method.Name;

            // TODO: Determine Description

            // Determine parameters
            foreach (var par in method.GetParameters())
            {
                // Create parameter
                parameters.Add(new ParameterDefinition()
                {
                    // Description, // TODO: From attributes
                    IsOptional = par.IsOptional,
                    Name = par.Name,  // TODO: From attributes
                    ParameterType = par.ParameterType
                });

                // Set position
                paramPositionTable[par.Name] = par.Position;

            }
        }

        /// <inheritdoc/>
        public object Invoke()
        {
            // Placeholder
            object[] vals = null;

            // Have values to pass in?
            if (values != null)
            {
                // Convert parameters to value array
                vals = (from v in values
                        orderby paramPositionTable[v.Name]
                        select v.Value).ToArray();
            }

            // Invoke
            return method.Invoke(target, vals);
        }

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the method that will be invoked by the action.
        /// </summary>
        public MethodInfo Method => method;

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ParameterDefinition> Parameters => parameters;

        /// <inheritdoc/>
        public Type ReturnType
        {
            get
            {
                return method.ReturnType;
            }
        }

        /// <summary>
        /// Gets the object that is the target of the method invocation.
        /// </summary>
        public object Target => target;

        /// <inheritdoc/>
        public ParameterValueSet Values
        {
            get
            {
                return values;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                values = value;
            }
        }
    }
}
