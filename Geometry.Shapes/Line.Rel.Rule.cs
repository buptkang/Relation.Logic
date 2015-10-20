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

    public static class LineGenerationRule
    {
        public static LineSymbol GenerateLine(Point pt1, Point pt2)
        {
            if (pt1.Equals(pt2)) return null;

            Debug.Assert(pt1.Concrete);
            Debug.Assert(pt2.Concrete);

           /* if (pt1.XCoordinate.Equals(pt2.XCoordinate))
            {
                double d;
                LogicSharp.IsDouble(pt1.XCoordinate, out d);
                var line1 = new Line(1, null, -1 * d);
                return new LineSymbol(line1);
            }

            if (pt1.YCoordinate.Equals(pt2.YCoordinate))
            {
                double d;
                LogicSharp.IsDouble(pt1.YCoordinate, out d);
                var line2 = new Line(null, 1, -1 * d);
                return new LineSymbol(line2);
            }*/

            //Strategy: y = mx+b, find slope m and intercept b
            //step 1: calc slope

            double p1x, p1y, p2x, p2y;
            LogicSharp.IsDouble(pt1.XCoordinate, out p1x);
            LogicSharp.IsDouble(pt1.YCoordinate, out p1y);
            LogicSharp.IsDouble(pt2.XCoordinate, out p2x);
            LogicSharp.IsDouble(pt2.YCoordinate, out p2y);


            var a = p1y - p2y;
            var b = p2x - p1x;
            var c = (p1x - p2x)*p1y + (p2y - p1y)*p1x;

            int intB;
            int intC;
            bool cond1 = LogicSharp.IsInt(b/a, out intB);
            bool cond2 = LogicSharp.IsInt(c/a, out intC);

            if (cond1 && cond2)
            {
                a = 1;
                b = intB;
                c = intC;
            }

            if (a < 0.0d)
            {
                a = -1*a;
                b = -1*b;
                c = -1*c;
            }

            var line = new Line(a,b,c) {InputType = LineType.Relation}; 
            return new LineSymbol(line);
        }

        /// <summary>
        /// Goal must be slope or intercept
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static LineSymbol GenerateLine(Point pt, double? slope, double? intercept)
        {
            Debug.Assert(pt.Concrete);
            double X, Y;
            LogicSharp.IsDouble(pt.XCoordinate, out X);
            LogicSharp.IsDouble(pt.YCoordinate, out Y);

            if (slope != null)
            {
                double intercept1 = Y - slope.Value * X;
                var line = new Line(slope, intercept1);
                line.InputType = LineType.Relation;
                return new LineSymbol(line);  
            }
            if (intercept != null)
            {
                double slope1 = 0.0;
                slope1 = (Y - intercept.Value) / X;
                var line = new Line(slope1, intercept);
                line.InputType = LineType.Relation;
                return new LineSymbol(line);  
            }
            throw new Exception("Cannot reach here!");
        }

        /// <summary>
        /// Goal must have both the slope and the intercept
        /// </summary>
        /// <param name="goal1"></param>
        /// <param name="goal2"></param>
        /// <returns></returns>
        public static Line GenerateLine(double slope, double intercept)
        {
            var line =  new Line(slope, intercept);
            line.InputType = LineType.Relation;
            return line;
        }

        public static string IdentityPoints = "Cannot build the line as two identify points!"; 
    }
}
