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
    using System.Linq.Expressions;

    public static partial class TraceInstructionalDesign
    {
        /*
         * Annotated scaffolding
         *
         */ 
        public static void FromPointsToLineSegment(LineSegmentSymbol lss)
        {
            var ls = lss.Shape as LineSegment;
            Debug.Assert(ls != null);
            var pt1Symbol = new PointSymbol(ls.Pt1);
            var pt2Symbol = new PointSymbol(ls.Pt2);

            string strategy;

            TraceStep ts00 = new TraceStep(null, pt1Symbol, PlottingRule.PlottingStrategy, PlottingRule.Plot(pt1Symbol));
            TraceStep ts01 = new TraceStep(null, pt2Symbol, PlottingRule.PlottingStrategy, PlottingRule.Plot(pt2Symbol));

            TraceStep ts02 = null;
            if (ls.Pt1.Concrete && ls.Pt2.Concrete)
            {
                ts02 = new TraceStep(null, lss, PlottingRule.PlottingStrategy, PlottingRule.Plot(lss));
            }
            strategy = "Plot a Line Segment passing through two points.";
            lss._innerLoop.Add(ts00);
            lss._innerLoop.Add(ts01);
            if (ts02 != null)
            {
                lss._innerLoop.Add(ts02);
            }
            lss.GenerateATrace(strategy);

/*            string stepMetaRule = "Consider draw a line segment passing through the two points.";
            string stepAppliedRule = String.Format("Draw the line segment passing through two points {0} and {1}",
                pt1Symbol, pt2Symbol);

            var traceStep = new TraceStep(null, lss, stepMetaRule, stepAppliedRule);
            lss._innerLoop.Add(traceStep);*/
            
        }

        /*
         * Automatic scaffolding
         * 
         * Distance function (x-x0)^2+(y-y0)^2 = d^2
         * 
         * Forward chaining to derive d.
         * Backward chaining to derive other four parameters.
         */
        private static Tuple<object, object> DistanceSubstitution(LineSegmentSymbol lss)
        {
            var ls = lss.Shape as LineSegment;
            var lst = new List<TraceStep>();
            string step1metaRule = "The Distance Function between two points it: d^2=(x0-x1)^2+(y0-y1)^2";
            string step1AppliedRule = String.Format(
                "Substitute two points into the distance function d^2=({0}-{1})^2+({2}-{3})^2",
                ls.Pt1.XCoordinate.ToString(),
                ls.Pt2.XCoordinate.ToString(),
                ls.Pt1.YCoordinate.ToString(),
                ls.Pt2.YCoordinate.ToString());

            var pt1X = new Var("x0");
            var pt1Y = new Var("y0");
            var pt2X = new Var("x1");
            var pt2Y = new Var("y1");

            var term1_1 = new Term(Expression.Subtract, new List<object>() { pt1X, pt2X });
            var term11_1 = new Term(Expression.Power, new List<object>() { term1_1, 2.0 });
            var term2_1 = new Term(Expression.Subtract, new List<object>() { pt1Y, pt2Y });
            var term22_1 = new Term(Expression.Power, new List<object>() { term2_1, 2.0 });
            var rhs_1 = new Term(Expression.Add, new List<object>() { term11_1, term22_1 });

            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() { variable, 2.0 });

            var term1 = new Term(Expression.Subtract, new List<object>() { ls.Pt1.XCoordinate, ls.Pt2.XCoordinate });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2 = new Term(Expression.Subtract, new List<object>() { ls.Pt1.YCoordinate, ls.Pt2.YCoordinate });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(lhs, rhs);

            var old_eq = new Equation(lhs, rhs_1);
            var trace = new TraceStep(old_eq, eq, step1metaRule, step1AppliedRule);
            lst.Add(trace);
            string strategy = "Substitute two points coordinates into the distance function.";
            var newTuple = new Tuple<object, object>(strategy, lst);
            return newTuple;
        }

        //forward chaining
        public static void FromLineSegmentToDistance(LineSegmentSymbol lss)
        {
            //1. Substitute two points coordinates into the distance function.
            //2. Manipulate the expression to derive the goal.
            ////////////////////////////////////////////////////////
            //1.
            lss.Traces.Add(DistanceSubstitution(lss));

            ////////////////////////////////////////////////////////
            //2.
            var ls = lss.Shape as LineSegment;         
            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() { variable, 2.0 });
            var term1  = new Term(Expression.Subtract, new List<object>() {ls.Pt1.XCoordinate, ls.Pt2.XCoordinate});
            var term11 = new Term(Expression.Power, new List<object>() {term1, 2.0});
            var term2  = new Term(Expression.Subtract, new List<object>() {ls.Pt1.YCoordinate, ls.Pt2.YCoordinate});
            var term22 = new Term(Expression.Power, new List<object>() {term2, 2.0});
            var rhs = new Term(Expression.Add, new List<object>() {term11, term22});
            var eq = new Equation(lhs, rhs);

            object obj1;
            bool result = eq.IsEqGoal(out obj1);
            EqGoal eqGoal = null;
            Debug.Assert(result);
            var lst1 = obj1 as List<object>;
            foreach (var temp in lst1)
            {
                var tempGoal = temp as EqGoal;
                if (tempGoal == null) continue;
                double dNum;
                bool tempResult = LogicSharp.IsDouble(tempGoal.Rhs, out dNum);
                if (tempResult && dNum > 0)
                {
                    eqGoal = tempGoal;
                    break;
                }
            }
            Debug.Assert(eqGoal != null);
            lss.Traces.AddRange(eqGoal.Traces);
        }

        //backward chaining
        public static object FromLineSegmentToDistance(LineSegmentSymbol lss, double value)
        {
            //0. Plotting existing knowledge
            //1. Substitute two points coordinates into the distance function.
            //2. Manipulate the expression to derive the goal.
            ////////////////////////////////////////////////////////

            var ls = lss.Shape as LineSegment;
            Debug.Assert(ls != null);

            FromPointsToLineSegment(lss);

            var term1_1 = new Term(Expression.Multiply, new List<object>() {-1, ls.Pt2.XCoordinate});
            var term1   = new Term(Expression.Add, new List<object>() { ls.Pt1.XCoordinate, term1_1 });
            var term11  = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2_2 = new Term(Expression.Multiply, new List<object>() {-1, ls.Pt2.YCoordinate});
            var term2   = new Term(Expression.Add, new List<object>() { ls.Pt1.YCoordinate, term2_2});
            var term22  = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs     = new Term(Expression.Add, new List<object>() { term11, term22 });
            var lhs     = new Term(Expression.Power, new List<object>() {value, 2.0});

            var eq = new Equation(lhs, rhs);

            object obj1;
            bool result = eq.IsEqGoal(out obj1);

            if (result)
            {
                var lst = obj1 as List<object>;
                if (lst != null)
                {
                    foreach (object tempObj in lst)
                    {
                        var gGoal = tempObj as EqGoal;
                        if (gGoal != null)
                        {
                            var newTraces = new List<Tuple<object, object>>(); 
                            newTraces.AddRange(lss.Traces);
                            newTraces.Add(DistanceSubstitution(lss));
                            newTraces.AddRange(gGoal.Traces);
                            gGoal.Traces = newTraces;
                        }
                    }
                }
                return obj1;
            }
            return null;
        }
    }
}