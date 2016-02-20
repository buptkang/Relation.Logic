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

using System.Runtime.InteropServices;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    public static partial class TraceInstructionalDesign
    {
        #region Form transformation Trace

        public static void FromOneFormToAnother(LineSymbol source, LineSymbol target)
        {
            if (source.OutputType.Equals(LineType.SlopeIntercept))
            {
                if (target.OutputType.Equals(LineType.GeneralForm))
                {
                    FromLineSlopeIntercetpToLineGeneralForm(target);
                }
            }

            if (source.OutputType.Equals(LineType.GeneralForm))
            {
                if (target.OutputType.Equals(LineType.SlopeIntercept))
                {
                    FromLineGeneralFormToSlopeIntercept(target);
                }
            }
        }

        #region Line General Form => Slope-Intercept Form

        public static string strategy_general_si =
          "Given the line general form ax+by+c=0, the slope-intercept form is y = -(a/b)x-c/b.";

        /// <summary>
        /// Trace from Line General Form to Slope-Intercept Form
        /// ax+by+c=0 => y=mx+b
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static void FromLineGeneralFormToSlopeIntercept(LineSymbol ls)
        {
            string strategy = "Given a general form line ax+by+c=0, the slope-intercept form of it is y = -(a/b)x-c/b.";

            var line = ls.Shape as Line;
            Debug.Assert(line != null);

            var newLS = new LineSymbol(line);
            newLS.OutputType = LineType.SlopeIntercept;

            var x = new Var("x");
            var y = new Var("y");
            var xTerm = new Term(Expression.Multiply, new List<object>() { line.A, x });
            var yTerm = new Term(Expression.Multiply, new List<object>() { line.B, y });
            var oldEqLeft = new Term(Expression.Add, new List<object>() { xTerm, yTerm, line.C });
            var oldEq = new Equation(oldEqLeft, 0);

            var invertXTerm = new Term(Expression.Multiply, new List<object>() { -1, xTerm });
            var intertZ = new Term(Expression.Multiply, new List<object>() { -1, line.C });

            var internalRight = new Term(Expression.Add, new List<object>() { invertXTerm, intertZ });
            var internalEq = new Equation(yTerm, internalRight);

            var finalXTerm = new Term(Expression.Multiply, new List<object>() { line.Slope, x });
            var finalRight = new Term(Expression.Add, new List<object>() { finalXTerm, line.Intercept });
            var lastEq = new Equation(yTerm, finalRight);

            string step1metaRule = "Given the line general form ax+by+c=0, move x term and Constant term to the right side of equation.";
            string step1AppliedRule = String.Format("Move {0}x and {1} to right side of equation.", ls.SymA, ls.SymC);

            string kc = EquationsRule.RuleConcept(EquationsRule.EquationRuleType.Transitive);
            var ts0 = new TraceStep(null, internalEq, kc, step1metaRule, step1AppliedRule);
            ls._innerLoop.Add(ts0);

            string appliedRule = string.Format("divide coefficient b in both side of equation.");

            kc = EquationsRule.RuleConcept(EquationsRule.EquationRuleType.Inverse);

            var ts1 = new TraceStep(null, lastEq, kc, AlgebraRule.AlgebraicStrategy, appliedRule);
            ls._innerLoop.Add(ts1);
            ls.GenerateATrace(strategy);
        }

        #endregion

        #region Slope-Intercept Form => Line General Form

        public static string strategy_si_general =
         "Given the line slope-intercept from y=mx+b, the general form of a line is mx-y+b=0.";

        public static void FromLineSlopeIntercetpToLineGeneralForm(LineSymbol ls)
        {
            var line = ls.Shape as Line;
            Debug.Assert(line != null);
            Debug.Assert(ls.OutputType == LineType.GeneralForm);

            string step1metaRule = "Given the line slope-intercept from y=mx+b, move y term to the right side of equation.";
            string step1AppliedRule = String.Format("Move y to the right side of equation");

            string kc = GeometryScaffold.KC_LinePatternsTransform;

            var ts = new TraceStep(ls.SlopeInterceptForm, ls.GeneralForm, kc, step1metaRule, step1AppliedRule);
            string strategy = strategy_si_general;
            ls._innerLoop.Add(ts);
            ls.GenerateATrace(strategy);
        }

        #endregion

        #endregion

        #region Graphing

        public static void LineSlopeIntercepToGraph(LineSymbol ls)
        {
            string strategy = strategy_graphing;

            //Plotting shapes
            //1. plotting Y-Intercept
            //2. plotting X-Intercept
            //3. plotting the line

            //////////////////////////////////////////////////////////////
            // Step 1:
            var pt = new Point(0, ls.SymIntercept);
            var ptSym = new PointSymbol(pt);

            var ts0 = new TraceStep(null, ptSym, null, PlottingRule.PlottingStrategy, PlottingRule.Plot(ptSym));
            ls._innerLoop.Add(ts0);
            //////////////////////////////////////////////////////////////
            // Step 2:

            var line = ls.Shape as Line;
            Debug.Assert(line != null);

            Equation eq;
            if (line.B == null)
            {
                
            }
            else
            {
                var x = new Var("x");
                //step 2.1
                var term = new Term(Expression.Multiply, new List<object>() { line.Slope, x });
                var term1 = new Term(Expression.Add, new List<object>() { term, line.Intercept });
                eq = new Equation(term1, 0);
                object obj;
                EqGoal gGoal = null;
                bool result = eq.IsEqGoal(out obj);
                if (result)
                {
                    gGoal = obj as EqGoal;
                }
                if (gGoal != null)
                {
                    double dX;
                    LogicSharp.IsDouble(gGoal.Rhs, out dX);
                    var pt1 = new Point(dX, 0);
                    var ptSym1 = new PointSymbol(pt1);
                    var ts1 = new TraceStep(null, ptSym1, null, PlottingRule.PlottingStrategy, PlottingRule.Plot(ptSym1));
                    ls._innerLoop.Add(ts1);
                }
            }

            /////////////////////////////////////////////////////////////////

            const string step1MetaRule = "Given the line slope-intercept form y=mx+b, plot the line by passing points (0,b) and (-b/m,0).";
            string step1AppliedRule = String.Format("Plotting the line passing through (0,{0}) and ({1},0) ", ls.SymIntercept, ls.SymC);
            //var ts = new TraceStep(null, ls.SlopeInterceptForm, step1MetaRule, step1AppliedRule);


            string kc = GeometryScaffold.KC_LineGraphing;

            var ts = new TraceStep(null, ls, kc, step1MetaRule, step1AppliedRule);
            ls._innerLoop.Add(ts);

            //////////////////////////////////////////////////////////////////

            ls.GenerateATrace(strategy);
        }

        //Graphing
        public static string strategy_graphing =
            "Given the slope-intercept from y=mx+b, plot the line by passing points (0,b) and (-b/m,0)";

        #endregion

        #region Property Deriving

        #region Slope-Intercept Form => Slope Form

        /*
         * Automatic scaffolding
         * 
         * Distance function (x-x0)^2+(y-y0)^2 = d^2
         * 
         * Forward chaining to derive d.
         * Backward chaining to derive other four parameters.
         */
        private static Tuple<object, object> SlopeSubstitution(Point pt1, Point pt2, double value)
        {
            //out strategy
            string strategy = "Substitute two points and slope value into the slope function.";

            //1. Substitute two points into the line slope function.
            //2. Substitute slope property into the line slope function.
            var lst = new List<TraceStep>();

            ///////////////////////////////////////////////////////////////////
            var variable = new Var('m');
            //1. 
            
            string step1metaRule = "Consider the Line Slope Function: m=(y1-y0)/(x1-x0)";
            string step1AppliedRule = String.Format(
                "Substitute two points value into the slope function {0}=({1}-{2})/({3}-{4})",
                variable,
                pt2.YCoordinate.ToString(),
                pt1.YCoordinate.ToString(),
                pt2.XCoordinate.ToString(),
                pt1.XCoordinate.ToString());

         /*   var pt1X = new Var("x0");
            var pt1Y = new Var("y0");
            var pt2X = new Var("x1");
            var pt2Y = new Var("y1");*/

         /*   var term1_1 = new Term(Expression.Subtract, new List<object>() { pt1X, pt2X });
            var term2_1 = new Term(Expression.Subtract, new List<object>() { pt1Y, pt2Y });
            var rhs_1 = new Term(Expression.Divide, new List<object>() { term2_1, term1_1 });
           
            var old_eq = new Equation(variable, rhs_1);*/

            var term1_11 = new Term(Expression.Subtract, new List<object>() { pt2.XCoordinate, pt1.XCoordinate });
            var term2_11 = new Term(Expression.Subtract, new List<object>() { pt2.YCoordinate, pt1.YCoordinate });
            var rhs_11 = new Term(Expression.Divide, new List<object>() { term1_11, term2_11 });
            var eq = new Equation(variable, rhs_11);

            string kc = GeometryScaffold.KC_LineSlopeForm;



            var trace1 = new TraceStep(null, eq, kc, step1metaRule, step1AppliedRule);
            //lst.Add(trace1);
            lst.Add(trace1);

           ///////////////////////////////////////////////////////////////////////////

            //2.
            var newEq = new Equation(value, rhs_11);

            string step2metaRule = "Substitute slope property into the Line Slope Function: m=(y1-y0)/(x1-x0)";
            string step2AppliedRule = String.Format(
                "Substitute slope value into the slope function {0}=({1}-{2})/({3}-{4})",
                value,
                pt2.YCoordinate.ToString(),
                pt1.YCoordinate.ToString(),
                pt2.XCoordinate.ToString(),
                pt1.XCoordinate.ToString());

            kc = SubstitutionRule.SubstituteKC();

            var trace2 = new TraceStep(null, newEq, kc, step2metaRule, step2AppliedRule);
            //lst.Add(trace1);
            lst.Add(trace2);

            ////////////////////////////////////////////////////////////////////////////

            var newTuple = new Tuple<object, object>(strategy, lst);
            return newTuple;
        }

        public static string strategy_si_slope =
          "Given the line slope-intercept form y=mx+b, the slope is m.";

        //forward solving
        public static void FromLineToSlope(LineSymbol ls, EqGoal goal)
        {
            //one strategy, one step.
            var line = ls.Shape as Line;
            Debug.Assert(line != null);
            var lst = new List<TraceStep>();
            string step1metaRule = "Given the line slope intercept form y=mx+b, the slope is m.";
            string step1AppliedRule = String.Format("Given line slope-intercept form {0}, the slope is {1}.", ls.ToString(), ls.SymSlope);

            string kc = GeometryScaffold.KC_LineSlopeForm;
            
            var ts = new TraceStep(ls, goal, kc, step1metaRule, step1AppliedRule);
            lst.Add(ts);
            var strategy = strategy_si_slope;
            var tuple = new Tuple<object, object>(strategy, lst);
            goal.Traces.Add(tuple);
        }

        //backward solving
        public static object FromLineToSlope(LineSymbol ls, double value)
        {
            var line = ls.Shape as Line;
            Debug.Assert(line != null);

            var pt1 = line.Rel1 as Point;
            var pt2 = line.Rel2 as Point;

            if (pt1 == null || pt2 == null) return null;

            //(pt2.Y-pt1.Y)/(pt2.X-pt1.X) = slope

            var yInverse = new Term(Expression.Multiply, new List<object>() { -1, pt1.YCoordinate });
            var term1 = new Term(Expression.Add, new List<object>() { pt2.YCoordinate, yInverse });
            var xInverse = new Term(Expression.Multiply, new List<object>() { -1, pt1.XCoordinate });
            var term2 = new Term(Expression.Add, new List<object>() { pt2.XCoordinate, xInverse });
            var term11 = new Term(Expression.Divide, new List<object>() { term1, term2 });
            var eq = new Equation(term11, value);

            object obj1;
            bool result = eq.IsEqGoal(out obj1);
            if (!result) return null;

            var eqGoal = obj1 as EqGoal;
            if (eqGoal == null) return null;

            var newTraces = new List<Tuple<object, object>>();
            newTraces.Add(SlopeSubstitution(pt1, pt2, value));
            Debug.Assert(eqGoal.Traces.Count == 1);
            var trace = eqGoal.Traces[0];
            var subStrategy = "Derive unknown variable by manipulating the current algebraic expression.";
            var newTrace = new Tuple<object, object>(subStrategy, trace.Item2);
            newTraces.Add(newTrace);
            //newTraces.AddRange(eqGoal.Traces);
            eqGoal.Traces = newTraces;
            return new List<object>() { eqGoal };
        }
        #endregion

        #region Slope-Intercept Form => Intercept Form

        //forward solving
        public static void FromLineToIntercept(LineSymbol ls, EqGoal goal)
        {
            var line = ls.Shape as Line;
            Debug.Assert(line != null);
            var lst = new List<TraceStep>();
            string step1metaRule = "Given the line slope intercept form y=mx+K, the y-intercept is K.";
            string step1AppliedRule = String.Format("Given line slope-intercept form {0}, the slope is {1}.", ls.ToString(), ls.SymIntercept);

            string kc = GeometryScaffold.KC_LineInterceptForm;



            var ts = new TraceStep(ls, goal, kc, step1metaRule, step1AppliedRule);




            lst.Add(ts);
            var strategy = strategy_si_intercept;
            var tuple = new Tuple<object, object>(strategy, lst);
            goal.Traces.Add(tuple);
        }

        public static string strategy_si_intercept =
              "Given the line slope-intercept form y=mx+k, the y-intercept is k.";

        #endregion

        #endregion

        #region Relation to Line Trace

        /*
         * given m=2, k=3, y=3x+2
         */
        public static void FromSlopeInterceptToLineSlopeIntercept(EqGoal slopeGoal, EqGoal interceptGoal, LineSymbol ls)
        {
            //1. Substitute slope and intercept properties into the line slope-intercept form y=mx+b.
            ////////////////////////////////////////////////////////


            var ts0 = new TraceStep(null, slopeGoal, GeometryScaffold.KC_LineSlopeForm, PlottingRule.PlottingStrategy, PlottingRule.Plot(slopeGoal));
            var ts1 = new TraceStep(null, interceptGoal, GeometryScaffold.KC_LineInterceptForm, PlottingRule.PlottingStrategy, PlottingRule.Plot(interceptGoal));
            ls._innerLoop.Add(ts0);

            var abstractLs = new Line(ls.Shape.Label, slopeGoal.Lhs, interceptGoal.Lhs);
            var abstractLss = new LineSymbol(abstractLs);
            var internalLs = new Line(ls.Shape.Label, ls.SymSlope, interceptGoal.Lhs);
            var internalLss = new LineSymbol(internalLs);

            var traceStep1 = new TraceStep(abstractLss, internalLss, SubstitutionRule.SubstituteKC(), SubstitutionRule.SubstitutionStrategy, SubstitutionRule.ApplySubstitute(abstractLss, slopeGoal));
            ls._innerLoop.Add(traceStep1);

            ls._innerLoop.Add(ts1);
            /*
                        var rule = "Substitute given property to line slope-intercept form.";
                        var appliedRule1 = string.Format("Substitute slope={0} into y=mx+b", ls.SymSlope);
                        var appliedRule2= string.Format("Substitute intercept={0} into y=mx+b", ls.SymIntercept);*/
            var traceStep2 = new TraceStep(internalLss, ls, SubstitutionRule.SubstituteKC(), SubstitutionRule.SubstitutionStrategy, SubstitutionRule.ApplySubstitute(internalLss, interceptGoal));
            ls._innerLoop.Add(traceStep2);

            string strategy = "Substitute slope and intercept properties into the line slope-intercept form y = mx + b.";
            ls.GenerateATrace(strategy);
        }

        public static void FromPointPointToLine(PointSymbol ps1, PointSymbol ps2, LineSymbol ls)
        {
            var m = new Var("m");
            var k = new Var("k");
            var x = new Var("x");
            var y = new Var("y");
            var term = new Term(Expression.Multiply, new List<object>() { m, x });
            var term1 = new Term(Expression.Add, new List<object>() { term, k });
            var eqPattern = new Equation(y, term1);

            var term2 = new Term(Expression.Multiply, new List<object>() {m, ps1.SymXCoordinate});
            var term22 = new Term(Expression.Add, new List<object>() {term2, k});
            var eqPattern1 = new Equation(ps1.SymYCoordinate, term22);

            var term3 = new Term(Expression.Multiply, new List<object>() {m, ps2.SymXCoordinate});
            var term33 = new Term(Expression.Add, new List<object>() { term3, k });
            var eqPattern2 = new Equation(ps2.SymYCoordinate, term33);

            string strategy = "Generate a line by substituting two given points into the line slope-intercept form y=mx+k.";

            var ts0 = new TraceStep(eqPattern, eqPattern1, SubstitutionRule.SubstituteKC(),SubstitutionRule.SubstitutionStrategy,
                SubstitutionRule.ApplySubstitute(eqPattern, ps1));
            var ts1 = new TraceStep(eqPattern, eqPattern2, SubstitutionRule.SubstituteKC(),SubstitutionRule.SubstitutionStrategy,
                SubstitutionRule.ApplySubstitute(eqPattern, ps2));

            string kc = GeometryScaffold.KC_LineSlopeForm;

            var ts2 = new TraceStep(null, ls, kc, "calculate m and k through the above two linear equations.",
                "calculate m and k through linear equation and retrieve y=mx+k line form.");

            ls._innerLoop.Add(ts0);
            ls._innerLoop.Add(ts1);
            ls._innerLoop.Add(ts2);

            ls.GenerateATrace(strategy);
        }


        public static void FromPointSlopeToLine(PointSymbol ps, EqGoal goal, LineSymbol ls)
        {
            string strategy = "Substitute point and slope property into line form.";

            // 1. Substitute slope property into the line slope-intercept form.
            // 2. Calculate the line intercept by substituting the point into line pattern.
            
            //////////////////////////////////////////////////////////////////////

            string strategy1 = "Substitute slope property into the line slope-intercept form.";

            var m = new Var("m");
            var k = new Var("k");
            var x = new Var("x");
            var y = new Var("y");
            var term = new Term(Expression.Multiply, new List<object>() {m, x});
            var term1 = new Term(Expression.Add, new List<object>() {term, k});
            var eqPattern = new Equation(y, term1);

            var term2 = new Term(Expression.Multiply, new List<object>() {goal.Rhs,x});
            var term3 = new Term(Expression.Add, new List<object>() {term2, k});
            var eqInternal1 = new Equation(y, term3);

            var appliedRule1 = SubstitutionRule.ApplySubstitute(eqPattern, goal);

            var ts0 = new TraceStep(eqPattern, eqInternal1, SubstitutionRule.SubstituteKC(), strategy1, appliedRule1);
            ls._innerLoop.Add(ts0);

            //////////////////////////////////////////////////////////////////////

            var point = ps.Shape as Point;
            Debug.Assert(point != null);

            string strategy2 = "Calculate the line intercept by substituting the point into line pattern.";

            var term4 = new Term(Expression.Multiply, new List<object>() { goal.Rhs, point.XCoordinate});
            var term5 = new Term(Expression.Add, new List<object>() {term4, k});
            var eqinternal2 = new Equation(point.YCoordinate, term5);

            object obj;
            bool result = eqinternal2.IsEqGoal(out obj);
            var eqGoal = obj as EqGoal;
            Debug.Assert(eqGoal != null);

            var gTuple = eqGoal.Traces[0];
            var gLst = gTuple.Item2 as List<TraceStep>;
            Debug.Assert(gLst != null);
            ls._innerLoop.AddRange(gLst);

            var appliedRule2 = SubstitutionRule.ApplySubstitute(eqInternal1, ps);

            var ts = new TraceStep(eqInternal1, ls, SubstitutionRule.SubstituteKC(), strategy2, appliedRule2);
            ls._innerLoop.Add(ts);
            ls.GenerateATrace(strategy);
        }

        #endregion
    }

    /* /// <summary>
/// Trace from Line General Form To Slope
/// </summary>
/// <param name="target"></param>
/// <returns></returns>
public static List<TraceStep> FromLineGeneralFormToSlopeTrace(object target)
{
    return null;
    /* var line = Shape as Line;
    Debug.Assert(line != null);
    Debug.Assert(line.InputType == LineType.GeneralForm);

    var lst = new List<TraceStep>();

    var lhs = new Var('m');
    var rhs = new Term(Expression.Divide, new List<object>() { SymA, NegSymB });
    var eq1 = new Equation(lhs, rhs);
    string step1metaRule = "Given the line general form ax+by+c=0, the slope m =-a/b.";
    string step1AppliedRule = String.Format("Substitute a and b into slope function : m = {0} / {1}", SymA, NegSymB);
    var ts = new TraceStep(this, eq1, step1metaRule, step1AppliedRule);

    lst.Add(ts);

    string step2metaRule = ArithRule.CalcRule("Divide");
    string step2AppliedRule = ArithRule.CalcRule("Divide", SymA, NegSymB, SymSlope);
    var ts2 = new TraceStep(eq1, target, step2metaRule, step2AppliedRule);
    lst.Add(ts2);
    return lst;#1#
}

*/
}