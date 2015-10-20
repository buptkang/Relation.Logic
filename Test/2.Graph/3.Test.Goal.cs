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

namespace AlgebraGeometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.Text;
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestGoal
    {
        [Test]
        public void Test_Reify_1()
        {
            //a = 1, a*b = -1;
            //true positive
            var graph = new RelationGraph();
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 1);
            var lhsTerm = new Term(Expression.Multiply, new List<object>() { a, b });
            var equation = new Equation(lhsTerm, -1);
            graph.AddNode(eqGoal);
            var en = graph.AddNode(equation) as EquationNode;
            Assert.NotNull(en);
            Assert.True(en.Equation.CachedEntities.Count == 1);
            Assert.True(graph.Nodes.Count == 3);
        }

        [Test]
        public void Test_UnReify_1_0()
        {
            //a = 1, a*b = -1;
            //true positive
            var graph = new RelationGraph();
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 1);
            var lhsTerm = new Term(Expression.Multiply, new List<object>() { a, b });
            var equation = new Equation(lhsTerm, -1);
            graph.AddNode(eqGoal);
            var en = graph.AddNode(equation) as EquationNode;
            Assert.NotNull(en);
            Assert.True(en.Equation.CachedEntities.Count == 1);
            Assert.True(graph.Nodes.Count == 3);

            graph.DeleteNode(equation);
            Assert.True(graph.Nodes.Count == 1);
        }

        [Test]
        public void Test_UnReify_1_1()
        {
            //a = 1, a*b = -1;
            //true positive
            var graph = new RelationGraph();
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 1);
            var lhsTerm = new Term(Expression.Multiply, new List<object>() { a, b });
            var equation = new Equation(lhsTerm, -1);
            graph.AddNode(eqGoal);
            var en = graph.AddNode(equation) as EquationNode;
            Assert.NotNull(en);
            Assert.True(en.Equation.CachedEntities.Count == 1);
            Assert.True(graph.Nodes.Count == 3);

            graph.DeleteNode(eqGoal);
            Assert.True(graph.Nodes.Count == 1);

            var eqNode = graph.Nodes[0] as EquationNode;
            Assert.NotNull(eqNode);
            Assert.True(eqNode.Equation.CachedEntities.Count == 0);
        }

        [Test]
        public void Test_Reify_2()
        {
            // a=2, b=a
            //true positive
            var graph = new RelationGraph();
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);
            var equation = new Equation(b, a);
            graph.AddNode(eqGoal);
            var en = graph.AddNode(equation) as EquationNode;
            Assert.NotNull(en);
            Assert.True(en.Equation.CachedEntities.Count == 1);
            Assert.True(graph.Nodes.Count == 3);
        }

        [Test]
        public void Test_UnReify_2_0()
        {
            // a=2, b=a
            //true positive
            var graph = new RelationGraph();
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);
            var equation = new Equation(b, a);
            graph.AddNode(eqGoal);
            var en = graph.AddNode(equation) as EquationNode;
            Assert.NotNull(en);
            Assert.True(en.Equation.CachedEntities.Count == 1);
            Assert.True(graph.Nodes.Count == 3);

            graph.DeleteNode(equation);
            Assert.True(graph.Nodes.Count == 1);
        }

    }
}
