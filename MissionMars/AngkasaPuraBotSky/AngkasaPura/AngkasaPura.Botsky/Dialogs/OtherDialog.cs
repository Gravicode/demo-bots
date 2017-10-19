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
    public class OtherDialog : IDialog<object>
    {
        const string FindMyTagOption = "I want find my tag (people, things, etc)";
        const string ShoppingOnlineOption = "I want to buy something...";
     
        public async Task StartAsync(IDialogContext context)
        {
            this.ShowOptions(context);
        }

        
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { FindMyTagOption, ShoppingOnlineOption }, "Here is our third party services, please choose menu below ?", "Please select again boss.", 2);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case FindMyTagOption:
                        context.Call(new TagTrackerDialog (), this.ResumeAfterOptionDialog);
                        break;

                    case ShoppingOnlineOption:
                        context.Call(new ShoppingDialog(), this.ResumeAfterOptionDialog);
                        break;
                   
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(MESSAGESINFO.TOO_MANY_ATTEMPT);
                
            }
        }



        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                //await context.PostAsync($"Thank you for using our services, if you don't get any result, please try again.");
            }
            catch (Exception ex)
            {
                //await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}