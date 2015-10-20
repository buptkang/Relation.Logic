=====================================================================================
//P(Relation|Label) AB can mean a line
//P(Line | RelationGraph, NList) X=2 can mean Line
==========================================================
Pattern Match Scenario 1:

Input sequence:
1: A(x,2) [Point]     => Point
2: x=2 [Goal, Line]   => Goal

Input sequence:
1: x=2 [Goal, Line]   => Line

Input sequence:
1: x=2 [Goal, Line]      => Line
2: A(x,2) [Point]        => Point 
Update: x=2 [Goal, Line] => Goal 

===========================================================

Pattern Match Scenario 2:

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: AB [Label]     => [Line, LineSegment]
4: User Input to solve uncertainty

Input sequence:
1: AB [Label]      => [Label]
2: A(2,3)          => Point
3: B(3,4)          => Point
Update: AB [Label] => [Line, LineSegment]
4: User Input to solve uncertainty

4: AB[Label]      => LineSegment
5: d[Label] => distance of LineSegment
===========================================================

Pattern Match Scenario 3:

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: AB[Label]      => [Line, LineSegment]
4: User Input to solve uncertainty:
   AB[Label]      => Line
5: d[Label]       => Label

Inference
1. Shape Entity:   e.g general form of line
2. Shape Property: the slope of line
3. Shape Relation: e.g given slope and intercept, construct the line.

Paramter Goal Constraints:
1.Parameter: Goal or Label, (optional ShapeType)
2.Parameter: Goal or ,      (optional ShapeType)
3.Parameter: (optional Goal or Label), ShapeType

------------------------------------

Examples:
------------------------------------
A: 
1. a point "A(2,3)"
2. a line  "2x-4y+1=0"
-------------------------------------
B:
1. a point "A(x,3)"
2. a line  "ax-4y+1=0"
-------------------------------------
Relation means the connection(edge) between graph node on the graph.
Relation depends on the symbolic characters of entity.


Assumption: Relation-based object can even exist without the relation.
Reason: eg. If a symbol "AB" specifically means a line, then the individual 
character can be updated later to represent its sub-element.

Eg input sequence:
1. A line relation-based object "AB" => no relation
2. A point A(2,3) => create relation between "A" and "AB"
3. A point B(3,4) => create relation between "B" and "AB"
Satisfy Update (A,B) 
=> Unify relation-based object to Non-Relation based object.

--------------------------------------
C:
true positive Assumption: two points: A(2,3) and B(3,4)
1. a line "AB" 

false negative assumption: two popints: C(2,3) and B(3,4)
1. a line "AB"

---------------------------------------
D:
true positive Assumption: two points: A(2,x) and B(3,4)
1. a line "AB" 

Line relation(reification process): 
given two points A(2,2) and B(3,3)-> AB
===============================================================

Input Example:

Pattern Match Scenario 1: (deterministic)

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: Hat(AB)[Line]  => Line

Input sequence:
1: Hat(AB) [Line]  => Line
2: A(2,3)          => Point
3: B(3,4)          => Point
