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
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class PointAcronym
    {
        public const string X = "x";
        public const string X1 = "X";
        public const string Y = "y";
        public const string Y1 = "Y";
    }

    public static class LineAcronym
    {
        public const string A = "A";
        public const string A1 = "a";

        public const string B = "B";
        public const string B1 = "b";

        public const string C = "C";
        public const string C1 = "c";

        public const string Slope1 = "m";
        public const string Slope2 = "s";
        public const string Slope3 = "S";

        public const string Intercept1 = "k";
        public const string Intercept2 = "I";
        public const string Intercept3 = "i";

        public const string GeneralForm1 = "lineG";
        public const string GeneralForm2 = "line-G";
        public const string GeneralForm3 = "line-ABC";
        public const string GeneralForm4 = "LineGeneralForm";

        public const string SlopeInterceptForm1 = "line-mk";
        public const string SlopeInterceptForm2 = "lineS";
        public const string SlopeInterceptForm3 = "si";
        public const string GraphLine = "graph";

        public static bool EqualSlopeInterceptFormLabels(string label)
        {
            Debug.Assert(label != null);
            return label.Equals(SlopeInterceptForm1) ||
                   label.Equals(SlopeInterceptForm2);
        }

        public static bool EqualGeneralFormLabels(string label)
        {
            Debug.Assert(label != null);
            return label.Equals(GeneralForm1) ||
                   label.Equals(GeneralForm2) ||
                   label.Equals(GeneralForm3) ||
                   label.Equals(GeneralForm4);
        }

        public static List<string> Properties = new List<string>()
        {
            A, A1, B, B1, C, C1, Slope1, Slope2, Slope3, Intercept1, Intercept2, Intercept3
        };
    }

    public static class LineSegmentAcronym
    {
        public const string Distance1 = "d";
        public const string Distance2 = "D";

        public static bool EqualDistanceLabel(string label)
        {
            Debug.Assert(label != null);
            return label.Equals(Distance1) ||
                   label.Equals(Distance2);
        }
    }

    public static class DefaultLabels
    {
        public const string LineDefault = "line";
        public const string PointDefault = "point";
        public const string CircleDefault = "circle";

        public static bool EqualDefaultLabel(string label)
        {
            Debug.Assert(label != null);

            return label.StartsWith(LineDefault)
                   || label.StartsWith(PointDefault)
                   || label.StartsWith(CircleDefault);

            /*
                        return label.Equals(LineDefault) || label.Equals(PointDefault)
                               || label.Equals(CircleDefault);
            */
        }
    }

}
