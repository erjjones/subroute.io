// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Subroute.Core.Compiler.Intellisense.KeywordRecommenders
{
    internal class ULongKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public ULongKeywordRecommender()
            : base(SyntaxKind.ULongKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            var syntaxTree = context.SyntaxTree;
            return
                context.IsAnyExpressionContext ||
                context.IsDefiniteCastTypeContext ||
                context.IsStatementContext ||
                context.IsGlobalStatementContext ||
                context.IsObjectCreationTypeContext ||
                context.IsGenericTypeArgumentContext ||
                context.IsEnumBaseListContext ||
                context.IsIsOrAsTypeContext ||
                context.IsLocalVariableDeclarationContext ||
                context.IsFixedVariableDeclarationContext ||
                context.IsParameterTypeContext ||
                context.IsPossibleLambdaOrAnonymousMethodParameterTypeContext ||
                context.IsImplicitOrExplicitOperatorTypeContext ||
                context.IsPrimaryFunctionExpressionContext ||
                context.IsCrefContext ||
                syntaxTree.IsAfterKeyword(position, SyntaxKind.ConstKeyword, cancellationToken) ||
                syntaxTree.IsAfterKeyword(position, SyntaxKind.StackAllocKeyword, cancellationToken) ||
                context.IsDelegateReturnTypeContext ||
                syntaxTree.IsGlobalMemberDeclarationContext(position, SyntaxKindSet.AllGlobalMemberModifiers, cancellationToken) ||
                context.IsMemberDeclarationContext(
                    validModifiers: SyntaxKindSet.AllMemberModifiers,
                    validTypeDeclarations: SyntaxKindSet.ClassInterfaceStructTypeDeclarations,
                    canBePartial: false,
                    cancellationToken: cancellationToken);
        }
    }
}
