﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotsBot.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DotsBot.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to and.
        /// </summary>
        internal static string and {
            get {
                return ResourceManager.GetString("and", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Back.
        /// </summary>
        internal static string CancellableDialog_back {
            get {
                return ResourceManager.GetString("CancellableDialog_back", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You do not seem to have any pending orders.
        /// </summary>
        internal static string CrmQueryFailed {
            get {
                return ResourceManager.GetString("CrmQueryFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Goodbye.
        /// </summary>
        internal static string goodbye {
            get {
                return ResourceManager.GetString("goodbye", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Talk to a person.
        /// </summary>
        internal static string HelpDialog_connect_operator {
            get {
                return ResourceManager.GetString("HelpDialog_connect_operator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End.
        /// </summary>
        internal static string HelpDialog_end {
            get {
                return ResourceManager.GetString("HelpDialog_end", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Looks like you&apos;re stuck. Select one of these options..
        /// </summary>
        internal static string HelpDialog_Prompt {
            get {
                return ResourceManager.GetString("HelpDialog_Prompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start over.
        /// </summary>
        internal static string HelpDialog_start_over {
            get {
                return ResourceManager.GetString("HelpDialog_start_over", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In how many installments would you like to divide the amount? Choose one of the options below (2 to 24)..
        /// </summary>
        internal static string MluviiDialog_instalments_prompt {
            get {
                return ResourceManager.GetString("MluviiDialog_instalments_prompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unfortunately I could not connect you with the operator. Would you like to start over?.
        /// </summary>
        internal static string MluviiDialog_operator_failed {
            get {
                return ResourceManager.GetString("MluviiDialog_operator_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pokud si nepřejete nyní zadávat své údaje, můžu Vás místo toho spojit s operátorem..
        /// </summary>
        internal static string MluviiDialog_person_form_cancelled {
            get {
                return ResourceManager.GetString("MluviiDialog_person_form_cancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We are offering you credit for product {0} of a total value of {1}, divided into {2} monthly installments of {3} and interest rate of {4}%..
        /// </summary>
        internal static string MluviiDialog_product_offer {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sign with call center operator.
        /// </summary>
        internal static string MluviiDialog_product_offer_choice_not_tincans {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_choice_not_tincans", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Digitally sign.
        /// </summary>
        internal static string MluviiDialog_product_offer_choice_sign_online {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_choice_sign_online", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unfortunately, the provided details are incorrect. I will connect you with a person now.
        /// </summary>
        internal static string MluviiDialog_product_offer_sign_failed {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_sign_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you, the contract is on its way to your mailbox {0}. Product {1} can be picked up at AlzaBox 1561 / 4a Vyskočilova Street (Microsoft Building)..
        /// </summary>
        internal static string MluviiDialog_product_offer_signed {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_signed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Interest rate: {0}.
        /// </summary>
        internal static string MluviiDialog_product_offer_subTitle {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_subTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Credit agreement nr. {0}.
        /// </summary>
        internal static string MluviiDialog_product_offer_title {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sign please (name surname).
        /// </summary>
        internal static string MluviiDialog_product_offer_your_signature_here {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_your_signature_here", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unfortunately the provided details do not match. Please sign in the following format: Name Surname.
        /// </summary>
        internal static string MluviiDialog_product_offer_your_signature_here_retry {
            get {
                return ResourceManager.GetString("MluviiDialog_product_offer_your_signature_here_retry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Virtual Assistant.
        /// </summary>
        internal static string MluviiDialog_virtual_assistant {
            get {
                return ResourceManager.GetString("MluviiDialog_virtual_assistant", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OK, please wait while I check for an available operator.
        /// </summary>
        internal static string MluviiDialog_wait_checking_available_operators {
            get {
                return ResourceManager.GetString("MluviiDialog_wait_checking_available_operators", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to These operators are available: {0}.
        /// </summary>
        internal static string OperatorConnect_available_list {
            get {
                return ResourceManager.GetString("OperatorConnect_available_list", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Who would you like to talk to?.
        /// </summary>
        internal static string OperatorConnect_select_operator {
            get {
                return ResourceManager.GetString("OperatorConnect_select_operator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Still looking for available operators.
        /// </summary>
        internal static string OperatorConnect_still_looking {
            get {
                return ResourceManager.GetString("OperatorConnect_still_looking", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are being connected to {0}. Please standby..
        /// </summary>
        internal static string OperatorConnect_wait {
            get {
                return ResourceManager.GetString("OperatorConnect_wait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We are sorry, it seems no operators are available at the moment..
        /// </summary>
        internal static string OperatorSelection_none_availible {
            get {
                return ResourceManager.GetString("OperatorSelection_none_availible", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I didnt&apos; undestant. Please try again..
        /// </summary>
        internal static string RetryText {
            get {
                return ResourceManager.GetString("RetryText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Back.
        /// </summary>
        internal static string RootDialog_Checkout_Cancel {
            get {
                return ResourceManager.GetString("RootDialog_Checkout_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operator.
        /// </summary>
        internal static string WelcomeMessage_operator {
            get {
                return ResourceManager.GetString("WelcomeMessage_operator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, would you like to sign a credit agreement for {1} with the help of our call center operator or with me - the Virtual Assistant?.
        /// </summary>
        internal static string WelcomeMessage_prompt {
            get {
                return ResourceManager.GetString("WelcomeMessage_prompt", resourceCulture);
            }
        }
    }
}
