﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Templates.Analyzer.RuleEngines.JsonEngine.Expressions;
using Microsoft.Azure.Templates.Analyzer.RuleEngines.JsonEngine.Operators;
using Microsoft.Azure.Templates.Analyzer.Types;
using Microsoft.Azure.Templates.Analyzer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Templates.Analyzer.RuleEngines.JsonEngine.UnitTests
{
    [TestClass]
    public class NotExpressionTests
    {
        [DataTestMethod]
        [DataRow(null, "", false, DisplayName = "Not evaluates to false")]
        [DataRow(null, "some.path", true, DisplayName = "Not evaluates to true")]
        [DataRow("someResource/type", "some.path", true, DisplayName = "Resource Type specified, valid path, not evaluates to true")]
        public void Evaluate_SingleLeafExpression_ReturnsResultsOfOperatorEvaluation(string resourceType, string path, bool expectedEvaluationResult)
        {
            // Arrange
            var mockJsonPathResolver = new Mock<IJsonPathResolver>();
            var mockLineResolver = new Mock<ILineNumberResolver>().Object;

            var mockOperator = new Mock<LeafExpressionOperator>().Object;
            mockOperator.IsNegative = true;

            var mockLeafExpression = new Mock<LeafExpression>(mockLineResolver, mockOperator, new ExpressionCommonProperties { ResourceType = resourceType, Path = path });

            var jsonRuleResult = new JsonRuleResult
            {
                Passed = expectedEvaluationResult
            };

            mockJsonPathResolver
                .Setup(s => s.Resolve(It.Is<string>(p => p == path)))
                .Returns(new List<IJsonPathResolver> { mockJsonPathResolver.Object });

            if (!string.IsNullOrEmpty(resourceType))
            {
                mockJsonPathResolver
                    .Setup(s => s.ResolveResourceType(It.Is<string>(type => type == resourceType)))
                    .Returns(new List<IJsonPathResolver> { mockJsonPathResolver.Object });
            }

            var leafExpressionresults = new JsonRuleResult[] { jsonRuleResult };

            mockLeafExpression
                .Setup(s => s.Evaluate(mockJsonPathResolver.Object))
                .Returns(new JsonRuleEvaluation(mockLeafExpression.Object, expectedEvaluationResult, leafExpressionresults));

            var notExpression = new NotExpression(mockLeafExpression.Object, new ExpressionCommonProperties { ResourceType = resourceType, Path = path });

            // Act
            var evaluation = notExpression.Evaluate(jsonScope: mockJsonPathResolver.Object);

            // Assert
            Assert.AreEqual(expectedEvaluationResult, evaluation.Passed);
            Assert.AreEqual(expectedEvaluationResult, evaluation.Results.First().Passed);

            Assert.IsTrue(mockLeafExpression.Object.Operator.IsNegative);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Evaluate_NullScope_ThrowsException()
        {
            var mockLineResolver = new Mock<ILineNumberResolver>().Object;
            var mockOperator = new Mock<LeafExpressionOperator>().Object;
            var mockLeafExpression = new Mock<LeafExpression>(mockLineResolver, mockOperator, new ExpressionCommonProperties { ResourceType = "", Path = "" });

            new NotExpression(mockLeafExpression.Object, new ExpressionCommonProperties()).Evaluate(jsonScope: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullExpressions_ThrowsException()
        {
            new NotExpression(null, new ExpressionCommonProperties());
        }
    }
}
