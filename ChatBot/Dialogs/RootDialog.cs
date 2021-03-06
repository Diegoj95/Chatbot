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
                SetDni,
                ShowData
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt), TextPromptValidator));
        }
        private enum Validator
        {
            Name,
            Email,
            Rut,
        }

        //ESTE METODO LE PREGUNTA AL USUARIO SU NOMBRE
        private async Task<DialogTurnResult> SetName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Antes de comenzar necesito algunos datos", cancellationToken: cancellationToken);
            await Task.Delay(1000);
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Por favor ingresa tu nombre")},
                cancellationToken
            );
        }

        //ESTE METODO LE PREGUNTA AL USUARIO SE CORREO ELECTRONICO
        private async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt), 
                new PromptOptions { Prompt = MessageFactory.Text("Por favor, ingresa tu correo electronico"), RetryPrompt = MessageFactory.Text("Ingresa un correo válido"), Validations = Validator.Email }, 
                cancellationToken);
        }

        //ESTE METODO LE PREGUNTA AL USUARIO SU RUT
        private async Task<DialogTurnResult> SetDni(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Ingresa tu rut"), RetryPrompt = MessageFactory.Text("Por favor, ingresa un rut válido"), Validations = Validator.Rut },
                cancellationToken);
        }


        //ESTE METODO SE ENCARGA DE VALIDAR LAS RESPUESTAS DEL USUARIO POR MEDIO DE EXPRESIONES REGULARES
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


                case Validator.Rut:

                    string rut = @"\b(\d{1,3}(?:(\.?)\d{3}){2}(-)[\dkK])\b";
                    string input2 = promptContext.Context.Activity.Text;

                    Match m2 = Regex.Match(input2, rut);

                    if (m2.Success)
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

        
        private async Task<DialogTurnResult> ShowData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Genial, gracias por registrar tus datos.", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        
    }
}
