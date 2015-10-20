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
    using System.Linq;
    using CSharpLogic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
         */

        [Test]
        public void TestScenario_01_WorkedExample_1()
        {
            var pt1 = new Point("A", 2, 0);
            var pt1Symbol = new PointSymbol(pt1);
            var pt2 = new Point("B", 5, 4);
            var pt2Symbol = new PointSymbol(pt2);

            var graph = new RelationGraph();
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var query = new Query("d"); // one constraint
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);            
            Assert.True(queryNode.Query.Success);
            Assert.True(queryNode.Query.CachedEntities.Count == 1);

            var cachedGoal = queryNode.Query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.ToString().Equals("d=5"));
            //Trace Check
          /*  Assert.True(cachedGoal.Traces.Count == 3);
            Assert.True(cachedGoal._innerLoop.Count == 0);*/
        }

        [Test]
        public void TestScenario_01_WorkedExample_2_0()
        {
            /*
             * 1: A(2,3) [Point] => Point
             * 2: B(3,4) [Point] => Point
             * 3: AB [Label]     => [Line, LineSegment]
             */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label);
            var qn = graph.AddNode(query);
            Assert.Null(qn);
            Assert.False(query.Success);
            Assert.NotNull(query.FeedBack);
            var types = query.FeedBack as List<ShapeType>;
            Assert.NotNull(types);
            Assert.True(types.Count == 2);
            var shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 2);
        }

        [Test]
        public void TestScenario_01_WorkedExample_2_1()
        {
            /*
               * 1: A(2,3) [Point] => Point
               * 2: B(3,4) [Point] => Point
               * 3: AB [Label]     => [Line, LineSegment]
             */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            var query = new Query(ShapeType.Line); //deterministic
            var qn = graph.AddNode(query) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(qn.Query.Equals(query));
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);

            Assert.True(qn.InternalNodes.Count == 1);
            var sn = qn.InternalNodes[0] as ShapeNode;
            Assert.NotNull(sn);
            var ls = sn.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.ToString().Equals("x-y+1=0"));
        }

        [Test]
        public void TestScenario_01_WorkedExample_2_2()
        {
            var pt1 = new Point("A", 2, 0);
            var pt1Symbol = new PointSymbol(pt1);
            var pt2 = new Point("B", 5, 4);
            var pt2Symbol = new PointSymbol(pt2);

            var graph = new RelationGraph();
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var query2 = new Query("AB", ShapeType.LineSegment);
            var queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.NotNull(queryNode2);
            Assert.True(queryNode2.Query.Success);
            Assert.True(queryNode2.Query.CachedEntities.Count == 1);
            var cachedLss = queryNode2.Query.CachedEntities.ToList()[0] as LineSegmentSymbol;
            Assert.NotNull(cachedLss);
            //Assert.True(cachedLss.Traces.Count == 0);

            var query = new Query("d"); // one constraint
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(queryNode.Query.Success);
            Assert.True(queryNode.Query.CachedEntities.Count == 1);

            var cachedGoal = queryNode.Query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.ToString().Equals("d=5"));
            //Trace Check
            //Assert.True(cachedGoal.Traces.Count == 2);
        }

    }
}