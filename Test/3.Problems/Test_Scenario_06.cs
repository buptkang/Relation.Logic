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
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * Problem 6: A line passes through points (2,3) and (4,y), the slope of this line is 5. What is the y-intercept of the line?
         */

        [Test]
        public void TestScenario_06_WorkedExample_1()
        {
            var graph = new RelationGraph();
            var pt1 = new Point(2, 3);
            var pt1Symbol = new PointSymbol(pt1);
            var y = new Var("y");
            var pt2 = new Point(4, y);
            var pt2Symbol = new PointSymbol(pt2);
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var m = new Var("m");
            var eqGoal = new EqGoal(m, 5);
            graph.AddNode(eqGoal);
            Assert.True(graph.Nodes.Count == 4);

            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 1);
            Assert.True(pt2Symbol.CachedSymbols.Count == 1);

            var query2 = new Query("lineG");
            var queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.NotNull(queryNode2);
            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.Count == 1);
            var cachedLine = query2.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.ToString().Equals("5x-y-7=0"));

            var query3 = new Query("k");
            var queryNode3 = graph.AddNode(query3) as QueryNode;
            Assert.NotNull(queryNode3);
            Assert.True(query3.Success);
            Assert.True(query3.CachedEntities.Count == 1);
            var cachedGoal = query3.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.ToString().Equals("k=-7"));
        }

        [Test]
        public void TestScenario_06_WorkedExample_2()
        {
            var graph = new RelationGraph();
            var pt1 = new Point(2, 3);
            var pt1Symbol = new PointSymbol(pt1);
            var y = new Var("y");
            var pt2 = new Point(4, y);
            var pt2Symbol = new PointSymbol(pt2);
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var m = new Var("m");
            var eqGoal = new EqGoal(m, 5);
            graph.AddNode(eqGoal);
            Assert.True(graph.Nodes.Count == 4);

            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 1);
            Assert.True(pt2Symbol.CachedSymbols.Count == 1);

            var query3 = new Query("k");
            var queryNode3 = graph.AddNode(query3) as QueryNode;
            Assert.NotNull(queryNode3);
            Assert.True(query3.Success);
            Assert.True(query3.CachedEntities.Count == 1);
            var cachedGoal = query3.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.ToString().Equals("k=-7"));
        }


        [Test]
        public void TestScenario_06_WorkedExample_3()
        {
            var graph = new RelationGraph();
            var pt1 = new Point(2, 3);
            var pt1Symbol = new PointSymbol(pt1);
            graph.AddNode(pt1Symbol);

            var query2 = new Query("lineG");
            var queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.Null(queryNode2);
            Assert.False(query2.Success);

            var y = new Var("y");
            var pt2 = new Point(4, y);
            var pt2Symbol = new PointSymbol(pt2);
            
            graph.AddNode(pt2Symbol);

            var m = new Var("m");
            var eqGoal = new EqGoal(m, 5);
            graph.AddNode(eqGoal);
            Assert.True(graph.Nodes.Count == 5);
           
            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 1);
            Assert.True(pt2Symbol.CachedSymbols.Count == 1);

            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.Count == 1);
            var cachedLine = query2.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.ToString().Equals("5x-y-7=0"));
        }
    }
}
