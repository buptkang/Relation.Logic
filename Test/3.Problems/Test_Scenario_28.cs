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
         * Problem 28: There are two points A(2,y) and B(-1,4). The y-coordinate of point A is -1. What is the distance betweeen these two points?
         */
        [Test]
        public void TestScenario_28_WorkedExample_0()
        {
            var graph = new RelationGraph();
            var y = new Var("y");
            var pt1 = new Point("A", 2, y);
            var pt1Symbol = new PointSymbol(pt1);
            var pt2 = new Point("B", -1, 4);
            var pt2Symbol = new PointSymbol(pt2);
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);
            var eqGoal = new EqGoal(y, -1);
            graph.AddNode(eqGoal);

            Assert.True(pt1Symbol.CachedSymbols.Count == 1);
            var ptNode = graph.RetrieveShapeNode(pt1Symbol);
            Assert.True(ptNode.InEdges.Count == 1);

            var query2 = new Query("d");
            var queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.NotNull(queryNode2);
            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.Count == 1);
            var cachedGoal = query2.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.ToString().Equals("d=5.83"));
        }

        [Test]
        public void TestScenario_28_WorkedExample_1()
        {
            //Sequence matters
        }
    }
}
