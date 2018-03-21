using System.Configuration;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using MluviiBot.BLL;
using MluviiBot.BotAssets;
using MluviiBot.BotAssets.Dialogs;
using MluviiBot.Dialogs;

namespace MluviiBot
{
    public class MluviiBotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DialogFactory>()
                .Keyed<IDialogFactory>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RootDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();
            
            builder.RegisterType<MluviiDialog>()
                .InstancePerDependency();
            
            builder.RegisterType<FakeCrmService>()
                .As<ICrmService>()
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