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

using System.Linq.Expressions;
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
        #region Search API

        public bool FindQuery(string query)
        {
            foreach (var node in Nodes)
            {
                var queryNode = node as QueryNode;
                if (queryNode == null) continue;
                var queryTag = queryNode.Query;

                if (queryTag.Constraint1 != null && queryTag.Constraint1.ToString().Equals(query))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<GraphNode> OutEdgeNodes(GraphNode node)
        {
            return node.OutEdges.Select(ge => ge.Target).ToList();
        }

        private List<GraphNode> InEdgeNodes(GraphNode node)
        {
            return node.InEdges.Select(ge => ge.Source).ToList();
        }

        public bool FoundCycleInGraph()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                GraphNode node1 = Nodes[i];
                for (int j = Nodes.Count - 1; j > i; j--)
                {
                    GraphNode node2 = Nodes[j];

                    foreach (var ge in node2.OutEdges)
                    {
                        var targetNode = ge.Target;
                        if (targetNode.Equals(node1)) return true;
                    }
                }
            }
            return false;
        }

        private GraphNode SearchNode(object obj)
        {
            var shape = obj as ShapeSymbol;
            if (shape != null)
            {
                foreach (var gn in _nodes)
                {
                    var shapeNode = gn as ShapeNode;
                    if (shapeNode != null)
                    {
                        if (shapeNode.ShapeSymbol.Equals(shape)) return gn;
                    }

                    var queryNode = gn as QueryNode;
                    if (queryNode != null)
                    {
                        object returnObj = queryNode.SearchInternalNode(obj);
                        if (returnObj != null) return (GraphNode)returnObj;
                    }
                }
            }
            var goal = obj as EqGoal;
            if (goal != null)
            {
                foreach (var gn in _nodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        if (goalNode.Goal.Equals(goal)) return gn;
                    }

                    var queryNode = gn as QueryNode;
                    if (queryNode != null)
                    {
                        object returnObj = queryNode.SearchInternalNode(obj);
                        if (returnObj != null) return (GraphNode)returnObj;
                    }
                }
            }

            var eq = obj as Equation;
            if (eq != null)
            {
                foreach (var en in _nodes)
                {
                    var eqNode = en as EquationNode;
                    if (eqNode != null)
                    {
                        if (eqNode.Equation != null 
                            && eqNode.Equation.Equals(eq)) 
                            return eqNode;
                    }
                }
            }



            var query = obj as Query;
            if (query != null)
            {
                foreach (var gn in _nodes)
                {
                    var qn = gn as QueryNode;
                    if (qn != null && qn.Query.QueryQuid.Equals(query.QueryQuid))
                        return qn;
                }
            }

            return null;
        }

        private List<GraphNode> SearchSynNodes(GraphNode gn)
        {
            var lst = new List<GraphNode>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                GraphNode node1 = Nodes[i];
                if(node1.Synthesized) lst.Add(node1);
            }
            
            //TODO, should delete all on-path syn-node
          /*  bool hasChange;
            do
            {
                hasChange = false;
                //safe delete upper synthesized node
                for (int i = 0; i < lst.Count; i++)
                {
                    GraphNode synNode = lst[i];
                    if (FindPath(gn, synNode)) continue;
                    hasChange = true;
                    lst.Remove(synNode);
                }
            } while (hasChange);*/

            return lst;
        }

        private bool FindPath(GraphNode source, GraphNode target)
        {
            /*if (source == null) return false;            
            if (source.Equals(target)) return true;
            if (source.OutEdges.Count == 0) return false;

            foreach (var edge in source.OutEdges)
            {
                var nextNode = edge.Target;
                if (nextNode.Equals(target)) return true;

                bool result = FindPath(nextNode, target);
                if (result) return true;
            }
            return false;*/
            return FindPath(source, source, target);
        }

        private bool FindPath(GraphNode source, GraphNode rootSource, GraphNode target)
        {
            if (source == null) return false;
            if (source.Equals(target)) return true;
            if (source.OutEdges.Count == 0) return false;

            foreach (var edge in source.OutEdges)
            {
                var nextNode = edge.Target;
                if (rootSource.Equals(nextNode)) return false;
                bool result = FindPath(nextNode, rootSource, target);
                if (result) return true;
            }
            return false;
        }

        private void SearchSynNodes(GraphNode currentNode, GraphNode rootNode, List<GraphNode> lst)
        {
            if (currentNode.OutEdges.Count == 0) return;

            foreach (var edge in currentNode.OutEdges)
            {
                var nextNode = edge.Target;
                if (nextNode.Equals(rootNode)) return;
                if (nextNode.Synthesized) lst.Add(nextNode);
                SearchSynNodes(nextNode, rootNode, lst);
            }
        }

        private bool BelongQueryNode(GraphNode currentNode, out QueryNode queryNode)
        {
            queryNode = null;
            var lst = RetrieveQueryNodes();
            foreach (QueryNode qn in lst)
            {
                if (qn.InternalNodes.Contains(currentNode))
                {
                    queryNode = qn;
                    return true;
                }
            }
            return false;
        }

        private GraphNode FindSinkNode(GraphNode gn)
        {
            return FindSinkNode(gn, gn);
        }

        private GraphNode FindSinkNode(GraphNode currentNode, GraphNode self)
        {
            if (currentNode == null) return null;
            if (currentNode.InEdges.Count == 0) return currentNode;

            foreach (var edge in currentNode.InEdges)
            {
                var prevNode = edge.Source;
                if (self.Equals(prevNode)) return null;
                GraphNode result = FindSinkNode(prevNode, self);
                if (result!= null) return result;
            }
            return null;
        }

        #endregion

        #region Search by uncertainty TODO

        public object RetrieveCacheValue(object obj)
        {
            return (from pair in Cache where pair.Key.Equals(obj) select pair.Value).FirstOrDefault();
        }

        private List<KeyValuePair<object, object>> RetrieveMultiObjectsPairs()
        {
            var lst = new List<KeyValuePair<object, object>>();
            foreach (KeyValuePair<object, object> pair in Cache)
            {
                //var str = pair.Key as String;
                var lstTemp = pair.Key as List<object>;
                //if (str != null || lstTemp != null)
                if (lstTemp != null)
                {
                    lst.Add(pair);
                }
            }
            return lst;
        }

        #endregion

        #region Output Test API

        public List<QueryNode> RetrieveQueryNodes()
        {
            return _nodes.OfType<QueryNode>().ToList();
        }

        public ShapeNode RetrieveShapeNode(ShapeSymbol ss)
        {
            foreach (var gn in Nodes)
            {
                var sn = gn as ShapeNode;
                if (sn == null) continue;
                if (sn.ShapeSymbol.Equals(ss)) return sn;
            }
            return null;
        }

        public QueryNode RetrieveQueryNode(Query query)
        {
            foreach (var gn in _nodes)
            {
                var qn = gn as QueryNode;
                if (qn != null && qn.Query.QueryQuid.Equals(query.QueryQuid))
                {
                    return qn;
                }
            }
            return null;
        }

        public IEnumerable<GoalNode> RetrieveGoalNodes()
        {
            var goalNodes = new List<GoalNode>();
            foreach (var gn in _nodes)
            {
                var goalNode = gn as GoalNode;
                if (goalNode != null) goalNodes.Add(goalNode);
                var queryNode = gn as QueryNode;
                if (queryNode != null) goalNodes.AddRange(queryNode.RetrieveGoalNodes());
            }
            return goalNodes;
        }

        public List<Goal> RetrieveGoals()
        {
            var lst = new List<Goal>();
            foreach (var gn in _nodes)
            {
                var goalNode = gn as GoalNode;
                if (goalNode != null)
                {
                    lst.Add(goalNode.Goal);
                }
            }
            return lst;
        }

        public List<ShapeNode> RetrieveShapeNodes()
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (var gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null) shapeNodes.Add(sn);
                var queryNode = gn as QueryNode;
                if (queryNode != null) shapeNodes.AddRange(queryNode.RetrieveShapeNodes());
            }
            return shapeNodes;
        }

        public List<ShapeNode> RetrieveShapeNodes(ShapeType type)
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (GraphNode gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null && sn.IsShapeType(type)) shapeNodes.Add(sn);
                var qn = gn as QueryNode;
                if (qn != null) shapeNodes.AddRange(qn.RetrieveShapeNodes(type));
            }
            return shapeNodes;
        }

        public List<ShapeSymbol> RetrieveShapeSymbols()
        {
            var lstNodes = RetrieveShapeNodes();
            return lstNodes.Select(sn => sn.ShapeSymbol).ToList();
        }

        public List<ShapeSymbol> RetrieveShapeSymbols(ShapeType type)
        {
            var lstNodes = RetrieveShapeNodes(type);
            return lstNodes.Select(sn => sn.ShapeSymbol).ToList();
        }

        public List<Shape> RetrieveShapes()
        {
            var lstSs = RetrieveShapeSymbols();
            return lstSs.Select(ss => ss.Shape).ToList();
        }

        public List<Shape> RetrieveShapes(ShapeType st)
        {
            var lstSs = RetrieveShapeSymbols(st);
            return lstSs.Select(ss => ss.Shape).ToList();
        }

        #endregion
    }
}
