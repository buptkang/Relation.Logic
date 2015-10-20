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
    public partial class TestPoint
    {
        #region Point Checking

        [Test]
        public void Test_check()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            Assert.False(point.Concrete);
            point = new Point(2.0, 3.0);
            Assert.True(point.Concrete);
        }

        [Test]
        public void Test_check1()
        {
            var pt = new Point(1, 1);
            Assert.True(pt.XCoordinate.Equals(1.0));
            Assert.True(pt.YCoordinate.Equals(1.0));
        }

        #endregion

        #region Point Rounding Test

        [Test]
        public void Test_Point_Rounding_1()
        {
            var point = new Point(2.01111212, 3.12121211);
            Assert.True(point.XCoordinate.Equals(2.0));
            Assert.True(point.YCoordinate.Equals(3.1));
        }

        #endregion

        #region Numeric and Symbolic Integration Test

        [Test]
        public void Test1_Integration()
        {
            //TODO
            /*            var x = new Var('x');
                        var y = new Var('y');
                        var point = new Point(x, y);
                        var ptSym = new PointSymbol(point);
                        Assert.True(ptSym.ToString().Equals("(x,y)"));
                        point.Label = "A";
                        Assert.True(ptSym.ToString().Equals("A(x,y)"));

                        Goal goal1 = new EqGoal(y, -4);
                        Goal goal2 = new EqGoal(x, -3);

                        var lst = new List<Goal>()
                        {
                            goal1, goal2
                        };

                        point.Reify(lst);
                        Assert.True(ptSym.ToString().Equals("A(-3,-4)"));*/
        }

        [Test]
        public void Test1_Symbolic()
        {
            var pt = new Point(1, 1);
            var ptSym = new PointSymbol(pt);

            Assert.True(ptSym.ToString().Equals("(1,1)"));

            pt = new Point("A", 1, 1);
            ptSym = new PointSymbol(pt);
            Assert.True(ptSym.ToString().Equals("A(1,1)"));
        }

        [Test]
        public void Test2_Symbolic()
        {
            var pt = new Point("x", -2);
            var ptSym = new PointSymbol(pt);
            Assert.True(ptSym.ToString().Equals("(x,-2)"));
        }

        [Test]
        public void Test_Sub_1()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            var goal1 = new EqGoal(x, -3);
            var goal2 = new EqGoal(y, -4);
            ps.Reify(goal1);
            ps.Reify(goal2);
            Assert.True(ps.CachedGoals.Count == 2);
            Assert.True(ps.CachedSymbols.Count == 1);
        }

        #endregion

        #region Reification

        [Test]
        public void Test_reify_1()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            var t = new Var('t');
            var eqGoal = new EqGoal(t, 1); // t=1               
            bool result = ps.Reify(eqGoal);
            Assert.False(result);
        }

        [Test]
        public void Test_Unreify_0()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = ps.Reify(eqGoal);

            result = ps.UnReify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 0);
            Assert.True(ps.CachedSymbols.Count == 0);
        }

        [Test]
        public void Test_Unreify_00()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = ps.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 1);
            Assert.True(ps.CachedSymbols.Count == 1);

            var eqGoal2 = new EqGoal(y, 2); // y=2               
            result = ps.Reify(eqGoal2);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 2);
            Assert.True(ps.CachedSymbols.Count == 1);

            result = ps.UnReify(eqGoal); // undo x=1
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 1);
            Assert.True(ps.CachedSymbols.Count == 1);
            var gps = ps.CachedSymbols.ToList()[0] as PointSymbol;
            Assert.NotNull(gps);
            var gPoint = gps.Shape as Point;
            Assert.NotNull(gPoint);
            Assert.True(gPoint.XCoordinate.Equals(x));
            Assert.True(gPoint.YCoordinate.Equals(2));

            result = ps.UnReify(eqGoal2); // undo y = 2
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 0);
            Assert.True(ps.CachedSymbols.Count == 0);
        }
        #endregion
    }
}
