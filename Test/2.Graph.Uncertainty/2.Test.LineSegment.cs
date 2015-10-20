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
    using NUnit.Framework;

    [TestFixture]
    public partial class TestLineSegment
    {
        [Test]
        public void TestLineSegment_Uncertainty_0()
        {
            /*
			 * Input sequence:
			 * 1: (AB) [LineSegment]  => LineSegment
			 * 2: A(2,3)          => Point
			 * 3: B(3,4)          => Point
			 * Update: AB [Label] => LineSegment
			 */

            /* var graph = new RelationGraph();
             var lineSeg = new LineSegment("AB");
             //Assert.True(lineSeg.RelationStatus);
             graph.AddNode(lineSeg); //api call
             Assert.True(graph.Nodes.Count == 1);

             var A = new Point("A", 2, 3);
             graph.AddNode(A); //api call
             Assert.True(graph.Nodes.Count == 2);

             List<ShapeNode> nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
             Assert.True(nodes.Count == 1);
             Assert.True(nodes[0].InEdges.Count == 1);
             Assert.True(nodes[0].OutEdges.Count == 0);

             nodes = graph.RetrieveShapeNodes(ShapeType.Point);
             Assert.True(nodes.Count == 1);
             Assert.True(nodes[0].InEdges.Count == 0);
             Assert.True(nodes[0].OutEdges.Count == 1);

             var B = new Point("B", 3, 4);
             graph.AddNode(B); //api call
             Assert.True(graph.Nodes.Count == 3);

             nodes = graph.RetrieveShapeNodes(ShapeType.LineSegment);
             Assert.True(nodes.Count == 1);
             Assert.True(nodes[0].InEdges.Count == 2);
             Assert.True(nodes[0].OutEdges.Count == 0);

             var lineObj = nodes[0].Shape as LineSegment;
             Assert.NotNull(lineObj);
             Assert.True(lineObj.Label.Equals("AB"));
             Assert.NotNull(lineObj.Pt1);
             Assert.True(lineObj.Pt1.Equals(A));
             Assert.NotNull(lineObj.Pt2);
             Assert.True(lineObj.Pt2.Equals(B));
             //Assert.True(lineObj.RelationStatus);
             Assert.True(lineObj.CachedSymbols.Count == 1);*/
        }
    
        
    
    }
}
