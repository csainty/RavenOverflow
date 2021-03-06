﻿using System.Web.Mvc;
using AutoMapper;
using Raven.Client;
using RavenOverflow.Core.Services;
using RavenOverflow.Services;
using RavenOverflow.Web.Areas.Question.Controllers;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;
using RavenOverflow.Web.Models.AutoMapping;
using Xunit;

namespace RavenOverflow.Tests.Controllers
{
    // ReSharper disable InconsistentNaming

    public class QuestionControllerFacts : RavenDbTestBase
    {
        public QuestionControllerFacts()
        {
            // WebSite requires AutoMapper mappings.
            AutoMapperBootstrapper.ConfigureMappings();
            Mapper.AssertConfigurationIsValid();
        }

        [Fact]
        public void GivenAnInValidQuestion_Create_ReturnsAResultView()
        {
            using (IDocumentSession documentSession = DocumentStore.OpenSession())
            {
                // Arrange.
                IQuestionService questionService = new QuestionService(documentSession);
                var questionsController = new QuestionsController(documentSession, questionService);
                ControllerUtilities.SetUpControllerContext(questionsController);
                var createInputModel = new CreateInputModel
                                           {
                                               Content = "Some content",
                                               Subject = null,
                                               // RuRoh - dat ist missin'
                                               Tags = "tag1 tag2 tag3-3-3"
                                           };

                // Now pretend the model binding raised an error with the input model.
                questionsController.ModelState.AddModelError("key", "error message");

                // Act.
                var result = questionsController.Create(createInputModel) as ViewResult;

                // Assert.
                Assert.NotNull(result);
                Assert.Equal("Create", result.ViewName);
            }
        }

        [Fact]
        public void GivenAValidQuestionAndNoOneIsLoggedIn_Create_ReturnsAResultView()
        {
            using (IDocumentSession documentSession = DocumentStore.OpenSession())
            {
                // Arrange.
                IQuestionService questionService = new QuestionService(documentSession);
                var questionsController = new QuestionsController(documentSession, questionService);
                ControllerUtilities.SetUpControllerContext(questionsController);
                var createInputModel = new CreateInputModel
                                           {
                                               Content = "Some content",
                                               Subject = "Subject",
                                               Tags = "tag1 tag2 tag3-3-3"
                                           };
                // Act.
                var result = questionsController.Create(createInputModel) as ViewResult;

                // Assert.
                Assert.NotNull(result);
                Assert.Equal("Create", result.ViewName);
            }
        }

        [Fact]
        public void GivenAValidQuestionAndALoggedInUser_Create_AddsTheQuestionAndRedicects()
        {
            using (IDocumentSession documentSession = DocumentStore.OpenSession())
            {
                // Arrange.
                IQuestionService questionService = new QuestionService(documentSession);
                var questionsController = new QuestionsController(documentSession, questionService);
                ControllerUtilities.SetUpControllerContext(questionsController, "users/1");
                var createInputModel = new CreateInputModel
                                           {
                                               Content = "Some content",
                                               Subject = "Subject",
                                               Tags = "tag1 tag2 tag3-3-3"
                                           };
                // Act.
                var result = questionsController.Create(createInputModel) as RedirectToRouteResult;

                // Assert.
                Assert.NotNull(result);
                Assert.Equal("Index", result.RouteValues["action"]);
            }
        }

        // ReSharper restore InconsistentNaming
    }
}