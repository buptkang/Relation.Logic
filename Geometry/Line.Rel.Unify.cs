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

    public static class LineBinaryRelation
    {
        /// <summary>
        /// construct a line through two points
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static object Unify(PointSymbol pt1, PointSymbol pt2, EqGoal goal = null)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;

            //Line build process
            if (pt1.Shape.Concrete && pt2.Shape.Concrete)
            {
                var point1 = pt1.Shape as Point;
                var point2 = pt2.Shape as Point;
               
                Debug.Assert(point1 != null);
                Debug.Assert(point2 != null);

                var winPt1 = new System.Windows.Point((double) point1.XCoordinate, (double) point1.YCoordinate);
                var winPt2 = new System.Windows.Point((double) point2.XCoordinate, (double) point2.YCoordinate);

                var lineSymbol = LineGenerationRule.GenerateLine(point1, point2);

                var line = lineSymbol.Shape as Line;
                Debug.Assert(line != null);

                line.Rel1 = winPt1;
                line.Rel2 = winPt2;

                TraceInstructionalDesign.FromPointPointToLine(pt1, pt2, lineSymbol);
                return lineSymbol;
            }
            else
            {
                //lazy evaluation    
                //Constraint solving on Graph
                var line = new Line(null); //ghost line
                line.Rel1 = pt1.Shape;
                line.Rel2 = pt2.Shape;
                var ls =  new LineSymbol(line);

                #region Reification Purpose

                if (pt1.Shape.Concrete && !pt2.Shape.Concrete)
                {
                    var point1 = pt1.Shape as Point;
                    if (pt2.CachedSymbols.Count != 0)
                    {
                        foreach (ShapeSymbol ss in pt2.CachedSymbols)
                        {
                            var ps = ss as PointSymbol;
                            Debug.Assert(ps != null);
                            Debug.Assert(ps.Shape.Concrete);
                            var cachePoint = ps.Shape as Point;
                            Debug.Assert(cachePoint != null);
                            var gline = LineGenerationRule.GenerateLine(point1, cachePoint);
                            gline.Traces.AddRange(ps.Traces);
                            TraceInstructionalDesign.FromPointPointToLine(pt1, pt2, gline);
                            ls.CachedSymbols.Add(gline);
                        }
                    }
                }

                if (!pt1.Shape.Concrete && pt2.Shape.Concrete)
                {
                    var point2 = pt2.Shape as Point;
                    if (pt1.CachedSymbols.Count != 0)
                    {
                        foreach (ShapeSymbol ss in pt1.CachedSymbols)
                        {
                            var ps = ss as PointSymbol;
                            Debug.Assert(ps != null);
                            Debug.Assert(ps.Shape.Concrete);
                            var cachePoint = ps.Shape as Point;
                            Debug.Assert(cachePoint != null);
                            var gline = LineGenerationRule.GenerateLine(cachePoint, point2);
                            gline.Traces.AddRange(ps.Traces);
                            TraceInstructionalDesign.FromPointPointToLine(pt1, pt2, gline);
                            ls.CachedSymbols.Add(gline);
                        }
                    }
                }

                if (!pt1.Shape.Concrete && !pt2.Shape.Concrete)
                {
                    foreach (ShapeSymbol ss1 in pt1.CachedSymbols)
                    {
                        foreach (ShapeSymbol ss2 in pt2.CachedSymbols)
                        {
                            var ps1 = ss1 as PointSymbol;
                            Debug.Assert(ps1 != null);
                            Debug.Assert(ps1.Shape.Concrete);
                            var cachePoint1 = ps1.Shape as Point;
                            Debug.Assert(cachePoint1 != null);
                            var ps2 = ss2 as PointSymbol;
                            Debug.Assert(ps2 != null);
                            Debug.Assert(ps2.Shape.Concrete);
                            var cachePoint2 = ps2.Shape as Point;
                            Debug.Assert(cachePoint2 != null);
                            var gline = LineGenerationRule.GenerateLine(cachePoint1, cachePoint2);
                            gline.Traces.AddRange(ps1.Traces);
                            gline.Traces.AddRange(ps2.Traces);
                            TraceInstructionalDesign.FromPointPointToLine(pt1, pt2, gline);
                            ls.CachedSymbols.Add(gline);
                        }
                    }
                }

                #endregion

                if (goal != null)
                {
                    return ls.Unify(goal);
                }

                return ls;
            }
        }

        /// <summary>
        /// construct a line through a point and a goal,
        /// e.g A(1,2) ^ S = 2=> Conjunctive Norm Form
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static LineSymbol Unify(PointSymbol pt, EqGoal goal)
        {
            var variable1 = goal.Lhs as Var;
            Debug.Assert(variable1 != null);

            if(LineAcronym.EqualSlopeLabels(variable1.ToString()))
            {
                double dValue;
                bool result = LogicSharp.IsDouble(goal.Rhs, out dValue);
                if (result)
                {
                    if (!pt.Shape.Concrete) return null;
                    var line = LineGenerationRule.GenerateLine((Point)pt.Shape, dValue, null);
                    if (pt.Traces.Count != 0)
                    {
                        line.Traces.AddRange(pt.Traces);
                    }
                    if (goal.Traces.Count != 0)
                    {
                        line.Traces.AddRange(goal.Traces);
                    }

                    TraceInstructionalDesign.FromPointSlopeToLine(pt, goal, line);

                    line.OutputType = LineType.SlopeIntercept;
                    return line;
                }
                else
                {
                    var line = new Line(null); //ghost line
                    var ls = new LineSymbol(line);
                    ls.OutputType = LineType.SlopeIntercept;
                    return ls;
                }
            }
            return null;
        }

        public static LineSymbol Unify(EqGoal goal, PointSymbol pt)
        {
            return Unify(pt, goal);
        }

        /// <summary>
        /// construct a line through two goals
        /// e.g  m=2, k=3 => conjunctive norm form
        /// </summary>
        /// <param name="goal1"></param>
        /// <param name="goal2"></param>
        /// <returns></returns>
        public static LineSymbol Unify(EqGoal goal1, EqGoal goal2)
        {
            var variable1 = goal1.Lhs as Var;
            var variable2 = goal2.Lhs as Var;
            Debug.Assert(variable1 != null);
            Debug.Assert(variable2 != null);

            var dict = new Dictionary<string, object>();

            string slopeKey = "slope";
            string interceptKey = "intercept";
            if (LineAcronym.EqualSlopeLabels(variable1.ToString()))
            //if (variable1.ToString().Equals(LineAcronym.Slope1))
            {
                dict.Add(slopeKey, goal1.Rhs);
            }
            if (LineAcronym.EqualInterceptLabels(variable1.ToString()))
            //if (variable1.ToString().Equals(LineAcronym.Intercept1))
            {
                dict.Add(interceptKey, goal1.Rhs);
            }
            if (LineAcronym.EqualSlopeLabels(variable2.ToString()))
            //if (variable2.ToString().Equals(LineAcronym.Slope1))
            {
                if (dict.ContainsKey(slopeKey)) return null;
                dict.Add(slopeKey, goal2.Rhs);
            }
            if(LineAcronym.EqualInterceptLabels(variable2.ToString()))
            //if (variable2.ToString().Equals(LineAcronym.Intercept1))
            {
                if (dict.ContainsKey(interceptKey)) return null;
                dict.Add(interceptKey, goal2.Rhs);
            }

            if (dict.Count == 2 &&
                dict[slopeKey] != null &&
                dict[interceptKey] != null)
            {
                if (LogicSharp.IsNumeric(dict[slopeKey]) &&
                    LogicSharp.IsNumeric(dict[interceptKey]))
                {
                    double dSlope, dIntercept;
                    LogicSharp.IsDouble(dict[slopeKey], out dSlope);
                    LogicSharp.IsDouble(dict[interceptKey], out dIntercept);
                    var line = LineGenerationRule.GenerateLine(dSlope, dIntercept);
                    var ls = new LineSymbol(line) { OutputType = LineType.SlopeIntercept };
                    
                    TraceInstructionalDesign.FromSlopeInterceptToLineSlopeIntercept(goal1, goal2, ls);
                    return ls;
                }
                else
                {
                    //lazy evaluation    
                    //Constraint solving on Graph
                    var line = new Line(null); //ghost line
                    var ls = new LineSymbol(line);
                    ls.OutputType = LineType.SlopeIntercept;
                    return ls;
                }
            }
            return null;
        }
    }

    public static class LineUnaryRelation
    {
        public static bool Unify(this LineSymbol shapeSymbol, object constraint, out object output)
        {
            output = shapeSymbol.Unify(constraint);
            return output != null;
        }

        public static object Unify(this LineSymbol ls, object constraint)
        {
            var refObj = constraint as string;
            var eqGoal = constraint as EqGoal;

            if (refObj != null)
            {
                #region forward searching

                if (LineAcronym.EqualSlopeLabels(refObj))
                {
                     return ls.InferSlope(refObj);
                }

                if (LineAcronym.EqualInterceptLabels(refObj))
                {
                    return ls.InferIntercept(refObj);
                }

                if (LineAcronym.EqualGeneralFormLabels(refObj))
                {
                    return ls.InferGeneralForm(refObj);
                }

                if (LineAcronym.EqualSlopeInterceptFormLabels(refObj))
                {
                    return ls.InferSlopeInterceptForm();
                }

                if (LineAcronym.GraphLine.Equals(refObj))
                {
                    return ls.InferGraph();
                }
                #endregion
            }

            if (eqGoal != null)
            {
                var rhs = eqGoal.Rhs;
                double dNum;
                bool isDouble = LogicSharp.IsDouble(rhs, out dNum);
                if (!isDouble) return null;

                var lhs = eqGoal.Lhs.ToString();

                if (LineAcronym.EqualSlopeLabels(lhs))
                {
                    var obj = ls.InferSlope(dNum);
                    var lstObj = obj as List<object>;
                    Debug.Assert(lstObj != null);
                    var eqGoal1 = lstObj[0] as EqGoal;
                    if (eqGoal1 != null)
                    {
                        var newTraces = new List<Tuple<object, object>>();
                        newTraces.AddRange(eqGoal.Traces);
                        newTraces.AddRange(eqGoal1.Traces);
                        eqGoal1.Traces = newTraces;
                    }
                    return obj;
                }
            }
            return null;
        }

        private static LineSymbol InferGraph(this LineSymbol inputLineSymbol)
        {
/*            var ls = inputLineSymbol.InferSlopeInterceptForm();
            TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);*/
            return inputLineSymbol;
        }

        /// <summary>
        /// ax+by+c=0 =========> y = -(a/b)x-(c/b)
        /// </summary>
        /// <param name="inputLineSymbol"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private static LineSymbol InferSlopeInterceptForm(this LineSymbol inputLineSymbol)
        {
            //TODO
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.SlopeIntercept;
            ls.Traces.AddRange(inputLineSymbol.Traces);
            TraceInstructionalDesign.FromOneFormToAnother(inputLineSymbol, ls);
            return ls;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="inputLineSymbol"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private static LineSymbol InferGeneralForm(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.GeneralForm;
            ls.Traces.AddRange(inputLineSymbol.Traces);
            TraceInstructionalDesign.FromOneFormToAnother(inputLineSymbol, ls);
            return ls;
        }

        //forward solving
        private static EqGoal InferSlope(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;           
            Debug.Assert(line != null);
            var goal = new EqGoal(new Var(label), line.Slope);
            goal.Traces.AddRange(inputLineSymbol.Traces);
            TraceInstructionalDesign.FromLineToSlope(inputLineSymbol, goal);
            return goal;
        }

        //backward solving
        private static object InferSlope(this LineSymbol inputLineSymbol, double value)
        {
           return TraceInstructionalDesign.FromLineToSlope(inputLineSymbol, value);
        }

        private static object InferIntercept(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            if (line.Intercept != null)
            {
                var goal = new EqGoal(new Var(label), line.Intercept);
                goal.Traces.AddRange(inputLineSymbol.Traces);
                TraceInstructionalDesign.FromLineToIntercept(inputLineSymbol, goal);
                return goal;                
            }

            if (inputLineSymbol.CachedSymbols.Count != 0)
            {
                var goalList = new List<object>();
                foreach (var lss in inputLineSymbol.CachedSymbols)
                {
                    var cachedLss = lss as LineSymbol;
                    Debug.Assert(cachedLss != null);
                    var cachedLs = cachedLss.Shape as Line;
                    Debug.Assert(cachedLs != null);
                    var goal = new EqGoal(new Var(label), cachedLs.Intercept);
                    goal.Traces.AddRange(cachedLss.Traces);
                    TraceInstructionalDesign.FromLineToIntercept(cachedLss, goal); 
                    goalList.Add(goal);
                }
                return goalList;
            }

            return null;
        }
    }
}
