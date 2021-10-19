using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.RegularExpressions;

namespace ChatBot.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        public RootDialog()
        {
            var waterfallStep = new WaterfallStep[]
            {
                SetName,
                SetEmail,
                //SetDni,
                //ShowData
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt), TextPromptValidator));
        }
        private enum Validator
        {
            Name,
            Email,
            //Rut,
        }

        private async Task<DialogTurnResult> SetName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Necesito algunos datos", cancellationToken: cancellationToken);
            await Task.Delay(1000);
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Por favor ingresa tu nombre")},
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt), 
                new PromptOptions { Prompt = MessageFactory.Text("Ingresa tu correo electronico"), RetryPrompt = MessageFactory.Text("Ingresa un correo valido"), Validations = Validator.Email }, 
                cancellationToken);
        }

        private async Task<bool> TextPromptValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            switch (promptContext.Options.Validations != null ? (Validator)promptContext.Options.Validations : (Validator)(-1))
            {
                case Validator.Email:

                    string email = @"((\S+)([@]{1})(\S[^.]+)(\.)([a-zA-Z]{1,5})((\.)([a-zA-Z]{1,5}))?)";
                    string input = promptContext.Context.Activity.Text;

                    Match m = Regex.Match(input, email);

                    if (m.Success)
                    {
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        return await Task.FromResult(false);
                    }

                default:
                    return await Task.FromResult(true);
            }
        }

        /*
        private async Task<DialogTurnResult> ShowData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Genial, gracias por registrar tus datos.", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        */
    }
}
