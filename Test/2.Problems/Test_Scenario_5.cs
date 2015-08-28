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
        public void TestScenario5_input()
        {
            //(2*y)+(2*x)+(-1*y)+(2*x)+4=0
            var x = new Var('x');
            var y = new Var('y');

            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term4 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term = new Term(Expression.Add, new List<object>() { term1, term2, term3, term4, 4}); 
            var eq = new Equation(term, 0);
            LineSymbol ls;
            bool result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.Traces.Count == 7);
            Assert.True(ls.StrategyTraces.Count == 1);
        }

        [Test]
        public void TestScenario5_Question1_1()
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

            //si=
            var variable = new Var("si");
            var query = new Query(variable);
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
            Assert.True(cachedLs.Traces.Count == 8);
            Assert.True(cachedLs.StrategyTraces.Count == 2);
        }

        [Test]
        public void TestScenario5_Question1_2()
        {
            var graph = new RelationGraph();
            //general line form input
            var gLine = new Line(4, 1, 4);
            var lineSymbol = new LineSymbol(gLine);
            Assert.True(gLine.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

            //graph=
            var variable = new Var("graph");
            var query = new Query(variable);
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
            Assert.True(cachedLs.Traces.Count == 2);
            Assert.True(cachedLs.StrategyTraces.Count == 2);
        }

        [Test]
        public void TestScenario5_Question1_2_Tutor()
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

            //graph=
            var variable = new Var("graph");
            var query = new Query(variable);
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
            Assert.True(cachedLs.Traces.Count == 9);
            Assert.True(cachedLs.StrategyTraces.Count == 3);
        }

        [Test]
        public void TestScenario5_Question2_1()
        {
            var graph = new RelationGraph();
            //general line form input
            var gLine = new Line(1,-1,1);
            var lineSymbol = new LineSymbol(gLine);
            Assert.True(gLine.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

            //m=
            var variable = new Var("m");
            var query = new Query(variable);
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var cachedGoalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(cachedGoalNode);
            var cachedGoal = cachedGoalNode.Goal as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.ToString().Equals("1"));
            Assert.True(cachedGoal.Traces.Count == 2);
            Assert.True(cachedGoal.StrategyTraces.Count == 2);
        }

        [Test]
        public void TestScenario5_Question2_2()
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
            //general line form input
            var gLine = lineSymbol.Shape as Line;
            Assert.NotNull(gLine);
            Assert.True(gLine.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

            //m=
            var variable = new Var("m");
            var query = new Query(variable);
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var cachedGoalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(cachedGoalNode);
            var cachedGoal = cachedGoalNode.Goal as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.ToString().Equals("-4"));
            Assert.True(cachedGoal.Traces.Count == 9);
            Assert.True(cachedGoal.StrategyTraces.Count == 3);
        }

        [Test]
        public void TestScenario5_Question12_1()
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
            //general line form input
            var line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.True(line.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

            //graph=
            var variable = new Var("graph");
            var query = new Query(variable);
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
            Assert.True(cachedLs.Traces.Count == 9);
            Assert.True(cachedLs.StrategyTraces.Count == 3);

            //m=
            var variable1 = new Var("m");
            var query1 = new Query(variable1);
            GraphNode gn1 = graph.AddNode(query1);
            var qn1 = gn1 as QueryNode;
            Assert.True(qn1 != null);
            Assert.True(qn1.InternalNodes.Count == 1);
            var cachedGoalNode = qn1.InternalNodes[0] as GoalNode;
            Assert.NotNull(cachedGoalNode);
            var cachedGoal = cachedGoalNode.Goal as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.ToString().Equals("-4"));
            Assert.True(cachedGoal.Traces.Count == 1);
            Assert.True(cachedGoal.StrategyTraces.Count == 1);
        }

        [Test]
        public void TestScenario5_Question2_2_UserMode()
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
            //general line form input
            var gLine = lineSymbol.Shape as Line;
            Assert.NotNull(gLine);
            Assert.True(gLine.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);

            //m=
            var variable = new Var("m");
            var query = new Query(variable);
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var cachedGoalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(cachedGoalNode);
            var cachedGoal = cachedGoalNode.Goal as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.ToString().Equals("-4"));
            Assert.True(cachedGoal.Traces.Count == 9);
            Assert.True(cachedGoal.StrategyTraces.Count == 3);

            ///////////////// User Model
            var userTerm1 = new Term(Expression.Multiply, new List<object>() { 4, x });
            var userTerm  = new Term(Expression.Add, new List<object>() { userTerm1, y, 4});
            var eq1 = new Equation(userTerm, 0);
            LineSymbol lineSymbol1;
            bool result1 = eq1.IsLineEquation(out lineSymbol1);
            Assert.True(result1);
            var node = graph.AddNode(lineSymbol1, true);

            Assert.True(graph.UserNodes.Count == 1);
            Assert.True(graph.Nodes.Count == 2);
        }

    
    }
}