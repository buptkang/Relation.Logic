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
    using System.Linq;
    using CSharpLogic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    public static class LineEquationExtension
    {
        public static bool IsLineEquation(this Equation eq, out LineSymbol ls, bool allowEval = true)
        {
            Debug.Assert(eq != null);
            Debug.Assert(eq.Rhs != null);
            ls = null;

            Line line;
            bool matched = SatisfySpecialForm(eq, out line);
            if (matched)
            {
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;
                if (eq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, eq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);
                return true;
            }

            matched = SatisfyLineSlopeInterceptForm(eq, out line);
            if (matched)
            {
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;
                if (eq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, eq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);
                return true;
            }

/*            if (!allowEval)
            {
                matched = SatisfyLineGeneralForm(eq, out line);
                if (matched)
                {
                    ls = new LineSymbol(line);
                    line.Label = eq.EqLabel;
                    return true;
                }
                matched = SatisfyLineSlopeInterceptForm(eq, out line);
                if (matched)
                {
                    ls = new LineSymbol(line);
                    line.Label = eq.EqLabel;
                    return true;
                }
                return false;
            }*/

            object obj;
            bool? result = eq.Eval(out obj, true, true); // without transitive equational rule.
            if (result != null) return false;

            if (eq.CachedEntities.Count != 1) return false;

            var outputEq = eq.CachedEntities.ToList()[0] as Equation;
            if (outputEq == null) return false;

            matched = SatisfySpecialForm(outputEq, out line);
            if (matched)
            {
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;

                if (outputEq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, outputEq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);
                return true;
            }

            //Equation Semantic Unification
            //general     form of line equation ax+by+c=0
            //point-slope form of line equation y = mx + b

         

            matched = SatisfyLineGeneralForm(outputEq, out line);
            if (matched)
            {
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;
                if (outputEq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, outputEq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }

                //Instruction Design
                TraceInstructionalDesign.FromLineGeneralFormToSlopeIntercept(ls);
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);
               
              /*  List<Tuple<object, object>> trace = eq.CloneTrace();               
                ls.Traces.AddRange(trace);
                List<Tuple<object, object>> lst = ls.Traces.Intersect(trace).ToList();
                if (lst.Count == 0) ls.Traces.AddRange(trace);*/
                return true;
            }


            matched = SatisfyLineSlopeInterceptForm(outputEq, out line);
            if (matched)
            {
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;
                if (outputEq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, outputEq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);

               /* List<Tuple<object,object>> trace = eq.CloneTrace();
                ls = new LineSymbol(line);
                line.Label = eq.EqLabel;
                ls.Traces.AddRange(trace);
                List<Tuple<object, object>> lst = ls.Traces.Intersect(trace).ToList();
                if (lst.Count == 0) ls.Traces.AddRange(trace);*/
                return true;
            }
            eq.ClearTrace();
            //eq.CachedEntities.Clear();

            return false;
        }

        private static bool SatisfySpecialForm(Equation equation, out Line line)
        {
            line = null;
          
            bool isRhsNum = LogicSharp.IsNumeric(equation.Rhs);
            if (!isRhsNum) return false;

            double dNum;
            LogicSharp.IsDouble(equation.Rhs, out dNum);

            double rhs = -1*dNum;
            object xCoeff;
            bool result = IsXTerm(equation.Lhs, out xCoeff);
            if (result)
            {
                line = new Line(null, xCoeff, null, rhs);
                return true;
            }
            object yCoeff;
            result = IsYTerm(equation.Lhs, out yCoeff);
            if (result)
            {
                line = new Line(null, null, yCoeff, rhs);
                return true;
            }
            result = IsXYTerm(equation.Lhs, out xCoeff, out yCoeff);
            if (result)
            {
                line = new Line(null, xCoeff, yCoeff, rhs);
                return true;
            }
            return false;
        }

        private static bool SatisfyLineGeneralForm(Equation equation, out Line line)
        {
            line = null;
            var lhsTerm = equation.Lhs as Term;
            if (lhsTerm == null) return false;

            var rhsZero = equation.Rhs.Equals(0);

            if (!rhsZero) return false;


            line = lhsTerm.UnifyLineTerm();
            return line != null;
        }

        private static bool SatisfyLineSlopeInterceptForm(Equation equation, out Line line)
        {
            line = null;
            var lhsVar = equation.Lhs as Var;
            if (lhsVar != null && (lhsVar.Equals(yTerm) || lhsVar.Equals(YTerm)))
            {
                var rhsTerm = equation.Rhs as Term;
                if (rhsTerm == null) return false;

                line = rhsTerm.UnifyLineSlopeInterceptTerm();
                return line != null;
            }
            return false;
        }

        #region Slope-Intercept Form

        private static Line UnifyLineSlopeInterceptTerm(this Term term)
        {
            if (term.Op.Method.Name.Equals("Add"))
            {
                var argLst = term.Args as List<object>;
                if (argLst == null) return null;
                if (argLst.Count > 2)
                {
                    return null;
                }
                var dict = new Dictionary<string, object>();
                return UnifyLineSlopeInterceptTerm(argLst, 0, dict);
            }
            else
            {
                var lst = new List<object>() { term };
                var dict = new Dictionary<string, object>();
                return UnifyLineSlopeInterceptTerm(lst, 0, dict);
            }
        }

        private static Line UnifyLineSlopeInterceptTerm(List<object> args, int index,
                                        Dictionary<string, object> dict)
        {

            if (index == args.Count)
            {
                object slope = dict.ContainsKey(A) ? dict[A] : null;
                object intercept = dict.ContainsKey(C) ? dict[C] : null;
                if (slope == null) return null;
                return new Line(slope, intercept);
            }

            object currArg = args[index];
            bool finalResult = false;

            object coeff;
            bool result = IsXTerm(currArg, out coeff);
            if (result)
            {
                if (dict.ContainsKey(A))
                    throw new Exception("cannot contain two terms with same var");
                dict.Add(A, coeff);
                finalResult = true;
            }
            double d;
            result = LogicSharp.IsDouble(currArg, out d);
            if (result)
            {
                if (dict.ContainsKey(C))
                {
                    dict[C] = d;
                }
                else
                {
                    dict.Add(C, d);
                }
                finalResult = true;
            }

            if (currArg is string)
            {
                if (dict.ContainsKey(C))
                {
                    dict[C] = currArg;
                }
                else
                {
                    dict.Add(C, new Var(currArg));
                }

                finalResult = true;
            }

            var variable = currArg as Var;
            if(variable != null)
            {
                if (dict.ContainsKey(C))
                {
                    dict[C] = variable;
                }
                else
                {
                    dict.Add(C, variable);                    
                }

                finalResult = true;
            }

            if (finalResult)
            {
                return UnifyLineSlopeInterceptTerm(args, index + 1, dict);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region General Form

        private readonly static List<string> SpecialLabels = new List<string>()
        {
            "X", "x", "Y","y"
        };

        private static readonly List<Var> SpecialVars = new List<Var>()
        {
            new Var('x'),
            new Var('X'),
            new Var('y'),
            new Var('Y')
        };

        public static Regex xRegex = new Regex(@"x$");
        public static Regex XRegex = new Regex(@"X$");
        public static Regex yRegex = new Regex(@"y$");
        public static Regex YRegex = new Regex(@"Y$");

        private static readonly string A = "a";
        private static readonly string B = "b";
        private static readonly string C = "c";

        private readonly static Var XTerm = new Var('X');
        private readonly static Var xTerm = new Var('x');
        private readonly static Var YTerm = new Var('Y');
        private static readonly Var yTerm = new Var('y');

        private static Line UnifyLineTerm(this Term term)
        {
            if (term.Op.Method.Name.Equals("Add"))
            {
                var argLst = term.Args as List<object>;
                if (argLst == null) return null;
                if (argLst.Count > 3)
                {
                    return null;
                }
                var dict = new Dictionary<string, object>();
                return UnifyLineTerm(argLst, 0, dict);
            }
            else
            {
                var lst = new List<object>() { term };
                var dict = new Dictionary<string, object>();
                return UnifyLineTerm(lst, 0, dict);
            }
        }

        private static Line UnifyLineTerm(List<object> args, int index,
                                        Dictionary<string, object> dict)
        {

            if (index == args.Count)
            {
                object xCord = dict.ContainsKey(A) ? dict[A] : null;
                object yCord = dict.ContainsKey(B) ? dict[B] : null;
                object cCord = dict.ContainsKey(C) ? dict[C] : null;
                return new Line(xCord, yCord, cCord);
            }

            object currArg = args[index];
            bool finalResult = false;

            object coeff;
            bool result = IsXTerm(currArg, out coeff);
            if (result)
            {
                if (!dict.ContainsKey(A))
                {
                    dict.Add(A, coeff);
                    finalResult = true;                    
                }
            }
            result = IsYTerm(currArg, out coeff);
            if (result)
            {
                if (!dict.ContainsKey(B))
                {
                    dict.Add(B, coeff);
                    finalResult = true;                    
                }
            }
            double d;
            result = LogicSharp.IsDouble(currArg, out d);
            if (result)
            {
                if (!dict.ContainsKey(C))
                {
                    dict.Add(C, d);
                    finalResult = true;                    
                }
            }

            if (currArg is string)
            {
                if (!dict.ContainsKey(C))
                {
                    dict.Add(C, new Var(currArg));
                    finalResult = true;                    
                }
            }

            if (finalResult)
            {
                return UnifyLineTerm(args, index + 1, dict);
            }
            else
            {
                return null;
            }
        }

        private static bool IsXTerm(object obj, out object coeff)
        {
            coeff = null;
            var variable = obj as Var;
            if (variable != null)
            {
                if (variable.Equals(XTerm) || variable.Equals(xTerm))
                {
                    coeff = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            var term = obj as Term;
            if (term != null && term.Op.Method.Name.Equals("Multiply"))
            {
                var lst = term.Args as List<object>;
                Debug.Assert(lst != null);
                var lastObj = lst[lst.Count - 1];
                if (lastObj.Equals(xTerm) || lastObj.Equals(XTerm))
                {
                    if (lst.Count == 2)
                    {
                        coeff = lst[0].ToString();
                        return true;
                    }
                    else
                    {
                        coeff = new Term(Expression.Multiply, lst.GetRange(0, lst.Count - 1));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool IsYTerm(object obj, out object coeff)
        {
            coeff = null;
            var variable = obj as Var;
            if (variable != null)
            {
                if (variable.Equals(YTerm) || variable.Equals(yTerm))
                {
                    coeff = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            var term = obj as Term;
            if (term != null && term.Op.Method.Name.Equals("Multiply"))
            {
                var lst = term.Args as List<object>;
                Debug.Assert(lst != null);
                var lastObj = lst[lst.Count - 1];
                if (lastObj.Equals(yTerm) || lastObj.Equals(YTerm))
                {
                    if (lst.Count == 2)
                    {
                        coeff = lst[0].ToString();
                        return true;
                    }
                    else
                    {
                        coeff = new Term(Expression.Multiply, lst.GetRange(0, lst.Count - 1));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool IsXYTerm(object obj, out object xCoeff, out object yCoeff)
        {
            xCoeff = null;
            yCoeff = null;
            var term = obj as Term;
            if (term == null) return false;
            var argLst = term.Args as List<object>;
            if (argLst == null) return false;
            if (argLst.Count != 2) return false;

            var arg1 = argLst[0];
            var arg2 = argLst[1];

            if (IsXTerm(arg1, out xCoeff) && IsYTerm(arg2, out yCoeff))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
