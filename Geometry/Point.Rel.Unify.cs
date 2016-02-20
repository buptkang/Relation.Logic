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
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class PointBinaryRelation
    {
        public static object Unify(PointSymbol pt1, PointSymbol pt2, EqGoal goal = null)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;

            //Mid-point build process
            if (pt1.Shape.Concrete && pt2.Shape.Concrete)
            {
                var point1 = pt1.Shape as Point;
                var point2 = pt2.Shape as Point;
                Debug.Assert(point1 != null);
                Debug.Assert(point2 != null);
                var midPoint = PointGenerationRule.GenerateMidPoint(point1, point2);
                TraceInstructionalDesign.FromPointsToMidPoint(pt1, pt2, midPoint);
                return midPoint;
            }
            return null;
        }
    }
}
