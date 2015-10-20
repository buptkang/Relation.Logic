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
    using System.Linq;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * There is a line, the slope of it is 3, the y-intercept of it is 2. 
         * What is the slope intercept form of this line? 
         * What is the general form of this line?
         */
        [Test]
        public void TestScenario_04_WorkedExample_0()
        {
            var graph = new RelationGraph();

            var m = new Var("m");
            var eqGoal1 = new EqGoal(m, 3);
            var k = new Var("k");
            var eqGoal2 = new EqGoal(k, 2);

            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);

            var query1 = new Query("lineS");
            var queryNode1 = graph.AddNode(query1) as QueryNode;
            Assert.NotNull(queryNode1);
            Assert.True(query1.Success);
            Assert.True(query1.CachedEntities.Count == 1);
            var cachedLine1 = query1.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine1);
            Assert.True(cachedLine1.ToString().Equals("y=3x+2"));
            Assert.True(cachedLine1.Traces.Count == 1);

            Assert.True(graph.Nodes.Count == 3);

            var query = new Query("lineG");
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(graph.Nodes.Count == 4);
            Assert.True(cachedLine.Traces.Count == 2);
             var lt = cachedLine.Shape.GetInputType();
             Assert.NotNull(lt);
             Assert.True(lt.Equals(LineType.Relation));
             Assert.True(cachedLine.ToString().Equals("3x-y+2=0"));
        }

        [Test]
        public void TestScenario_04_WorkedExample_1()
        {
            var graph = new RelationGraph();

            var m = new Var("m");
            var eqGoal1 = new EqGoal(m, 3);
            var k = new Var("k");
            var eqGoal2 = new EqGoal(k, 2);

            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);

            var query = new Query("lineG");
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            var lt = cachedLine.Shape.GetInputType();
            Assert.NotNull(lt);
            Assert.True(lt.Equals(LineType.Relation));
            Assert.True(cachedLine.ToString().Equals("3x-y+2=0"));
            Assert.True(cachedLine.Traces.Count == 1);

            var query1 = new Query("lineS");
            var queryNode1 = graph.AddNode(query1) as QueryNode;
            Assert.NotNull(queryNode1);
            Assert.True(query1.Success);
            Assert.True(query1.CachedEntities.Count == 1);
            var cachedLine1 = query1.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine1);
            Assert.True(cachedLine1.ToString().Equals("y=3x+2"));

            Assert.True(cachedLine1.Traces.Count == 2);
        }
    }
}
