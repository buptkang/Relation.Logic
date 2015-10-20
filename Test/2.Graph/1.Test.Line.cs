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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestLine
    {
		[Test]
        public void TestLine_Unify_Reify_0()
        {
            var graph = new RelationGraph();
            var a = new Var("a");
		    var line = new Line(a, 1, 1.0);
		    var ls = new LineSymbol(line);
		    graph.AddNode(ls);

            List<ShapeSymbol> lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            var lineSymbol = lines[0] as LineSymbol;
            Assert.NotNull(lineSymbol);
            Assert.True(lineSymbol.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(a, 1); // a=1
            graph.AddNode(eqGoal);

            lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            var currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
            Assert.True(currLine.CachedSymbols.Count == 1);
            var cachedLineSymbol = currLine.CachedSymbols.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLineSymbol);
            var cachedLine = cachedLineSymbol.Shape as Line;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.A.Equals(1.0));
            Assert.True(cachedLine.B.Equals(1.0));
            Assert.True(cachedLine.C.Equals(1.0));

            graph.DeleteNode(eqGoal);
            lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
            Assert.True(currLine.CachedSymbols.Count == 0);
            Assert.True(currLine.CachedGoals.Count == 0);

		    var eqGoal2 = new EqGoal(a, 3);
            graph.AddNode(eqGoal2);

            lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
            Assert.True(currLine.CachedSymbols.Count == 1);
		    Assert.True(currLine.CachedGoals.Count == 1);

            graph.DeleteNode(eqGoal2);

            lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
            Assert.True(currLine.CachedSymbols.Count == 0);
            Assert.True(currLine.CachedGoals.Count == 0);

          
        }

        [Test]
        public void TestLine_Unify_Reify_1()
        {
            var graph = new RelationGraph();

            var a = new Var('a');
            var point = new Point(2, a);
            var ps = new PointSymbol(point);
            graph.AddNode(ps);
            var line = new Line(1, a, 1.0);
            var ls = new LineSymbol(line);
            graph.AddNode(ls);

            List<ShapeSymbol> points = graph.RetrieveShapeSymbols(ShapeType.Point);
            Assert.True(points.Count == 1);
            var pt = points[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.True(pt.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(a, 1); // a=1
            graph.AddNode(eqGoal);
            points = graph.RetrieveShapeSymbols(ShapeType.Point);
            Assert.True(points.Count == 1);
            pt = points[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.True(pt.CachedSymbols.Count == 1);
            var cachedPs = pt.CachedSymbols.ToList()[0] as PointSymbol;
            Assert.NotNull(cachedPs);
            var cachedPt = cachedPs.Shape as Point;
            Assert.NotNull(cachedPt);
            Assert.True(cachedPt.XCoordinate.Equals(2.0));
            Assert.True(cachedPt.YCoordinate.Equals(1.0));

            var lines = graph.RetrieveShapeSymbols(ShapeType.Line);
            Assert.True(lines.Count == 1);
            var currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
            Assert.True(currLine.CachedSymbols.Count == 1);
            var cachedLineSymbol = currLine.CachedSymbols.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLineSymbol);
            var cachedLine = cachedLineSymbol.Shape as Line;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.A.Equals(1.0));
            Assert.True(cachedLine.B.Equals(1.0));
            Assert.True(cachedLine.C.Equals(1.0));
        }

        [Test]
        public void TestLine_Unify_Reify_2()
        {
            /* 
             * (2,x), (3,4)
             * line pass through two points
             */

            var graph = new RelationGraph();
            var x = new Var('x');
            var point = new Point(2, x);
            var ps = new PointSymbol(point);
            var point1 = new Point(3, 4);
            var ps1 = new PointSymbol(point1);
            graph.AddNode(ps);
            graph.AddNode(ps1);

            var query = new Query(ShapeType.Line);
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(x, 1); // x=2
            graph.AddNode(eqGoal);

            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
         
            var eqGoal2 = new EqGoal(x, 2); // x=2
            graph.AddNode(eqGoal2);
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 2);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);


/*            graph.DeleteNode(eqGoal2);
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 1);
            Assert.True(graph.Nodes.Count == 4);

            graph.DeleteNode(eqGoal);
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 0);
            Assert.True(graph.Nodes.Count == 3);*/

        }

        [Test]
        public void TestLine_Unify_Reify_3()
        {
            /* 
             * (2,x), (3,4)
             * line pass through two points
             */

        /*    var graph = new RelationGraph();
            var x = new Var('x');
            var point = new Point(2, x);
            var ps = new PointSymbol(point);
            var point1 = new Point(3, 4);
            var ps1 = new PointSymbol(point1);
            graph.AddNode(ps);
            graph.AddNode(ps1);

            var query = new Query(ShapeType.Line);
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(x, 1); // x=2
            graph.AddNode(eqGoal);

            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 1);

            var eqGoal2 = new EqGoal(x, 2); // x=2
            graph.AddNode(eqGoal2);
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 2);

            graph.DeleteNode(ps1);

            Assert.True(graph.Nodes.Count == 3); //QueryNode will be deleted as well.
            queryNode = graph.RetrieveQueryNode(query);
            Assert.Null(queryNode);*/
        }

        [Test]
        public void TestLine_Unify_Reify_4()
        {
            /* 
             * (2,x), (3,4)
             * line pass through two points
             */
           /* var graph = new RelationGraph();
            var x = new Var('x');
            var point = new Point(2, x);
            var ps = new PointSymbol(point);
            var point1 = new Point(3, 4);
            var ps1 = new PointSymbol(point1);
            graph.AddNode(ps);
            graph.AddNode(ps1);

            var query = new Query(ShapeType.Line);
            var queryNode = graph.AddNode(query) as QueryNode;
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            var cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(x, 1); // x=2
            graph.AddNode(eqGoal);

            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 1);

            var eqGoal2 = new EqGoal(x, 2); // x=2
            graph.AddNode(eqGoal2);
            queryNode = graph.RetrieveQueryNode(query);
            Assert.NotNull(queryNode);
            Assert.True(queryNode.InternalNodes.Count == 1);
            Assert.True(query.Success);
            Assert.True(query.CachedEntities.Count == 1);
            cachedLine = query.CachedEntities.ToList()[0] as LineSymbol;
            Assert.NotNull(cachedLine);
            Assert.True(cachedLine.CachedSymbols.Count == 2);

            graph.DeleteNode(query);

            Assert.True(graph.Nodes.Count == 4); //QueryNode will be deleted as well.
            queryNode = graph.RetrieveQueryNode(query);
            Assert.Null(queryNode);*/
        }


    }
}
