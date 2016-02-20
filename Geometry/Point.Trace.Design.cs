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

using System.Linq;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    public static partial class TraceInstructionalDesign
    {
        public static void FromPointsToMidPoint(PointSymbol ps1, PointSymbol ps2, PointSymbol midPoint)
        {
            //1. plot the ps1, ps2 and the line
            //2. generate x coordinate
            //3. generate y coordinate
            //4. generate point (x,y)

            string strategy;

            var ts00 = new TraceStep(null, ps1, null, PlottingRule.PlottingStrategy, PlottingRule.Plot(ps1));
            var ts01 = new TraceStep(null, ps2, null, PlottingRule.PlottingStrategy, PlottingRule.Plot(ps2));
            var ls = LineBinaryRelation.Unify(ps1, ps2) as LineSymbol;
            var ts02 = new TraceStep(null, ls, null, PlottingRule.PlottingStrategy, PlottingRule.Plot(ls));

            strategy = "Plot the given two points and the connected line.";
            midPoint._innerLoop.Add(ts00);
            midPoint._innerLoop.Add(ts01);
            midPoint._innerLoop.Add(ts02);
            midPoint.GenerateATrace(strategy);
            ////////////////////////////////////////////////

            var xCoord = new Var("x");
            var pt1 = ps1.Shape as Point;
            Debug.Assert(pt1 != null);
            var pt2 = ps2.Shape as Point;
            Debug.Assert(pt2 != null);

            var term1 = new Term(Expression.Add, new List<object>(){pt1.XCoordinate, pt2.XCoordinate});
            var term2 = new Term(Expression.Divide, new List<object>(){term1, 2});
            var eq = new Equation(xCoord, term2);

            /////////////////////////////////////////////////

            object obj;
            bool result = eq.IsEqGoal(out obj);
            EqGoal goal1 = null;
            if (result)
            {
                goal1 = obj as EqGoal;
                Debug.Assert(goal1!=null);
                Debug.Assert(goal1.Traces.Count == 1);
                var currentTrace = goal1.Traces.ToList()[0];
                strategy = "Generate x coordinate of midpoint.";
                var newTrace = new Tuple<object, object>(strategy, currentTrace.Item2);
                midPoint.Traces.Add(newTrace);           
            }

            //////////////////////////////////////////////////

            var yCoord = new Var("y");
            term1 = new Term(Expression.Add, new List<object>() { pt1.YCoordinate, pt2.YCoordinate });
            term2 = new Term(Expression.Divide, new List<object>() { term1, 2 });
            var eq1 = new Equation(yCoord, term2);

            result = eq1.IsEqGoal(out obj);
            EqGoal goal2 = null;
            if (result)
            {
                goal2 = obj as EqGoal;
                Debug.Assert(goal2 != null);
                Debug.Assert(goal2.Traces.Count == 1);
                var currentTrace1 = goal2.Traces.ToList()[0];
                strategy = "Generate y coordinate of midpoint.";
                var newTrace = new Tuple<object, object>(strategy, currentTrace1.Item2);
                midPoint.Traces.Add(newTrace);
            }

            //////////////////////////////////////////////////////

            var ps = new Point(xCoord, yCoord);
            var psym = new PointSymbol(ps);

            var mid = midPoint.Shape as Point;
            Debug.Assert(mid != null);
            var ps_inter1 = new Point(mid.XCoordinate, yCoord);
            var psym_inter1 = new PointSymbol(ps_inter1);
            var metaRule = "Consider substitute generate x coordinate into the point form.";
            var appliedRule = SubstitutionRule.ApplySubstitute(goal1, psym);

            var ts1 = new TraceStep(psym, psym_inter1, null, metaRule, appliedRule);
            metaRule = "Consider substitute generate y coordinate into the point form.";
            appliedRule = SubstitutionRule.ApplySubstitute(goal2, psym_inter1);
            var ts2 = new TraceStep(psym_inter1, midPoint, null, metaRule, appliedRule);

            strategy = "Generate point (x,y) by subsitute generated x and y.";
            midPoint._innerLoop.Add(ts1);
            midPoint._innerLoop.Add(ts2);            
            midPoint.GenerateATrace(strategy);
        }
    }
}
