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

    /// <summary>
    /// Symbolic Reification, bottom-up forward chaining
    /// </summary>
    public partial class RelationGraph
    {
        public void Reify(GraphNode gn, GraphNode rootNode = null)
        {
            var shapeNode = gn as ShapeNode;
            var goalNode = gn as GoalNode;
            var equationNode = gn as EquationNode;

            if (shapeNode != null)
            {
                if (rootNode != null)
                {
                    Reify(shapeNode, rootNode);
                }
                else
                {
                    Reify(shapeNode, shapeNode);
                }

            }
            else if(goalNode != null)
            {
                if (rootNode != null)
                {
                    Reify(goalNode, rootNode);
                }
                else
                {
                    Reify(goalNode, goalNode);
                }
            }
            else if (equationNode != null)
            {
                if (rootNode != null)
                {
                    Reify(equationNode, rootNode);
                }
                else
                {
                    Reify(equationNode, equationNode);
                }
            }
        }

        #region Equation Reify

        private void Reify(EquationNode eqNode, GraphNode rootNode)
        {
            foreach (GraphEdge ge in eqNode.OutEdges)
            {
                var upperNode = ge.Target;
                if (upperNode.Equals(rootNode)) return;
                if (upperNode.Synthesized) return;

                Reify(upperNode, rootNode); //depth first search
            }
        }

        private void UnReify(EquationNode eqNode)
        {
            //TODO
        }

        #endregion

        #region Reify ShapeNode

        private void Reify(ShapeNode shapeNode, GraphNode rootNode)
        {
            foreach (GraphEdge ge in shapeNode.OutEdges)
            {
                var upperNode = ge.Target;
                if (upperNode.Equals(rootNode)) return;
                if (upperNode.Synthesized) return;

                #region Target Node Analysis
                var upperShapeNode = upperNode as ShapeNode;
                if (upperShapeNode != null)
                {
                    var lstSource = InEdgeNodes(upperShapeNode);
                    if (lstSource.Count == 2)
                    {
                        Reify(upperShapeNode, lstSource[0], lstSource[1]);
                    }
                }
                #endregion
                
                Reify(upperNode, rootNode); //depth first search
            }
        }

        private void UnReify(ShapeNode shapeNode)
        {
            foreach (GraphEdge ge in shapeNode.OutEdges)
            {
                var upperNode = ge.Target as ShapeNode;
                if (upperNode == null) continue;
                var lstSource = InEdgeNodes(upperNode);
                if (lstSource.Count != 2) continue;
                if (Reify(upperNode, lstSource[0], lstSource[1]))
                {
                    UnReify(upperNode); //depth first search
                }
            }
        }

        #endregion

        private bool Reify(ShapeNode currentShapeNode, GraphNode sourceNode1, GraphNode sourceNode2)
        {
            var sn1 = sourceNode1 as ShapeNode;
            var sn2 = sourceNode2 as ShapeNode;

            if (sn1 != null && sn2 != null)
            {
                var shapeSymbol1 = sn1.ShapeSymbol;
                var shapeSymbol2 = sn2.ShapeSymbol;
                var currentShape = currentShapeNode.ShapeSymbol;
                return RelationLogic.Reify(currentShape, shapeSymbol1, shapeSymbol2);
            }
            return false;
        }

        #region Reify GoalNode

        private void Reify(GoalNode goalNode, GraphNode rootNode)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            if (!eqGoal.Concrete) return;

            foreach (GraphEdge ge in goalNode.OutEdges)
            {
                var upperNode = ge.Target;
                if (upperNode.Equals(rootNode)) return;
                if (upperNode.Synthesized) return;

                #region Target Node Analysis

                var shapeNode = upperNode as ShapeNode;
                if (shapeNode != null)
                {
                    if (shapeNode.ShapeSymbol.Shape.Concrete) continue;
/*                    if (DispatchReify(shapeNode.ShapeSymbol, eqGoal))
                    {
                        Reify(shapeNode); //depth first search
                    }                    */
                    DispatchReify(shapeNode.ShapeSymbol, eqGoal);

                }
                var eqNode = upperNode as EquationNode;
                if (eqNode != null)
                {
                    DispatchReify(eqNode.Equation, eqGoal);
/*                    if (DispatchReify(eqNode.Equation, eqGoal))
                    {
                        Reify(eqNode);
                    }*/
                }

                #endregion

                Reify(upperNode, rootNode); //depth first search
            }
        }

        private void UnReify(GoalNode goalNode)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            if (!eqGoal.Concrete) return;

            for (int i = 0; i < goalNode.OutEdges.Count; i++)
            {
                GraphEdge outEdge = goalNode.OutEdges[i];
                if (outEdge.Target.Equals(goalNode)) return; // cyclic halt
                var shapeNode = outEdge.Target as ShapeNode;
                if (shapeNode != null)
                {
                    if (DispatchUnreify(shapeNode.ShapeSymbol, eqGoal))
                    {
                        UnReify(shapeNode); //dfs
                    }
                }
                var eqNode = outEdge.Target as EquationNode;
                if (eqNode != null)
                {
                    if (DispatchUnreify(eqNode.Equation, eqGoal))
                    {
                        UnReify(eqNode);//dfs
                    }
                }
            }
        }

        #endregion

        #region Reify Utils

        private bool DispatchReify(Equation eq, EqGoal goal)
        {
            if (eq == null) return false;
            return eq.Reify(goal);
        }

        private bool DispatchUnreify(Equation eq, EqGoal goal)
        {
            if (eq == null) return false;
            return eq.UnReify(goal);
        }

        private bool DispatchReify(ShapeSymbol ss, EqGoal goal)
        {
            var ps = ss as PointSymbol;
            var ls = ss as LineSymbol;
            if (ps != null)
            {
                return ps.Reify(goal);
            }
            if (ls != null)
            {
                return ls.Reify(goal);
            }

            return false;
        }

        private bool DispatchUnreify(ShapeSymbol ss, EqGoal goal)
        {
            var ps = ss as PointSymbol;
            var ls = ss as LineSymbol;
            if (ps != null)
            {
                return ps.UnReify(goal);
            }
            if (ls != null)
            {
                return ls.UnReify(goal);
            }
            return false;
        }

        #endregion
    }
}