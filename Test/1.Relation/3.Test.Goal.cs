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
        public void Test_Unify_1()
        {
            //a = 1, a*b = -1;
            //true positive
           
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 1);

            var lhsTerm = new Term(Expression.Multiply, new List<object>() {a, b});
            var equation = new Equation(lhsTerm, -1);
            var eqNode = new EquationNode(equation);

            object obj;
            bool result = RelationLogic.ConstraintCheck(eqNode, eqGoal, null, out obj);
            Assert.True(result);
            Assert.NotNull(obj);
        }

        [Test]
        public void Test_Unify_2()
        {
            // a=2, b=a
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);

            var equation = new Equation(b, a);
            var eqNode = new EquationNode(equation);

            object obj;
            bool result = RelationLogic.ConstraintCheck(eqNode, eqGoal, null, out obj);
            Assert.True(result);
            Assert.NotNull(obj);
        }

        [Test]
        public void Test_Unify_3()
        {
            // a=2, b=a
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);
            var goalNode = new GoalNode(eqGoal);

            var equation = new Equation(b, a);

            object obj;
            bool result = RelationLogic.ConstraintCheck(goalNode, equation, null, out obj);
            Assert.True(result);
            Assert.NotNull(obj);
        }

        [Test]
        public void Test_Unify_4()
        {
            // c=2, b=a
            var c = new Var("c");
            var b = new Var("b");
            var eqGoal = new EqGoal(c, 2);
            var goalNode = new GoalNode(eqGoal);

            var a = new Var("a");
            var equation = new Equation(b, a);

            object obj;
            bool result = RelationLogic.ConstraintCheck(goalNode, equation, null, out obj);
            Assert.False(result);
        }
    }
}
