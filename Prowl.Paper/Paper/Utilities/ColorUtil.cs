// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Prowl.Vector;

namespace Prowl.Paper.Utilities;

public static class ColorUtil
{
    public static Color FromArgb(float a, float r, float g, float b) => new Color(r, g, b, a);
}
