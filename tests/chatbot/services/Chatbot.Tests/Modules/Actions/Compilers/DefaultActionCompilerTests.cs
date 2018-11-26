namespace Chatbot.Tests.Modules.Actions.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Modules.Actions.Compilers;
    using Chatbot.Modules.Actions.Models;
    using Xunit;
    using Action = Chatbot.Modules.Actions.Models.Action;

    [Trait("Modules", "Compilers")]
    public class DefaultActionCompilerTests
    {
        private readonly DefaultActionCompiler actionCompiler;

        public DefaultActionCompilerTests()
        {
            actionCompiler = new DefaultActionCompiler();
        }

        [Fact]
        public void Compile_ActionChainIsNull_ReturnEmptyCommand()
        {
            Assert.Empty(actionCompiler.Compile(null, null));
        }

        [Fact]
        public void Compile_ActionChainIsEmpty_ReturnEmptyCommand()
        {
            Assert.Empty(actionCompiler.Compile(Enumerable.Empty<Action>(), null));
        }

        [Fact]
        public void Compile_Always_ReturnCompiledCommand()
        {
            var actions = new List<Action>
            {
                new Action(1, "View Outstanding Report", "/os", 10, 1, 0, false, 0, 1, 0),
                new Action(2, "View Today Win Loss", "/wl", 20, 2, 0, true, 0, 1, 0),
                new Action(3, "View Balance Info", "/bal", 30, 3, 0, true, 0, 1, 0),

                new Action(4, "Specific Direct Downline", "", 50, 1, 1, false, 0, 1, 0),
                new Action(5, "Top 50 Outstanding", "50", 40, 1, 1, true, 0, 1, 0),
                new Action(6, "Please input username", "", 60, 1, 4, true, 0, 2, 0),

                new Action(7, "Top x Outstanding", "", 70, 1, 1, false, 0, 1, 0),
                 new Action(8, "Please input num of downline", "", 60, 1, 7, true, 0, 2, 0),
            };
            var userActions = new List<UserAction>
            {
                new UserAction(1, 1, 1, string.Empty, DateTime.Now),
                new UserAction(2, 1, 4, string.Empty, DateTime.Now),
                new UserAction(3, 1, 4, string.Empty, DateTime.Now),
                new UserAction(4, 1, 4, string.Empty, DateTime.Now),
                new UserAction(5, 1, 6, "supertest0", DateTime.Now),
                new UserAction(6, 1, 6, "supertest1", DateTime.Now)
            };

            var compiledCommand = actionCompiler.Compile(actions, userActions);

            Assert.Equal("/os supertest1", compiledCommand);
        }
    }
}