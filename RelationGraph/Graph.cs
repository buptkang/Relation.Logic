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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CSharpLogic;

    public partial class RelationGraph
    {
        #region Constructor and Properties

        private List<GraphNode> _nodes;
        public List<GraphNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }

        public RelationGraph()
        {
            _nodes = new List<GraphNode>();
            _preCache = new Dictionary<object, object>();
        }

        /// <summary>
        /// Key: Pattern Match Result
        /// Value: Predict Pattern Match Result (GraphNode)
        /// </summary>
        private Dictionary<object, object> _preCache;
        public Dictionary<object, object> Cache
        {
            get { return _preCache; }
        }

        #endregion

        #region public Input API

        public GraphNode AddNode(object obj)
        {
            var shape = obj as ShapeSymbol;
            var goal = obj as Goal;
            var query = obj as Query;
            var equation = obj as Equation;

            if (shape != null)
            {
                var shapeNode = AddShapeNode(shape);
                _preCache.Add(shape, shapeNode);
                ReAddQueryNode();
                return shapeNode;
            }
            if (goal != null)
            {
                var goalNode = AddGoalNode(goal);
                _preCache.Add(goal, goalNode);
                ReAddQueryNode();
                return goalNode;
            }
            if (query != null)
            {
                query.Success = false;
                query.FeedBack = null;
                query.CachedEntities.Clear();
                RemoveQueryFromCache(query);
                query.PropertyChanged += query_PropertyChanged;
                bool result = AddQueryNode(query, out obj);
                _preCache.Add(query.QueryQuid, new Tuple<Query, object>(query, obj));
                if (!result) return null;
                var qn = obj as QueryNode;
                Debug.Assert(qn != null);
                return qn;
            }
            if (equation != null)
            {
                var eqNode = AddEquationNode(equation);
                _preCache.Add(equation, eqNode);
                ReAddQueryNode();
                return eqNode;
            }
            throw new Exception("Cannot reach here");
        }

        public bool DeleteNode(object obj)
        {
            bool result = false;
            var shape = obj as ShapeSymbol;
            var goal = obj as EqGoal;
            var query = obj as Query;
            var equation = obj as Equation;
            if (shape != null) result = DeleteShapeNode(shape);
            if (goal != null) result = DeleteGoalNode(goal);
            if (query != null) result = DeleteQueryNode(query);
            if (equation != null) result = DeleteEqNode(equation);
            ReAddQueryNode();
            return result;
        }

        private void AddNode(GraphNode node)
        {
            var sn = node as ShapeNode;
            var gn = node as GoalNode;
            var qn = node as QueryNode;
            var en = node as EquationNode;
            if (sn != null)
            {
                AddNode(sn.ShapeSymbol);
                return;
            }
            if (gn != null)
            {
                AddNode(gn.Goal);
                return;
            }
            if (qn != null)
            {
                AddNode(qn.Query);
                return;
            }
            if (en != null) AddNode(en.Equation);
        }

        private void DeleteNode(GraphNode node)
        {
            var shapeNode = node as ShapeNode;
            var goalNode  = node as GoalNode;

            if (shapeNode != null) DeleteShapeNode(shapeNode.ShapeSymbol);
            if (goalNode  != null) DeleteGoalNode(goalNode.Goal);
        }

        #region Equation Node CRUD
        private EquationNode AddEquationNode(Equation equation)
        {
            //1. build relation using geometry constraint-solving (synthesize new node) (Top-Down)
            //2. Reify itself (Bottom-up)
            object relations;
            ConstraintSatisfy(equation, out relations);
            EquationNode eqNode = CreateEqNode(equation, relations);

/*            GraphNode sinkNode = FindSinkNode(eqNode);
            Reify(sinkNode);*/
            
            Reify(eqNode);

/*            foreach (object obj in goalRels)
            {
                var goalNode = obj as GoalNode;
                if (goalNode == null) continue;
                Reify(goalNode);
                //Reify(eqNode);
            }*/
            return eqNode;
        }

        private bool DeleteEqNode(Equation equation)
        {
            var eqNode = SearchNode(equation) as EquationNode;
            if (eqNode == null) return false;
            _nodes.Remove(eqNode);
            UpdateUpperQueryNode(eqNode);
            UnReify(eqNode);
            DeleteSynNode(eqNode);
            UnBuildRelation(eqNode);
            _preCache.Remove(equation);
            return true;
        }
       
        #endregion

        #region Query Node CRUD

        private bool AddQueryNode(Query query, out object obj)
        {
            //1. build relation using geometry constraint-solving
            //2. Reification

            ResetNodeRelatedFlag();
            bool result = ConstraintSatisfy(query, out obj);

            if (result)
            {
                obj = CreateQueryNode(query, obj);
                query.Success = true;
            }
            else
            {
                if (obj != null)
                {
                    query.Success = false;
                    query.FeedBack = obj;
                }
            }

            if (!result) return false;
            var qn = obj as QueryNode;
            _nodes.Add(qn);
            return true;
        }

        private bool DeleteQueryNode(Query query)
        {
            RemoveQueryFromCache(query);
            query.PropertyChanged -= query_PropertyChanged;
            var queryNode = SearchNode(query) as QueryNode;
            if (queryNode == null) return false;

            foreach (GraphNode gn in queryNode.InternalNodes)
            {
                foreach (GraphEdge inGe in gn.InEdges)
                {
                    var sourceNode = inGe.Source;
                    sourceNode.OutEdges.Remove(inGe);
                }
                foreach (GraphEdge outGe in gn.OutEdges)
                {
                    var targetNode = outGe.Target;
                    targetNode.InEdges.Remove(outGe);
                }
            }
            query.CachedEntities.Clear();
            _nodes.Remove(queryNode);
            return true;
        }

        private void UpdateUpperQueryNode(GraphNode gn)
        {
            for (int i = 0; i < gn.OutEdges.Count; i++)
            {
                var targetNode = gn.OutEdges[i].Target;
                QueryNode tempQueryNode;
                if (BelongQueryNode(targetNode, out tempQueryNode))
                {
                    DeleteNode(tempQueryNode.Query);
                    AddNode(tempQueryNode.Query);
                }
            }
        }
        #endregion

        #region Shape Node CRUD

        private ShapeNode AddShapeNode(ShapeSymbol shape)
        {
            //1. build relation using geometry constraint-solving (synthesize new node) (Top-Down)
            object relations;
            ConstraintSatisfy(shape, out relations);
            List<object> goalRels;
            ShapeNode shapeNode = CreateShapeNode(shape, relations, out goalRels);
            //Search bottom up goalNodes, start reify
            foreach (object obj in goalRels)
            {
                var goalNode = obj as GoalNode;
                if (goalNode == null) continue;
                Reify(goalNode);
            }
            return shapeNode;
        }

        private bool DeleteShapeNode(ShapeSymbol shape)
        {
            var shapeNode = SearchNode(shape) as ShapeNode;
            if (shapeNode == null) return false;
            _nodes.Remove(shapeNode);
            UpdateUpperQueryNode(shapeNode);
            UnReify(shapeNode);
            DeleteSynNode(shapeNode);
            UnBuildRelation(shapeNode);
            _preCache.Remove(shape);
            return true;
        }

        public bool RelationExist(ShapeSymbol shape)
        {
            object relations;
            ConstraintSatisfy(shape, out relations);
            var lst = relations as List<Tuple<object, object>>;
            Debug.Assert(lst != null);
            return lst.Count != 0;
        }

        #endregion

        #region EqGoal Node CRUD

        private GoalNode AddGoalNode(Goal goal)
        {
            //1. build relation using geometry constraint-solving (synthesize new node) (Top-Down)
            //2. Reify itself (Bottom-up)
            var eqGoal = goal as EqGoal;
            Debug.Assert(eqGoal != null);
            Debug.Assert(eqGoal.Lhs != null);
            Debug.Assert(eqGoal.Rhs != null);
            var lhs = eqGoal.Lhs.ToString();
            Debug.Assert(lhs != null);

            object relations;
            ConstraintSatisfy(eqGoal, out relations);
            GoalNode gGoalNode = CreateGoalNode(eqGoal, relations);
            Reify(gGoalNode);
            return gGoalNode;
        }

        private bool DeleteGoalNode(Goal goal)
        {
            var goalNode = SearchNode(goal) as GoalNode;
            if (goalNode == null) return false;
            _nodes.Remove(goalNode);
            UnReify(goalNode);
            DeleteSynNode(goalNode);
            UnBuildRelation(goalNode);
            _preCache.Remove(goal);
            return true;
        }

        private void DeleteSynNode(GraphNode gn)
        {
            //find all related syn node
            var synLst = SearchSynNodes(gn);

            foreach (GraphNode tempGn in synLst)
            {
                DeleteNode(tempGn);
            }
        }

        public bool RelationExist(EqGoal goal)
        {
            object relations;
            ConstraintSatisfy(goal, out relations);
            var lst = relations as List<Tuple<object, object>>;
            Debug.Assert(lst != null);
            return lst.Count != 0;
        }

        #endregion

        #endregion

        #region Query Usage

        private void ResetNodeRelatedFlag()
        {
            foreach (var gn in _nodes)
            {
                gn.Related = false;
            }
        }

        private void RemoveQueryFromCache(Query query)
        {
            object keyObj = null;
            foreach (KeyValuePair<object, object> pair in _preCache)
            {
                if (pair.Key is Guid)
                {
                    Debug.Assert(pair.Key != null);
                    var tempGuid = (Guid)pair.Key;
                    if (tempGuid.CompareTo(query.QueryQuid) == 0)
                    {
                        keyObj = pair.Key;
                        break;
                    }
                }
            }
            if (keyObj != null) _preCache.Remove(query.QueryQuid);
        }
        
        void query_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var query = sender as Query;
            DeleteNode(query);
            AddNode(query);
        }

        private void ReAddQueryNode()
        {
            foreach (KeyValuePair<object, object> pair in _preCache.ToList())
            {
                if (pair.Key is Guid)
                {
                    var tuple2 = pair.Value as Tuple<Query, object>;
                    Debug.Assert(tuple2 != null);
                    var query = tuple2.Item1;
                    Debug.Assert(query != null);

/*                    if (tuple2.Item2 == null)
                    {*/
                        DeleteNode(query);
                        AddNode(query);                        
                    //}
                }
            }
        }

        #endregion
    }
}