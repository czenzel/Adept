// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// Supplies a value to a parameter of an action.
    /// </summary>
    public class ParameterValue
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
