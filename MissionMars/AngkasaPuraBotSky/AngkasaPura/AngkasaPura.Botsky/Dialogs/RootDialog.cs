namespace AngkasaPura.Botsky.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using AngkasaPura.Botsky.Helpers;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        const string LaporanOption = "I want to report";
        const string FlightOption = "Flight Information";
        const string LuggageOption = "Luggage Information";
        const string FacilityOption = "Facility Information (dine, taxi, hotel, shop)";
        const string ImportantNoOption = "Important Number Information";
        const string APTVOption = "Angkasa Pura Television";
        const string NewsOption = "Latest News";
        const string ReportOption = "Angkasa Pura Internal Report";
        const string ThirdPartyOption = "Our partner services";
        const string FAQOption = "FAQ (Question and Answers)";
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.ToLower().Contains("tolong") || message.Text.ToLower().Contains("bantuan") || message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem"))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else
            {
                this.ShowOptions(context);
            }
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { LaporanOption, FlightOption, LuggageOption, FacilityOption, ImportantNoOption, APTVOption, NewsOption, ThirdPartyOption, FAQOption, ReportOption }, "Hello Boss, can I help you ?", "Please select again boss.", 10);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case LaporanOption:
                        context.Call(new ReportDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case FlightOption:
                        context.Call(new FlightDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case FAQOption:
                        context.Call(new FAQDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case FacilityOption:
                        context.Call(new FacilityDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case ImportantNoOption:
                        context.Call(new ImportantNoDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case LuggageOption:
                        context.Call(new LuggageDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case APTVOption:
                        context.Call(new APTVDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case NewsOption:
                        context.Call(new NewsDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case ReportOption:
                        context.Call(new ReportAPDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case ThirdPartyOption:
                        context.Call(new OtherDialog(), this.ResumeAfterOptionDialog);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(MESSAGESINFO.TOO_MANY_ATTEMPT);

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                await context.PostAsync($"Thank you for using our services, if you don't get any result, please try again.");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}