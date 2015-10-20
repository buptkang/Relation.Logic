# Relation.Logic

This library provides a [constraint-solving](https://en.wikipedia.org/wiki/Constraint_satisfaction_problem) module based on any logical substitution module, such as [CSharp.Logic](https://github.com/buptkang/CSharp.Logic). The core data structure of this libary is a [constraint graph](https://en.wikipedia.org/wiki/Constraint_graph), which supports both unary and binary constraint inferences. If you need more references about CSP, constraint graph or CSP based search, please refer to [Artificial Intelligence: A Modern Approach](http://aima.cs.berkeley.edu/) and its corresponding [source repo](https://code.google.com/p/aima-python/).

=====================================================
| ShapeNode         |  Concrete    | Non-Concrete   | 
=====================================================
| Relation based    |     C        |       D        |
-----------------------------------------------------
|Non-Relation based |     A        |       B        |
=====================================================

## Code Examples

Test case structures:

0.Shape:    Testify all shape properties.
1.Relation: Testify relation unification procedure.
2.Graph:    Testify both relation unification and reification procedure.
2.Graph.Uncertainty: Testify uncertainty issue on the graph.
3.Problems: Model problem solving

Below problems can be found in in high-school geometry textbooks:

>Problem 1: Find the distance betweeen A(2,0) and B(5,4)?

>Problem 5 Given an equation 2y+2x-y+2x+4=0, graph this equation's corresponding shape? What is the slope of this line? 

Could the system tell users how to solve this problem and internal steps to derive that result?

Check out the unit test sample code to solve the above two problems, [Problem1 Solving](https://github.com/buptkang/Relation.Logic/blob/master/Test/2.Problems/Test.Scenario_1.cs), [Problem5 Solving](https://github.com/buptkang/Relation.Logic/blob/master/Test/2.Problems/Test_Scenario_5.cs).

## Build upon Relation.Logic

This libary can be used as for different purposes. The direct usage is to further build [cognitive tutor](https://en.wikipedia.org/wiki/Cognitive_tutor) to solve tasks, such as math problem-solving.  

This libary can also be used to solve CSP problems, such as [TSP](https://en.wikipedia.org/wiki/Travelling_salesman_problem) or [NQueen problem](https://en.wikipedia.org/wiki/Eight_queens_puzzle).

## License

Copyright (c) 2015 Bo Kang

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
