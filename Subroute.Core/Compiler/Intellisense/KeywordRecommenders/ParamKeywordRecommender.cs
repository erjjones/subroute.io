// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Subroute.Core.Compiler.Intellisense.KeywordRecommenders
{
    internal class ParamKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public ParamKeywordRecommender()
            : base(SyntaxKind.ParamKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            var token = context.TargetToken;

            if (token.Kind() == SyntaxKind.OpenBracketToken &&
                token.Parent.IsKind(SyntaxKind.AttributeList))
            {
                if (token.GetAncestor<PropertyDeclarationSyntax>() != null ||
                    token.GetAncestor<EventDeclarationSyntax>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
