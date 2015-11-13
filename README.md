# Relation.Logic

<p align="justify">
This library implements [constraint-solving](https://en.wikipedia.org/wiki/Constraint_satisfaction_problem) that is based on a logical module, such as [CSharp.Logic](https://github.com/buptkang/CSharp.Logic). The core data structure is a [constraint graph](https://en.wikipedia.org/wiki/Constraint_graph), which supports both unary and binary constraint inferences. If you need more references about CSP, constraint graph or CSP based search, please refer to [Artificial Intelligence: A Modern Approach](http://aima.cs.berkeley.edu/) and its corresponding [source repo](https://code.google.com/p/aima-python/).
</p>

## Code Examples

1. Consider one algebra-and-geometry problem: Find the distance betweeen A(2,0) and B(5,4)? Check out unit test to automate reason this question and generate intermediate steps to solve this question? [Test.Scenario_01.cs](https://github.com/buptkang/Relation.Logic/blob/master/Test/3.Problems/Test.Scenario_01.cs).

2. Consider another algebra-and-geometry problem: Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape? What is the slope of this line? Check out unit test to automate reason this question and generate intermediate steps to solve this question? [Test_Scenario_05.cs](https://github.com/buptkang/Relation.Logic/blob/master/Test/3.Problems/Test.Scenario_05.cs).

## License

Copyright (c) 2015 Bo Kang
<p align="justify">
Licensed under the Apache License, Version 2.0 (the "License") you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
</p>
