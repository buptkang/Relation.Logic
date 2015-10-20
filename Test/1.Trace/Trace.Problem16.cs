/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace AlgebraGeometry
{
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public partial class TestTrace
    {
        [Test]
        public void Problem16()
        {
            var pt1 = new Point(-1, 2);
            var ps1 = new PointSymbol(pt1);

            var pt2 = new Point(5, 8);
            var ps2 = new PointSymbol(pt2);

            var pt3 = new Point(2, 4);
            var midPoint = new PointSymbol(pt3);

            TraceInstructionalDesign.FromPointsToMidPoint(ps1, ps2, midPoint);
        }
    }
}
