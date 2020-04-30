/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace JintUnity.SystemCompat.Linq.Expressions {


    public sealed class DefaultExpression : Expression {
        private readonly Type _type;

        internal DefaultExpression(Type type) : base((ExpressionType)51,type){
            _type = type;
        }

    }

    public partial class ExpressionEx {
        /// <summary>
        /// Creates an empty expression that has <see cref="SystemCompat.Void"/> type.
        /// </summary>
        /// <returns>
        /// A <see cref="DefaultExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to 
        /// <see cref="F:ExpressionType.Default"/> and the <see cref="P:Expression.Type"/> property set to <see cref="SystemCompat.Void"/>.
        /// </returns>
        public static DefaultExpression Empty() {
            return new DefaultExpression(typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="DefaultExpression"/> that has the <see cref="P:Expression.Type"/> property set to the specified type.
        /// </summary>
        /// <param name="type">A <see cref="SystemCompat.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="DefaultExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to 
        /// <see cref="F:ExpressionType.Default"/> and the <see cref="P:Expression.Type"/> property set to the specified type.
        /// </returns>
        public static DefaultExpression Default(Type type) {
            if (type == typeof(void)) {
                return Empty();
            }
            return new DefaultExpression(type);
        }
    }
}