// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adept.Input
{
    public class ParameterDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOptional { get; set; }
        public Type ParameterType { get; set; }
    }
}
