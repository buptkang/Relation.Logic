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
    using NUnit.Framework;

    [TestFixture]
    public partial class TestLineSegment
    {
        [Test]
        public void TestLineSegment_Unify_Reify_0()
        {
            /*
            * Input sequence:
            * 1: (AB) [LineSegment]  => LineSegment
            * 2: A(2,3)          => Point
            * 3: B(2,3)          => Point
            * Update: AB [Label] => LineSegment
            */
             var graph = new RelationGraph();

             var ptA = new Point("A", 2, 3);
             var ptASym = new PointSymbol(ptA);
             graph.AddNode(ptASym); //api call
             Assert.True(graph.Nodes.Count == 1);

             var query = new Query(ShapeType.LineSegment);
             var queryNode = graph.AddNode(query) as QueryNode;
             Assert.Null(queryNode);

             var ptB = new Point("B", 3, 4);
             var PtBSym = new PointSymbol(ptB);
             graph.AddNode(PtBSym); //api call
             Assert.True(graph.Nodes.Count == 3);

             queryNode = graph.RetrieveQueryNode(query);
             Assert.NotNull(queryNode);
        }
    }
}