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

    public static class CircleEquationExtension
    {
        public static bool IsCircleEquation(
            this Equation eq, 
            out CircleSymbol cs, bool allowEval = false)
        {
            Debug.Assert(eq != null);
            Debug.Assert(eq.Rhs != null);
            cs = null;
            Circle circle;
            bool matched = SatisfySpecialForm(eq, out circle);
            if (matched)
            {
                cs = new CircleSymbol(circle);
                circle.Label = eq.EqLabel;
                #region TODO Trace
                /*                if (eq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, eq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);*/
                #endregion
                return true;
            }

            object obj;
            bool? result = eq.Eval(out obj, true, true); // without transitive equational rule.
            if (result != null) return false;

            var gEq = obj as Equation;
            if (gEq == null) return false;
            matched = SatisfyGeneralForm(gEq, out circle);
            if (matched)
            {
                cs = new CircleSymbol(circle);
                circle.Label = eq.EqLabel;
                #region TODO Trace
                /*                if (eq.Traces.Count == 1)
                {
                    var strategy = "Generate a line by manipulating algebraic equation.";
                    var newTrace = new Tuple<object, object>(strategy, eq.Traces[0].Item2);
                    ls.Traces.Add(newTrace);
                    //ls.ImportTrace(eq);
                }
                TraceInstructionalDesign.LineSlopeIntercepToGraph(ls);*/
                #endregion
                return true;
            }


            return false;
        }

        private static bool SatisfyGeneralForm(Equation equation,
            out Circle circle)
        {
            circle = null;
            var term = equation.Lhs as Term;

            if (term != null && term.Op.Method.Name.Equals("Add"))
            {
                var lst = term.Args as List<object>;
                if (lst != null && lst.Count == 3)
                {
                    bool isNum = LogicSharp.IsNumeric(lst[2]);

                    if (isNum)
                    {
                        double dNum;
                        LogicSharp.IsDouble(lst[2], out dNum);
                        dNum *= -1;
                        object coeffX, coeffY;
                        bool xTerm1 = IsXSquareTerm(lst[0], out coeffX);
                        bool yTerm1 = IsYSquareTerm(lst[1], out coeffY);
                        if (xTerm1 && yTerm1)
                        {
                            var pt = new Point(coeffX, coeffY);
                            circle = new Circle(pt, Math.Pow(dNum, 0.5));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool SatisfySpecialForm(Equation equation, out Circle circle)
        {
            circle = null;

            bool isRhsNum = LogicSharp.IsNumeric(equation.Rhs);
            if (!isRhsNum) return false;
            double dNum;
            LogicSharp.IsDouble(equation.Rhs, out dNum);
            var term = equation.Lhs as Term;
            if (term == null) return false;
            if (!term.Op.Method.Name.Equals("Add")) return false;
            var lst = term.Args as List<object>;
            if (lst == null || lst.Count != 2) return false;

            object coeffX, coeffY;
            bool xTerm = IsXSquareTerm(lst[0], out coeffX);
            bool yTerm = IsYSquareTerm(lst[1], out coeffY);
            if (xTerm && yTerm)
            {
                var pt = new Point(coeffX, coeffY);
                circle = new Circle(pt, Math.Pow(dNum,0.5));
                return true;
            }
            xTerm = false;
            yTerm = false;
            xTerm = IsXSquareTerm(lst[1], out coeffX);
            yTerm = IsYSquareTerm(lst[0], out coeffY);
            if (xTerm && yTerm)
            {
                var pt = new Point(coeffX, coeffY);
                circle = new Circle(pt, Math.Pow(dNum, 0.5));
                return true;
            }
            return false;
        }

        private static bool IsXSquareTerm(object obj, out object coeff)
        {
            coeff = null;
            var term = obj as Term;
            if (term == null) return false;
            if (!term.Op.Method.Name.Equals("Power")) return false;
            var lst = term.Args as List<object>;
            if (lst == null) return false;
            if (lst[1].ToString().Equals("2.0") || lst[1].ToString().Equals("2"))
            {
                object lst0 = lst[0];
                bool isXTerm = IsXTerm(lst0, out coeff);
                if (isXTerm && coeff.Equals(1))
                {
                    coeff = 0;
                    return true;
                }
                isXTerm = false;
                isXTerm = IsXWithConstTerm(lst0, out coeff);
                if (isXTerm) return true;
            }
            return false;
        }

        private static bool IsYSquareTerm(object obj, out object coeff)
        {
            coeff = null;
            var term = obj as Term;
            if (term == null) return false;
            if (!term.Op.Method.Name.Equals("Power")) return false;
            var lst = term.Args as List<object>;
            if (lst == null) return false;
            if (lst[1].ToString().Equals("2.0") || lst[1].ToString().Equals("2"))
            {
                object lst0 = lst[0];
                bool isYTerm = IsYTerm(lst0, out coeff);
                if (isYTerm && coeff.Equals(1))
                {
                    coeff = 0;
                    return true;
                }
                isYTerm = false;
                isYTerm = IsYWithConstTerm(lst0, out coeff);
                if (isYTerm) return true;
            }
            return false;
        }

        private static bool IsXWithConstTerm(object obj, out object coeff)
        {
            coeff = null;
            var term = obj as Term;
            if (term == null) return false;

            if (term.Op.Method.Name.Equals("Add") ||
                term.Op.Method.Name.Equals("Substract"))
            {
                var lst = term.Args as List<object>;
                if (lst == null) return false;
                bool isXTerm = IsXTerm(lst[0], out coeff);
                if (coeff ==null || !coeff.ToString().Equals("1")) return false;
                if (!isXTerm) return false;
                bool isRhsNum = LogicSharp.IsNumeric(lst[1]);
                if (!isRhsNum) return false;
                double dNum;
                LogicSharp.IsDouble(lst[1], out dNum);
                if (term.Op.Method.Name.Equals("Add"))
                {
                    dNum *= -1;
                }
                coeff = dNum;
                return true;
            }
            return false;
        }

        private static bool IsYWithConstTerm(object obj, out object coeff)
        {
            coeff = null;
            var term = obj as Term;
            if (term == null) return false;

            if (term.Op.Method.Name.Equals("Add") ||
                term.Op.Method.Name.Equals("Substract"))
            {
                var lst = term.Args as List<object>;
                if (lst == null) return false;
                bool isYTerm = IsYTerm(lst[0], out coeff);
                if (coeff == null || !coeff.ToString().Equals("1")) return false;
                if (!isYTerm) return false;
                bool isRhsNum = LogicSharp.IsNumeric(lst[1]);
                if (!isRhsNum) return false;
                double dNum;
                LogicSharp.IsDouble(lst[1], out dNum);
                if (term.Op.Method.Name.Equals("Add"))
                {
                    dNum *= -1;
                }
                coeff = dNum;
                return true;
            }
            return false;
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

        private readonly static Var XTerm = new Var('X');
        private readonly static Var xTerm = new Var('x');
        private readonly static Var YTerm = new Var('Y');
        private static readonly Var yTerm = new Var('y');
    }
}
