using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSharpLogic;
using NUnit.Framework.Constraints;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        public static bool ConstraintCheck(GraphNode obj1, object constraint1, object constraint2, out object output)
        {
            if (constraint1 == null) return ConstraintCheck(obj1, constraint2, out output);
            if (constraint2 == null) return ConstraintCheck(obj1, constraint1, out output);
          
            var shapeType = constraint2 as ShapeType?;
            Debug.Assert(constraint1 != null);
            Debug.Assert(shapeType != null);

            output = null;
            var shape1 = obj1 as ShapeNode;
            var goal1 = obj1 as GoalNode;
            var query1 = obj1 as QueryNode;

            //Combinatoric Pattern Match
            if (shape1 != null) return ConstraintCheck(shape1, constraint1, constraint2, out output);
            if (goal1 != null)  return ConstraintCheck(goal1, constraint1, constraint2, out output);
            if (query1 != null) return ConstraintCheck(query1, constraint1, constraint2, out output);
            //TODO other relations
            return false;
        }

        private static bool ConstraintCheck(ShapeNode sn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(GoalNode gn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(QueryNode gn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(GraphNode gn, object constraint, out object output)
        {
            var shapeNode = gn as ShapeNode;
            var goalNode  = gn as GoalNode;
            var queryNode = gn as QueryNode;
            var eqNode = gn as EquationNode;
            if (shapeNode != null) return ConstraintCheck(shapeNode, constraint, out output);
            if (goalNode != null) return ConstraintCheck(goalNode, constraint, out output);
            if (queryNode != null) return ConstraintCheck(queryNode, constraint, out output);
            if (eqNode != null) return ConstraintCheck(eqNode, constraint, out output);
            throw new Exception("Graph.Unify.UnaryConstraint.cs: Cannot reach here");
        }

        /// <summary>
        /// e.g given  y = 2x + 1, ask: m = ?
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="constraint"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool ConstraintCheck(ShapeNode sn, object constraint, out object output)
        {
            output = null;
            var label = constraint as string;
            if (label != null) //Query
            {
                return sn.ShapeSymbol.UnifyProperty(label, out output); 
            }
            var goal = constraint as EqGoal;
            if (goal != null) //Match
            {
                bool matched = sn.ShapeSymbol.UnifyExplicitProperty(goal);
                if (matched)
                {
                    output = new Tuple<object, object>(goal,sn); 
                    return true;
                }
            }

            return false;
        }

        private static bool ConstraintCheck(GoalNode gn, object constraint, out object output)
        {
            output = null;
            var label = constraint as string;

            var eqGoal = gn.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            if (label != null && gn.Satisfy(label))
            {
                output = gn.Goal;
                //output = new EqGoal(new Var(label), eqGoal.Rhs);
                return true;
            }

            var goal = constraint as EqGoal;
            if (goal != null)
            {
                return false;
            }

            var eq = constraint as Equation;
            if (eq != null && gn.Satisfy(eq))
            {
                var lst = new List<Tuple<object, object>>();
                eq.Reify(eqGoal);
                for (int i = 0; i < eq.CachedEntities.Count; i++)
                {
                    var cachedEq = eq.CachedEntities.ToList()[i] as Equation;
                    if (cachedEq == null) continue;

                    object obj1;
                    bool tempResult = cachedEq.IsEqGoal(out obj1);
                    if (tempResult)
                    {
                        var gGoal = obj1 as EqGoal;
                        Debug.Assert(gGoal != null);
                        var newTraces = new List<Tuple<object, object>>();
                        newTraces.AddRange(eqGoal.Traces);
                        for (int j = 0; j < cachedEq.Traces.Count - 1; j++)
                        {
                            newTraces.Add(cachedEq.Traces[j]);
                        }
                        var trace = cachedEq.Traces[cachedEq.Traces.Count - 1];
                        string strategy = "Derive new property by manipulating the given algebraic equation.";
                        var newTrace = new Tuple<object, object>(strategy, trace.Item2);
                        newTraces.Add(newTrace);
                        gGoal.Traces = newTraces;
                        lst.Add(new Tuple<object, object>(eq, gGoal));
                    }
                }
                lst.Add(new Tuple<object, object>(gn, eq));
                output = lst;
                return true;
            }

            var variable = constraint as Var;
            if (variable != null && gn.Satisfy(variable))
            {
                //output = new EqGoal(variable, eqGoal.Rhs);
                output = gn.Goal;
                return true;
            }
            return false;
        }

        private static bool ConstraintCheck(EquationNode en, object constraint, out object output)
        {
            output = null;
            var eqGoal = constraint as EqGoal;
            if (eqGoal == null) return false;
            var variable = eqGoal.Lhs as Var;
            if (variable != null && en.Satisfy(variable))
            {
                var lst = new List<Tuple<object, object>>();
                

                var equation = en.Equation;
                Debug.Assert(equation != null);

                equation.Reify(eqGoal);
                for (int i = 0; i < equation.CachedEntities.Count; i++)
                {
                    var cachedEq = equation.CachedEntities.ToList()[i] as Equation;
                    if (cachedEq == null) continue;

                    object obj1;
                    bool tempResult = cachedEq.IsEqGoal(out obj1);
                    if (tempResult)
                    {
                        var gGoal = obj1 as EqGoal;
                        Debug.Assert(gGoal != null);
                        lst.Add(new Tuple<object, object>(en, gGoal));
                    }
                }

                lst.Add(new Tuple<object, object>(constraint, en));
                //output = en.Equation;
                output = lst;
                return true;
            }
            return false;
        }

        private static bool ConstraintCheck(QueryNode gn, object constraint, out object output)
        {
            output = null;
            var st = constraint as ShapeType?;
            if (st != null)
            {
                //TODO
                throw new Exception("TODO");
            }

            var label = constraint as string;
            if (label != null)
            {
                #region Internal Analysis

                var nodes = gn.InternalNodes;
                //TODO
                foreach (var node in nodes)
                {
                    var goalNode = node as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = ConstraintCheck(goalNode, label, out output);
                        if (result) goalNode.Related = true;
                        return result;
                    }
                    var shapeNode = node as ShapeNode;
                    if (shapeNode != null)
                    {
                        bool result = ConstraintCheck(shapeNode, label, out output);
                        if (result) shapeNode.Related = true;
                        return result;
                    }
                }

                #endregion

                return false;
            }

            var eqGoal = constraint as EqGoal;
            if (eqGoal != null)
            {
                var nodes = gn.InternalNodes;
                foreach (var node in nodes)
                {
                    var goalNode = node as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = ConstraintCheck(goalNode, eqGoal, out output);
                        if (result)
                        {
                            goalNode.Related = true;
                            return true;
                        }
                    }
                }
                return false;
            }

            var equation = constraint as Equation;
            if (equation != null)
            {
                #region Internal Nodes Eval
                var nodes = gn.InternalNodes;
                foreach (var node in nodes)
                {
                    var goalNode = node as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = ConstraintCheck(goalNode, equation, out output);
                        if (result)
                        {
                            goalNode.Related = true;
                        }
                    }
                }
                #endregion
                return output != null;
            }
            return false;
        }
    }
}
