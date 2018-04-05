using System.Configuration;
using Autofac;
using DotsBot.BLL;
using DotsBot.BotAssets.Dialogs;
using DotsBot.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace DotsBot
{
    public class DotsBotModule : Module
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

            builder.RegisterType<DebugDialog>()
                .InstancePerDependency();

            builder.RegisterType<AvailibleOperatorsDialog>()
                .InstancePerDependency();

            builder.RegisterType<HelpScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DebugScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}