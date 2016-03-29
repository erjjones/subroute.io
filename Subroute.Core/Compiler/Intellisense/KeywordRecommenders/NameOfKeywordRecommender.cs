// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Subroute.Core.Compiler.Intellisense.KeywordRecommenders
{
    internal class NameOfKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public NameOfKeywordRecommender()
            : base(SyntaxKind.NameOfKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.IsAnyExpressionContext ||
                context.IsStatementContext ||
                context.IsGlobalStatementContext ||
                IsAttributeArgumentContext(context);
        }

        private bool IsAttributeArgumentContext(CSharpSyntaxContext context)
        {
            return
                context.IsAnyExpressionContext &&
                context.LeftToken.GetAncestor<AttributeSyntax>() != null;
        }
    }
}
