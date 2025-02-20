﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Templates.Analyzer.RuleEngines.JsonEngine.Expressions;
using Microsoft.Azure.Templates.Analyzer.Utilities;
using Newtonsoft.Json;

namespace Microsoft.Azure.Templates.Analyzer.RuleEngines.JsonEngine.Schemas
{
    /// <summary>
    /// The schema for not expression in JSON rules.
    /// </summary>
    internal class NotExpressionDefinition : ExpressionDefinition
    {
        /// <summary>
        /// Gets or sets the expressions found in not.
        /// </summary>
        [JsonProperty]
        public ExpressionDefinition Not { get; set; }

        /// <summary>
        /// Creates a <see cref="NotExpression"/> capable of evaluating JSON using the expressions specified in the JSON rule.
        /// </summary>
        /// <param name="jsonLineNumberResolver">An <see cref="ILineNumberResolver"/> to
        /// pass to the created <see cref="Expression"/>.</param>
        /// <param name="isNegative">Whether to negate the evaluation.</param>
        /// <returns>The NotExpression.</returns>
        public override Expression ToExpression(ILineNumberResolver jsonLineNumberResolver, bool isNegative = true)
            => new NotExpression(this.Not.ToExpression(jsonLineNumberResolver, true), GetCommonProperties(jsonLineNumberResolver));

        /// <summary>
        /// Validates the <see cref="NotExpressionDefinition"/> for valid syntax
        /// </summary>
        internal override void Validate()
        {
            if (this.Not == null)
            {
                throw new JsonException($"Null expressions are not valid. Please specify an expression in the not operator.");
            }
        }
    }
}
