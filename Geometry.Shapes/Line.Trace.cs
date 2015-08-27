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

    public partial class LineSymbol
    {
        /// <summary>
        /// Trace from Line General Form To Slope
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<TraceStep> FromLineGeneralFormToSlopeTrace(object target)
        {
            var line = Shape as Line;
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
            return lst;
        }

        public static string strategy_si_slope =
           "Given the line slope-intercept form y=mx+b, the slope is m.";

        /// <summary>
        /// Trace from Line Slope-Intercept Form to Slope 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public void FromLineSlopeInterceptFormToSlopeTrace(
            object target, out List<TraceStep> lst, out string strategy)
        {
            var line = Shape as Line;
            Debug.Assert(line != null);
            
            lst = new List<TraceStep>();
            string step1metaRule = "Given the line slope intercept form y=mx+b, the slope is m.";
            string step1AppliedRule = String.Format("Given line slope-intercept form {0}, the slope is {1}.", this.ToString(), SymSlope);
            var ts = new TraceStep(this, target, step1metaRule, step1AppliedRule);
            lst.Add(ts);
            strategy = strategy_si_slope;
        }

        public static string strategy_general_si =
            "Given the line general form ax+by+c=0, the slope-intercept form is y = -(a/b)x-c/b.";

        /// <summary>
        /// Trace from Line General Form to Slope-Intercept Form
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public void FromLineGeneralFormToSlopeInterceptTrace(
            object target, out List<TraceStep> steps, out string strategy)
        {
            var line = Shape as Line;
            Debug.Assert(line != null);
            Debug.Assert(line.InputType == LineType.GeneralForm);
            steps = new List<TraceStep>();
            string step1metaRule = "Given the line general form ax+by+c=0, move X term and Constant term to the right side of equation.";
            string step1AppliedRule = String.Format("Move {0}x and {1} to right side of equation: m = {0} / {1}", SymA, SymC);
            var ts = new TraceStep(this, target, step1metaRule, step1AppliedRule);
            steps.Add(ts);
            strategy = strategy_general_si;
        }

        public void FromSlopeIntercepToGraph
            (object target, out List<TraceStep> steps, out string strategy)
        {
            var line = Shape as Line;
            Debug.Assert(line != null);
            steps = new List<TraceStep>();
            string step1metaRule = "Given the line slope-intercept form y=mx+b, plot the line by passing points (0,b) and (-b/m,0).";
            string step1AppliedRule = String.Format("Plotting the line passing through (0,{0}) and ({1},0) ", SymIntercept, SymC);
            var ts = new TraceStep(this, target, step1metaRule, step1AppliedRule);
            steps.Add(ts);
            strategy = strategy_graphing;
        }

        //Graphing
        public static string strategy_graphing =
            "Given the slope-intercept from y=mx+b, plot the line by passing points (0,b) and (-b/m,0)";
    }
}