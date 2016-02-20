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
    using System.Linq.Expressions;
    using System.Collections.Generic;

    [TestFixture]
    public partial class TestProblemSolvingScenarios
    {
        /*
         * Problem 1: Find the distance betweeen A(2,0) and B(5,4)?
         * 
         * Problem 2:  There exists two points A(2,4) and B(5,v), the distance between A and B is 5. 
         * What is the value of v?
         * 
         * Problem 3: A line passes through two points A(2,0), B(0,3) respectively.
         * 1) What is the slope of this line?  
         * 2) What is the standard form of this line?
         * 
         * Problem 4: There is a line, the slope of it is 3, the y-intercept of it is 2. 
         * What is the slope intercept form of this line? 
         * What is the general form of this line?
         * 
         * Problem 5: Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape?
         * What is the slope of this line? 
         * 
         * Problem 6: A line passes through points (2,3) and (4,y), the slope of this line is 5. 
         * What is the y-intercept of the line?
         * 
         * Problem 28: There are two points A(2,y) and B(-1,4). The y-coordinate of point A is -1. 
         * What is the distance betweeen these two points?
         * 
         * 
         * Problem 61: Line A is perpendicular to the line 2y=4x+8 and passes through (-2,-8). 
         * Draw line A on the geometric coordinate canvas. 
         * Write the equation of line A on the algebraic canvas.
         * 
         * Problem 62: Line C passes through the point (5,2). It is parallel to the line 4x+6y-12=0. 
         * Draw line C on the geometric coordinate canvas. 
         * Write the equation of line C on the algebraic canvas.
         * 
         * Problem 63: Find the minimum distance between the point (1,−3) and the line y = -x + 6. 
         * Draw this line, point, and the line segment between them onto the geometric coordinate canvas. 
         * Write the value of the distance, d, in the algebraic canvas. Round your answer to two decimal numbers.
         * 
         * Problem 64: Circle A is centered about the origin and has a radius of 5. Line B is tangent to circle A at the point (-3,4). 
         * Draw circle and line on the geometric coordinate canvas. 
         * What is the equation of the line that is tangent to circle A at the point (-3,4)?
         * 
         * Problem 65: The equation of line A is 2y-4x = 10. Line B is perpendicular to line A. 
         * Write the equation of line B, given that it passes through the point (1,2). 
         * Graph this equation.
         * 
         * Problem 66: Line D has the same slope as the line y=2x-6. Line D passes through the point (0,5). 
         * Draw line D on the geometric coordinate canvas. 
         * Write the equation of line D on the algebraic canvas.
         * 
         * Problem 67: Find the distance between the point (-2,-4) and the line 3y=-x+6. 
         * Draw this line, point, and the line segment between them onto the geometric coordinate canvas. 
         * Write the value of the distance, d, in the algebraic canvas. Round your answer to two decimal numbers.
         * 
         * Problem 68: Circle A is described by the equation (x-2)^2+(y-3)^2 = 25. 
         * What is the equation of a line that is tangent to this circle at the point (5,7)? 
         * Draw this circle and the tangent line on the geometric coordinate canvas. 
         * What is the equation of this tangent line?
         * 
         */

        #region Problem 1

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

        #endregion

        #region Problem 2

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
            Assert.True(gGoal1.Traces.Count == 3);
            //2. check point trace
            Assert.True(pt2Symbol.CachedSymbols.Count == 2);
            var cachedPt1 = pt2Symbol.CachedSymbols.ToList()[0];
            Assert.NotNull(cachedPt1);
            Assert.True(cachedPt1.Traces.Count == 4);
            var cachedPt2 = pt2Symbol.CachedSymbols.ToList()[1];
            Assert.NotNull(cachedPt2);
            Assert.True(cachedPt2.Traces.Count == 4);
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

        #endregion

        #region Problem 3

        [Test]
        public void TestScenario2_Relation_1()
        {
            /*
			 * 1: A(2,3) [Point] => Point
			 * 2: B(3,4) [Point] => Point
			 * 3: AB [Label]     => [Line, LineSegment]
			 * 4: ask slope = ?
			 */
            var graph = new RelationGraph();
            var ptA = new Point("A", 2, 3);
            var ptASymbol = new PointSymbol(ptA);
            var ptB = new Point("B", 3, 4);
            var ptBSymbol = new PointSymbol(ptB);
            graph.AddNode(ptASymbol);
            graph.AddNode(ptBSymbol);
            const string label = "AB";
            var query = new Query(label, ShapeType.Line);
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
            var shapes = graph.RetrieveShapeNodes();
            Assert.True(shapes.Count == 3);

            var variable = new Var('m');
            var query1 = new Query(variable); //m=
            qn = graph.AddNode(query1) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(qn.InternalNodes.Count == 1);
            var gn = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(gn);
            var eqGoal = gn.Goal as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Lhs.Equals(variable));
            Assert.True(eqGoal.Rhs.Equals(1));

            shapes = graph.RetrieveShapeNodes();
            Assert.True(shapes.Count == 3);
        }



        [Test]
        public void TestScenario3_CSP_1()
        {
            /*
             * Input sequence:
             * 1. y = 2x + 1
             * 2: m = ?
             */
            var graph = new RelationGraph();
            var line = new Line(2, 1);
            var lineSymbol = new LineSymbol(line);
            Assert.True(line.InputType == LineType.SlopeIntercept);
            graph.AddNode(lineSymbol);
            var variable = new Var('m');
            var query = new Query(variable);
            GraphNode gn = graph.AddNode(query);
            var qn = gn as QueryNode;
            Assert.True(qn != null);

            Assert.True(qn.InternalNodes.Count == 1);
            var goalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(goalNode);
            var eqGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(2.0));
            Assert.True(eqGoal.Lhs.Equals(variable));

            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            var cachedGoal = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(2.0));
        }

        [Test]
        public void TestScenario3_CSP_2()
        {
            /*
             * Input sequence:
             * 1. 2x + y + 1 = 0
             * 2: m = ?
             */
            var graph = new RelationGraph();
            var line = new Line(2, 1, 1);
            var lineSymbol = new LineSymbol(line);
            Assert.True(line.InputType == LineType.GeneralForm);
            graph.AddNode(lineSymbol);
            var variable = new Var('m');
            var query = new Query(variable);
            var qn = graph.AddNode(query) as QueryNode;
            Assert.True(qn != null);
            Assert.True(qn.InternalNodes.Count == 1);
            var goalNode = qn.InternalNodes[0] as GoalNode;
            Assert.NotNull(goalNode);
            var eqGoal = goalNode.Goal as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(-2));
            Assert.True(eqGoal.Lhs.Equals(variable));
            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            var cachedGoal = query.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(-2));
        }

        [Test]
        public void TestScenario3_CSP_3()
        {
            var graph = new RelationGraph();
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal2 = new EqGoal(k, 2); //k=2
            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);
            var query = new Query(ShapeType.Line);
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
            Assert.True(ls.ToString().Equals("y=3x+2"));

            //Output Usage
            Assert.True(query.CachedEntities.Count == 1);
            ls = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.ToString().Equals("y=3x+2"));
        }

        [Test]
        public void TestScenario3_CSP_4()
        {
            var graph = new RelationGraph();
            var m = new Var('m');
            var k = new Var('k');
            var eqGoal1 = new EqGoal(m, 3); //m=3
            var eqGoal3 = new EqGoal(m, 4); //m=4
            var eqGoal2 = new EqGoal(k, 2); //k=2
            graph.AddNode(eqGoal1);
            graph.AddNode(eqGoal2);
            graph.AddNode(eqGoal3);
            var query = new Query(ShapeType.Line);
            var qn = graph.AddNode(query) as QueryNode;
            Assert.NotNull(qn);
            Assert.NotNull(qn.Query);
            Assert.True(qn.Query.Equals(query));
            Assert.True(query.Success);
            Assert.Null(query.FeedBack);
            Assert.True(qn.InternalNodes.Count == 2);

            Assert.True(query.CachedEntities.Count == 2);
        }


        #endregion

        #region Problem 4

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
            Assert.True(cachedLine.Traces.Count == 2);

            var query1 = new Query("lineS");
            var queryNode1 = graph.AddNode(query1) as QueryNode;
            Assert.NotNull(queryNode1);
            Assert.True(query1.Success);
            Assert.True(query1.CachedEntities.Count == 1);
            var cachedLine1 = query1.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine1);
            Assert.True(cachedLine1.ToString().Equals("y=3x+2"));

            Assert.True(cachedLine1.Traces.Count == 1);
        }


        #endregion

        #region Problem 5

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
            Assert.True(cachedLs.OutputType == LineType.GeneralForm);
            //Assert.True(cachedLs.ToString().Equals("y=-4x-4"));
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

        #endregion

        #region Problem 6

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



        #endregion

        #region Problem 28

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

        #endregion

    }
}