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
    using System.Linq.Expressions;
    using System.Collections.Generic;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape?
         * What is the slope of this line? 
         */

        [Test]
        public void TestScenario_05_WorkedExample_0()
        {
            /*
             * 2y+2x-y+2x+4=0
             * 
             * si=
             */ 

            //(2*y)+(2*x)+(-1*y)+(2*x)+4=0
            var x = new Var('x');
            var y = new Var('y');

            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term4 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term = new Term(Expression.Add, new List<object>() { term1, term2, term3, term4, 4 });
            var eq = new Equation(term, 0);
            LineSymbol lineSymbol;
            bool result = eq.IsLineEquation(out lineSymbol);
            Assert.True(result);

            var graph = new RelationGraph();
            graph.AddNode(lineSymbol);

            var query = new Query("graph");
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var cachedShapeNode = qn.InternalNodes[0] as ShapeNode;
            Assert.NotNull(cachedShapeNode);
            var cachedLs = cachedShapeNode.ShapeSymbol as LineSymbol;
            Assert.NotNull(cachedLs);
            Assert.True(cachedLs.OutputType == LineType.SlopeIntercept);
            Assert.True(cachedLs.ToString().Equals("y=-4x-4"));
            //Assert.True(cachedLs.Traces.Count == 5);

            var query2 = new Query("s");
            graph.AddNode(query2);
            Assert.True(query2.Success);
            Assert.True(query2.CachedEntities.Count == 1);
            var gGoal = query2.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.ToString().Equals("-4"));
            //Assert.True(gGoal.Traces.Count == 6);
        }

        [Test]
        public void TestScenario_05_WorkedExample_1()
        {
            var graph = new RelationGraph();
            //general line form input
            var gLine = new Line(4, 1, 4);
            var lineSymbol = new LineSymbol(gLine);
            Assert.True(gLine.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

          
        }
    
    }
}