﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextConfiguration : IContextMetadata
    {
        Environment_e Environment { get; }
    }
}
