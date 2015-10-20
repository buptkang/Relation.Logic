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

using CSharpLogic;

namespace AlgebraGeometry
{
    using NUnit.Framework;

    [TestFixture]
    public partial class TestLineRelation
    {
        #region Line Unify from two points

        [Test]
        public void Test_CreateLine_1()
        {
            //x-y+1=0
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(2.0, 3.0);
            var ptSym1 = new PointSymbol(pt1);
            var ptSym2 = new PointSymbol(pt2);
            var lineSym = LineBinaryRelation.Unify(ptSym1, ptSym2) as LineSymbol;
            Assert.NotNull(lineSym);
            var line = lineSym.Shape as Line;
            Assert.NotNull(line);
            Assert.True(line.A.Equals(-1.0));
            Assert.True(line.B.Equals(1.0));
            Assert.True(line.C.Equals(-1.0));
        }

        [Test]
        public void Test_CreateLine_2()
        {
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(1.0, 2.0);
            var ptSym1 = new PointSymbol(pt1);
            var ptSym2 = new PointSymbol(pt2);
            var lineSym = LineBinaryRelation.Unify(ptSym1, ptSym2);
            Assert.Null(lineSym);
        }

        [Test]
        public void Test_CreateLine_3()
        {
            var pt1 = new Point(2.0, 1.0);
            var pt2 = new Point(3.0, 1.0);
            var ptSym1 = new PointSymbol(pt1);
            var ptSym2 = new PointSymbol(pt2);
            var lineSym = LineBinaryRelation.Unify(ptSym1, ptSym2) as LineSymbol;
            Assert.NotNull(lineSym);
            var line = lineSym.Shape as Line;
            Assert.NotNull(line);
            Assert.True(line.A.Equals(0.0));
            Assert.True(line.B.Equals(1.0));
            Assert.True(line.C.Equals(-1.0));
        }

        [Test]
        public void Test_CreateLine_4()
        {
            var pt1 = new Point(2.0, 1.0);
            var pt2 = new Point(2.0, 2.0);
            var ptSym1 = new PointSymbol(pt1);
            var ptSym2 = new PointSymbol(pt2);
            var lineSym = LineBinaryRelation.Unify(ptSym1, ptSym2) as LineSymbol;
            Assert.NotNull(lineSym);
            var line = lineSym.Shape as Line;
            Assert.NotNull(line);
            Assert.True(line.A.Equals(1.0));
            Assert.True(line.B.Equals(0.0));
            Assert.True(line.C.Equals(-2.0));
        }

        [Test]
        public void Test_CreateLine_5()
        {
            var x = new Var('x');
            var point = new Point(2, x);
            var ps = new PointSymbol(point);
            var psNode = new ShapeNode(ps);
            var point1 = new Point(3, 4);
            var ps1 = new PointSymbol(point1);
            var psNode1 = new ShapeNode(ps1);

            object obj;
            bool value = RelationLogic.ConstraintCheck(psNode, psNode1, null, ShapeType.Line, out obj);
            Assert.True(value);
            var ls = obj as LineSymbol;
            Assert.NotNull(ls);
            Assert.False(ls.Shape.Concrete);
        }

        [Test]
        public void Test_CreateLine_6()
        {
            var point = new Point(-1, 2);
            var ps = new PointSymbol(point);
            var psNode = new ShapeNode(ps);
            var point1 = new Point(5, 8);
            var ps1 = new PointSymbol(point1);
            var psNode1 = new ShapeNode(ps1);

            object obj;
            bool value = RelationLogic.ConstraintCheck(psNode, psNode1, null, ShapeType.Line, out obj);
            Assert.True(value);
            var ls = obj as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.Shape.Concrete);
            Assert.True(ls.ToString().Equals("x-y+3=0"));
        }

        [Test]
        public void Test_CreateLine_7()
        {
            var m = new Var("m");
            var eqGoal = new EqGoal(m, 1);
            var goalNode = new GoalNode(eqGoal);
            var point1 = new Point(5, 8);
            var ps1 = new PointSymbol(point1);
            var psNode1 = new ShapeNode(ps1);

            object obj;
            bool value = RelationLogic.ConstraintCheck(psNode1, goalNode, "lineG", null, out obj);
            Assert.True(value);
            var ls = obj as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.Shape.Concrete);
            Assert.True(ls.ToString().Equals("x-y+3=0"));
        }

        #endregion

        #region Line Unify from two Goals

        [Test]
        public void Test_CreateLine_TwoGoal_1()
        {
            var m = new Var('m');
            var k = new Var('k');
            var goal1 = new EqGoal(m, 2.0);
            var goal2 = new EqGoal(k, 1.0);

            var lineSym = LineBinaryRelation.Unify(goal1, goal2);
            Assert.NotNull(lineSym);
            
            Assert.True(lineSym.SymSlope.Equals("2"));
            Assert.True(lineSym.SymIntercept.Equals("1"));
        }

        #endregion

        #region Relation between a line and a point

        [Test]
        public void Test_Relation_Point_Line_1()
        {
            var pt   = new Point(0,-4);
            var ps = new PointSymbol(pt);
            var line = new Line(null, 4, 1, 4);
            var ls = new LineSymbol(line);
            bool result = ls.UnifyShape(ps);
            Assert.True(result);
        }

        [Test]
        public void Test_Relation_Point_Line_2()
        {
            var pt = new Point(0, -3);
            var ps = new PointSymbol(pt);
            var line = new Line(null, 4, 1, 4);
            var ls = new LineSymbol(line);
            bool result = ls.UnifyShape(ps);
            Assert.False(result);
        }

        #endregion
    }
}