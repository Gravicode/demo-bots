using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Resource;
using System.Resources;
using System.Text;
using System.Threading;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using System.Configuration;
using Newtonsoft.Json;
using AngkasaPura.Botsky.Business;

namespace AngkasaPura.Botsky.Dialogs
{
    [Serializable]
    public class ReportDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please input your report.");
            var ReportFormDialog = FormDialog.FromForm<Laporan>(Laporan.BuildForm, FormOptions.PromptInStart);
            context.Call(ReportFormDialog, this.ResumeAfterReportFormDialog);
        }
        private async Task ResumeAfterReportFormDialog(IDialogContext context, IAwaitable<Laporan> result)
        {
            try
            {
                var hasil = await result;
                //do nothing
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "bos membatalkan laporan, dialog ditutup.";
                }
                else
                {
                    reply = $"Ada masalah teknis euy:( Detailnya: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

    }
    public enum TipeLaporan
    {
        [Terms("complain")]
        Complain = 1,
        [Terms("advice")]
        Advice,
        [Terms("critic")]
        Critic,
        [Terms("emergency")]
        Emergency
    }
    [Serializable]
    public class Laporan
    {
        public string NoLaporan;
        public DateTime TglLaporan;
        [Prompt("What's your name ? {||}")]
        public string Nama;

        [Prompt("Your phone number ? {||}")]
        public string Telpon;

        [Prompt("Your email ? {||}")]
        public string Email;

        [Prompt("What kind of report that you want to say ? {||}")]
        public TipeLaporan TipeLaporan;

        [Prompt("Please input your report.. {||}")]
        public string Keterangan;

        [Prompt("Where it happened ? {||}")]
        public string Lokasi;

        [Prompt("Please type the date and time (year/month/date jam:menit) ? {||}")]
        public DateTime Waktu;

        [Prompt("Please input the priority scale, 1 [not important] - 10 [important] ? ")]
        public int SkalaPrioritas = 1;

        public static IForm<Laporan> BuildForm()
        {

            OnCompletionAsyncDelegate<Laporan> processReport = async (context, state) =>
            {
                await Task.Run(() =>
                {
                    state.NoLaporan = $"LP-{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}";
                    state.TglLaporan = DateTime.Now;
                    Complain com = new Complain() { Email=state.Email, Keterangan=state.Keterangan, Lokasi=state.Lokasi, Nama=state.Nama, NoLaporan=state.NoLaporan , SkalaPrioritas=state.SkalaPrioritas , Telpon=state.Telpon , TglLaporan=state.TglLaporan ,TipeLaporan=state.TipeLaporan  , Waktu=state.Waktu   };
                    AirportData.InsertComplain(com);
                    
                }
                );

            };
            var builder = new FormBuilder<Laporan>(false);
            var form = builder
                        .Field(nameof(Nama))
                        .Field(nameof(Telpon))
                        .Field(nameof(Email))
                        .Field(nameof(TipeLaporan))
                        .Field(nameof(Keterangan))
                        .Field(nameof(Lokasi))
                        .Field(nameof(Waktu))
                        .Field(nameof(SkalaPrioritas), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, skala valid" };
                                bool res = int.TryParse(value.ToString(), out int jml);
                                if (res)
                                {
                                    if (jml <= 0)
                                    {
                                        result.Feedback = "please input with correct number, minimum value is 1";
                                        result.IsValid = false;
                                    }
                                    else if (jml > 10)
                                    {
                                        result.Feedback = "please input the correct number, maximum value is 10";
                                        result.IsValid = false;
                                    }
                                }
                                else
                                {
                                    result.Feedback = "please input with number";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Confirm(async (state) =>
                        {
                            var pesan = $"We have received report from {state.Nama} about {state.TipeLaporan.ToString()}, is it valid ?";
                            return new PromptAttribute(pesan);
                        })
                        .Message($"Thanks for your report.")
                        .OnCompletion(processReport)
                        .Build();
            return form;
        }
    }

    

}