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
    using System;
    using System.Diagnostics;

    public static class PointGenerationRule
    {
        public static PointSymbol GenerateMidPoint(Point pt1, Point pt2)
        {
            if (pt1.Equals(pt2)) return null;
            Debug.Assert(pt1.Concrete);
            Debug.Assert(pt2.Concrete);
            double p1x, p1y, p2x, p2y;
            LogicSharp.IsDouble(pt1.XCoordinate, out p1x);
            LogicSharp.IsDouble(pt1.YCoordinate, out p1y);
            LogicSharp.IsDouble(pt2.XCoordinate, out p2x);
            LogicSharp.IsDouble(pt2.YCoordinate, out p2y);
            var midX = (p1x + p2x)/2;
            var midY = (p1y + p2y)/2;
            var midPoint = new Point(midX, midY);
            return new PointSymbol(midPoint);
        }
    }
}
