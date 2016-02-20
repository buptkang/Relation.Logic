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

    public partial class CircleSymbol : ShapeSymbol
    {
        public override void UndoGoal(EqGoal goal, object parent)
        {
            throw new NotImplementedException();
        }

        public override bool UnifyProperty(string label, out object obj)
        {
            throw new NotImplementedException();
        }

        public override bool UnifyExplicitProperty(EqGoal goal)
        {
            throw new NotImplementedException();
        }

        public override bool UnifyProperty(EqGoal goal, out object obj)
        {
            throw new NotImplementedException();
        }

        public override bool UnifyShape(ShapeSymbol ss)
        {
            throw new NotImplementedException();
        }

        public override bool ApproximateMatch(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Circle : QuadraticCurve
    {

    }

    public static class CircleEquationExtension
    {
        public static bool IsCircleEquation(this QuadraticCurveSymbol qcs, out CircleSymbol cs)
        {
            cs = null;
            return false;
        }
    }

        //case "R" : case "r":
        //            return circle.Radius;
        //        case "C":  case "c":
        //            return circle.CentralPt;
        //        case "P": case "p":
        //            return circle.Perimeter;
        //        case "S": case "s":
        //            return circle.Area;                 


    /*                    case "R":
                case "r":
                    propertyExpr = circleExpr.CircleRadiusExpr;
                    tracer = circleExpr.CircleRadiusTrace;
                    return true;
                case "CP":
                case "Cp":
                case "cp":
                    propertyExpr = circleExpr.CircleCenterPtExpr;
                    tracer = circleExpr.CircleCentralPtTrace;
                    return true;

                case "PR":
                case "Pr":
                case "pr":
                    propertyExpr = circleExpr.CircleStandardFormExpr;
                    tracer = circleExpr.CircleStandardFormTrace;
                    return true;
*/
}
