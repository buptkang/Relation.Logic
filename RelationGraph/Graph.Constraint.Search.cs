using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Pattern Match String (Unification)
    /// Graph Search -- Constraint Solving 
    /// Goal oriented Search
    /// </summary>
    public partial class RelationGraph
    {
        // Lazy evaluation of relation based on the existing nodes,
        /// if the relation exists, build the relation.
        public bool ConstraintSatisfy(Query query, out object obj)
        {
            obj = null;
            object refObj = query.Constraint1;
            ShapeType? st = query.Constraint2;
            if (refObj == null && st == null) throw new Exception("TODO: brute search to create all relations.");
            bool result = false;
            if (refObj == null) result = ConstraintSatisfy(null, st, out obj);
            else
            {
                var term = refObj as Term;
                if (term != null)
                {
                    result = ConstraintSatisfy(term, out obj);
                }
                else
                {
                    result = ConstraintSatisfy(refObj.ToString(), st, out obj);
                }
            }
            return result;
        }

        /// <summary>
        /// a+1=, 2+3+5=
        /// </summary>
        /// <param name="term"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ConstraintSatisfy(Term term, out object obj)
        {
            var evalObj = term.Eval();
            var generatedEq = new Equation(term, evalObj);
            generatedEq.ImportTrace(term);

            var evalTerm = evalObj as Term;
            var evalVar = evalObj as Var;

            var dict = new Dictionary<object, object>();
            var connectLst = new List<GraphNode>();
            if (evalTerm != null)
            {
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var goalNode = _nodes[i] as GoalNode;
                    if (goalNode != null)
                    {
                        if (evalTerm.ContainsVar((EqGoal)goalNode.Goal))
                        {
                            connectLst.Add(goalNode);
                        }
                    }
                }
            }
            if (evalVar != null)
            {
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var goalNode = _nodes[i] as GoalNode;
                    if (goalNode != null)
                    {
                        var eqGoal = goalNode.Goal as EqGoal;
                        Debug.Assert(eqGoal != null);
                        var lhsVar = eqGoal.Lhs as Var;
                        Debug.Assert(lhsVar != null);
                        if (lhsVar.Equals(evalVar))
                        {
                            connectLst.Add(goalNode);
                        }
                    }
                }
            }
            dict.Add(connectLst, generatedEq);
            obj = dict;
            return true;
        }

        /// <summary>
        /// This func takes charge of pattern match string with 
        /// existing nodes
        /// Two constraint
        /// Priority: label constraint > shapeType constraint
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="label"></param>
        /// <param name="st"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ConstraintSatisfy(string label, ShapeType? st, out object obj)
        {
            obj = null;
            var graphNodes = _nodes;

            var dict = new Dictionary<object, object>();

            #region Unary Relation Checking

            for (var i = 0; i < graphNodes.Count; i++)
            {
                var tempNode = graphNodes[i];
                bool result = RelationLogic.ConstraintCheck(tempNode, label, st, out obj);
                if (result)
                {
                    var queryNode = tempNode as QueryNode;
                    if (queryNode == null) dict.Add(tempNode, obj);
                    else
                    {
                        foreach (var tempGn in queryNode.InternalNodes)
                        {
                            if (!tempGn.Related) continue;
                            dict.Add(tempGn, obj);
                        }
                    }
                }
            }

            if (dict.Count != 0)
            {
                //Conflict resolution
                ConflictResolve(dict);
            }

            #endregion

            #region Binary Relation Checking
            //Two layer for loop to iterate all Nodes
            //binary relation build up

            // overfitting: 
            // underfitting: 
            for (var i = 0; i < graphNodes.Count - 1; i++)
            {
                var outerNode = graphNodes[i];
                for (var j = i + 1; j < graphNodes.Count; j++)
                {
                    var innerNode = graphNodes[j];
                    var tuple = new Tuple<GraphNode, GraphNode>(outerNode, innerNode);
                    bool result = RelationLogic.ConstraintCheck(outerNode, innerNode, label, st, out obj);

                    if (result)
                    {
                        var lst = obj as List<object>;
                        if (lst != null)
                        {
                            object source, target;
                            source = tuple;
                            foreach (var tempObj in lst)
                            {
                                target = tempObj;
                                dict.Add(source, target);
                                source = target;
                            }
                        }
                        else
                        {
                            dict.Add(tuple, obj);
                        }
                    }
                    else
                    {
                        if (obj != null) return false;
                    }
                }
            }
            #endregion

            //TODO analysis
            if (dict.Count != 0)
            {
                //Conflict resolution
                //ConflictResolve(dict);
                ConflictResolve2(dict);
                obj = dict;
                return true;
            }
            return false;
        }

        public bool ConstraintSatisfy(EqGoal goal, out object obj)
        {
            var graphNodes = _nodes;

            var lst = new List<Tuple<object, object>>();

            #region Unary Relation Checking

            for (var i = 0; i < graphNodes.Count; i++)
            {
                var tempNode = graphNodes[i];
                bool result = RelationLogic.ConstraintCheck(tempNode, goal, null, out obj);
                if (result)
                {
                    var tempTuple = obj as Tuple<object, object>;
                    if (tempTuple != null)
                    {
                        lst.Add(tempTuple);
                    }
                    var tempTuples = obj as List<Tuple<object, object>>;
                    if (tempTuples != null)
                    {
                        lst.AddRange(tempTuples);
                    }
                }
            }

            #endregion

            #region Binary Relation Checking
            //Two layer for loop to iterate all Nodes
            //binary relation build up

            // overfitting: 
            // underfitting: 
            for (var i = 0; i < graphNodes.Count - 1; i++)
            {
                var outerNode = graphNodes[i];
                for (var j = i + 1; j < graphNodes.Count; j++)
                {
                    var innerNode = graphNodes[j];
                    bool result = RelationLogic.ConstraintCheck(outerNode, innerNode, goal, null, out obj);
                    if (result)
                    {
                        var tempTuple = obj as Tuple<object, object>;
                        if (tempTuple != null)
                        {
                            lst.Add(tempTuple);
                        }
                        var tempTuples = obj as List<Tuple<object, object>>;
                        if (tempTuples != null)
                        {
                            lst.AddRange(tempTuples);
                        }
                    }
                }
            }

            obj = lst;
            #endregion
            
            return lst.Count != 0;
        }

        public bool ConstraintSatisfy(ShapeSymbol ss, out object obj)
        {
            var graphNodes = _nodes;

            var lst = new List<Tuple<object, object>>();

            #region Unary Relation Checking

            for (var i = 0; i < graphNodes.Count; i++)
            {
                var goalNode = graphNodes[i] as GoalNode;
                if (goalNode != null)
                {
                    var goal = goalNode.Goal as EqGoal;
                    if (goal != null)
                    {
                        if (ss.UnifyExplicitProperty(goal))
                        {
                            lst.Add(new Tuple<object, object>(goalNode, ss));
                        }
                    }                    
                }
                var shapeNode = graphNodes[i] as ShapeNode;
                if (shapeNode != null)
                {
                    var tempSs = shapeNode.ShapeSymbol;
                    if (tempSs != null)
                    {
                        if (ss.UnifyShape(tempSs))
                        {
                            lst.Add(new Tuple<object, object>(ss, tempSs));
                        }
                        if (tempSs.UnifyShape(ss))
                        {
                            lst.Add(new Tuple<object, object>(tempSs, ss));
                        }
                    }
                }                
            }

            #endregion

            obj = lst;
            return lst.Count != 0;
        }

        public bool ConstraintSatisfy(Equation eq, out object obj)
        {
            obj = null;

            var graphNodes = _nodes;

            var lst = new List<Tuple<object, object>>();

            #region Unary Relation Checking
            for (var i = 0; i < graphNodes.Count; i++)
            {
                var tempNode = graphNodes[i];
                bool result = RelationLogic.ConstraintCheck(tempNode, eq, null, out obj);
                if (result)
                {
                    var tempTuple = obj as Tuple<object, object>;
                    if (tempTuple != null)
                    {
                        lst.Add(tempTuple);
                    }
                    var tempTuples = obj as List<Tuple<object, object>>;
                    if (tempTuples != null)
                    {
                        lst.AddRange(tempTuples);
                    }
                }
            }
            #endregion
            obj = lst;
            return lst.Count != 0;
        }

        #region Utils


        private void ConflictResolve2(Dictionary<object, object> dict)
        {
            bool deleteFlag;
            do
            {
                deleteFlag = false;
                List<object> list = dict.Values.ToList();
                int itemCount = list.Count;
                itemCount--;
                for (var i = 0; i < itemCount; i++)
                {
                    if (list[i].ToString().Equals(list[i + 1].ToString()))
                    {
                        dict.Remove(dict.Keys.ToList()[i]);
                        deleteFlag = true;
                    }
                }
            } while (deleteFlag);
        }

        private void ConflictResolve(Dictionary<object, object> dict)
        {
            bool deleteFlag;
            do
            {
                deleteFlag = false;
                List<object> list = dict.Keys.ToList();
                int itemCount = list.Count;
                itemCount--;
                for (var i = 0; i < itemCount; i++)
                {
                    object deletedNode;
                    if (SatisfyRelation(list[i], list[i + 1], out deletedNode))
                    {
                        if (dict.ContainsKey(deletedNode))
                        {
                            dict.Remove(deletedNode);
                        }
                        deleteFlag = true;
                    }
                }
            } while (deleteFlag);
        }

        /// <summary>
        /// TODO recursive search
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="deleteNode"></param>
        /// <returns></returns>
        private bool SatisfyRelation(object obj1, object obj2, out object deleteNode)
        {
            deleteNode = null;
            var gn1 = obj1 as GraphNode;
            var gn2 = obj2 as GraphNode;
            var tuple1 = obj1 as Tuple<GraphNode, GraphNode>;
            var tuple2 = obj2 as Tuple<GraphNode, GraphNode>;
            if (gn1 != null && gn2 != null) return false; //return SatisfyRelation(gn1, gn2, out deleteNode);
            if (tuple1 != null && tuple2 != null) return SatisfyRelation(tuple1, tuple2, out deleteNode);
            if (gn1 != null && tuple2 != null) return SatisfyRelation(tuple2, gn1, out deleteNode);
            if (tuple1 != null && gn2 != null) return SatisfyRelation(tuple1, gn2, out deleteNode);

            return false;
        }

        private bool SatisfyRelation(Tuple<GraphNode, GraphNode> tuple1, GraphNode gn2, out object deleteNode)
        {
            deleteNode = null;

            var tuple1Node1 = tuple1.Item1;
            var tuple1Node2 = tuple1.Item2;

            object obj;
            bool cond1 = SatisfyRelation(tuple1Node1, gn2, out obj);

            bool cond2 = SatisfyRelation(tuple1Node2, gn2, out obj);

            if (cond1 || cond2)
            {
                deleteNode = tuple1;
                return true;
            }
            return false;
        }

        private bool SatisfyRelation(Tuple<GraphNode, GraphNode> tuple1, Tuple<GraphNode, GraphNode> tuple2, out object deleteNode)
        {
            var tuple1Node1 = tuple1.Item1;
            var tuple1Node2 = tuple1.Item2;
            var tuple2Node1 = tuple2.Item1;
            var tuple2Node2 = tuple2.Item2;

            bool cond1 = SatisfyRelation(tuple1, tuple2Node1, out deleteNode);
            if (cond1)
            {
                deleteNode = tuple1;
                return true;
            }

            cond1 = SatisfyRelation(tuple1, tuple2Node2, out deleteNode);
            if (cond1)
            {
                deleteNode = tuple1;
                return true;
            }

            cond1 = SatisfyRelation(tuple2, tuple1Node1, out deleteNode);
            if (cond1)
            {
                deleteNode = tuple2;
                return true;
            }

            cond1 = SatisfyRelation(tuple2, tuple1Node2, out deleteNode);
            if (cond1)
            {
                deleteNode = tuple2;
                return true;
            }
            return false;
        }

        private bool SatisfyRelation(GraphNode gn1, GraphNode gn2, out object deleteNode)
        {
            deleteNode = null;
            Debug.Assert(gn1 != null);
            Debug.Assert(gn2 != null);
            foreach (var edge in gn1.OutEdges)
            {
                if (edge.Target == null) continue;
                if (edge.Target.Equals(gn2))
                {
                    if (gn1.Synthesized)
                    {
                        deleteNode = gn2;
                    }
                    else
                    {
                        deleteNode = gn1;                        
                    }

                    return true;
                }
            }
            foreach (var edge in gn2.OutEdges)
            {
                if (edge.Target == null) continue;
                if (edge.Target.Equals(gn1))
                {
                    if (gn2.Synthesized)
                    {
                        deleteNode = gn1;
                    }
                    else
                    {
                        deleteNode = gn2;                        
                    }
                    return true;
                }
            }

            //TODO fix it later
            if (gn1.Synthesized && gn2.Synthesized) return false;

            bool cond1 = FindPath(gn1, gn2);
            bool cond2 = FindPath(gn2, gn1);

            if (cond1)
            {
                deleteNode = gn1;
                return true;
            }

            if (cond2)
            {
                deleteNode = gn2;
                return true;
            }

            return false;
        }


        #endregion
    }
}
