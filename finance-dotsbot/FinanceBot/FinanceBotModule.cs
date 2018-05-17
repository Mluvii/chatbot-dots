using System.Configuration;
using Autofac;
using FinanceBot.BLL;
using FinanceBot.BotAssets.Dialogs;
using FinanceBot.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace FinanceBot
{
    public class FinanceBotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DialogFactory>()
                .Keyed<IDialogFactory>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<MainDialog>()
                .InstancePerDependency();

            builder.RegisterType<EntryDialog>()
                .InstancePerDependency();

            builder.RegisterType<CrmService>()
                .As<ICrmService>()
                .WithParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string),
                    (pi, ctx) => ConfigurationManager.AppSettings[pi.Name])
                .InstancePerDependency();

            builder.RegisterType<HandoverDialog>()
                .InstancePerDependency();

            builder.RegisterType<HelpDialog>()
                .InstancePerDependency();

            builder.RegisterType<AvailibleOperatorsDialog>()
                .InstancePerDependency();

            builder.RegisterType<HelpScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}
