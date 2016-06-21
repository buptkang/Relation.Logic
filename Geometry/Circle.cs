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

using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    using CSharpLogic;
    using System;

    [Serializable]
    public partial class Circle
    {
        public double Radius { get; set; }
        public Point CenterPt { get; set; }

        public Circle(Point center, double radius)
        {
            Radius = radius;
            CenterPt = center;
        }

        #region Utils

        public override bool Concrete
        {
            get { return CenterPt.Concrete; }
        }

        #endregion

        #region IEquatable
        //TODO
        #endregion



    }

    [Serializable]
    public partial class CircleSymbol : ShapeSymbol
    {
        public CircleSymbol(Circle _circle)
            : base(_circle)
        {

        }

        public override object RetrieveConcreteShapes()
        {
            var circle = Shape as Circle;
            Debug.Assert(circle != null);
            if (CachedSymbols.Count == 0) return this;
            return CachedSymbols.ToList();
        }

        public override object GetOutputType()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var circle = Shape as Circle;
            Debug.Assert(circle != null);

            var builder = new StringBuilder();
            builder.Append("(x");

            double dNum;
            bool isDouble = LogicSharp.IsDouble(circle.CenterPt.XCoordinate, out dNum);
            if (isDouble)
            {
                if (dNum > 0)
                {
                    builder.Append("-").Append(dNum);
                }
                else
                {
                    builder.Append("+").Append(Math.Abs(dNum));
                }
            }
            else
            {
                builder.Append("-").Append(circle.CenterPt.XCoordinate);
            }
            builder.Append(")^2+");

            builder.Append("(y");
            isDouble = LogicSharp.IsDouble(circle.CenterPt.YCoordinate, out dNum);
            if (isDouble)
            {
                if (dNum > 0)
                {
                    builder.Append("-").Append(dNum);
                }
                else
                {
                    builder.Append("+").Append(Math.Abs(dNum));
                }
            }
            else
            {
                builder.Append("-").Append(circle.CenterPt.YCoordinate);
            }
            builder.Append(")^2");
            builder.Append("=").Append(circle.Radius).Append("^2");

            return builder.ToString();
        }

        #region Symbolic Format

        public PointSymbol SymCentral
        {
            get
            {
                var circle = Shape as Circle;
                Debug.Assert(circle != null);
                return new PointSymbol(circle.CenterPt);
            }
        }

        public string SymRadius
        {
            get
            {
                var circle = Shape as Circle;
                Debug.Assert(circle != null);
                return circle.Radius.ToString();
            }
        }

       /* public string CircleStandardForm
        {
            get
            {
                if (CentralPt.XCoordinate.Equals(0.0) && CentralPt.YCoordinate.Equals(0.0))
                {
                    return string.Format("x^2+y^2={0}^2", Radius > 0d ? SymRadius : NegSymRadius);
                }
                else if ((!CentralPt.XCoordinate.Equals(0.0)) && CentralPt.YCoordinate.Equals(0.0))
                {
                    return string.Format("(x{0})^2+y^2={1}^2",
                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);
                }
                else if (CentralPt.XCoordinate.Equals(0.0) && (!CentralPt.YCoordinate.Equals(0.0)))
                {
                    return string.Format("x^2+(y{0})^2={1}^2",
                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);
                }
                else
                {
                    return string.Format("(x{0})^2+(y{1})^2={2}^2",
                        CentralPt.XCoordinate > 0d ? string.Format("-{0}", CentralPt.SymXCoordinate) : string.Format("+{0}", CentralPt.NegSymXCoordinate),
                        CentralPt.YCoordinate > 0d ? string.Format("-{0}", CentralPt.SymYCoordinate) : string.Format("+{0}", CentralPt.NegSymYCoordinate),
                        Radius > 0d ? SymRadius : NegSymRadius);
                }
            }
        }*/
        /*
                public string CircleGeneralForm
                {
                    get 
                    {
                        return string.Format("{0}x^2 {1}x {2}y^2 {3}y {4} = 0", SymA,
                              D > 0d ? string.Format("+{0}", SymD) : string.Format("-{0}", NegSymD),
                              B > 0d ? string.Format("+{0}", SymB) : string.Format("-{0}", NegSymB),
                              E > 0d ? string.Format("+{0}", SymE) : string.Format("-{0}", NegSymE),
                              F > 0d ? string.Format("+{0}", SymF) : string.Format("-{0}", NegSymF)); 
                    }
                }
         */
        #endregion


     /*   public static Expr GenerateCircleGeneralForm(Circle circle)
        {
            string str = String.Format("(x{0})^2 + (y{1})^2 = {2}^2",
                circle.CentralPt.XCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.XCoordinate)
                    : String.Format(" + {0}", -circle.CentralPt.XCoordinate),
                circle.CentralPt.YCoordinate > 0
                    ? String.Format(" - {0}", circle.CentralPt.YCoordinate)
                    : String.Format(" + {0}", -circle.CentralPt.YCoordinate),
                circle.Radius);
            return starPadSDK.MathExpr.Text.Convert(str);
        }

        public static Expr GenerateCircleTrace2(Circle circle)
        {
            string str = String.Format("CP = ({0}, {1}), R = {2}", circle.CentralPt.XCoordinate,
                circle.CentralPt.YCoordinate, circle.Radius);

            return starPadSDK.MathExpr.Text.Convert(str);
        }*/

    }

    //namespace AGSemantic.KnowledgeBase
    //{
    //    public sealed class Circle : QuadraticCurve
    //    {
    //        public Point CentralPt { get; set; }
    //        public double Radius { get; set; }
    //        public double Perimeter { get; set; }
    //        public double Area { get; set; }
    //    }

    //    public sealed class Ellipse : QuadraticCurve
    //    {
    //        public Point CentralPt { get; set; }

    //        public double RadiusAlongXAxis { get; set; }
    //        public double RadiusAlongYAxis { get; set; }
    //        public double FociDistance { get; set; }

    //        public Point LeftFoci { get; set; }
    //        public Point RightFoci { get; set; }
    //        //public double Perimeter { get; set; }
    //        //public double Area { get; set; }

    //        #region Symbolic Format

    //        public string SymRadiusA
    //        {
    //            get
    //            {
    //                if ((RadiusAlongXAxis % 1).Equals(0))
    //                {
    //                    return Int32.Parse(RadiusAlongXAxis.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return RadiusAlongXAxis.ToString();
    //                }
    //            }
    //        }

    //        public string SymRadiusB
    //        {
    //            get
    //            {
    //                if ((RadiusAlongYAxis % 1).Equals(0))
    //                {
    //                    return Int32.Parse(RadiusAlongYAxis.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return RadiusAlongYAxis.ToString();
    //                }                
    //            }
    //        }

    //        public string SymFociC
    //        {
    //            get
    //            {
    //                if ((FociDistance % 1).Equals(0))
    //                {
    //                    return Int32.Parse(FociDistance.ToString()).ToString();
    //                }
    //                else
    //                {
    //                    return FociDistance.ToString();
    //                }  
    //            }
    //        }

    //        public string EllipseStandardForm
    //        {
    //            get
    //            {
    //                return string.Format("x^2/{0}^2+y^2/{1}^2=1",SymRadiusA,SymRadiusB);
    //            }
    //        }

    //        public string SymRadiusAProperty
    //        {
    //            get
    //            {
    //                return string.Format("A = {0}", SymRadiusA);
    //            }
    //        }

    //        public string SymRadiusBProperty
    //        {
    //            get
    //            {
    //                return string.Format("B = {0}", SymRadiusB);
    //            }
    //        }

    //        public string SymFociCProperty
    //        {
    //            get { return string.Format("C = {0}", SymFociC); }
    //        }


    //        public string FociTrace1
    //        {
    //            get
    //            {
    //                return string.Format("C^2 = {0}^2 - {1}^2", SymRadiusA, SymRadiusB);
    //            }
    //        }

    //        public string FociTrace2
    //        {
    //            get
    //            {
    //                return string.Format("C^2 = {0}", Math.Pow(FociDistance, 2.0));
    //            }
    //        }

    //        public string FocalPoint1
    //        {
    //            get
    //            {
    //                return string.Format("FP1 = (-{0},0)", SymFociC);
    //            }
    //        }

    //        public string FocalPoint2
    //        {
    //            get { return string.Format("FP2 = ({0},0)", SymFociC); }
    //        }

    //        #endregion
    //    }
    //}






}
