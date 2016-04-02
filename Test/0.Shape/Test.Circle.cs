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
    public partial class TestCircle
    {
        #region Circle Checking

        [Test]
        public void Test_check1()
        {
            var pt = new Point(3.0, -2.0);
            var circle = new Circle(pt, 2.0);
    
            Assert.True(circle.Radius.Equals(2.0));
        }

        #endregion
    }
}
