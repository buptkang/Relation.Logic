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
using System.Runtime.Remoting;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * There exists two points A(2,4) and B(5,v), the distance between A and B is 5. 
         *  What is the value of v?
         */
        [Test]
        public void TestScenario_02_WorkedExample_0()
        {
            //add three nodes in sequence

            var graph = new RelationGraph();
            var pt1 = new Point("A", 2, 4);
            var pt1Symbol = new PointSymbol(pt1);
            var v = new Var("v");
            var pt2 = new Point("B", 5, v);
            var pt2Symbol = new PointSymbol(pt2);
           
            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var d = new Var("d");
            var eqGoal = new EqGoal(d, 5);
            graph.AddNode(eqGoal);

            Assert.True(graph.Nodes.Count == 5);

            //Form a Cycle Directed Graph
            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 2);

            Assert.True(pt2Symbol.CachedSymbols.Count == 2);

            var query = new Query("v");
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 2);
            var gGoal1 = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(gGoal1);
           
            var query2 = new Query("AB");
            var queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.Null(queryNode2);
            Assert.False(query2.Success);

            query2.Constraint2 = ShapeType.LineSegment;
            queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.NotNull(queryNode2);
            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.ToList().Count == 2);

           /* query2.Constraint2 = ShapeType.Line;
            queryNode2 = graph.AddNode(query2) as QueryNode;
            Assert.NotNull(queryNode2);
            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.ToList().Count == 2);*/

            //Instructional Scaffolding Verify (Trace Generation)

            //1. check v=0's trace
            Assert.True(gGoal1.Traces.Count == 2); 
            //2. check point trace
            Assert.True(pt2Symbol.CachedSymbols.Count == 2);
            var cachedPt1 = pt2Symbol.CachedSymbols.ToList()[0];
            Assert.NotNull(cachedPt1);
            Assert.True(cachedPt1.Traces.Count == 3);
            var cachedPt2 = pt2Symbol.CachedSymbols.ToList()[1];
            Assert.NotNull(cachedPt2);
            Assert.True(cachedPt2.Traces.Count == 3);
            //3. check AB's plotting trace
            var cachedLineSeg1 = query2.CachedEntities.ToList()[0] as LineSegmentSymbol;
            Assert.NotNull(cachedLineSeg1);
            //Assert.True(cachedLineSeg1.Traces.Count == 3);
        }

        [Test]
        public void TestScenario_02_WorkedExample_1()
        {
            //add three nodes in sequence
            //delete d=5 

            var graph = new RelationGraph();

            var pt1 = new Point("A", 2, 4);
            var pt1Symbol = new PointSymbol(pt1);
            var v = new Var("v");
            var pt2 = new Point("B", 5, v);
            var pt2Symbol = new PointSymbol(pt2);

            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var d = new Var("d");
            var eqGoal = new EqGoal(d, 5);
            graph.AddNode(eqGoal);
            Assert.True(graph.Nodes.Count == 5);

            //Form a Cycle Directed Graph
            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 2);

            graph.DeleteNode(eqGoal);
            Assert.True(graph.Nodes.Count == 2);

            var pt1Node = graph.RetrieveShapeNode(pt1Symbol);
            Assert.NotNull(pt1Node);
            Assert.True(pt1Node.InEdges.Count == 0);
            Assert.True(pt1Node.OutEdges.Count == 0);

            var pt2Node = graph.RetrieveShapeNode(pt2Symbol);
            Assert.NotNull(pt2Node);
            Assert.True(pt2Node.InEdges.Count == 0);
            Assert.True(pt2Node.OutEdges.Count == 0);
        }

        [Test]
        public void TestScenario_02_WorkedExample_2()
        {
            //add three nodes in sequence
            //delete point A

            var graph = new RelationGraph();

            var pt1 = new Point("A", 2, 4);
            var pt1Symbol = new PointSymbol(pt1);
            var v = new Var("v");
            var pt2 = new Point("B", 5, v);
            var pt2Symbol = new PointSymbol(pt2);

            graph.AddNode(pt1Symbol);
            graph.AddNode(pt2Symbol);

            var d = new Var("d");
            var eqGoal = new EqGoal(d, 5);
            graph.AddNode(eqGoal);
            Assert.True(graph.Nodes.Count == 5);

            //Form a Cycle Directed Graph
            Assert.True(graph.FoundCycleInGraph());
            var ptNode = graph.RetrieveShapeNode(pt2Symbol);
            Assert.True(ptNode.InEdges.Count == 2);

            graph.DeleteNode(pt1Symbol);
            Assert.True(graph.Nodes.Count == 2);

            var pt2Node = graph.RetrieveShapeNode(pt2Symbol);
            Assert.NotNull(pt2Node);
            Assert.True(pt2Node.InEdges.Count == 0);
            Assert.True(pt2Node.OutEdges.Count == 0);
        }
    }
}
