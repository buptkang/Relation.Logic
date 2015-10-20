﻿/*******************************************************************************
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

using System.Collections.ObjectModel;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public partial class PointSymbol
    {
        /// <summary>
        /// This method quarantees that the substitution term is unique toward each variable.
        /// 
        /// dict: {{x:3},{x:4}} is not allowed 
        /// dict: {{x:3}, {y:4}} is allowed
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dict"></param>
        /// <param name="ps"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public bool Reify(EqGoal goal)
        {
            var point = Shape as Point;
            Debug.Assert(point != null);

            //EqGoal tempGoal = goal.GetLatestDerivedGoal();
            EqGoal tempGoal = goal;
            bool cond1 = Var.IsVar(tempGoal.Lhs) && LogicSharp.IsNumeric(tempGoal.Rhs);
            bool cond2 = Var.IsVar(tempGoal.Rhs) && LogicSharp.IsNumeric(tempGoal.Lhs);
            Debug.Assert(cond1 || cond2);

            if (point.Concrete) return false;

            object xResult = EvalGoal(point.XCoordinate, tempGoal);
            object yResult = EvalGoal(point.YCoordinate, tempGoal);

            //Atomic operation
            if (!point.XCoordinate.Equals(xResult))
            {
                GenerateXCacheSymbol(xResult, goal);
                RaiseReify(null);
                return true;
            }
            else if (!point.YCoordinate.Equals(yResult))
            {
                GenerateYCacheSymbol(yResult, goal);
                RaiseReify(null);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UnReify(EqGoal goal)
        {
            var point = Shape as Point;
            Debug.Assert(point != null);
            if (!ContainGoal(goal)) return false;
            var updateLst = new ObservableCollection<ShapeSymbol>();
            var unchangedLst = new ObservableCollection<ShapeSymbol>();
            foreach (var shape in CachedSymbols.ToList())
            {
                var pt = shape as PointSymbol;
                if (pt == null) continue;
                if (pt.ContainGoal(goal))
                {
                    pt.UndoGoal(goal, point);
                    if (pt.CachedGoals.Count != 0)
                    {
                        updateLst.Add(pt);
                    }
                }
                else
                {
                    unchangedLst.Add(shape);
                }
            }

            if (unchangedLst.Count != 0)
            {
                CachedSymbols = unchangedLst;
            }
            else
            {
                CachedSymbols = updateLst;
            }

            RemoveGoal(goal);
            return true;
        }

        #region Reify Utilities

        public void GenerateXCacheSymbol(object obj, EqGoal goal)
        {        
            var point = Shape as Point;
            Debug.Assert(point != null);
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.X,goal));
            if (CachedSymbols.Count == 0)
            {
                #region generate new object

                var gPoint = new Point(point.Label, obj, point.YCoordinate);
                var gPointSymbol = new PointSymbol(gPoint);
                gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.X, goal));
                CachedSymbols.Add(gPointSymbol);
                gPointSymbol.Traces.AddRange(goal.Traces);
                //Transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gPoint.Traces.Insert(0, goal.Traces[i]);
                }

                //Substitution trace
                string rule = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this, gPointSymbol, rule, appliedRule);
                gPointSymbol._innerLoop.Add(ts);
                string strategy = "Reify a Point's x-coordinate by substituing a given fact.";
                gPointSymbol.GenerateATrace(strategy);
                //gPoint.Traces.Insert(0,ts);
                #endregion
            }
            else
            {
                #region Iterate existing point object 

                foreach (ShapeSymbol ss in CachedSymbols.ToList())
                {
                    var pt = ss.Shape as Point;
                    if (pt != null)
                    {
                        var xResult = LogicSharp.Reify(pt.XCoordinate, goal.ToDict());
                        if (!pt.XCoordinate.Equals(xResult))
                        {
                            var gPt = new Point(pt.Label, pt.XCoordinate, pt.YCoordinate);
                            var gPointSymbol = new PointSymbol(gPt);
                            gPointSymbol.Traces.AddRange(goal.Traces);
                            //substitute
                            pt.XCoordinate = xResult;
                            ss.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.X, goal));

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                pt.Traces.Insert(0, goal.Traces[i]);
                            }

                            string rule        = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(pt, goal);

                            var ts = new TraceStep(ss, gPointSymbol, rule, appliedRule);

                            gPointSymbol._innerLoop.Add(ts);
                            string strategy = "Reify a Point's x-coordinate by substituing a given fact.";
                            gPointSymbol.GenerateATrace(strategy);
                            //pt.Traces.Insert(0,ts);
                        }
                        else
                        {
                            //generate
                            var gPoint = new Point(pt.Label, obj, pt.YCoordinate);
                            var gPointSymbol = new PointSymbol(gPoint);
                            gPointSymbol.Traces.AddRange(goal.Traces);
                            gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.X, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in CachedGoals)
                            {
                                if (pair.Key.Equals(PointAcronym.Y))
                                {
                                    gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.Y, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gPointSymbol);

                            //substitute
                            //Add traces from pt to gPoint
                            for (int i = pt.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, pt.Traces[i]);
                            }

                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, goal.Traces[i]);
                            }

                            string rule        = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(ss, goal);

                            var ts = new TraceStep(ss, gPointSymbol,rule, appliedRule);
                            gPointSymbol._innerLoop.Add(ts);
                            string strategy = "Reify a Point's x-coordinate by substituing a given fact.";
                            gPointSymbol.GenerateATrace(strategy);
                            //gPoint.Traces.Insert(0,ts);
                        }
                    }
                }
                #endregion 
            }
        }

        public void GenerateYCacheSymbol(object obj, EqGoal goal)
        {
            var point = Shape as Point;
            Debug.Assert(point != null);
            CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.Y, goal));
            if (CachedSymbols.Count == 0)
            {
                var gPoint = new Point(point.Label, point.XCoordinate, obj);
                var gPointSymbol = new PointSymbol(gPoint);
                gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.Y, goal));
                CachedSymbols.Add(gPointSymbol);
                gPointSymbol.Traces.AddRange(goal.Traces);

                //transform goal trace
                for (int i = goal.Traces.Count - 1; i >= 0; i--)
                {
                    gPoint.Traces.Insert(0, goal.Traces[i]);
                }
                //Substitution trace
                var rule        = SubstitutionRule.ApplySubstitute();
                var appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this, gPointSymbol, rule, appliedRule);
                gPointSymbol._innerLoop.Add(ts);


                string strategy = "Reify a Point's y-coordinate by substituing a given fact.";

                gPointSymbol.GenerateATrace(strategy);
                //gPoint.Traces.Insert(0, ts);
            }
            else
            {
                foreach (ShapeSymbol ss in CachedSymbols.ToList())
                {
                    var pt = ss.Shape as Point;
                    if (pt != null)
                    {
                        var yResult = LogicSharp.Reify(pt.YCoordinate, goal.ToDict());
                        if (!pt.YCoordinate.Equals(yResult))
                        {
                            var gPt = new Point(pt.Label, pt.XCoordinate, pt.YCoordinate);
                            var gPointSymbol = new PointSymbol(gPt);
                            //substitute
                            pt.YCoordinate = yResult;
                            ss.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.Y, goal));
                            gPointSymbol.Traces.AddRange(goal.Traces);
                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                pt.Traces.Insert(0, goal.Traces[i]);
                            }
                            string rule        = SubstitutionRule.ApplySubstitute();
                            string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                            var ts = new TraceStep(this,gPointSymbol,rule, appliedRule);
                            gPointSymbol._innerLoop.Add(ts);
                            string strategy = "Reify a Point's y-coordinate by substituing a given fact.";
                            gPointSymbol.GenerateATrace(strategy);
                            //pt.Traces.Insert(0, ts);
                        }
                        else
                        {
                            //generate
                            var gPoint = new Point(pt.Label, pt.XCoordinate, obj);
                            var gPointSymbol = new PointSymbol(gPoint);
                            gPointSymbol.Traces.AddRange(goal.Traces);
                            gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.Y, goal));
                            foreach (KeyValuePair<object, EqGoal> pair in ss.CachedGoals)
                            {
                                if (pair.Key.Equals(PointAcronym.X))
                                {
                                    gPointSymbol.CachedGoals.Add(new KeyValuePair<object, EqGoal>(PointAcronym.X, pair.Value));
                                }
                            }
                            CachedSymbols.Add(gPointSymbol);
                            //substitute
                            //Add traces from pt to gPoint
                            for (int i = pt.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, pt.Traces[i]);
                            }
                            //transform goal trace
                            for (int i = goal.Traces.Count - 1; i >= 0; i--)
                            {
                                gPoint.Traces.Insert(0, goal.Traces[i]);
                            }
                            var rule = SubstitutionRule.ApplySubstitute();
                            var appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                            var ts = new TraceStep(this, gPointSymbol, rule, appliedRule);

                            gPointSymbol._innerLoop.Add(ts);
                            string strategy = "Reify a Point's y-coordinate by substituing a given fact.";
                            gPointSymbol.GenerateATrace(strategy);
                            //gPoint.Traces.Insert(0, ts);
                        }
                    }
                }
            }
        }
        
        #endregion

        #region Unreify Utilities

        /// <summary>
        /// for generated shapes
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="parent"></param>
        public override void UndoGoal(EqGoal goal, object p)
        {
            var parent = p as Point;
            if (parent == null) return;

            var current = Shape as Point;
            Debug.Assert(current != null);

            string field = null;
            foreach (KeyValuePair<object, EqGoal> eg in CachedGoals)
            {
                if (eg.Value.Equals(goal))
                {
                    field = eg.Key.ToString();
                }
            }

            if (PointAcronym.X.Equals(field))
            {
                current.XCoordinate = parent.XCoordinate;
            }
            else if (PointAcronym.Y.Equals(field))
            {
                current.YCoordinate = parent.YCoordinate;
            }

            this.RemoveGoal(goal);
        }

        #endregion
    }
}
