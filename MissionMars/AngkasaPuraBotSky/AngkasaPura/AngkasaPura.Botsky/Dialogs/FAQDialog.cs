﻿

using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using System.Configuration;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace AngkasaPura.Botsky.Dialogs
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-qnamaker
    [Serializable]
    public class FAQDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        public FAQDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnASubscriptionKey"], ConfigurationManager.AppSettings["QnAKnowledgebaseId"])))
        {
        }
    }
}