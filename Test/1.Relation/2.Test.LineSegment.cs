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
    using System.Runtime.Remoting;
    using System.Text;
    using CSharpLogic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestLineSegment
    {
        #region Line segment type checking and validation

        [Test]
        public void Test1()
        {
            var pt = new Point(1.0, 2.0);
            var pt2 = new Point(2.0, 4.0);
            var lineSeg = new LineSegment(pt, pt2);
           // Assert.True(lineSeg.RelationStatus);
            Assert.NotNull(lineSeg.Label);
        }

        [Test]
        public void Test2()
        {
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(1.0, 2.0);
            try
            {
                var lineSeg = new LineSegment(pt1, pt2);
                Assert.Null(lineSeg);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }


        [Test]
        public void Test_CreateLineSegment_1()
        {
/*            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(2.0, 3.0);
            var lineSeg = LineSegRelation.Unify(pt1, pt2);
            Assert.NotNull(lineSeg);*/
        }

        [Test]
        public void Test_CreateLineSegment_2()
        {
           /* var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(1.0, 2.0);
            var lineSeg = LineSegRelation.Unify(pt1, pt2);
            Assert.Null(lineSeg);*/
        }

        [Test]
        public void Test_CreateLineSegment_3()
        {
            var x = new Var('x');
            var point = new Point(2, x);
            var ps = new PointSymbol(point);
            var psNode = new ShapeNode(ps);
            var point1 = new Point(3, 4);
            var ps1 = new PointSymbol(point1);
            var psNode1 = new ShapeNode(ps1);

            object obj;
            bool value = RelationLogic.ConstraintCheck(psNode, psNode1, null, ShapeType.LineSegment, out obj);
            Assert.True(value);
            var lss = obj as LineSegmentSymbol;
            Assert.NotNull(lss);
            Assert.False(lss.Shape.Concrete);
        }

        #endregion

        [Test]
        public void Test_Unify()
        {
            var x = new Var("x");
            var pt1 = new Point(x, 2.0);
            var pt2 = new Point(3.0, 5.0);
            var ls = new LineSegment(pt1, pt2);
            var lss = new LineSegmentSymbol(ls);

            var d = new Var("d");
            var eqGoal = new EqGoal(d, 5); 
          
            object obj;
            bool result = lss.UnifyProperty(eqGoal, out obj);
            Assert.True(result);

            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
            var gGoal1 = lst[0] as EqGoal;
            Assert.NotNull(gGoal1);
            Assert.True(gGoal1.Lhs.ToString().Equals("x"));
            Assert.True(gGoal1.Rhs.ToString().Equals("7"));
            Assert.True(gGoal1.Traces.Count == 1);

            var gGoal2 = lst[1] as EqGoal;
            Assert.NotNull(gGoal2);
            Assert.True(gGoal2.Lhs.ToString().Equals("x"));
            Assert.True(gGoal2.Rhs.ToString().Equals("-1"));
            Assert.True(gGoal2.Traces.Count == 1);
        }    
    }
}
