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
        private GraphNode SearchUserNode(object obj)
        {
            var shape = obj as ShapeSymbol;
            if (shape != null)
            {
                foreach (var gn in _userNodes)
                {
                    var shapeNode = gn as ShapeNode;
                    if (shapeNode != null)
                    {
                        if (shapeNode.ShapeSymbol.Equals(shape)) return gn;
                    }

                }
            }
            var goal = obj as EqGoal;
            if (goal != null)
            {
                foreach (var gn in _userNodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        if (goalNode.Goal.Equals(goal)) return gn;
                    }
                }
            }
            var query = obj as Query;
            if (query != null)
            {
                foreach (var gn in _userNodes)
                {
                    var qn = gn as QueryNode;
                    if (qn != null && qn.Query.QueryQuid.Equals(query.QueryQuid))
                        return qn;
                }
            }

            return null;
        }
    }
}
