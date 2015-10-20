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

using System.Security.AccessControl;
using NUnit.Framework;

namespace AlgebraGeometry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CSharpLogic;

    public partial class RelationGraph
    {
        private QueryNode CreateQueryNode(Query query, object obj)
        {
            var queryNode = new QueryNode(query);

            var dict = obj as Dictionary<object, object>;
            if (dict == null) return queryNode;

            #region Unary and Binary Relation

            foreach (KeyValuePair<object, object> pair in dict)
            {
                var unaryNode = pair.Key as GraphNode;
                var binaryNode = pair.Key as Tuple<GraphNode, GraphNode>;
                var uunaryNode = pair.Key as List<GraphNode>;

                if (unaryNode != null)
                {
                    #region Unary Node
                    var eqGoal = pair.Value as EqGoal;
                    var shapeSymbol = pair.Value as ShapeSymbol;
                    if (eqGoal != null)
                    {
                        var gGoalNode = new GoalNode(eqGoal);
                        queryNode.InternalNodes.Add(gGoalNode);
                        CreateEdge(unaryNode, gGoalNode);
                        continue;
                    }
                    if (shapeSymbol != null)
                    {
                        var gShapeNode = new ShapeNode(shapeSymbol);
                        queryNode.InternalNodes.Add(gShapeNode);
                        var sourceNode = pair.Key;
                        Debug.Assert(sourceNode != null);
                        CreateEdge(unaryNode, gShapeNode);
                        continue;
                    }
                    #endregion
                }

                if (binaryNode != null)
                {
                    #region Binary Node
                    var gShape = pair.Value as ShapeSymbol;
                    if (gShape != null)
                    {
                        var shapeNode = new ShapeNode(gShape);
                        queryNode.InternalNodes.Add(shapeNode);
                        var sourceNode1 = binaryNode.Item1;
                        var sourceNode2 = binaryNode.Item2;
                        Debug.Assert(sourceNode1 != null);
                        Debug.Assert(sourceNode2 != null);
                        CreateEdge(sourceNode1, shapeNode);
                        CreateEdge(sourceNode2, shapeNode);
                    }

                    var gGoal = pair.Value as EqGoal;
                    if (gGoal != null)
                    {
                        var goalNode = new GoalNode(gGoal);
                        queryNode.InternalNodes.Add(goalNode);
                        var sourceNode1 = binaryNode.Item1;
                        var sourceNode2 = binaryNode.Item2;
                        Debug.Assert(sourceNode1 != null);
                        Debug.Assert(sourceNode2 != null);
                        CreateEdge(sourceNode1, goalNode);
                        CreateEdge(sourceNode2, goalNode);
                    }

                    #endregion
                }

                var findNode = SearchNode(pair.Key);
                if (findNode != null)
                {
                    #region Find Node
                    var eqGoal = pair.Value as EqGoal;
                    var shapeSymbol = pair.Value as ShapeSymbol;
                    if (eqGoal != null)
                    {
                        var gGoalNode = new GoalNode(eqGoal);
                        queryNode.InternalNodes.Add(gGoalNode);
                        CreateEdge(findNode, gGoalNode);
                        continue;
                    }
                    if (shapeSymbol != null)
                    {
                        var gShapeNode = new ShapeNode(shapeSymbol);
                        queryNode.InternalNodes.Add(gShapeNode);
                        var sourceNode = pair.Key;
                        Debug.Assert(sourceNode != null);
                        CreateEdge(findNode, gShapeNode);
                        continue;
                    }
                    #endregion
                }

                var findNodeInCurrentqQuery = queryNode.SearchInternalNode(pair.Key);
                if (findNodeInCurrentqQuery != null)
                {
                    #region Find Node
                    var eqGoal = pair.Value as EqGoal;
                    var shapeSymbol = pair.Value as ShapeSymbol;
                    if (eqGoal != null)
                    {
                        var gGoalNode = new GoalNode(eqGoal);
                        queryNode.InternalNodes.Add(gGoalNode);
                        CreateEdge(findNodeInCurrentqQuery, gGoalNode);
                        continue;
                    }
                    if (shapeSymbol != null)
                    {
                        var gShapeNode = new ShapeNode(shapeSymbol);
                        queryNode.InternalNodes.Add(gShapeNode);
                        var sourceNode = pair.Key;
                        Debug.Assert(sourceNode != null);
                        CreateEdge(findNodeInCurrentqQuery, gShapeNode);
                        continue;
                    }
                    #endregion
                }

                if (uunaryNode != null)
                {
                    var equation = pair.Value as Equation;
                    var eqNode = new EquationNode(equation);
                    queryNode.InternalNodes.Add(eqNode);
                    foreach (GraphNode gn in uunaryNode)
                    {
                        CreateEdge(gn, eqNode);
                    }
                }
            }

            #endregion

            return queryNode;
        }

        private QueryNode CreateQueryNode(EqGoal goal)
        {
            var query = new Query(goal.Lhs);
            var queryNode = new QueryNode(query);

            var iGoalNode = new GoalNode(goal);
            queryNode.InternalNodes.Add(iGoalNode);
            return queryNode;
        }

        private QueryNode CreateQueryNode(List<EqGoal> goals)
        {
            var query = new Query(goals[0].Lhs);
            var queryNode = new QueryNode(query);
            foreach (EqGoal igoal in goals)
            {
                var iGoalNode = new GoalNode(igoal);
                queryNode.InternalNodes.Add(iGoalNode);
            }
            return queryNode;
        }

        private GoalNode CreateGoalNode(EqGoal goal, object obj)
        {
            var gn = new GoalNode(goal);

            var lst = obj as List<Tuple<object, object>>;
            if (lst == null) return gn;
            foreach (var tuple in lst)
            {
                if (tuple.Item1.Equals(goal))
                {
                    BuildRelation(gn, tuple.Item2);
                }
                else if (tuple.Item2.Equals(goal))
                {
                    BuildRelation(tuple.Item1, gn);
                }
                else
                {
                   /* var gn1 = tuple.Item1 as GraphNode;
                    if (gn1 != null)
                    {
                        BuildRelation(gn1, tuple.Item2);                        
                    }
                    var gn2 = tuple.Item2 as GraphNode;
                    if (gn2 != null)
                    {
                        BuildRelation(tuple.Item1, gn2);
                    }*/
                }
            }

            _nodes.Add(gn);
            return gn;
        }

        private ShapeNode CreateShapeNode(ShapeSymbol ss, object obj, out List<object> relGoals)
        {
            var gn = new ShapeNode(ss);
            relGoals = new List<object>();
            var lst = obj as List<Tuple<object, object>>;
            if (lst == null) return gn;

            foreach (var tuple in lst)
            {
                if (tuple.Item1.Equals(ss))
                {
                    BuildRelation(gn, tuple.Item2);
                    relGoals.Add(tuple.Item2);
                }

                if (tuple.Item2.Equals(ss))
                {
                    BuildRelation(tuple.Item1, gn);
                    relGoals.Add(tuple.Item1);
                }
            }

            _nodes.Add(gn);
            return gn;
        }

        private EquationNode CreateEqNode(Equation eq, object obj)
        {
            var eqNode = new EquationNode(eq);

            var lst = obj as List<Tuple<object, object>>;
            if (lst == null) return eqNode;

            foreach (var tuple in lst)
            {
                if (tuple.Item1.Equals(eq))
                {
                    BuildRelation(eqNode, tuple.Item2);
                }
                
                if (tuple.Item2.Equals(eq))
                {
                    BuildRelation(tuple.Item1, eqNode);
                }
            }
            _nodes.Add(eqNode);
            return eqNode;
        }

        #region GoalNode Relations

        private void BuildRelation(GraphNode goalNode, object obj)
        {
            var unaryNode  = obj as GraphNode;
            var binaryNode = obj as Tuple<GraphNode, GraphNode>;

            var eqGoal = obj as EqGoal;  //new node synthesize

            if (unaryNode != null)
            {
                CreateEdge(goalNode, unaryNode);
            }
            if (binaryNode != null)
            {
                var node1 = binaryNode.Item1;
                var node2 = binaryNode.Item2;
                CreateEdge(goalNode, node1);
                CreateEdge(goalNode, node2);
            }
            if (eqGoal != null)
            {
                GoalNode synNode = AddGoalNode(eqGoal);
                synNode.Synthesized = true;
                CreateEdge(goalNode, synNode);
            }
        }

        private void BuildRelation(object obj, GraphNode goalNode)
        {
            var unaryNode = obj as GraphNode;
            var binaryNode = obj as Tuple<object, object>;
            var eqGoal = obj as EqGoal;  //new node synthesize

            if (unaryNode != null)
            {
                CreateEdge(unaryNode,goalNode);
            }

            if (binaryNode != null)
            {
                var node1 = binaryNode.Item1 as GraphNode;
                var node2 = binaryNode.Item2 as GraphNode;
                CreateEdge(node1, goalNode);
                CreateEdge(node2, goalNode);
            }

            if (eqGoal != null)
            {
                GoalNode synNode = AddGoalNode(eqGoal);
                synNode.Synthesized = true;
                CreateEdge(synNode, goalNode);
            }
        }

        private void UnBuildRelation(GraphNode graphNode)
        {
            for (int i = 0; i < graphNode.OutEdges.Count; i++)
            {
                GraphEdge outEdge = graphNode.OutEdges[i];
                GraphNode targetNode = outEdge.Target;
                foreach (var edge in targetNode.InEdges)
                {
                    if (edge.Source.Equals(graphNode)) continue;
                    var binaryAltNode = edge.Source;
                    binaryAltNode.OutEdges.Remove(edge);
                }
                targetNode.InEdges.Clear();
            }
            for (int j = 0; j < graphNode.InEdges.Count; j++)
            {
                var inEdge = graphNode.InEdges[j];
                var sourceNode = inEdge.Source;
                sourceNode.OutEdges.Remove(inEdge);
            }
        }

        #endregion

        #region Edge Utilities

        private void CreateEdge(GraphNode source, GraphNode target)
        {
            if (source == null || target == null) return;
            var graphEdge = new GraphEdge(source, target);
            source.OutEdges.Add(graphEdge);
            target.InEdges.Add(graphEdge);
        }

        #endregion
    }
}
