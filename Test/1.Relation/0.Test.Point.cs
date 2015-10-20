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

using System;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public partial class TestPointRelation
    {
        [Test]
        public void Test_Unify_1()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            var shapeNode = new ShapeNode(ps);

            var eqGoal  = new EqGoal(x, 1); // x=1

            object obj;
            bool result = RelationLogic.ConstraintCheck(shapeNode, eqGoal, null, out obj);
            Assert.True(result);
            Assert.NotNull(obj);

            var lst = obj as List<Tuple<object, object>>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 1);
            var tuple = lst[0];
            Assert.True(tuple.Item1.Equals(shapeNode));
            Assert.True(tuple.Item2.Equals(eqGoal));
        }

        [Test]
        public void Test_Unify_2()
        {
            //true positive
            var x = new Var('x');
            var point = new Point(x, 4);
            var ps = new PointSymbol(point);
            var shapeNode = new ShapeNode(ps);

            var point1 = new Point(4, 5);
            var ps1 = new PointSymbol(point1);
            var shapeNode1 = new ShapeNode(ps1);

            var eqGoal = new EqGoal(x, 9);

            object obj;
            bool result = RelationLogic.ConstraintCheck(shapeNode, shapeNode1, eqGoal, null, out obj);
            Assert.False(result);
        }
    }
}
