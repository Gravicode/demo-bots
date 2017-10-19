using System;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using AngkasaPura.Botsky.Business;
using AngkasaPura.Botsky.Helpers;

namespace AngkasaPura.Botsky.Dialogs
{
    [Serializable]
    public class FlightDialog : IDialog<object>
    {
        const string Flight1 = "Query by Flight No";
        const string Flight2 = "Query by Air Line";
        public async Task StartAsync(IDialogContext context)
        {
            this.ShowOptions(context);
           
        }

       
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { Flight1, Flight2 }, "You can get flight information by answer this question..", "Please choose again.", 2);
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<FlightQuery1> result)
        {
            try
            {
                var hasil = await result;
                await context.PostAsync(hasil.Result);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = MESSAGESINFO.CANCEL_DIALOG;
                }
                else
                {
                    reply = $"{MESSAGESINFO.ERROR_INFO} Detail: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task ResumeAfterOptionDialog2(IDialogContext context, IAwaitable<FlightQuery2> result)
        {
            try
            {
                var hasil = await result;
                await context.PostAsync(hasil.Result);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = MESSAGESINFO.CANCEL_DIALOG;
                }
                else
                {
                    reply = $"{MESSAGESINFO.ERROR_INFO} Detail: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case Flight1:
                        {
                            await context.PostAsync("You can get flight information by answer this question..");
                            var FlightFormDialog = FormDialog.FromForm<FlightQuery1>(FlightQuery1.BuildForm, FormOptions.PromptInStart);

                            context.Call(FlightFormDialog, this.ResumeAfterOptionDialog);
                        }
                        break;

                    case Flight2:
                        {
                            await context.PostAsync("You can get flight information by answer this question..");
                            var FlightFormDialog = FormDialog.FromForm<FlightQuery2>(FlightQuery2.BuildForm, FormOptions.PromptInStart);

                            context.Call(FlightFormDialog, this.ResumeAfterOptionDialog2);
                        }
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Weeew! Terlalu banyak nyoba bos :(. Ojo khawatir, silakan coba lagi bos!");
                
            }
        }

    }


    [Serializable]
    //[Template(TemplateUsage.NotUnderstood, "Ane ga paham \"{0}\".", "Coba lagi ya, ane tidak dapat nilai \"{0}\".")]
    public class FlightQuery1
    {
        public string Result;
        public DateTime QueryDate;
        [Prompt("Type your flight number ? {||}")]
        public string FlightNo;
        
        public static IForm<FlightQuery1> BuildForm()
        {

            OnCompletionAsyncDelegate<FlightQuery1> processOrder = async (context, state) =>
            {
                await Task.Run(() =>
                {
                   
                    state.QueryDate = DateTime.Now;
                    var data = AirportData.GetFlightByCode(state.FlightNo);
                    if(data != null && data.Count > 0)
                    {
                        var item = data[0];
                        state.Result = $"AFSKEY:{item.AFSKEY}, FLIGHT_NO : {item.FLIGHT_NO}, LEG:{item.LEG}, LEG_DESCRIPTION:{item.LEG_DESCRIPTION}, SCHEDULED:{item.SCHEDULED}, ESTIMATED :{item.ESTIMATED}, ACTUAL:{item.ACTUAL}, CATEGORY_CODE :{item.CATEGORY_CODE}, CATEGORY_NAME : {item.CATEGORY_NAME}, REMARK_CODE:{item.REMARK_CODE}, REMARK_DESC_ENG:{item.REMARK_DESC_ENG}, REMARK_DESC_IND:{item.REMARK_DESC_IND},TERMINAL_ID:{item.TERMINAL_ID}, GATE_CODE : {item.GATE_CODE}, GATE_OPEN_TIME : {item.GATE_OPEN_TIME}, GATE_CLOSE_TIME : {item.GATE_CLOSE_TIME}, STATION1 :{item.STATION1}, STATION1_DESC : {item.STATION1_DESC}, STATION2 :{item.STATION2}, STATION2_DESC : {item.STATION2_DESC}";
                    }
                    else
                    {
                        state.Result = $"Data is not found. Please try again..";
                    }
                    /*
                     */

                });
            };
            var builder = new FormBuilder<FlightQuery1>(false);
            var form = builder
                        .Field(nameof(FlightNo))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }

    [Serializable]
    //[Template(TemplateUsage.NotUnderstood, "Ane ga paham \"{0}\".", "Coba lagi ya, ane tidak dapat nilai \"{0}\".")]
    public class FlightQuery2
    {
        public string Result;
        public DateTime QueryDate;
        [Prompt("Your air line ? {||}")]
        public string Airline;

        public static IForm<FlightQuery2> BuildForm()
        {

            OnCompletionAsyncDelegate<FlightQuery2> processOrder = async (context, state) =>
            {
                await Task.Run( () =>
                {

                    state.QueryDate = DateTime.Now;
                    var data = AirportData.GetFlightByAirline(state.Airline);
                    if (data == null || data.Count > 0)
                    {
                        var item = data[0];
                        state.Result = $"AFSKEY:{item.AFSKEY}, FLIGHT_NO : {item.FLIGHT_NO}, LEG:{item.LEG}, LEG_DESCRIPTION:{item.LEG_DESCRIPTION}, SCHEDULED:{item.SCHEDULED}, ESTIMATED :{item.ESTIMATED}, ACTUAL:{item.ACTUAL}, CATEGORY_CODE :{item.CATEGORY_CODE}, CATEGORY_NAME : {item.CATEGORY_NAME}, REMARK_CODE:{item.REMARK_CODE}, REMARK_DESC_ENG:{item.REMARK_DESC_ENG}, REMARK_DESC_IND:{item.REMARK_DESC_IND},TERMINAL_ID:{item.TERMINAL_ID}, GATE_CODE : {item.GATE_CODE}, GATE_OPEN_TIME : {item.GATE_OPEN_TIME}, GATE_CLOSE_TIME : {item.GATE_CLOSE_TIME}, STATION1 :{item.STATION1}, STATION1_DESC : {item.STATION1_DESC}, STATION2 :{item.STATION2}, STATION2_DESC : {item.STATION2_DESC}";
                    }
                    else
                    {
                        state.Result=$"Data is not found. Please try again..";
                    }
                });
            };
            var builder = new FormBuilder<FlightQuery2>(false);
            var form = builder
                        .Field(nameof(Airline))
                        .OnCompletion(processOrder)
                        .Build();
            return form;
        }
    }
}