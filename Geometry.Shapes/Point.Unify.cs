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
    using System.Diagnostics;

    public static class PointUnaryRelation
    {
        public static bool Unify(this PointSymbol shapeSymbol, object constraint, out object output)
        {
            output = shapeSymbol.Unify(constraint);
            return output != null;
        }

        public static object Unify(this PointSymbol ps, object constraint)
        {
            var refObj = constraint as string;
            Debug.Assert(refObj != null);
            switch (refObj)
            {
                case PointAcronym.X:
                case PointAcronym.X1:
                    return ps.InferXCoord(refObj);
                case PointAcronym.Y:
                case PointAcronym.Y1:
                    return ps.InferYCoord(refObj);
            }
            return null;
        }

        private static EqGoal InferXCoord(this PointSymbol inputPointSymbol, string label)
        {
            var point = inputPointSymbol.Shape as Point;
            Debug.Assert(point != null);
            var goal = new EqGoal(new Var(label), point.XCoordinate);
            return goal;
        }

        private static EqGoal InferYCoord(this PointSymbol inputPointSymbol, string label)
        {
            var point = inputPointSymbol.Shape as Point;
            Debug.Assert(point != null);
            var goal = new EqGoal(new Var(label), point.YCoordinate);
            return goal;
        }

    }
}
