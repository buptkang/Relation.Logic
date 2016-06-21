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

namespace AlgebraGeometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSharpLogic;

    [Serializable]
    public partial class Point : Shape
    {
        private object _xCoord;
        private object _yCoord;

        public object XCoordinate
        {
            get
            {
                return _xCoord;
            }
            set
            {
                _xCoord = value;
                NotifyPropertyChanged("XCoordinate");
            }
        }

        public object YCoordinate
        {
            get
            {
                return _yCoord;
            }
            set
            {
                _yCoord = value;
                NotifyPropertyChanged("YCoordinate");
            }
        }

        public Point(string label, object xcoordinate, object ycoordinate)
            : base(ShapeType.Point, label)
        {
            _xCoord = xcoordinate;
            _yCoord = ycoordinate;

            double d; 
            if (LogicSharp.IsDouble(xcoordinate, out d))
            {
                _xCoord = Math.Round(d, 1);
            }

            if (LogicSharp.IsDouble(ycoordinate, out d))
            {
                _yCoord = Math.Round(d, 1);
            }
        }

        public Point(object xcoordinate, object ycoordinate)
            : this(null, xcoordinate, ycoordinate)
        { 
        }

        #region Pass by reference experiment, not used in the upper level

        public bool AddXCoord(object x)
        {
            if (LogicSharp.IsNumeric(x))
            {
                Properties.Add(XCoordinate, x);
                return true;
            }
            return false;
        }

        public bool AddYCoord(object y)
        {
            if (LogicSharp.IsNumeric(y))
            {
                Properties.Add(YCoordinate, y);
                return true;
            }
            return false;
        }

        #endregion 

        public override bool Concrete
        {
            get { return !Var.ContainsVar(XCoordinate) && !Var.ContainsVar(YCoordinate); }
        }

        public override List<Var> GetVars()
        {
            var lst = new List<Var>();
            lst.Add(Var.GetVar(XCoordinate));
            lst.Add(Var.GetVar(YCoordinate));
            return lst;
        }

        #region IEqutable

        public override bool Equals(object obj)
        {
            var shape = obj as Shape;
            if (shape != null)
            {
                return Equals(shape);
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(Shape other)
        {
            if (other is Point)
            {
                var pt = other as Point;
                if (XCoordinate.Equals(pt.XCoordinate) && YCoordinate.Equals(pt.YCoordinate))
                {
                    return true;
                }
                if (!XCoordinate.Equals(pt.XCoordinate))
                {
                    return false;
                }
                if(!YCoordinate.Equals(pt.YCoordinate))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return XCoordinate.GetHashCode() ^ YCoordinate.GetHashCode();
        }

        #endregion

        public PointType InputType { get; set; } 
        public override object GetInputType() { return InputType;  }        
    }

    public enum PointType
    {
        Relation,
        General
    }

    [Serializable]
    public partial class PointSymbol : ShapeSymbol
    {
        public override bool UnifyProperty(string label, out object obj)
        {
            return this.Unify(label, out obj);
        }

        public override bool UnifyExplicitProperty(EqGoal goal)
        {
            var goalLabel = goal.Lhs.ToString();
            Debug.Assert(goalLabel != null);
            if (Shape.Concrete) return false;
            if (SymXCoordinate.Equals(goalLabel)) return true;
            if (SymYCoordinate.Equals(goalLabel)) return true;
            return false;
        }

        public override bool UnifyProperty(EqGoal goal, out object obj)
        {
            //Point does not have any backward solving procedure.
            obj = null;
            return false;
        }

        public override bool UnifyShape(ShapeSymbol ss)
        {
            return false;
        }

        public override object RetrieveConcreteShapes()
        {
            if (CachedSymbols.Count == 0) return this;
            return CachedSymbols.ToList();
        }

        public PointSymbol(Point pt) : base(pt)
        {

        }

        public string SymXCoordinate
        {
            get
            {
                var pt = Shape as Point;
                if (pt.Properties.ContainsKey(pt.XCoordinate))
                {
                    object value = pt.Properties[pt.XCoordinate];
                    return value.ToString();
                }
                else
                {
                    return pt.XCoordinate.ToString();
                }
            }
        }

        public string SymYCoordinate
        {
            get
            {
                var pt = Shape as Point;
                if (pt.Properties.ContainsKey(pt.YCoordinate))
                {
                    object value = pt.Properties[pt.YCoordinate];
                    return value.ToString();
                }
                else
                {
                    return pt.YCoordinate.ToString();
                }
            }
        }

        public override string ToString()
        {
            if (Shape.Label != null)
            {
                return String.Format("{0}({1},{2})", Shape.Label, SymXCoordinate, SymYCoordinate);
            }
            else
            {
                return String.Format("({0},{1})", SymXCoordinate, SymYCoordinate);
            }
        }

        public PointType OutputType { get; set; }
        public override object GetOutputType() { return OutputType; }

        public override bool ApproximateMatch(object obj)
        {
            var ps = obj as PointSymbol;
            var ls = obj as LineSegmentSymbol;

            if (ps != null)
            {
                var pShape = Shape as Point;
                if (pShape == null) return false;
                var pShape1 = ps.Shape as Point;
                if (pShape1 == null) return false;

                bool cond1 = LogicSharp.NumericEqual(pShape.XCoordinate, pShape1.XCoordinate);
                bool cond2 = LogicSharp.NumericEqual(pShape.YCoordinate, pShape1.YCoordinate);
                if (cond1 && cond2) return true;

                if (CachedSymbols.Count == 0) return false;

                foreach (var temp in CachedSymbols)
                {
                    var cachedPt = temp as PointSymbol;
                    Debug.Assert(cachedPt != null);
                    bool inResult = cachedPt.ApproximateMatch(obj);
                    if (inResult) return true;
                }                
            }

            if (ls != null)
            {
                var lineSeg = ls.Shape as LineSegment;
                Debug.Assert(lineSeg != null);
                bool cond1 = ApproximateMatch(new PointSymbol(lineSeg.Pt1));
                bool cond2 = ApproximateMatch(new PointSymbol(lineSeg.Pt2));
                if (cond1 || cond2) return true;
            }

            return false;
        }
    }
}