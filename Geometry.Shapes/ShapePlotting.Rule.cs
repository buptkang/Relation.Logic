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

using CSharpLogic;

namespace AlgebraGeometry
{
    public class PlottingRule
    {
        public static string PlottingStrategy = "Consider to write existing facts onto the sketch paper";

        public static string Plot(ShapeSymbol ss)
        {
            if (ss.Shape.Concrete)
            {
                return string.Format("Plot shape {0} onto the geometrical side", ss);                
            }
            else
            {
                return string.Format("Write shape {0} onto the algebraic side", ss); 
            }
        }

        public static string Plot(EqGoal eqGoal)
        {
            return string.Format("Write Property {0} onto the algebraic side", eqGoal); 
        }
    }
}
